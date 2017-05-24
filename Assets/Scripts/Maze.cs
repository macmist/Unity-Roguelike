using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Can create a maze, connect it to rooms, and remove dead ends
/// </summary>
public class Maze : MonoBehaviour {
    private Stack<int> _walls;
    private Tile[,] _tiles;
    private Color _mazeColor;
    private int _width;
    private int _height;
    private Dungeon _dungeon;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="tiles">The tilemap to work on</param>
    public Maze(Tile[,] tiles)
    {
        _tiles = tiles;
        _width = _tiles.GetLength(0);
        _height = _tiles.GetLength(1);
        _mazeColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
        _dungeon = Dungeon.GetInstance();
    }


    #region maze generation
    private bool IsValidStartPoint(int i, int j)
    {
        for (int x = i - 1; x <= i + 1; x++)
            for (int y = j - 1; y <= j + 1; ++y)
            {
                if (x >= _width || y >= _height)
                    continue;
                if (_tiles[x, y] != Tile.WALL)
                    return false;
            }
        return true;
    }

    private XY PickStartPoint()
    {
        for (int i = 1; i < _width - 1; ++i)
            for (int j = 1; j < _height + 1; ++j)
            {
                if (!IsValidStartPoint(i, j))
                    continue;
                return new XY(i, j);
            }
        return null;
    }

    private bool IsValidWall(int x, int y)
    {
        if (x <= 0 || y <= 0 || x >= _width - 1 || y >= _height - 1)
            return false;
        for (int i = x - 1; i <= x + 1; ++i)
            for (int j = y - 1; j <= y + 1; ++j)
            {
                if (_tiles[i, j] != Tile.WALL)
                    return false;
            }

        return true;
    }


    private string GetPossibleDirections(int x, int y)
    {
        string res = "";
        if (IsValidWall(x + 2, y))
            res += "E";
        if (IsValidWall(x - 2, y))
            res += "W";
        if (IsValidWall(x, y + 2))
            res += "N";
        if (IsValidWall(x, y - 2))
            res += "S";
        return res;
    }


    private void AddToStack(int x, int y)
    {
        if (_walls == null)
            _walls = new Stack<int>();
        _walls.Push(y + x * _width);
    }

    private void PopStack(out int x, out int y)
    {
        int i = _walls.Pop();
        x = (int)Mathf.Floor(i / _width);
        y = i % _width;
    }


    /// <summary>
    /// Generates the random maze, using Recursive Backtracking algorithm
    /// </summary>
    /// <returns>The current state of the maze</returns>
    public IEnumerator CreateMaze()
    {
        XY c = PickStartPoint();
        int x = c.x;
        int y = c.y;

        _dungeon.AddToTilesAndTexture(x, y, Tile.CORRIDOR, _mazeColor);
        AddToStack(x, y);

        while (_walls.Count > 0)
        {
            string possiblesDirs = GetPossibleDirections(x, y);
            if (possiblesDirs.Length > 0)
            {
                int dir = Random.Range(0, possiblesDirs.Length);

                switch (possiblesDirs[dir])
                {
                    case 'N':
                        _dungeon.AddToTilesAndTexture(x, y + 2, Tile.CORRIDOR, _mazeColor);
                        _dungeon.AddToTilesAndTexture(x, y + 1, Tile.CORRIDOR, _mazeColor);
                        y += 2;
                        break;
                    case 'S':
                        _dungeon.AddToTilesAndTexture(x, y - 2, Tile.CORRIDOR, _mazeColor);
                        _dungeon.AddToTilesAndTexture(x, y - 1, Tile.CORRIDOR, _mazeColor);
                        y -= 2;
                        break;
                    case 'E':
                        _dungeon.AddToTilesAndTexture(x + 2, y, Tile.CORRIDOR, _mazeColor);
                        _dungeon.AddToTilesAndTexture(x + 1, y, Tile.CORRIDOR, _mazeColor);
                        x += 2;
                        break;
                    case 'W':
                        _dungeon.AddToTilesAndTexture(x - 2, y, Tile.CORRIDOR, _mazeColor);
                        _dungeon.AddToTilesAndTexture(x - 1, y, Tile.CORRIDOR, _mazeColor);
                        x -= 2;
                        break;
                    default: break;
                }
                AddToStack(x, y);

            }
            else
            {
                PopStack(out x, out y);
            }
        }
        yield return new WaitForSeconds(0);
    }

    #endregion

    #region open rooms
    private List<Room> PointIsConnectorToRooms(int x, int y)
    {
        if (_tiles[x, y] != Tile.WALL)
            return null;
        bool hasCorridorNext = false;
        bool hasRoomNext = false;
        List<Room> roomsNext = new List<Room>();

        for (int i = x - 1; i <= x + 1; ++i)
            for (int j = y - 1; j <= y + 1; ++j)
            {
                if ((i == x - 1 || i == x + 1) && (j == y - 1 || j == y + 1))
                    continue;
                if (_tiles[i, j] == Tile.CORRIDOR)
                    hasCorridorNext = true;
                else if (_tiles[i, j] == Tile.FLOOR)
                {
                    hasRoomNext = true;
                    foreach (Room r in _dungeon.GetRooms())
                    {
                        if (r.ContainsPoint(new XY(i, j)))
                            roomsNext.Add(r);
                    }
                }
            }
        if (hasCorridorNext && hasRoomNext)
            return roomsNext;
        return null;
    }

    private void SearchConnectors()
    {
        for (int i = 1; i < _width - 1; ++i)
            for (int j = 1; j < _height - 1; ++j)
            {
                List<Room> list = PointIsConnectorToRooms(i, j);
                if (list == null)
                    continue;
                foreach (Room r in list)
                    r.AddConnector(new XY(i, j));
            }
    }

    /// <summary>
    /// Opens for each room at least one connector
    /// </summary>
    public void OpenRooms()
    {
        SearchConnectors();
        List<Room> copyRooms = new List<Room>(_dungeon.GetRooms());
        while (copyRooms.Count > 0)
        {
            int n = Random.Range(0, copyRooms.Count);
            Room r = copyRooms[n];
            copyRooms.Remove(r);
            int nc = Random.Range(0, r.GetConnectors().Count);
            XY p = r.GetConnectors()[nc];
            _dungeon.AddToTilesAndTexture(p.x, p.y, Tile.CORRIDOR, _mazeColor);
            List<XY> toRemove = new List<XY>();
            foreach (XY c in r.GetConnectors())
            {
                if (p == c)
                    continue;
                if (Random.value > .99f)
                    _dungeon.AddToTilesAndTexture(c.x, c.y, Tile.CORRIDOR, _mazeColor);
                else
                    toRemove.Add(c);
            }
            foreach (XY c in toRemove)
                r.RemoveConnector(c);
        }
    }
    #endregion

    #region deadends
    private bool IsDeadEnd(int x, int y)
    {
        if (_tiles[x, y] != Tile.CORRIDOR)
            return false;
        int siblings = 0;
        if (_tiles[x + 1, y] == Tile.WALL)
            siblings++;
        if (_tiles[x - 1, y] == Tile.WALL)
            siblings++;
        if (_tiles[x, y + 1] == Tile.WALL)
            siblings++;
        if (_tiles[x, y - 1] == Tile.WALL)
            siblings++;
        return siblings >= 3;
    }

    private void FirstDeadEnd(int curx, int cury, out int x, out int y)
    {
        if (IsDeadEnd(curx + 1, cury))
        {
            x = curx + 1;
            y = cury;
            return;
        }
        if (IsDeadEnd(curx - 1, cury))
        {
            x = curx - 1;
            y = cury;
            return;
        }
        if (IsDeadEnd(curx, cury + 1))
        {
            x = curx;
            y = cury + 1;
            return;
        }
        if (IsDeadEnd(curx, cury - 1))
        {
            x = curx;
            y = cury - 1;
            return;
        }
        for (int i = 1; i < _width - 1; ++i)
            for (int j = 1; j < _height - 1; ++j)
            {
                if (!IsDeadEnd(i, j))
                    continue;
                x = i;
                y = j;
                return;
            }

        x = -1;
        y = -1;
    }

    /// <summary>
    /// Removes every dead end in the maze
    /// </summary>
    /// <returns>The current state of the maze</returns>
    public IEnumerator RemoveDeadEnds()
    {
        int count = 0;
        int curx = 1;
        int cury = 1;
        int x = 1;
        int y = 1;

        while (x > 0 && y > 0)
        {
            FirstDeadEnd(curx, cury, out x, out y);
            Debug.Log(x + " " + y);
            if (x == -1 || y == -1)
                break;
            _dungeon.AddToTilesAndTexture(x, y, Tile.WALL, Color.black);
            _tiles[x, y] = Tile.WALL;

            curx = x;
            cury = y;
            count++;
            yield return new WaitForSeconds(0);
        }
        Debug.Log("Deledted " + count + " dead ends");
        yield return new WaitForSeconds(0);
    }
    #endregion
}
