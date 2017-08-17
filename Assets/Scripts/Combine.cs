using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Combine : MonoBehaviour
{
    void Start()
    {
        Material m;
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        List<MeshFilter> meshfilter = new List<MeshFilter>();
        for (int i = 1; i < meshFilters.Length; i++)
            meshfilter.Add(meshFilters[i]);
        CombineInstance[] combine = new CombineInstance[meshfilter.Count];
        int np = 0;
        while (np < meshfilter.Count)
        {
            m = meshFilters[np].GetComponent<Renderer>().material;
            gameObject.GetComponent<Renderer>().material = m;

            combine[np].mesh = meshfilter[np].sharedMesh;
            combine[np].transform = meshfilter[np].transform.localToWorldMatrix;
            meshfilter[np].gameObject.SetActive(false);
            np++;
        }
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true, true);
        gameObject.AddComponent<MeshCollider>();
        transform.GetComponent<MeshCollider>().sharedMesh = transform.GetComponent<MeshFilter>().mesh;
        transform.gameObject.SetActive(true);
    }
}