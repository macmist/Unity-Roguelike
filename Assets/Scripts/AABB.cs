using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AABB {
    protected XY _center;
    protected XY _half;

    public AABB(XY center, XY half)
    {
        _center = center;
        _half = half;
    }

    public AABB() : this (new XY(), new XY())
    {

    }

    public XY center
    {
        get { return _center; }
        set { _center = value; }
    }

    public XY half
    {
        get { return _half; }
        set { _half = value; }
    }

    public int Bottom()
    {
        return center.y - half.y;
    }

    public int Top()
    {
        return center.y + half.y;
    }

    public int Left()
    {
        return center.x - half.x;
    }

    public int Right()
    {
        return center.x + half.x;
    }

    public XY BottomLeft()
    {
        return new XY(Left(), Bottom());
    }

    public XY BottomRight()
    {
        return new XY(Right(), Bottom());
    }

    public XY TopLeft()
    {
        return new XY(Left(), Top());
    }

    public XY TopRight()
    {
        return new XY(Right(), Top());
    }

    public bool Interects(AABB other)
    {
        XY distance = other.center - this.center;
        if (Mathf.Abs(distance.x) < this.center.x + other.center.x && Mathf.Abs(distance.y) < this.center.y + other.center.y)
            return true;
        return false;
    }
    public XY Size()
    {
        return half * 2;
    }

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
}
