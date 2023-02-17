using Godot;
using System;

public partial class Leaf : Node
{
    public Leaf RightChild;
    public Leaf LeftChild;
    public int XPos { get; private set; }
    public int ZPos { get; private set; }
    public int Width { get; private set; }
    public int Depth { get; private set; }
    private int _roomMin = 10;
    private int _roomMax;
    private const float AspectRatio = 1.333f; // ASPECT FOR CRT DISPLAY

    public Leaf(int x, int z, int width, int depth)
    {
        XPos = x;
        ZPos = z;
        Width = width;
        Depth = depth;
    }

    public bool Split()
    {
        if (Width <= _roomMin || Depth <= _roomMin) return false;

        Random rng = new Random();
        bool isHorizontalBasedOnRng = rng.Next(0, 100) > 50;
        bool canSplitHorizontal = CanSplitHorizontalCheck(isHorizontalBasedOnRng);

        _roomMax = (canSplitHorizontal ? Depth : Width) - _roomMin;

        if (_roomMax <= _roomMin) return false;

        if (canSplitHorizontal)
        {
            int cutDepth = rng.Next(_roomMin, _roomMax);
            LeftChild = new Leaf(XPos, ZPos, Width, cutDepth);
            RightChild = new Leaf(XPos, ZPos + cutDepth, Width, Depth - cutDepth);
        }
        else
        {
            int cutWidth = rng.Next(_roomMin, _roomMax);
            LeftChild = new Leaf(XPos, ZPos, cutWidth, Depth);
            RightChild = new Leaf(XPos + cutWidth, ZPos, Width - cutWidth, Depth);
        }

        return true;
    }

    public void CarveOutRoom(int[,] dungeonMap, int wallSpecifiedSize = 0)
    {
        int wallSize = wallSpecifiedSize == 0 ? new Random().Next(1, 3) : wallSpecifiedSize;
        for (var x = XPos + wallSize; x < Width + XPos - 1; x++)
        {
            for (var z = ZPos + wallSize; z < Depth + ZPos - 1; z++)
            {
                dungeonMap[x, z] = 1;
            }
        }
    }

    public Vector2 CalculateCenter(int xPos, int zPos, int depth, int width)
    {
        return new Vector2(xPos + width / 2, zPos + depth / 2);
    }

    private bool CanSplitHorizontalCheck(bool isHorizontalBasedOnRng)
    {
        if (Width > Depth && (float) Depth / Width >= AspectRatio)
        {
            return false;
        }

        if (Width < Depth && (float) Depth / Width >= AspectRatio)
        {
            return true;
        }

        return isHorizontalBasedOnRng;
    }
}