using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : AABB {

    public Room(XY center, XY half)
    {
        _center = center;
        _half = half;
    }

    public Room() : this (new XY(), new XY())
    {

    }

    public static Room CreateRandomRoom(AABB boundaries)
    {
        if (boundaries.half.x < Dungeon.MIN_ROOM_HALFSIZE || boundaries.half.y < Dungeon.MIN_ROOM_HALFSIZE)
            return null;
        int width = Random.Range(Dungeon.MIN_ROOM_HALFSIZE, boundaries.half.x - 1);
        int height = Random.Range(Dungeon.MIN_ROOM_HALFSIZE, boundaries.half.y - 1);

        int maxx = boundaries.Right() - width;
        int minx = boundaries.Left() + width + 1;

        int miny = boundaries.Bottom() + height + 1;
        int maxy = boundaries.Top() - height;

        int x = Random.Range(minx, maxx);
        int y = Random.Range(miny, maxy);

        return new Room(new XY(x, y), new XY(width, height));
    }
}
