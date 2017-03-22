using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Gives the type of tiles
/// </summary>
public enum Tile
{
    WALL,
    FLOOR,
    CORRIDOR
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
    public static int MIN_ROOM_HALFSIZE = 2;


    /// <summary>
    /// The max half size of a room
    /// </summary>
    public static int MAX_ROOM_HALFSIZE = 12;

    /// <summary>
    /// Describes the default size of the map
    /// </summary>
    private static int DEFAULT_SIZE = 96;

    /// <summary>
    /// The quadtree
    /// </summary>
    private Quadtree _tree;

    /// <summary>
    /// Describe the state of the tree
    /// </summary>
    private bool _treeIsInit = false;

    /// <summary>
    /// The color of the corridor
    /// </summary>
    private Color _corridorColor = Color.blue;

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
        _tiles = new Tile[boundaries.Size().x, boundaries.Size().y];
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


    #region Quadtree functions
    /// <summary>
    /// Is the tree initialized
    /// </summary>
    /// <returns>Wether the tree is initialized</returns>
    public bool TreeIsInit()
    {
        return _treeIsInit;
    }

    /// <summary>
    /// Initializes the tree
    /// </summary>
    public void InitQuadtree()
    {
        _tree = new Quadtree(_boundaries);
        _tree.BuildTree(1, 4);
        _tree.AddRoom();
        _treeIsInit = true;
    }

    /// <summary>
    /// Draws the limits of the tree
    /// </summary>
    public void DrawTreeLimits()
    {
        _tree.DrawLimits(_texture);
    }

    /// <summary>
    /// Draws the rooms of the tree
    /// </summary>
    public void DrawTreeRooms()
    {
        _tree.DrawRoom(_texture);
    }
    #endregion

    #region Tiles functions
    /// <summary>
    /// Prints the tile to the texture
    /// </summary>
    public void TilesToTexture()
    {
        XY size = Boundaries.Size();
        for (int i = 0; i < size.x; ++i)
            for (int j = 0; j < size.x; ++j)
            {
                Tile t = _tiles[i, j];
                if (t == Tile.WALL)
                    _texture.SetPixel(i, j, Color.black);
                else if (t == Tile.FLOOR)
                    _texture.SetPixel(i, j, Color.white);
                else
                    _texture.SetPixel(i, j, _corridorColor);
            }
        _texture.Apply();
    }

    /// <summary>
    /// Adds a room to the tile array
    /// </summary>
    /// <param name="room">The room to add</param>
    public void AddRoomToTiles(Room room)
    {
        for (int i = room.Left(); i < room.Right(); ++i)
            for (int j = room.Bottom(); j < room.Top(); ++j)
                _tiles[i, j] = Tile.FLOOR;
    }

    /// <summary>
    /// Change the point at x y to tile in _tiles and to color in _texture
    /// </summary>
    /// <param name="x">The position x</param>
    /// <param name="y">The position y</param>
    /// <param name="tile">The new tile value</param>
    /// <param name="color">The new texture color</param>
    public void AddToTilesAndTexture(int x, int y, Tile tile, Color color)
    {
        _tiles[x, y] = tile;
        _texture.SetPixel(x, y, color);
        _texture.Apply();
    }

    /// <summary>
    /// Links the rooms inside the tree
    /// </summary>
    public void LinkRooms()
    {
        _tree.LinkRooms("");
    }
    #endregion

    #region Corridors

    /// <summary>
    /// Adds a corridor at the point x y
    /// </summary>
    /// <param name="x">the position in x</param>
    /// <param name="y">the position in y</param>
    /// <param name="vertical">The movement is vertical</param>
    /// <param name="first">It is the first of the line</param>
    /// <returns></returns>
    public bool AddCorridor(int x, int y, bool vertical, bool first)
    {
        bool canAdd = true;
        // if we can add a corridor
        if (_tiles[x, y] == Tile.WALL)
        {
            if (vertical)
            {
                // If the movement is vertical, we check if it has corridor right and left to it
                if (x - 1 <= 0 || _tiles[x - 1, y] == Tile.CORRIDOR || x + 1 >= Boundaries.Size().x || _tiles[x + 1, y] == Tile.CORRIDOR)
                    canAdd = false;       
            }
            else
            {
                // Else we check if it has corridor up and down to it
                if (y - 1 <= 0 || _tiles[x, y - 1] == Tile.CORRIDOR || y + 1 >= Boundaries.Size().y || _tiles[x, y + 1] == Tile.CORRIDOR)
                    canAdd = false;
            }
            // If this is the first corridor of the line, or if we are allowed to, we draw
            if (first || canAdd)
                AddToTilesAndTexture(x, y, Tile.CORRIDOR, _corridorColor);
            // Return false if we found a corridor next. Then first will be false
            return canAdd;
        }
        return canAdd;
    }


    /// <summary>
    /// Creates a corridor between two rooms
    /// </summary>
    /// <param name="a">First Room</param>
    /// <param name="b">Second Room</param>
    public void CreateCorridorBetweenRooms(Room a, Room b)
    {
        // We can't link if there is 0 or 1 room
        if (a == null || b == null)
            return;

        //Take the min and max x and y
        int minx = Mathf.Min(a.center.x, b.center.x);
        int maxx = Mathf.Max(a.center.x, b.center.x);

        int miny = Mathf.Min(a.center.y, b.center.y);
        int maxy = Mathf.Max(a.center.y, b.center.y);

        bool first = false;
        // Draw a line from min to max x, at b.center.y
        for (int i = minx; i <= maxx; ++i)
            first = AddCorridor(i, b.center.y, false, first);

        first = false;
        // Draw a line from min to max y, at a.center.x
        for (int j = miny; j <= maxy; j++)
           first = AddCorridor(a.center.x, j, true, first);
    }
    #endregion
}