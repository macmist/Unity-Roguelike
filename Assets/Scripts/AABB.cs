using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents an Axis Aligned Boundary Box
/// </summary>
public class AABB {
    /// <summary>
    /// The center of the box
    /// </summary>
    protected XY _center;
    /// <summary>
    /// The halfsize of the box in x and y
    /// </summary>
    protected XY _half;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="center">The center point</param>
    /// <param name="half">The half size</param>
    public AABB(XY center, XY half)
    {
        _center = center;
        _half = half;
    }

    /// <summary>
    /// Default constructor. Initializes an 0 sized box
    /// </summary>
    public AABB() : this (new XY(), new XY())
    {

    }

    /// <summary>
    /// Getter Setter for the center
    /// </summary>
    public XY center
    {
        get { return _center; }
        set { _center = value; }
    }

    /// <summary>
    /// Getter Setter for the size
    /// </summary>
    public XY half
    {
        get { return _half; }
        set { _half = value; }
    }

    /// <summary>
    /// Get the bottom of the box
    /// </summary>
    /// <returns>The y position of the bottom of the box</returns>
    public int Bottom()
    {
        return center.y - half.y;
    }

    /// <summary>
    /// Get the top of the box
    /// </summary>
    /// <returns>The y position of the top of the box</returns>
    public int Top()
    {
        return center.y + half.y;
    }

    /// <summary>
    /// Get the left of the box
    /// </summary>
    /// <returns>The y position of the left of the box</returns>
    public int Left()
    {
        return center.x - half.x;
    }

    /// <summary>
    /// Get the right of the box
    /// </summary>
    /// <returns>The y position of the right of the box</returns>
    public int Right()
    {
        return center.x + half.x;
    }

    /// <summary>
    /// Get the Botton left point of the box
    /// </summary>
    /// <returns>The Bottom Left point of the box</returns>
    public XY BottomLeft()
    {
        return new XY(Left(), Bottom());
    }

    /// <summary>
    /// Get the Botton right point of the box
    /// </summary>
    /// <returns>The Bottom Right point of the box</returns>
    public XY BottomRight()
    {
        return new XY(Right(), Bottom());
    }

    /// <summary>
    /// Get the Top Left point of the box
    /// </summary>
    /// <returns>The Bottom Right point of the box</returns>
    public XY TopLeft()
    {
        return new XY(Left(), Top());
    }

    /// <summary>
    /// Get the Botton right point of the box
    /// </summary>
    /// <returns>The Tom Right point of the box</returns>
    public XY TopRight()
    {
        return new XY(Right(), Top());
    }

    /// <summary>
    /// Determines whether the box intersects with another one.
    /// </summary>
    /// <param name="other">The other box to check intersection</param>
    /// <returns>Whether the two box intersect each other</returns>
    public bool Intersects(AABB other)
    {
        XY distance = other.center - this.center;
        XY halfs = this.half + other.half;

        // The box intersects if each coordinates of the distance between their centers
        // is less than the sum of their halfs
        if (Mathf.Abs(distance.x) < halfs.x 
            && Mathf.Abs(distance.y) < halfs.y)
            return true;
        return false;
    }

    /// <summary>
    /// Return the actual size of the box, ie 2 * half
    /// </summary>
    /// <returns>An XY containing the box's size</returns>
    public XY Size()
    {
        return half * 2;
    }

    /// <summary>
    /// Draws the box with a random color to a texture
    /// </summary>
    /// <param name="texture">The texture to draw on</param>
    public void Draw(Texture2D texture)
    {
        Color color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
        for (int i = Left(); i < Right(); ++i)
        {
            for (int j = Bottom(); j < Top(); ++j)
            {
                texture.SetPixel(i, j, color);
            }
        }
        texture.Apply();
    }

    public override string ToString()
    {
        return "center: " + center.ToString() + " half: " + half.ToString(); 
    }
}
