using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A node class used by the AStar algorithm
/// Has a parent, a postion and a cost
/// </summary>
public class Node
{
    /// <summary>
    /// The node's parent
    /// </summary>
    public Node Parent { get; set; }

    /// <summary>
    /// The node's potions
    /// </summary>
    public XY Position { get; set; }

    /// <summary>
    /// The node's cost
    /// </summary>
    public int TotalCost { get; set; }

    /// <summary>
    /// The distance it took to go from start point to current point
    /// </summary>
    public int DistanceCost { get; set; }

    /// <summary>
    /// The danger of the current point
    /// </summary>
    public int DangerCost { get; set; }

    /// <summary>
    /// The estimation of the cost from this point to the end
    /// </summary>
    public int Heuristic { get; set; }

    /// <summary>
    /// The direction we came from
    /// </summary>
    public string Direction { get; set; }

    /// <summary>
    /// Check if the nodes are representing the same coordinates
    /// </summary>
    /// <param name="node">The node to compare coordinates with</param>
    /// <returns>Wether they have the same coordinates</returns>
    public bool SamePoint(Node node)
    {
        return node.Position.x == Position.x && node.Position.y == Position.y;
    }
}

/// <summary>
/// The AStar class that does the AStar algorithm
/// </summary>
public class AStar
{
    /// <summary>
    /// The list containing the point we still have to visit
    /// </summary>
    private List<Node> _openList;

    /// <summary>
    /// The visited points
    /// </summary>
    private List<Node> _closedList;

    /// <summary>
    /// The graph on which we do the pathfiding
    /// </summary>
    private Tile[,] _graph;

    /// <summary>
    /// The graph's width
    /// </summary>
    private int _width;

    /// <summary>
    /// The graph's height
    /// </summary>
    private int _height;

    /// <summary>
    /// The result node we'll send 
    /// </summary>
    private Node _res;

    /// <summary>
    /// The start room
    /// </summary>
    private Room _start;

    /// <summary>
    /// The end room
    /// </summary>
    private Room _end;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="graph">The graph</param>
    public AStar(Tile[,] graph)
    {
        _openList = new List<Node>();
        _closedList = new List<Node>();
        _graph = graph;
        _width = _graph.GetLength(0);
        _height = _graph.GetLength(1);
    }

    /// <summary>
    /// Check if the point is in or at 1 unit distance from a room
    /// </summary>
    /// <param name="p">The point to check</param>
    /// <returns>If the room is in or next to a room</returns>
    private bool IsInOrNearbyRoom(XY p)
    {
        for (int i = Mathf.Max(0, p.x - 1); i <= p.x + 1 && i < _width; ++i)
            for (int j = Mathf.Max(0, p.y - 1); j <= p.y && j < _height; ++j)
                if (_graph[i, j] == Tile.FLOOR)
                    return true;
        return false;
    }

    /// <summary>
    /// Check if the point is an obstacle.
    /// It is an obstacle if:
    /// - it is next or in a room
    /// - this room is not the start or the end room
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    private bool IsObstacle(XY p)
    {
        if (!IsInOrNearbyRoom(p))
            return false;
        return !(_start.IsNext(p, 1) || _end.IsNext(p, 1));
    }

    /// <summary>
    /// Computes the danger score.
    /// </summary>
    /// <param name="n">The node to check danger</param>
    /// <returns>The danger score</returns>
    private int ComputeDangerScore(Node n)
    {
        XY p = n.Position;
        int res = 0;
        if (IsObstacle(p))
            res += 25;
        if (_graph[p.x, p.y] == Tile.CORRIDOR)
            res -= 100;
        if (n.Parent != null && !n.Parent.Direction.Equals(n.Direction))
            res += 5;
        return res;
    }

    /// <summary>
    /// Calculate the square distance between two points
    /// </summary>
    /// <param name="xa">X coordinate of first point</param>
    /// <param name="ya">Y coordinate of first point</param>
    /// <param name="xb">X coordinate of second point</param>
    /// <param name="yb">Y coordinate of second point</param>
    /// <returns>The square distance between the two points</returns>
    private int SquareDistance(int xa, int ya, int xb, int yb)
    {
        int dx = xb - xa;
        int dy = yb - ya;
        return dx * dx + dy * dy;
    }

    /// <summary>
    /// As we don't have a priority queue, this function detects the node with the least 
    /// cost, removes it from the list and returns it
    /// </summary>
    /// <returns>The node with the best cost</returns>
    private Node PopMostAccurateNode()
    {
        if (_openList == null)
            return null;
        Node res = _openList[0];
        foreach (Node n in _openList)
        {
            if (n.TotalCost < res.TotalCost)
                res = n;
        }
        _openList.Remove(res);
        return res;
    }
    
    /// <summary>
    /// Creates the successor of the node
    /// </summary>
    /// <param name="parent">The node we came from</param>
    /// <param name="end">The node we are going to</param>
    /// <param name="point">The position of the new node</param>
    /// <param name="direction">The direction we came from</param>
    /// <returns>The newly created node</returns>
    private Node CreateSuccessor(Node parent, Node end, XY point, string direction)
    {
        Node node = new Node();
        node.Parent = parent;
        node.Position = point;
        node.Direction = direction;
        node.DistanceCost = parent.DistanceCost + 1;
        node.Heuristic = SquareDistance(end.Position.x, end.Position.y, point.x, point.y);
        node.DangerCost = ComputeDangerScore(node);
        node.TotalCost = node.DistanceCost + node.Heuristic + node.DangerCost;
        return node;
    }

    /// <summary>
    /// Create the list of all possible successors to the point
    /// </summary>
    /// <param name="parent">The node we create succesors from</param>
    /// <param name="end">The node we are going to</param>
    /// <returns>The list of all the successors</returns>
    private List<Node> GetSuccessors(Node parent, Node end)
    {
        List<Node> res = new List<Node>();
        if (parent.Position.x - 1 >= 0)
            res.Add(CreateSuccessor(parent, end, new XY(parent.Position.x - 1, parent.Position.y), "right"));
        if (parent.Position.x + 1 < _width)
            res.Add(CreateSuccessor(parent, end, new XY(parent.Position.x + 1, parent.Position.y), "left"));
        if (parent.Position.y - 1 >= 0)
            res.Add(CreateSuccessor(parent, end, new XY(parent.Position.x, parent.Position.y - 1), "top"));
        if (parent.Position.y + 1 < _height)
            res.Add(CreateSuccessor(parent, end, new XY(parent.Position.x, parent.Position.y + 1), "botom"));
        return res;
    }

    /// <summary>
    /// Checks if ther is a node in the two list which has same coordinates and a better cost
    /// </summary>
    /// <param name="node">The cost to check</param>
    /// <returns>If there is or no a better Node</returns>
    private bool ListsHaveBetterNode(Node node)
    {
        foreach (Node n in _openList)
            if (n.SamePoint(node) && n.TotalCost < node.TotalCost)
                return true;
        foreach(Node n in _closedList)
            if (n.SamePoint(node) && n.TotalCost < node.TotalCost)
                return true;
        return false;
    }

    /// <summary>
    /// The main loop of the algorithm
    /// </summary>
    /// <param name="end">The node we are going to</param>
    private void AStarLoop(Node end)
    {
        while (_openList.Count > 0)
        {
            Node q = PopMostAccurateNode();
            List<Node> successors = GetSuccessors(q, end);
            foreach(Node s in successors)
            {
                if (s.SamePoint(end) || _end.IsNext(s.Position, 0))
                {
                    _res = s;
                    return;
                }
                if (ListsHaveBetterNode(s))
                    continue;
                _openList.Add(s);
            }
            _closedList.Add(q);
        }
    }

    /// <summary>
    /// The Main function
    /// </summary>
    /// <param name="begin">The point we start from</param>
    /// <param name="end">The point we are going to</param>
    /// <param name="a">The room we start from</param>
    /// <param name="b">The room we are going to</param>
    /// <returns>The result node containing the path</returns>
    public Node DoAStar(XY begin, XY end, Room a, Room b)
    {
        _start = a;
        _end = b;
        _openList.Clear();
        _closedList.Clear();

        Node beginNode = new Node();
        beginNode.Position = begin;
        beginNode.Direction = "none";

        Node endNode = new Node();
        endNode.Position = end;
        endNode.Direction = "none";

        _openList.Add(beginNode);
        AStarLoop(endNode);
        return _res;
    }
}