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
    CORRIDOR,
    NONE
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

    public static int MIN_ROOM_MARGIN = 2;

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
    /// The AStar solver
    /// </summary>
    private AStar _astar;

    /// <summary>
    /// The maze generator
    /// </summary>
    private Maze _maze;

    /// <summary>
    /// If the maze has been constructed
    /// </summary>
    private bool _mazeConstructed = false;

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

    public Tile[,] Tiles
    {
        get { return _tiles; }
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

    /// <summary>
    /// Adds a room to the room list
    /// </summary>
    /// <param name="room">The room to add</param>
    public void AddRoomToList(Room room)
    {
        if (_rooms == null)
            _rooms = new List<Room>();
        _rooms.Add(room);
    }

    /// <summary>
    /// Returns the room list
    /// </summary>
    /// <returns>The room list</returns>
    public List<Room> GetRooms()
    {
        return _rooms;
    }
    /// <summary>
    /// Removes all useless corridors from the tree
    /// </summary>
    public void RemoveUselessCorridorsFromTree()
    {
        _tree.RemoveCorridor();
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
        if (room == null)
        {
            Debug.Log("OOps there is a null room");
            return;
        }
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
    public void AddCorridor(int x, int y)
    {
        if (_tiles[x, y] == Tile.WALL)
        {
             AddToTilesAndTexture(x, y, Tile.CORRIDOR, _corridorColor);
        }
    }

    /// <summary>
    /// Checks if the current point has corridor next
    /// </summary>
    /// <param name="x">The x position</param>
    /// <param name="y">The y position</param>
    /// <param name="checkVertically">If we check for neighbors vertically or not</param>
    /// <returns>If the point has corridors next</returns>
    public bool HasCorridorNext(int x, int y, bool checkVertically)
    {
        if (checkVertically)
            return ((y - 1 >= 0 && _tiles[x, y - 1] == Tile.CORRIDOR) || (y + 1 < _boundaries.Top() && _tiles[x, y + 1] == Tile.CORRIDOR));
        return ((x - 1 >= 0 && _tiles[x - 1, y] == Tile.CORRIDOR) || (x + 1 < _boundaries.Right() && _tiles[x + 1, y] == Tile.CORRIDOR));
    }

    /// <summary>
    /// Checks if the corridor has corridor next
    /// </summary>
    /// <param name="x">The x position</param>
    /// <param name="y">The y position</param>
    /// <returns>If the corridor has corridor next</returns>
    public bool HasCorridorAll(int x, int y)
    {
        return HasCorridorNext(x, y, true) || HasCorridorNext(x, y, false);
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

        if (_astar == null)
            _astar = new AStar(_tiles);

        Node path = _astar.DoAStar(a.center, b.center, a, b);

        while(path != null)
        {
            AddCorridor(path.Position.x, path.Position.y);
            path = path.Parent;
        }

        ////Take the min and max x and y
        //int minx = Mathf.Min(a.center.x, b.center.x);
        //int maxx = Mathf.Max(a.center.x, b.center.x);

        //int miny = Mathf.Min(a.center.y, b.center.y);
        //int maxy = Mathf.Max(a.center.y, b.center.y);

        //bool firstFound = false;
        //// Draw a line from min to max x, at b.center.y
        //for (int i = minx; i <= maxx; ++i)
        //{
        //    bool hasCorridorNext = HasCorridorNext(i, b.center.y, true);
        //    bool nextHasCorridorNext = HasCorridorNext(i + 1, b.center.y, true);
        //    if (i == minx && (hasCorridorNext || nextHasCorridorNext))
        //        firstFound = true;
        //    if (!hasCorridorNext || !firstFound || (hasCorridorNext && !nextHasCorridorNext))
        //        AddCorridor(i, b.center.y);
        //    if (i == maxx - 1 && nextHasCorridorNext)
        //        break;
        //    firstFound = nextHasCorridorNext;
        //}

        //firstFound = false;
        //// Draw a line from min to max y, at a.center.x
        //for (int j = miny; j <= maxy; j++)
        //{
        //    bool hasCorridorNext = HasCorridorNext(a.center.x, j, false);
        //    bool nextHasCorridorNext = HasCorridorNext(a.center.x, j + 1, false);
        //    if (j == miny && (hasCorridorNext || nextHasCorridorNext))
        //        firstFound = true;
        //    if (!hasCorridorNext || !firstFound || (hasCorridorNext && !nextHasCorridorNext))
        //        AddCorridor(a.center.x, j);
        //    if (j == maxy - 1 && nextHasCorridorNext)
        //        break;
        //    firstFound = nextHasCorridorNext;
        //}
        //RemoveUselessCorridors(a);
        //RemoveUselessCorridors(b);
    }

    /// <summary>
    /// Removes all useless corridors next to a room
    /// </summary>
    /// <param name="r">The room to remove corridors from</param>
    public void RemoveUselessCorridors(Room r)
    {
        if (r.Left() - 1 >= 0)
            RemoveCorridorLoop(r.Bottom() + 1, r.Top() - 1, r.Left() - 1, true);

        RemoveCorridorLoop(r.Bottom() + 1, r.Top() - 1, r.Right(), true);
        if (r.Bottom() - 1 >= 0)
            RemoveCorridorLoop(r.Left() + 1, r.Right() - 1, r.Bottom() - 1, false);

        RemoveCorridorLoop(r.Left() + 1, r.Right() - 1, r.Top(), false);

    }

    private void RemoveCorridorLoop(int min, int max, int fixedPos, bool verticalMovement)
    {
        for (int i = min; i < max; ++i)
        {
            if (verticalMovement)
            {
                if (!HasCorridorNext(fixedPos, i, false))
                    _tiles[fixedPos, i] = Tile.WALL;
            }
            else
            {
                if (!HasCorridorNext(i, fixedPos, true))
                    _tiles[i, fixedPos] = Tile.WALL;
            }
        }

    }
    #endregion

    #region Maze
    /// <summary>
    /// Creates the maze. Initialize it first if null
    /// </summary>
    public void CreateMaze()
    {
        if (_maze == null)
            _maze = new Maze(_tiles);
        
        GameObject.Find("Generator").GetComponent<Generator>().StartCoroutine(_maze.CreateMaze());
        _mazeConstructed = true;
    }

    /// <summary>
    /// Is the maze constructed ?
    /// </summary>
    /// <returns>If the maze is constructed</returns>
    public bool MazeReady()
    {
        return  _mazeConstructed;
    }

    /// <summary>
    /// Opens the rooms to the maze
    /// </summary>
    public void OpenRooms()
    {
        _maze.OpenRooms();
    }

    /// <summary>
    /// Delete every dead end (will remove everything if no rooms or no connection)
    /// </summary>
    public void RemoveDeadEnds()
    {
        GameObject.Find("Generator").GetComponent<Generator>().StartCoroutine(_maze.RemoveDeadEnds());
    }
    #endregion

    #region cleanup
    public bool IsUnwantedWall(int x, int y)
    {
        for (int i = x - 1; i <= x + 1; ++i)
            for (int j = y - 1; j <= y + 1; ++j)
            {
                if (i < 0 || j < 0 || i >= _tiles.GetLength(0) || j >= _tiles.GetLength(1))
                    continue;
                if (_tiles[i, j] == Tile.FLOOR || _tiles[i, j] == Tile.CORRIDOR)
                    return false;
            }
        return true;
    }

    public void RemoveUnwantedWalls()
    {
        for (int x = 0; x < _tiles.GetLength(0); ++x)
            for (int y = 0; y < _tiles.GetLength(1); ++y)
                if (IsUnwantedWall(x, y))
                    _tiles[x, y] = Tile.NONE;
    }
    #endregion
}