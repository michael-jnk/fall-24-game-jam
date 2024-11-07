using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    private Vector3 velocity;
    private GamDirector director;

    private float minX;
    private float minZ;

    public void Init(Vector3 velocity, GamDirector director, float minX = -30f, float minZ = -10f)
    {
        this.velocity = velocity;
        this.director = director;
        this.minX = minX;
        this.minZ = minZ;
    }

    void Update()
    {
        //if (!isPlaying) {
        //    if (playackSpd == 0)
        //        return;
        //    playackSpd -= Time.deltaTime;
        //    if (playackSpd < 0.05f)
        //    {
        //        playackSpd = 0;
        //        return;
        //    }
        //}

        //if (playackSpd < 1.0f)
        //    playackSpd += Time.deltaTime;
        //if (playackSpd < 0.95f)
        //    playackSpd = 1;

        transform.position = new Vector3(transform.position.x + (velocity.x*director.playackSpd*Time.deltaTime), transform.position.y + (velocity.y * director.playackSpd * Time.deltaTime), transform.position.z + (velocity.z * director.playackSpd * Time.deltaTime));

        if (transform.position.x < minX || transform.position.z < minZ)
            Destroy(this.gameObject);

    }
}
