using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratedTree : MonoBehaviour
{
    public GameObject[] treeModelPrefabs;
    public Material wood;
    public Material[] leafMats;
    private GameObject model;
    private static readonly System.Random rand = new System.Random();

    private static readonly float avgTreeScale = 1f;
    private static readonly float maxTreeScaleDiff = 1f;

    void Start()
    {
        model = Instantiate<GameObject>(treeModelPrefabs[rand.Next(0,treeModelPrefabs.Length)],
            this.transform.position,
            Quaternion.Euler(0, rand.Next(0,360), 0),
            transform);
        model.GetComponent<MeshRenderer>().material = wood;
        Material leafMat = leafMats[rand.Next(0, leafMats.Length)]; // ------------------------ do something to weight the leaf colors
        for (int i = 0; i < model.transform.childCount; i++)
            model.transform.GetChild(i).GetComponent<MeshRenderer>().material = leafMat;
        float newscale = avgTreeScale + (float)(rand.NextDouble() * maxTreeScaleDiff);
        model.transform.localScale = new Vector3(newscale, newscale, newscale);

        ObjectFactory parentFactory = gameObject.GetComponentInParent<ObjectFactory>();
        parentFactory.objWidth = parentFactory.originalObjWidth * newscale;
    }

    void Update()
    {
        // do something here to spawn lwaves
    }

}
