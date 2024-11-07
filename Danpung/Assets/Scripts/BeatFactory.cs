using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatFactory : MonoBehaviour
{

    public GameObject beatPrefab;
    public BeatHitter beatHitter;

    public const int preBeatLoad = 44100 * 3;

    private AudioSource audioSource;

    private bool initialized = false;
    private int nextBeat = -1;
    private int nextBeatIndex = -1;
    private BeatMapper.beatTrack beatTrack;

    public GamDirector director;

    void Update()
    {
        if (initialized)
        {
            if (nextBeatIndex != -1 && audioSource != null && audioSource.isPlaying)
            {
                if (beatTrack == null)
                {
                    Debug.LogError("beatFactory track is null");
                }
                else
                {
                    if (audioSource.timeSamples + preBeatLoad >= nextBeat)
                    {
                        CreateBeat(nextBeatIndex);
                        nextBeatIndex++;
                        if (nextBeatIndex == beatTrack.length)
                            nextBeatIndex = -1;
                        else
                            nextBeat = beatTrack.beats[nextBeatIndex];
                        //Debug.Log("next beat index is " + nextBeatIndex);

                    }
                }
            }
        }
        
    }

    public void Initialize(AudioSource audioSource, BeatMapper.beatTrack beatTrack)
    {
        this.audioSource = audioSource;
        this.beatTrack = beatTrack;
        nextBeatIndex = (beatTrack.length > 0) ? 0 : -1;
        nextBeat = (nextBeatIndex > -1) ? beatTrack.beats[0] : -1;
        //if (nextBeat != -1)
        //    Debug.Log("officially loaded track, first beat at sample " + beatTrack.beats[0]);
        while (nextBeatIndex != -1 && beatTrack.beats[nextBeatIndex] < audioSource.timeSamples + preBeatLoad)
        {
            CreateBeat(nextBeatIndex);
            nextBeatIndex++;
            if (nextBeatIndex == beatTrack.length)
                nextBeatIndex = -1;
            else
                nextBeat = beatTrack.beats[nextBeatIndex];
        }

        director.numSongBeats = beatTrack.length;

        initialized = true;
    }

    private void CreateBeat(int index)
    {
        if (beatTrack.beatTypes[index] == (ushort)BeatMapper.beatType.NONE) return;
        GameObject newBeat = Instantiate<GameObject>(beatPrefab, new Vector3(), Quaternion.Euler(0, 0, 0), this.transform);
        newBeat.GetComponent<Beat>().Setup(audioSource, beatTrack.beats[index], (BeatMapper.beatType)beatTrack.beatTypes[index], director);
        beatHitter.addBeats(newBeat);
    }
}
