using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using Spectrogram;
using NAudio.Wave;
using System.Linq;
using System.Drawing.Imaging;
using System.Drawing;
//using UnityEngine.Rendering.Universal.Internal;

using Microsoft.ML;
using Microsoft.ML.Data;
using JetBrains.Annotations;
using System.Runtime.InteropServices;
using System.Data;
using System;

public class MLHandler : MonoBehaviour
{

    // since bitmaps arent supported in unity for every platform, this code aint gonna get used
    //private void CreateSpectrogram(string fileName)
    //{
    //    var spectrogramName = fileName.Substring(0, fileName.Length - 4) + "-spectro.jpg";
    //    if (File.Exists(spectrogramName)) return;

    //    (double[] values, int sampleRate) = ReadMono(fileName);

    //    Debug.Log("value length: " + values.Length);

    //    var spec = new SpectrogramGenerator(sampleRate: sampleRate, fftSize: 2048, stepSize: 700, maxFreq: 3000);
    //    spec.Add(values);

    //    //var bitmap = spec.GetBitmap(intensity: 2);
    //    //spec.SaveImage(spectrogramName);

    //    // Create a Mel Spectrogram
    //    //Bitmap bmp = spec.GetBitmapMel();
    //    //bmp.Save("halMel.png", ImageFormat.Png);

    //    Debug.Log("chat did we make it?");
    //}

    //private (double[] audio, int sampleRate) ReadMono(string filePath, double multiplier = 16_000)
    //{
    //    var afr = new AudioFileReader(filePath);
    //    int sampleRate = afr.WaveFormat.SampleRate;
    //    int bytesPerSample = afr.WaveFormat.BitsPerSample / 8;
    //    int sampleCount = (int)(afr.Length / bytesPerSample);
    //    int channelCount = afr.WaveFormat.Channels;
    //    var audio = new List<double>(sampleCount);
    //    var buffer = new float[sampleRate * channelCount];
    //    int samplesRead = 0;
    //    while ((samplesRead = afr.Read(buffer, 0, buffer.Length)) > 0)
    //        audio.AddRange(buffer.Take(samplesRead).Select(x => x * multiplier));
    //    return (audio.ToArray(), sampleRate);
    //}

    // SUPER IMPORTANT ---------------------------------------------------------------------------------------- CHANGE SAMPLE WIDTH HERE
    // ---------------------------------------------------------------------------------- CHANGE IN BEATMAPPER TOO
    private static readonly int sampleWidth = 20000;

    public class IsSomethingSample
    {

        [LoadColumn(0)]
        [ColumnName("Label")]
        public bool isBeat { get; set; }

        [LoadColumn(1, 20000)] // ------------------------------------------------------RIGHT HERE AND BELOW (currently 6000)
        [VectorType(20000)]
        [ColumnName("Beat Samples")]
        public float[] beatSamples { get; set; }
    }

    public class IsSomethingPrediction
    {
        [ColumnName("Label")]
        public bool isBeat { get; set; }
        public float Probability { get; set; }
        public float Score { get; set; }
    }

    private static readonly string isBeatTrainingFilesPath = Application.streamingAssetsPath + "/TSVs/NoBeatTypes/"; //TODO
    private static readonly string isBassTrainingFilesPath = Application.streamingAssetsPath + "/TSVs/WithBeatTypes/"; //TODO
    private static readonly string isBeatModelFilePath = Application.streamingAssetsPath + "/isBeatModel.zip";
    private static readonly string isBassModelFilePath = Application.streamingAssetsPath + "/isBassModel.zip";
    static MLContext context;
    static IDataView trainingData;
    static ITransformer isBeatModel;
    static ITransformer isBassModel;

    static PredictionEngine<IsSomethingSample, IsSomethingPrediction> isBeatPredictionEngine;
    static PredictionEngine<IsSomethingSample, IsSomethingPrediction> isBassPredictionEngine;

    public static void teachModel() // this is the one I made that encapsulates all I saw in the video
    {
        context = new MLContext(seed: 0);
        //var pipeline = ProcessData();
        //var trainingPipeline = BuildAndTrainModel(trainingData, pipeline);
        processAndTrainIsSomethingTest(isBeatTrainingFilesPath, true);
        SaveModelAsFile(isBeatModelFilePath, isBeatModel);

        // now heres the fun part :D
        processAndTrainIsSomethingTest(isBassTrainingFilesPath, false);
        SaveModelAsFile(isBassModelFilePath, isBassModel);
    }

    private static readonly string musicPath = Path.Combine(Application.streamingAssetsPath, "Music/");

    public static float predictIsBeat(int sampleLocation, string songName, bool printDebugMsg = false)
    {
        if (context == null) context = new MLContext(seed: 0);
        if (isBeatModel == null) isBeatModel = context.Model.Load(isBeatModelFilePath, out var modelInputSchema); // DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDO SMTH HERE
        if (isBeatPredictionEngine == null) isBeatPredictionEngine = context.Model.CreatePredictionEngine<IsSomethingSample, IsSomethingPrediction>(isBeatModel);
        return predictIsSomething(sampleLocation, songName, isBeatModel, isBeatModelFilePath, isBeatPredictionEngine, printDebugMsg);
    }

    public static float predictIsBass(int sampleLocation, string songName, bool printDebugMsg = false)
    {
        if (context == null) context = new MLContext(seed: 0);
        if (isBassModel == null) isBassModel = context.Model.Load(isBassModelFilePath, out var modelInputSchema); // DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDO SMTH HERE
        if (isBassPredictionEngine == null) isBassPredictionEngine = context.Model.CreatePredictionEngine<IsSomethingSample, IsSomethingPrediction>(isBassModel);
        return predictIsSomething(sampleLocation, songName, isBassModel, isBassModelFilePath, isBassPredictionEngine, printDebugMsg);
    }

    private static float predictIsSomething(int sampleLocation, string songName, ITransformer model, string modelFilePath, PredictionEngine<IsSomethingSample, IsSomethingPrediction> engine, bool printDebugMsg = false) // and this one too
    {
        AudioFileReader afr = new AudioFileReader(musicPath + songName + ".mp3"); // ------------------------------- ALLOW OTHER FILE TYPES HERE IF TIME
        afr.Position = sampleLocation - (sampleWidth/2);

        // this stuff stolen from beatmapper
        List<float> audio = new List<float>(sampleWidth);
        float[] monoBuffer = new float[sampleWidth];
        float[] stereoBuffer = new float[sampleWidth*2];

        afr.Read(stereoBuffer, 0, stereoBuffer.Length);
        for (int j = 0; j < monoBuffer.Length; j++)
            monoBuffer[j] = stereoBuffer[j * 2];
        audio.AddRange(monoBuffer.Take(sampleWidth).Select(x => x * 16_000));
        monoBuffer = audio.ToArray();
        // up until here

        IsSomethingPrediction prediction = PredictIsSomethingGivenSamples(monoBuffer, model ,modelFilePath, engine);
        // this ill prpb have to return to later
        prediction.isBeat = Math.Abs(prediction.Score) > 3.0f; // ---------------------------------------------------------- come back here to change the score threshold

        if (printDebugMsg) Debug.Log($"guessed result at {sampleLocation} is: {prediction.isBeat}, probability: {prediction.Probability}, score: {prediction.Score}");
        return prediction.Score;
    }

    //private IEstimator<ITransformer> ProcessData()
    //{
    //    var pipeline = context.Transforms.Conversion.MapValueToKey(inputColumnName: "Beat Type", outputColumnName: "Label");
    //        //.Append(context.Transforms.Concatenate("Features", "Beat Samples"));
    //    //.AppendCacheCheckpoint(context); // ---------------------------------------------------- CUS THE DATA SET IS HIGH VOLUME ILL START WITH THIS OFF
    //    return pipeline;
    //}

    //private IEstimator<ITransformer> BuildAndTrainModel(IDataView trainingDataView, IEstimator<ITransformer> pipeline)
    //{
    //    var trainingPipeline = pipeline.Append(context.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Beat Samples"))
    //        .Append(context.Transforms.Conversion.MapKeyToValue("PredictedBeatType"));
    //    Debug.Log("About to fit, lets see how long this takes");
    //    //return null; // ---------------------------------------------------------------------------------------------------------------------------------------------------
    //    model = trainingPipeline.Fit(trainingDataView);
    //    Debug.Log("finally finished");
    //    return trainingPipeline;
    //}

    private static IEstimator<ITransformer> processAndTrainIsSomethingTest(string dataFilePath, bool isCheckingForBeats)
    {
        //context = new MLContext(seed: 0);

        //trainingData = context.Data.LoadFromTextFile<IsSomethingSample>(dataFilePath + "*.tsv");
        //DataOperationsCatalog.TrainTestData splitDataView = context.Data.TrainTestSplit(trainingData, testFraction: 0.2);

        //var kmeans = context.Transforms.Concatenate("Features", "Beat Samples")
        //    //.Append(context.Transforms.NormalizeMinMax("Features"))
        //    //.Append(context.BinaryClassification.Trainers.SdcaLogisticRegression("Label", "Beat Samples"));
        //    .Append(context.BinaryClassification.Trainers.LbfgsLogisticRegression("Label", "Beat Samples"));
        ////context.Transforms.Conversion.MapValueToKey("Beat Type")
        ////.Append(context.Transforms.NormalizeMinMax("Features"))
        ////.Append(context.Transforms.Concatenate("Features", "Features"))
        ////.Append(context.Transforms.NormalizeLpNorm("Features"))
        ////.Append(context.MulticlassClassification.Trainers.SdcaMaximumEntropy("Beat Type", "Beat Samples"))
        ////.Append(context.Transforms.Conversion.MapKeyToValue("Beat Type"));
        //Debug.Log("started fitting");
        //if (isCheckingForBeats)
        //    isBeatModel = kmeans.Fit(splitDataView.TrainSet);
        //else
        //    isBassModel = kmeans.Fit(splitDataView.TrainSet);
        //Debug.Log("finished fitting");

        ////evaluation here
        //Debug.Log("testing accuracy");

        //IDataView predictions = (isCheckingForBeats ? isBeatModel : isBassModel).Transform(splitDataView.TestSet);
        //CalibratedBinaryClassificationMetrics metrics = context.BinaryClassification.Evaluate(predictions, "Label");
        //Debug.Log($"Model testing accuracy:\nAccuracy: {metrics.Accuracy:P2}\nAuc: {metrics.AreaUnderRocCurve:P2}\nF1Score: {metrics.F1Score:P2}");

        //return kmeans;
        return null; // ----------------------- doing this cuz idk if unity will lat me build
    }

    private static void SaveModelAsFile(string filepath, ITransformer model)
    {
        context.Model.Save(model, trainingData.Schema, filepath);
    }

    private static IsSomethingPrediction PredictIsSomethingGivenSamples(float[] samples, ITransformer model, string modelFilePath, PredictionEngine<IsSomethingSample, IsSomethingPrediction> predEngine)
    {
        if (context == null) context = new MLContext();
        //if (model == null) model = context.Model.Load(modelFilePath, out var modelInputSchema);
        var beatInfo = new IsSomethingSample() { beatSamples = samples };
        
        var result = predEngine.Predict(beatInfo);
        return result;
    }

    // --------------------------------------------------------------------------------



}






//public class IsBeatPrediction
//{
//    private const int samples = 2;
    
//    [LoadColumn(0)]
//    public ushort beatType;

//    [LoadColumn(1,samples)]
//    [VectorType(samples)]
//    public ushort beatSamples;


//    [ColumnName("PredictedLabel")]
//    public ushort Prediction { get; set; }

//    public float Probability { get; set; }

//    public float Score { get; set; }
//}

//public class IsBeatModel
//{
//    private static string ModelPath = "IsBeatModel.zip";
//    private MLContext mlContext;
//    private ITransformer model;

//    public IsBeatModel()
//    {
//        mlContext = new MLContext(seed: 0);
//    }

//    IEstimator<ITransformer> ProcessData()
//    {

//    }

//    public void Train(IDataView trainingData)
//    {
//        var pipeline = mlContext.Transforms.Concatenate("Features", new[] { "PlaytimeHours", "AchievementsUnlocked" })
//            .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "BeatType", featureColumnName: "Samples"));

//        model = pipeline.Fit(trainingData);
//        mlContext.Model.Save(model, trainingData.Schema, ModelPath);
//    }

//    public ushort Predict(float[] samples)
//    {
//        var predictionEngine = mlContext.Model.CreatePredictionEngine<gameenjoymentprediction, gameenjoymentprediction ="">(model);
//        var prediction = predictionEngine.Predict(new IsBeatPrediction { PlaytimeHours = playtimeHours, AchievementsUnlocked = achievementsUnlocked });
//        return prediction.Prediction;
//    }
//}
