using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class BeatHitter : MonoBehaviour
{

    private float originalScale = 6f;
    public float biggerConst;
    private float shrinkSpeed = 2f;

    private bool MnormalSize = true;
    private bool BnormalSize = true;
    private Transform mTransform;
    private Transform bTransform;

    // add note detection here
    private List<GameObject> activeBeats = new List<GameObject>();

    void Start()
    {
        shrinkSpeed *= originalScale;

        //activeBeats = new List<GameObject>();

        mTransform = this.gameObject.transform.GetChild(0);
        bTransform = this.gameObject.transform.GetChild(1);
    }

    void Update()
    {
        if (!MnormalSize)
        {
            float newSize = mTransform.localScale.x - (shrinkSpeed * Time.deltaTime);
            if (newSize <= originalScale)
            {
                newSize = originalScale;
                MnormalSize = true;
            }
            mTransform.localScale = new Vector3(newSize, newSize, 1f);
        }
        if (!BnormalSize)
        {
            float newSize = bTransform.localScale.x - (shrinkSpeed * Time.deltaTime);
            if (newSize <= originalScale)
            {
                newSize = originalScale;
                BnormalSize = true;
            }
            bTransform.localScale = new Vector3(newSize, newSize, 1f);
        }

        if (activeBeats.Count > 0 && activeBeats[0].GetComponent<Beat>().isExpired())
        {
            // do something with score / lives here
            Destroy(activeBeats[0]);
            activeBeats.RemoveAt(0);
            //Debug.Log("destroyes one");
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            BeatMapper.beatType hitType;
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                hitType = BeatMapper.beatType.MELODY;
                MnormalSize = false;
                mTransform.localScale = new Vector3(biggerConst, biggerConst, 1f);
            }
            else
            {
                hitType = BeatMapper.beatType.BASS;
                BnormalSize = false;
                bTransform.localScale = new Vector3(biggerConst, biggerConst, 1f);
            }
            if (activeBeats.Count > 0)
            {
                foreach (GameObject obj in activeBeats)
                {
                    if (!obj.GetComponent<Beat>().hasBeenHit())
                    {
#pragma warning disable CS0642 // Possible mistaken empty statement
                        if (obj.GetComponent<Beat>().hitBeat(hitType) != Beat.hitType.MISSED);
#pragma warning restore CS0642 // Possible mistaken empty statement
                            break;
                    }
                }
                
            }
        }
    }

    public void addBeats(GameObject beat)
    {
        activeBeats.Add(beat);
    }
}
