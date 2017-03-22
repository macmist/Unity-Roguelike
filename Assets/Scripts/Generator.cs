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


    /// <summary>
    /// Creates the dungeon and initializes the plain material
    /// </summary>
	void Start () {
        _dungeon = Dungeon.GetInstance();
        Material material = new Material(Shader.Find("Diffuse"));
        material.mainTexture = _dungeon.Texture;

        plane.GetComponent<Renderer>().material = material;

        _dungeon.Boundaries.Draw(_dungeon.Texture);
        
    }

	void Update () {

	}

    /// <summary>
    /// Draws the limits of the tree
    /// </summary>
    public void DrawTreeLimits()
    {
        if (!_dungeon.TreeIsInit())
            _dungeon.InitQuadtree();
        _dungeon.DrawTreeLimits();
    }

    /// <summary>
    /// Draws the rooms of the tree
    /// </summary>
    public void DrawTreeRooms()
    {
        if (!_dungeon.TreeIsInit())
            _dungeon.InitQuadtree();
        _dungeon.DrawTreeRooms();
    }

    /// <summary>
    /// Draws the dungeon's tiles
    /// </summary>
    public void DrawTiles()
    {
        _dungeon.TilesToTexture();
    }

    /// <summary>
    /// Links the tree's rooms
    /// </summary>
    public void LinkRooms()
    {
        if (!_dungeon.TreeIsInit())
            _dungeon.InitQuadtree();
        _dungeon.LinkRooms();
    }
}
