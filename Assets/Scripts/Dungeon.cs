using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Tile
{
    WALL,
    FLOOR
}

public class Dungeon  {
    private static Dungeon instance;

    private Texture2D _texture;
    private Tile[,] _tiles;
    private AABB _boundaries;
    private List<Room> _rooms;

    public static int MIN_ROOM_HALFSIZE = 1;
    private static int DEFAULT_SIZE = 100;

    private Dungeon() : this(new AABB(new XY(DEFAULT_SIZE / 2, DEFAULT_SIZE / 2), new XY(DEFAULT_SIZE / 2, DEFAULT_SIZE / 2)))
    {

    }

    private Dungeon(AABB boundaries)
    {
        _boundaries = boundaries;
        _texture = new Texture2D(boundaries.Size().x, boundaries.Size().y, TextureFormat.ARGB32, false);
        _texture.filterMode = FilterMode.Point;
        _texture.wrapMode = TextureWrapMode.Clamp;
    }

    public static Dungeon GetInstance()
    {
        if (instance == null)
        {
            instance = new Dungeon();
        }
        return instance;
    }

    public static Dungeon GetInstance(AABB box)
    {
        if (instance == null)
        {
            instance = new Dungeon(box);
        }
        return instance;
    }

    public Texture2D Texture
    {
        get { return _texture; }
        set { _texture = value; }
    }

    public AABB Boundaries
    {
        get { return _boundaries; }
    }
}