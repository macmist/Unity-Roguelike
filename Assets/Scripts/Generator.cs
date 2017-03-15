using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A monobehaviour used to genereate the dungeon
/// </summary>
public class Generator : MonoBehaviour {
    /// <summary>
    /// The plane where we'll draw the texture
    /// </summary>
    public GameObject plane;

    /// <summary>
    /// The instance of the dungeon
    /// </summary>
    private Dungeon _dungeon;

    private Quadtree _tree;


    /// <summary>
    /// Creates the dungeon and initializes the plain material
    /// </summary>
	void Start () {
        _dungeon = Dungeon.GetInstance();
        Material material = new Material(Shader.Find("Diffuse"));
        material.mainTexture = _dungeon.Texture;

        plane.GetComponent<Renderer>().material = material;

        _dungeon.Boundaries.Draw(_dungeon.Texture);
        _tree = new Quadtree(_dungeon.Boundaries);
        _tree.BuildTree(1, 4);
        _tree.AddRoom();
    }

	void Update () {

	}

    public void DrawTreeLimits()
    {
        _tree.DrawLimits(_dungeon.Texture);
    }

    public void DrawTreeRooms()
    {
        _tree.DrawRoom(_dungeon.Texture);
    }
}
