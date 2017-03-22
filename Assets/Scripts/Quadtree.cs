using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A quadtree, used to split the map in tiny parts
/// </summary>
public class Quadtree {
    /// <summary>
    /// The current node size
    /// </summary>
    private AABB _box;

    /// <summary>
    /// The only room contained in the current node
    /// </summary>
    private Room _room;

    /// <summary>
    /// The north west child node
    /// </summary>
    private Quadtree _northWest;

    /// <summary>
    /// The north east child node
    /// </summary>
    private Quadtree _northEast;

    /// <summary>
    /// The south west child node
    /// </summary>
    private Quadtree _southWest;

    /// <summary>
    /// The south east child node
    /// </summary>
    private Quadtree _southEast;


    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="box">The node's bounds</param>
    public Quadtree(AABB box)
    {
        _box = box;
    }

    /// <summary>
    /// Recursively creates the tree by adding the child nodes
    /// </summary>
    /// <param name="currentDepth">The current depth of the tree</param>
    /// <param name="maxDepth">The max depth the tree can have</param>
    public void BuildTree(int currentDepth, int maxDepth)
    {
        // We want to stop if we have reached the max depth
        if (currentDepth > maxDepth)
            return;
        // Or if it is not possible to store 4 rooms in the space
        XY wh = _box.half / 2;
        if (wh.x < Dungeon.MIN_ROOM_HALFSIZE + 2 || wh.y < Dungeon.MIN_ROOM_HALFSIZE + 2)
            return;

        
        
        // Make sure they are even numbers
        int x = _box.Left() + 2 * Random.Range(Dungeon.MIN_ROOM_HALFSIZE + 1, _box.half.x - Dungeon.MIN_ROOM_HALFSIZE);
        int y = _box.Bottom() + 2 * Random.Range(Dungeon.MIN_ROOM_HALFSIZE + 1, _box.half.y - Dungeon.MIN_ROOM_HALFSIZE);


        // The half sizes
        int left = (x - _box.Left()) / 2;
        int right = (_box.Right() - x) / 2;
        int up = (_box.Top() - y) / 2;
        int down = (y - _box.Bottom()) / 2;

        // Creating each trees and build them
        _northWest = new Quadtree(new AABB(new XY(x - left, y + up) , new XY(left, up)));
        _northWest.BuildTree(currentDepth + 1, maxDepth);
        _northEast = new Quadtree(new AABB(new XY(x + right, y + up), new XY(right, up)));
        _northEast.BuildTree(currentDepth + 1, maxDepth);
        _southWest = new Quadtree(new AABB(new XY(x - left, y - down), new XY(left, down)));
        _southWest.BuildTree(currentDepth + 1, maxDepth);
        _southEast = new Quadtree(new AABB(new XY(x + right, y - down), new XY(right, down)));
        _southEast.BuildTree(currentDepth + 1, maxDepth);
    }

    /// <summary>
    /// Recursively draws the limits on the texture
    /// </summary>
    /// <param name="texture">The texture to draw on</param>
    public void DrawLimits(Texture2D texture)
    {
        // If each node exists we recall draw on it
        if (_northEast != null)
            _northEast.DrawLimits(texture);
        if (_northWest != null)
            _northWest.DrawLimits(texture);
        if (_southEast != null)
            _southEast.DrawLimits(texture);
        if (_southWest != null)
            _southWest.DrawLimits(texture);
        
        // But if there is no child we draw the  current node's box
        if (_northEast == null && _northWest == null && _southEast == null && _southWest == null)
            _box.Draw(texture);
    }

    /// <summary>
    /// Recursively add rooms
    /// </summary>
    public void AddRoom()
    {
        // If children exist, call AddRoom to em
        if (_northEast != null)
            _northEast.AddRoom();
        if (_northWest != null)
            _northWest.AddRoom();
        if (_southEast != null)
            _southEast.AddRoom();
        if (_southWest != null)
            _southWest.AddRoom();

        // Else add room to current node
        if (_northEast == null && _northWest == null && _southEast == null && _southWest == null)
        {
            _room = Room.CreateRandomRoom(_box);
            Dungeon.GetInstance().AddRoomToTiles(_room);
        }
    }

    /// <summary>
    /// Recursively draw the rooms
    /// </summary>
    /// <param name="texture">The texture to draw on</param>
    public void DrawRoom(Texture2D texture)
    {
        // If children exist, call DrawRoom to em
        if (_northEast != null)
            _northEast.DrawRoom(texture);
        if (_northWest != null)
            _northWest.DrawRoom(texture);
        if (_southEast != null)
            _southEast.DrawRoom(texture);
        if (_southWest != null)
            _southWest.DrawRoom(texture);

        // Else draw current's node room
        if (_northEast == null && _northWest == null && _southEast == null && _southWest == null)
            _room.Draw(texture);
    }

    /// <summary>
    /// Recursively links the rooms of the tree
    /// </summary>
    /// <param name="direction">The direction where the current quadtree is</param>
    /// <returns>The most logical room to link</returns>
    public Room LinkRooms(string direction)
    {
        // If we are in a leaf, we return its room
        if (_northEast == null && _northWest == null && _southEast == null && _southWest == null)
            return _room;

        Room ne = null;
        Room nw = null;
        Room se = null;
        Room sw = null;
        Dungeon d = Dungeon.GetInstance();

        // Retrieves rooms from each subtree
        if (_northEast != null)
            ne = _northEast.LinkRooms("NE");
        if (_northWest != null)
            nw = _northWest.LinkRooms("NW");
        if (_southEast != null)
            se = _southEast.LinkRooms("SE");
        if (_southWest != null)
            sw = _southWest.LinkRooms("SW");
        
        // Links rooms in a square
        d.CreateCorridorBetweenRooms(ne, nw);
        d.CreateCorridorBetweenRooms(nw, sw);
        d.CreateCorridorBetweenRooms(sw, se);
        d.CreateCorridorBetweenRooms(se, ne);

        // Return the room opposite to the current position
        switch(direction)
        {
            case "NE":
                return sw;
            case "NW":
                return se;
            case "SE":
                return nw;
            case "SW":
                return ne;
            default:
                return null;
        }
    }
}
