using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private int _width;
    private int _height;
    private int[,] _cells;
    public Grid(int width, int height)
    {
        _width = width;
        _height = height;
        _cells = new int[width, height];
    }
}
