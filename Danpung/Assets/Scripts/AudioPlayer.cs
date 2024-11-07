using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.Universal.Internal;

public class AudioPlayer : MonoBehaviour
{
    public GamDirector director;
    private AudioSource aSource;
    public string songName;
    private bool songNameSet;

    //private AudioPitchEstimator pitchEstimator;
                                                                                                                                            
    void Start()
    {
        aSource = GetComponent<AudioSource>();
        //pitchEstimator = GetComponent<AudioPitchEstimator>();

        StartCoroutine(WaitTilSongSet());

        //Uri songUri = new Uri(Path.GetFullPath(Path.Combine(Application.streamingAssetsPath, "Music/"+songName+".mp3"))); // add .AbsoluteUri to make it a string
    }

    public void setSongName(string name)
    {
        songName = name;
        songNameSet = true;
    }

    IEnumerator WaitTilSongSet()
    {
        while (!songNameSet) yield return null;
        StartCoroutine(GetPlayAudioClip(songName));
        GetComponent<BeatMapper>().Initialize();
    }

    IEnumerator GetPlayAudioClip(string sName)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(
            new Uri(Path.GetFullPath(Path.Combine(Application.streamingAssetsPath, ("Music/" + sName + ".mp3")))).AbsoluteUri
            , AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.isHttpError || www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                //Debug.Log("right direction???");
                //Debug.Log(aSource);
                aSource.clip = DownloadHandlerAudioClip.GetContent(www);
                StartCoroutine(waitForSongEnd());
                //aSource.PlayDelayed(1);
            }
        }
    }

    IEnumerator waitForSongEnd()
    {
        //float startTime = aSource.time;
        //Debug.Log($"clip length: {aSource.clip.length}, start time: {startTime}, asource time: {aSource.time}");
        while (!aSource.isPlaying)
            yield return null;
        while (aSource.isPlaying || !director.isPlaying)
            yield return null;
        director.EndGameScene();
    }

}
