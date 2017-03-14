using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Gives the type of tiles
/// </summary>
enum Tile
{
    WALL,
    FLOOR
}

/// <summary>
/// The main class for the dungeon. It is a singleton as we only want one instance.
/// Contains all that describes the inside of the dungeon.
/// </summary>
public class Dungeon  {
    /// <summary>
    /// The instance of the dungeon
    /// </summary>
    private static Dungeon instance;

    /// <summary>
    /// The texture on which we'll draw everything
    /// </summary>
    private Texture2D _texture;

    /// <summary>
    /// Array of Tiles on which we'll do all the logic
    /// </summary>
    private Tile[,] _tiles;

    /// <summary>
    /// The limits of the map
    /// </summary>
    private AABB _boundaries;

    /// <summary>
    /// The list of all the created rooms
    /// </summary>
    private List<Room> _rooms;

    /// <summary>
    /// Describes the minimal half size of a room
    /// </summary>
    public static int MIN_ROOM_HALFSIZE = 1;

    /// <summary>
    /// Describes the default size of the map
    /// </summary>
    private static int DEFAULT_SIZE = 100;


    /// <summary>
    /// Default constructor, creates a 100*100 dungeon with its center at (50,50)
    /// </summary>
    private Dungeon() : this(new AABB(new XY(DEFAULT_SIZE / 2, DEFAULT_SIZE / 2), new XY(DEFAULT_SIZE / 2, DEFAULT_SIZE / 2)))
    {

    }

    /// <summary>
    /// Other constructor, creates the dungeon with a custom boundary, and initializes the texture
    /// </summary>
    /// <param name="boundaries">Describe the limits of the map</param>
    private Dungeon(AABB boundaries)
    {
        _boundaries = boundaries;
        _texture = new Texture2D(boundaries.Size().x, boundaries.Size().y, TextureFormat.ARGB32, false);
        _texture.filterMode = FilterMode.Point;
        _texture.wrapMode = TextureWrapMode.Clamp;
    }

    /// <summary>
    /// Returns the instance of the dungeon, instantiate it first if needed
    /// </summary>
    /// <returns></returns>
    public static Dungeon GetInstance()
    {
        if (instance == null)
        {
            instance = new Dungeon();
        }
        return instance;
    }

    /// <summary>
    /// Same as before but with args
    /// </summary>
    /// <param name="box">The limits of the map to pass to the contructor</param>
    /// <returns></returns>
    public static Dungeon GetInstance(AABB box)
    {
        if (instance == null)
        {
            instance = new Dungeon(box);
        }
        return instance;
    }

    /// <summary>
    /// Getter Setter for the texture
    /// </summary>
    public Texture2D Texture
    {
        get { return _texture; }
        set { _texture = value; }
    }

    /// <summary>
    /// Getter Setter for the boundaries
    /// </summary>
    public AABB Boundaries
    {
        get { return _boundaries; }
    }
}