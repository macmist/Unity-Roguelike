using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Describe an X Y coordinates couple
/// Origin (0,0) is in bottom left corner
/// </summary>
public class XY {
    /// <summary>
    /// X coordinate
    /// </summary>
    private int _x;
    /// <summary>
    /// Y coordinate
    /// </summary>
    private int _y;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="x">x value</param>
    /// <param name="y">y value</param>
    public XY(int x, int y)
    {
        _x = x;
        _y = y;
    }

    /// <summary>
    /// Default constructor, initializes a point to (0,0)
    /// </summary>
    public XY() : this(0, 0)
    {

    }

    /// <summary>
    /// Getter Setter for _x
    /// </summary>
    public int x
    {
        get { return _x; }
        set { _x = value; }
    }

    /// <summary>
    /// Getter Setter for _y
    /// </summary>
    public int y
    {
        get { return _y; }
        set { _y = value; }
    }

    /// <summary>
    /// Adds two XY
    /// </summary>
    /// <param name="a">The first point</param>
    /// <param name="b">The second point</param>
    /// <returns>A new point corresponding to the addition</returns>
    public static XY operator+(XY a, XY b)
    {
        return new XY(a.x + b.x, a.y + b.y);
    }

    /// <summary>
    /// Substracts two XY
    /// </summary>
    /// <param name="a">The first point</param>
    /// <param name="b">The second point</param>
    /// <returns>A new point corresponding to the substractions</returns>
    public static XY operator-(XY a, XY b)
    {
        return new XY(a.x - b.x, a.y - b.y);
    }

    /// <summary>
    /// Multiplies a XY by an integer
    /// </summary>
    /// <param name="a">The first point</param>
    /// <param name="b">The multiplication operand</param>
    /// <returns>A new point corresponding to the multiplication</returns>
    public static XY operator*(XY a, int b)
    {
        return new XY(a.x * b, a.y * b);
    }
    /// <summary>
    /// Divides a XY by an integer
    /// </summary>
    /// <param name="a">The first point</param>
    /// <param name="b">The division operand</param>
    /// <returns>A new point corresponding to the division</returns>
    public static XY operator/(XY a, int n)
    {
        return new XY(a.x / n, a.y / n);
    }


    public override string ToString()
    {
        return "X: " + x + "; Y: " + y;
    }
}
