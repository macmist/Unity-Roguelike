using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Describes a Room. Basically it is a AABB with a few more things
/// </summary>
public class Room : AABB {

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="center">Given center</param>
    /// <param name="half">Given half</param>
    public Room(XY center, XY half)
    {
        _center = center;
        _half = half;
    }

    /// <summary>
    /// Default constructor. Creates a 0 sized room
    /// </summary>
    public Room() : this (new XY(), new XY())
    {

    }

    /// <summary>
    /// Check if the point is whitin a d distance from the room
    /// </summary>
    /// <param name="p">the point to check</param>
    /// <param name="d">the distance</param>
    /// <returns>If the point is at a d distance from the room</returns>
    public bool IsNext(XY p, int d)
    {
        if (p.x < _center.x - _half.x - d) return false;
        if (p.x >= _center.x + _half.x + d) return false;
        if (p.y < _center.y - _half.y - d)  return false; 
        if (p.y >= _center.y + _half.y + d) return false;
        return true;
    }

    /// <summary>
    /// Tries to create a random room inside a given box.
    /// It lefts at least one tile between the box and the room
    /// </summary>
    /// <param name="boundaries">The box where to create the new room</param>
    /// <returns>A new Room or null if the boundaries is too small</returns>
    public static Room CreateRandomRoom(AABB boundaries)
    {
        if (boundaries.half.x < Dungeon.MIN_ROOM_HALFSIZE + Dungeon.MIN_ROOM_MARGIN || boundaries.half.y < Dungeon.MIN_ROOM_HALFSIZE + Dungeon.MIN_ROOM_MARGIN)
            return null;

        int maxSizeX = Mathf.Min(boundaries.half.x - Dungeon.MIN_ROOM_MARGIN, Dungeon.MAX_ROOM_HALFSIZE);
        int maxSizeY = Mathf.Min(boundaries.half.y - Dungeon.MIN_ROOM_MARGIN, Dungeon.MAX_ROOM_HALFSIZE);

        // Create random width
        int width = Random.Range(Dungeon.MIN_ROOM_HALFSIZE, maxSizeX);
        int height = Random.Range(Dungeon.MIN_ROOM_HALFSIZE, maxSizeY);

        // We don't want the room to go outside the box
        int maxx = boundaries.Right() - width - (Dungeon.MIN_ROOM_MARGIN - 1);
        int minx = boundaries.Left() + width + Dungeon.MIN_ROOM_MARGIN;

        int miny = boundaries.Bottom() + height + Dungeon.MIN_ROOM_MARGIN;
        int maxy = boundaries.Top() - height - (Dungeon.MIN_ROOM_MARGIN - 1);

        // Create random center
        int x = Random.Range(minx, maxx);
        int y = Random.Range(miny, maxy);

        return new Room(new XY(x, y), new XY(width, height));
    }
}
