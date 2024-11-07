using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GeneratedLeaf : MonoBehaviour
{
    public GameObject[] leafModelPrefabs;
    public Material[] roundLeafMats;
    public Material[] shapedLeafMats;
    private GameObject model;
    private static readonly System.Random rand = new System.Random();

    private bool isRound;

    //private static readonly float avgTreeScale = 1f;
    //private static readonly float maxTreeScaleDiff = 1f;

    private static readonly float randLeafMaxSpace = 3f;
    //private static readonly float randLeafZ = 10f;

    void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        isRound = rand.Next(0, 2) == 0;
        model = Instantiate<GameObject>(leafModelPrefabs[isRound ? 0 : 1],
            this.transform.position,
            Quaternion.Euler(rand.Next(0,2) == 0 ? 0 : 180, rand.Next(0, 360), 0),
            transform);
        model.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        Material[] leafMats = isRound ? roundLeafMats : shapedLeafMats;
        Material leafMat = leafMats[rand.Next(0, leafMats.Length)]; // ------------------------ do something to weight the leaf colors
        model.GetComponent<MeshRenderer>().material = leafMat;
        //float newscale = avgTreeScale + (float)(rand.NextDouble() * maxTreeScaleDiff);
        //model.transform.localScale = new Vector3(newscale, newscale, newscale);

        ObjectFactory parentFactory = gameObject.GetComponentInParent<ObjectFactory>();
        parentFactory.objWidth = parentFactory.originalObjWidth + (float)rand.NextDouble()*randLeafMaxSpace;
    }

    void Update()
    {
        // do something here to spawn lwaves
    }

}
