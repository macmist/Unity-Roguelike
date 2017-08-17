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


    public void To3D()
    {
        GameObject floorPrefab = Resources.Load("Prefabs/FloorPrefab") as GameObject;
        GameObject wallPrefab = Resources.Load("Prefabs/WallPrefab") as GameObject;

        GameObject dungeon = new GameObject("Dungeon");
        dungeon.transform.position = Vector3.zero;

        GameObject walls = new GameObject("Walls");
        walls.transform.parent = dungeon.transform;

        GameObject floors = new GameObject("Floors");
        floors.transform.parent = dungeon.transform;

        float yWall = wallPrefab.GetComponent<Renderer>().bounds.size.y / 2  - floorPrefab.GetComponent<Renderer>().bounds.size.y / 2;

        Tile[,] tiles = _dungeon.Tiles;


        for (int i = 0; i < tiles.GetLength(0); ++i)
            for (int j = 0; j < tiles.GetLength(1); ++j)
            {
                switch (tiles[i, j])
                {
                    case Tile.WALL:
                        CreatePrefab(i * wallPrefab.transform.localScale.x, yWall, j * wallPrefab.transform.localScale.z, wallPrefab, walls);
                        break;
                    case Tile.FLOOR:
                        CreatePrefab(i * floorPrefab.transform.localScale.x, 0, j * floorPrefab.transform.localScale.z, floorPrefab, floors);
                        break;
                    case Tile.CORRIDOR:
                        CreatePrefab(i * floorPrefab.transform.localScale.x, 0, j * floorPrefab.transform.localScale.z, floorPrefab, floors);
                        break;
                    default: break;
                }
            }
    }


    public void CreatePrefab(float x, float y, float z, GameObject prefab, GameObject parent)
    {
        Vector3 position = new Vector3(x, y, z);
        GameObject go = Instantiate(prefab, position, Quaternion.identity);
        go.transform.parent = parent.transform;
    }
}
