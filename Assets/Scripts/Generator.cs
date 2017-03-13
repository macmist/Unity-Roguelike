using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour {
    public GameObject plane;
    private Dungeon _dungeon;
	// Use this for initialization
	void Start () {
        _dungeon = Dungeon.GetInstance();
        Material material = new Material(Shader.Find("Diffuse"));
        material.mainTexture = _dungeon.Texture;

        plane.GetComponent<Renderer>().material = material;

        _dungeon.Boundaries.Draw(_dungeon.Texture);
    }
	
	// Update is called once per frame
	void Update () {

	}
}
