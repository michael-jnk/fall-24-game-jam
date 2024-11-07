using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.Linq;
using UnityEditor;

public class BeatMapper : MonoBehaviour
{
    private string savePath;
    public AudioSource audioSource;
    private AudioPlayer audioPlayer;
    //private AudioPitchEstimator estimator;

    private beatTrack testTrack;

    public Boolean devMode = false;
    public Boolean beatMapperMode = false;
    private List<int> addableBeats;

    public BeatFactory beatFactory;

    private bool initialized = false;

    public void Initialize()
    {
        audioSource = GetComponent<AudioSource>();
        audioPlayer = GetComponent<AudioPlayer>();
        //estimator = GetComponent<AudioPitchEstimator>();

        savePath = Path.Combine(Application.streamingAssetsPath, "Music/");

        //int[] testMascaraBeats = { 17746, 55210, 69013, 156172, 166521, 180776, 212620, 222028, 227673, 246489, 256838, 263423, 323635, 368793, 426182, 457228, 513652, 759141 };
        //ushort[] testMascaraBeatTypes = { 0, 0, 0, 1, 1, 2, 1, 1, 2, 2, 2, 2, 2, 1, 1, 1, 0, 0 };
        //int[] testMascaraBeats = { 156172, 166521, 180776, 212620, 222028, 227673, 246489, 256838, 263423, 323635, 368793, 426182, 457228 };
        //ushort[] testMascaraBeatTypes = { 1, 1, 2, 1, 1, 2, 2, 2, 2, 2, 1, 1, 1 };
        //testTrack = new beatTrack(testMascaraBeats.Length, testMascaraBeats, testMascaraBeatTypes); saveTrack(testTrack);

        int[] beatsArray = { };
        ushort[] beatsTypesArray = { };

        if (!File.Exists(savePath + audioPlayer.songName + ".beattrack"))
        {
            //Debug.Log("created empty test track");
            //testTrack = new beatTrack(beatsArray.Length, beatsArray, beatsTypesArray);
            Debug.Log("beginning generating track");
            testTrack = generateBeatTrack(audioPlayer.songName);
            Debug.Log("generated a new test track");
        } else
        {
            testTrack = loadTrack(audioPlayer.songName);
            Debug.Log("loaded test track");
        }

        if (beatFactory == null)
            Debug.Log("null beatFactory error");
        else
        {
            beatFactory.Initialize(audioSource, testTrack);
        }


        // test for model training
        //saveTrackAsTSV(testTrack, audioPlayer.songName, false);
        //saveTrackAsTSV(loadTrack("MASCARA"), "MASCARA", false);
        //saveTrackAsTSV(loadTrack("KeyshiaColeLove"), "KeyshiaColeLove", false);
        //saveTrackAsTSV(loadTrack("LizzyMcAlpinerecklessdriving"), "LizzyMcAlpinerecklessdriving", false);
        //saveTrackAsTSV(loadTrack("MASCARA"), "MASCARA", true);
        //saveTrackAsTSV(loadTrack("KeyshiaColeLove"), "KeyshiaColeLove", true);
        //saveTrackAsTSV(loadTrack("LizzyMcAlpinerecklessdriving"), "LizzyMcAlpinerecklessdriving", true);

        //  ------------ pausing on this for now to do some annotations

        // another model test training, hoo boy
        //audioSource.Pause();
        //MLHandler.teachModel();


        //Debug.Log("start predicting is beat");
        //System.Random rand = new System.Random();
        //for (int i = 0; i < 10; i++)
        //{
        //    ml.predictIsBeat(rand.Next(0, 150000), audioPlayer.songName, printDebugMsg: true);
        //}
        //ml.predictIsBeat(156172, audioPlayer.songName); // should be a beat
        //ml.predictIsBeat(17746, audioPlayer.songName); // should be a blank
        //Debug.Log("end predicting");

        //Debug.Log("start predicting is beat");
        //System.Random rand = new System.Random();
        //for (int i = 67291; i < 356000; i += 44100 / 8)
        //{
        //    ml.predictIsBeat(i, audioPlayer.songName, true);
        //}
        //ml.predictIsBeat(156172, audioPlayer.songName, true); // should be a beat
        //ml.predictIsBeat(17746, audioPlayer.songName, true); // should be a blank
        //Debug.Log("end predicting");

        //Debug.Log("start predicting beat types");
        //for (int i = 0; i < 10; i++)
        //{
        //    ml.predictIsBass(testTrack.beats[i], audioPlayer.songName, true);
        //}

        //audioSource.Play();
        initialized = true;
    }

    //bool modelTaught = false;

    private List<int> trackMakerBeats = new List<int>();
    private List<ushort> trackMakerBeatTypes = new List<ushort>();

    void Update()
    {
        if (!initialized) return;
        //if (!modelTaught)
        //{
        //    // another model test training, hoo boy
        //    MLHandler ml = this.GetComponent<MLHandler>();
        //    audioSource.Pause();
        //    ml.teachModel();

        //    modelTaught = true;
        //    return;
        //}

        if (devMode)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (audioSource == null)
                    Debug.LogError("audio source is null");
                else if (audioSource.isPlaying)
                {
                    audioSource.Pause();
                }
                else
                {
                    audioSource.Play();
                }
            }

            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (audioSource == null)
                    Debug.LogError("audio source is null");
                else
                {
                    beatType input = (Input.GetKeyDown(KeyCode.UpArrow)) ? beatType.MELODY : beatType.BASS;
                    int sample = audioSource.timeSamples;
                    //float frequency = estimator.Estimate(audioSource);
                    //Debug.Log("sample " + sample + ", type " + ((input == beatType.MELODY) ? "melody" : "bass"));

                }
            }

            if (beatMapperMode)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    if (Input.GetKeyDown(KeyCode.T))
                    {
                        saveTrackAsTSV(loadTrack("ballsinyourjaw"), "ballsinyourjaw", false);
                        saveTrackAsTSV(loadTrack("ballsinyourjaw"), "ballsinyourjaw", true);
                        saveTrackAsTSV(loadTrack("KeyshiaColeLove"), "KeyshiaColeLove", false);
                        saveTrackAsTSV(loadTrack("KeyshiaColeLove"), "KeyshiaColeLove", true);
                        saveTrackAsTSV(loadTrack("LupeFiascoKickPush"), "LupeFiascoKickPush", false);
                        saveTrackAsTSV(loadTrack("LupeFiascoKickPush"), "LupeFiascoKickPush", true);
                        saveTrackAsTSV(loadTrack("MASCARA"), "MASCARA", false);
                        saveTrackAsTSV(loadTrack("MASCARA"), "MASCARA", true);
                        saveTrackAsTSV(loadTrack("LizzyMcAlpinerecklessdriving"), "LizzyMcAlpinerecklessdriving", false);
                        saveTrackAsTSV(loadTrack("LizzyMcAlpinerecklessdriving"), "LizzyMcAlpinerecklessdriving", true);
                        MLHandler.teachModel();
                    }
                    if (Input.GetKeyDown(KeyCode.S))
                    {
                        saveTrack(testTrack);
                    }
                    if (Input.GetKeyDown(KeyCode.DownArrow))
                    {
                        trackMakerBeats.Add(audioSource.timeSamples);
                        trackMakerBeatTypes.Add((ushort)BeatMapper.beatType.BASS);
                        //Debug.Log("tracked beat at: " + audioSource.timeSamples);
                    }
                    if (Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        trackMakerBeats.Add(audioSource.timeSamples);
                        trackMakerBeatTypes.Add((ushort)BeatMapper.beatType.MELODY);
                        //Debug.Log("tracked beat at: " + audioSource.timeSamples);
                    }
                    if (Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        audioSource.timeSamples += (44100 * 3);
                    }
                    if (Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        audioSource.timeSamples = Math.Max(0, audioSource.timeSamples - (44100 * 3));
                    }
                }
            }
        }

        
    }


    public enum beatType : ushort
    {
        NONE,
        BASS,
        MELODY,
    }

    private void saveTrack(beatTrack track)
    {
        if (trackMakerBeats.Count != 0)
        {
            int newLength = trackMakerBeats.Count;
            int[] newBeats = new int[track.length + trackMakerBeats.Count];
            ushort[] newBeatTypes = new ushort[track.length + trackMakerBeats.Count];
            int trackIterator = 0;
            for (int i = 0; i < newLength; i++)
            {
                if (trackIterator == track.length)
                {
                    newBeats[i] = trackMakerBeats[0];
                    trackMakerBeats.RemoveAt(0);
                    newBeatTypes[i] = trackMakerBeatTypes[0];
                    trackMakerBeatTypes.RemoveAt(0);
                } else if (trackMakerBeats.Count == 0)
                {
                    newBeats[i] = track.beats[trackIterator];
                    newBeatTypes[i] = track.beatTypes[trackIterator];
                    trackIterator += 1;
                } else
                {
                    if (trackMakerBeats[0] < track.beats[trackIterator])
                    {
                        newBeats[i] = trackMakerBeats[0];
                        trackMakerBeats.RemoveAt(0);
                        newBeatTypes[i] = trackMakerBeatTypes[0];
                        trackMakerBeatTypes.RemoveAt(0);
                    } else
                    {
                        newBeats[i] = track.beats[trackIterator];
                        newBeatTypes[i] = track.beatTypes[trackIterator];
                        trackIterator += 1;
                    }
                }
            }
            track.length = newLength;
            track.beats = newBeats;
            track.beatTypes = newBeatTypes;
        }
        string trackJSON = JsonUtility.ToJson(track);
        string savePathName = Path.Combine(savePath, audioPlayer.songName) + ".beattrack";
        File.WriteAllText(savePathName, trackJSON);
        Debug.Log("Successfully saved at " + savePathName);
    }

    private beatTrack loadTrack(string songname)
    {
        string jsonText = File.ReadAllText(savePath + songname + ".beattrack");
        return JsonUtility.FromJson<beatTrack>(jsonText);
    }

    [Serializable]
    public class beatTrack
    {
        public int length;
        public int[] beats; // recorded in samples
        public ushort[] beatTypes;

        public beatTrack(int length, int[] beats, ushort[] beatTypes)
        {
            this.length = length;
            this.beats = beats;
            this.beatTypes = beatTypes;
        }
    }
        static int sampleSize = 20000; // ---------------------------------------------------------------------- CHANGE IN MLHANDLER IF CHANGED HERE



    // save beatTrack method here
    private void saveTrackAsTSV(beatTrack track, string songname, bool withBeatTypes)
    {
        Debug.Log("started saving TSV");
        string audioPath = Path.Combine(Application.streamingAssetsPath, "Music/" + songname + ".mp3");
        string tsvPath = Path.Combine(Application.streamingAssetsPath, "TSVs/" + (withBeatTypes ? "WithBeatTypes/" : "NoBeatTypes/") + songname + ".tsv");


        // temp to do while testing ------------------------------------------------------------------------- DELETE IN THE FUTURE
        if (File.Exists(tsvPath))
        {
            File.Delete(tsvPath);
            Debug.Log("Deleted old tsv");
        }

        var afr = new AudioFileReader(audioPath);


        //int sampleRate = afr.WaveFormat.SampleRate;
        //int bytesPerSample = afr.WaveFormat.BitsPerSample / 8;
        //int sampleCount = (int)(afr.Length / bytesPerSample);
        //int channelCount = afr.WaveFormat.Channels;
        List<float> audio;
        float[] monoBuffer;
        float[] stereoBuffer;
        //int samplesRead = 0;
        //while ((samplesRead = afr.Read(buffer, 0, buffer.Length)) > 0)
        //    audio.AddRange(buffer.Take(samplesRead).Select(x => x * 16_000));
        //return (audio.ToArray(), sampleRate);

        if (!withBeatTypes) track = generateTrainingBlanks(track); 

        FileStream fs = File.Create(tsvPath);

        //AddText(fs,"Label\tBeat Samples");

        for (int i = 0; i < track.length; i++)
        {
            audio = new List<float>(sampleSize);
            monoBuffer = new float[sampleSize];
            stereoBuffer = new float[sampleSize*2];

            // read a number of samples with NAudio, write to file
            //string sampleValsString = "" + track.beatTypes[i];
            string sampleValsString = withBeatTypes ? 
                ("" + (track.beatTypes[i] == (ushort)beatType.BASS ? "true" : "false")) :
                ("" + (track.beatTypes[i] == 0 ? "false" : "true"));

            float[] Vals = new float[sampleSize];
            //Debug.Log(track.beats[i]);
            afr.Position = track.beats[i]-(sampleSize/2);
            //afr.Read(Vals, 0, Vals.Length);
            afr.Read(stereoBuffer, 0, stereoBuffer.Length);
            for (int j = 0; j < monoBuffer.Length; j++)
                monoBuffer[j] = stereoBuffer[j * 2];
            audio.AddRange(monoBuffer.Take(sampleSize).Select(x => x * 16_000));
            Vals = audio.ToArray();
            foreach (float value in Vals)
                sampleValsString += "\t" + value;
            if (i != track.length - 1)
                sampleValsString += "\n";
            AddText(fs, sampleValsString);
        }

        fs.Close();

        Debug.Log("TSV saving finished at " + tsvPath);

    }

    private static void AddText(FileStream fs, string input)
    {
        byte[] info = new UTF8Encoding(true).GetBytes(input);
        fs.Write(info, 0, info.Length);
    }

    // -------------------------------- FOR REFERENCE
    //private (double[] audio, int sampleRate) ReadFullMono(string filePath, double multiplier = 16_000)
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

    private beatTrack generateTrainingBlanks(beatTrack inputTrack)
    {
        const int minBeatGap = 20000;

        System.Random rand = new System.Random();

        List<int> beats = new List<int>();

        if (inputTrack.beats[0] > 20000)
            beats.Add(rand.Next((sampleSize/2), inputTrack.beats[0]-(sampleSize / 2)));
        beats.Add(inputTrack.beats[0]);
        for (int i = 1; i < inputTrack.length-1; i++)
        {
            beats.Add(inputTrack.beats[i]);
            if (inputTrack.beats[i + 1] - inputTrack.beats[i] > minBeatGap)
                beats.Add(rand.Next(inputTrack.beats[i] + (sampleSize / 2), inputTrack.beats[i+1] - (sampleSize / 2)));
        }
        beats.Add(inputTrack.beats[inputTrack.length-1]);

        int[] retBeats = beats.ToArray();
        int length = retBeats.Length;
        ushort[] retBeatTypes = new ushort[length];
        int nextOriginalBeat = 0;
        for (int i = 0; i < length; i++)
        {
            if (nextOriginalBeat < inputTrack.length && retBeats[i] == inputTrack.beats[nextOriginalBeat])
            {
                //Debug.Log("next oriignal beat: " + nextOriginalBeat);
                retBeatTypes[i] = inputTrack.beatTypes[nextOriginalBeat++];
            }
            else
                retBeatTypes[i] = (ushort)BeatMapper.beatType.NONE;
        }
        return new beatTrack(length, retBeats, retBeatTypes);
    }

    private static readonly int genTrackSampleDist = 44100 / 16;
    private static readonly float confidencePercentageMinimum = 1.0f;
    private static readonly int minBeatDist = 44100 / 8;
    private static readonly int closeBeatDist = 44100 / 10; // possibly change later
    private static readonly float closeBeatConfidencePercentage = 2.0f;

    private static beatTrack generateBeatTrack(string songName)
    {
        string audioPath = Path.Combine(Application.streamingAssetsPath, "Music/" + songName + ".mp3");
        //string saveTrackPath = Path.Combine(Application.streamingAssetsPath, "Music/" + songName + ".beattrack");

        //AudioFileReader afr = new AudioFileReader(audioPath);

        //float[] audioFile = new float[afr.Length / 8];
        //float[] audioStereoFileBuffer = new float[afr.Length / 4];

        //afr.Read(audioStereoFileBuffer, 0, audioStereoFileBuffer.Length);
        //for (int j = 0; j < audioFile.Length; j++)
        //    audioFile[j] = audioStereoFileBuffer[j * 2];

        //audioStereoFileBuffer = new float[0]; // to clear memory??
        float[] sampleScores = new float[((new AudioFileReader(audioPath).Length/8) - sampleSize) / genTrackSampleDist];

        //for (int currentTestSample = sampleSize / 2; currentTestSample + (sampleSize/2) < audioFile.Length; currentTestSample += genTrackSampleDist )
        int confidenceSum = 0;
        for (int i = 0; i < sampleScores.Length; i++)
        {
            int currentTestSample = sampleSize / 2 + (i * genTrackSampleDist);
            sampleScores[i] = MLHandler.predictIsBeat(currentTestSample, songName);
            confidenceSum += (int)Math.Abs(sampleScores[i]); // ------------------------------- it its found out that score should not be abs, change here
            //confidenceSum += (int)sampleScores[i];
        }
        confidenceSum /= sampleScores.Length;

        List<int> generatedBeats = new List<int>((int)(sampleScores.Length * confidencePercentageMinimum));

        // going through the beats and getting ones beating the score
        for (int i = 0; i < sampleScores.Length; i++)
        {
            if (sampleScores[i] > confidenceSum*confidencePercentageMinimum)
            {
                generatedBeats.Add(sampleSize / 2 + (i * genTrackSampleDist));
            }
        }

        // deleting ones from the list too close to each other
        for (int i = generatedBeats.Count-1; i > 0; i--)
        {
            if (generatedBeats[i] - generatedBeats[i-1] < closeBeatDist)
            {
                int currentBeatScoreIndex = (generatedBeats[i] - (sampleSize / 2)) / sampleSize; // i think this is the right math????
                int earlierBeatScoreIndex = (generatedBeats[i-1] - (sampleSize / 2)) / sampleSize;
                if (generatedBeats[i] - generatedBeats[i - 1] > minBeatDist &&
                    Math.Abs(sampleScores[currentBeatScoreIndex]) > confidenceSum * closeBeatConfidencePercentage &&
                    Math.Abs(sampleScores[earlierBeatScoreIndex]) > confidenceSum * closeBeatConfidencePercentage) // -------------------- and here too
                    //sampleScores[currentBeatScoreIndex] > confidenceSum * closeBeatConfidencePercentage &&
                    //sampleScores[earlierBeatScoreIndex] > confidenceSum * closeBeatConfidencePercentage)
                {
                        continue;
                }
                else if (Math.Abs(sampleScores[currentBeatScoreIndex]) > Math.Abs(sampleScores[earlierBeatScoreIndex])) // ------------ oh and here as well
                //else if (sampleScores[currentBeatScoreIndex] > sampleScores[earlierBeatScoreIndex])

                {
                    generatedBeats.RemoveAt(i);
                } else
                {
                    generatedBeats.RemoveAt(i-1);
                }
            }
        }
        int[] beats = generatedBeats.ToArray();
        generatedBeats.Clear();

        // submitting those values to see if its a melody or bass
        ushort[] beatTypes = new ushort[beats.Length];
        for (int i = 0; i < beatTypes.Length; i++)
        {
            //beatTypes[i] = (ushort)beatType.BASS; // fix with beat classification model soon
            beatTypes[i] = (ushort)(MLHandler.predictIsBass(beats[i], songName) < 0 ? beatType.BASS: beatType.MELODY);
        }

        // turning it all into a beattrack

        return new beatTrack(beats.Length, beats, beatTypes);
    }

    public static void genAndSaveTrack(string savpath, string songgame)
    {
        string trackJSON = JsonUtility.ToJson(generateBeatTrack(songgame));
        string savePathName = Path.Combine(savpath, songgame) + ".beattrack";
        File.WriteAllText(savePathName, trackJSON);
        Debug.Log("Successfully saved at " + savePathName);
    }

}
