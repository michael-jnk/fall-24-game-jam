using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    void Start()
    {
        transform.rotation =  Quaternion.Euler(transform.rotation.x, new System.Random().Next(0,4) * 90, transform.rotation.z);
    }
}
