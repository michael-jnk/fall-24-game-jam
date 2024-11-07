using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using UnityEngine;

public class ObjectFactory : MonoBehaviour
{
    public GamDirector director;
    public GameObject prefab;
    public float originalObjWidth;
    public float objWidth;
    public Vector3 objSpeed;
    public float xStart = 40f;
    public float xEnd = -40f;

    private GameObject lastSent;

    private bool initialized = false;

    public bool randomWidths = false;
    public float randomWidthVariance = 0f;
    public float randomZ = 0f;
    private static readonly System.Random rand = new System.Random();

    public void Start()
    {
        originalObjWidth = objWidth;
        lastSent = Instantiate<GameObject>(prefab, new Vector3(xEnd + objWidth, transform.position.y, transform.position.z), Quaternion.Euler(0, -90, 0), transform);
        lastSent.GetComponent<MovingObject>().Init(objSpeed, director, xEnd);
        // add something here to link each sidewalk to the main game controller
        while (lastSent.transform.position.x < xStart)
        {
            float randSpace = (randomWidths) ? (float)(rand.NextDouble()*randomWidthVariance) : 0f;
            float randZ = (float)(rand.NextDouble() * randomZ) - (randomZ / 2);
            lastSent = Instantiate<GameObject>(prefab, new Vector3(lastSent.transform.position.x + objWidth + randSpace, transform.position.y, transform.position.z + randZ), Quaternion.Euler(0, -90, 0), transform);
            lastSent.GetComponent<MovingObject>().Init(objSpeed, director, xEnd);
        }
        initialized = true;
    }

    void Update()
    {
        if (!initialized) return;
        if (lastSent == null || lastSent.transform.position.x < xStart)
        {
            float randSpace = (randomWidths) ? (float)(rand.NextDouble() * randomWidthVariance) : 0f;
            float randZ = (float)(rand.NextDouble() * randomZ) - (randomZ / 2);
            lastSent = Instantiate<GameObject>(prefab, new Vector3(lastSent.transform.position.x + objWidth + randSpace, transform.position.y, transform.position.z + randZ), Quaternion.Euler(0, -90, 0), transform);
            lastSent.GetComponent<MovingObject>().Init(objSpeed, director, xEnd);
        }
    }
}
