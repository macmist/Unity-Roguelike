using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XY {

    private int _x;
    private int _y;

    public XY(int x, int y)
    {
        _x = x;
        _y = y;
    }

    public XY() : this(0, 0)
    {

    }

    public int x
    {
        get { return _x; }
        set { _x = value; }
    }

    public int y
    {
        get { return _y; }
        set { _y = value; }
    }


    public static XY operator+(XY a, XY b)
    {
        return new XY(a.x + b.x, a.y + b.y);
    }

    public static XY operator-(XY a, XY b)
    {
        return new XY(a.x - b.x, a.y - b.y);
    }

    public static XY operator*(XY a, int b)
    {
        return new XY(a.x * b, a.y * b);
    }

    public static XY operator/(XY a, int n)
    {
        return new XY(a.x / n, a.y / n);
    }
}
