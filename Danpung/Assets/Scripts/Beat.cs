using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.XR;

public class Beat : MonoBehaviour
{
    private AudioSource audioSource;
    private int sample;
    private BeatMapper.beatType type;
    private hitType hit = hitType.NOTHIT;
    private bool expired = false;

    private GamDirector director;

    private Light lightEmitter;

    private static readonly float circleX = -7.99f;
    private static readonly float rightestX = 10.89f;

    private static readonly float melodyLoadY = -2.5f;
    private static readonly float bassLoadY = -3.5f;

    private static readonly int preBeatLife = BeatFactory.preBeatLoad;
    private static readonly int postBeatLife = 44100;

    private static readonly int badPrePeriod = 44100 / 2;
    private static readonly int okPeriod = 44100 / 4;
    private static readonly int goodPeriod = 44100 / 8;

    private static readonly Color invis = new Color(0,0,0,0);

    public Material[] mats; // good, ok, bad, miss
    private MeshRenderer mesh;

    void Start()
    {

    }

    void Update()
    {
        // instantiate at circle location + (set offscreen distance - circle location) * (samples til play/prebeatload)
        this.transform.localPosition = new Vector3(
            circleX + ((rightestX - circleX) * ((float)(sample - audioSource.timeSamples) / (float)preBeatLife)),
            this.transform.localPosition.y);

        //debugChildTiming.color = (Math.Abs(audioSource.timeSamples - sample) < goodPeriod) ? Color.green :
        //    (Math.Abs(audioSource.timeSamples - sample) < okPeriod) ? Color.yellow :
        //    (audioSource.timeSamples - sample > badPrePeriod) ? Color.red :
        //    invis;

        // do some checking to see if the players hit the note yet

        if (audioSource.timeSamples - sample > postBeatLife)
        {
            expired = true;
            // I'll let the beat hitter resolve destroying the beats since it has its own array of the beats to manage
        } else if (hit == hitType.NOTHIT && audioSource.timeSamples - sample > okPeriod)
        {
            hit = hitType.MISSED;
            director.misses++;
            //lightEmitter.color = Color.gray;
            mesh.material = mats[3];
        }
    }

    public void Setup(AudioSource audioSource, int sample, BeatMapper.beatType type, GamDirector director)
    {
        this.audioSource = audioSource;
        this.sample = sample;
        this.type = type;
        this.director = director;
        // instantiate at circle location + (set offscreen distance - circle location) * (samples til play/prebeatload)
        this.transform.localPosition = new Vector3(rightestX,(type == BeatMapper.beatType.MELODY) ? melodyLoadY : bassLoadY,0);
        this.lightEmitter = GetComponent<Light>();
        //Debug.Log("beat made with sample " + sample);
        this.expired = false;

        //debugChildTiming = this.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
        mesh = transform.GetChild(0).GetComponent<MeshRenderer>();
    }

    public enum hitType : ushort
    {
        NOTHIT,
        MISSED,
        BAD,
        OK,
        GOOD,
    }

    public bool hasBeenHit()
    {
        return hit != hitType.NOTHIT;
    }

    public hitType hitBeat(BeatMapper.beatType type)
    {
        if (hasBeenHit())
            return hit;

        hit = (Math.Abs(audioSource.timeSamples - sample) < goodPeriod) ? (type == this.type ? hitType.GOOD : hitType.BAD) :
            (Math.Abs(audioSource.timeSamples - sample) < okPeriod) ? (type == this.type ? hitType.OK : hitType.BAD) :
            (audioSource.timeSamples - sample > badPrePeriod) ? hitType.BAD :
            hitType.NOTHIT;
        //  I finally add effects into the game, set this beat to decay if expired, dissipate red if failed, glow yellow if ok???, glow sparkle dissipate green if good
        //string outMsg = "hit made, result: ";
        switch (hit)
        {
            case hitType.NOTHIT:
                return hitType.NOTHIT;
            case hitType.BAD:
                //outMsg += "BAD";
                lightEmitter.color = Color.red;
                mesh.material = mats[2];
                director.badHits++;
                break;
            case hitType.OK:
                //outMsg += "OK";
                director.okHits++;
                lightEmitter.color = Color.yellow;
                mesh.material = mats[1];
                break;
            case hitType.GOOD:
                //outMsg += "GOOD";
                director.goodHits++;
                lightEmitter.color = Color.green;
                mesh.material = mats[0];
                break;

        }
                lightEmitter.intensity = 0.5f;
        //Debug.Log(outMsg);
        return hit;
    }

    public bool isExpired()
    {
        return expired;
    }

}
