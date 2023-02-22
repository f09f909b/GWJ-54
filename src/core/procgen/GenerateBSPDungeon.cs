using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot.Collections;
using UnnamedProject.utils;

public partial class GenerateBSPDungeon : Node
{
    [Export] private int _mapDimensions = 100;
    [Export] private int _treeDepth = 2;
    [Export] private int _wallSize;
    [Export] private int _maxEnemiesToSpawn;
    [Export] private bool _debugOn;

    private int _mapWidth, _mapDepth;
    private int[,] _dungeonGridMap;
    private static List<List<Vector2I>> _potentialRooms = new();
    private List<Vector2> _corridors;
    private Leaf _root;

    private GridMap _gridMap;
    [Export] private PackedScene _playerPackedScene;
    [Export] private Array<Enemy> _enemyPool;
    [Export] private Array<Enemy> _powerUpPool;

    [Export] private NodePath _gridMapPath;
    [Export] private NodePath _playerPath;
    [Export] private NodePath _enemyPoolPath;

    public override void _Ready()
    {
        _mapWidth = _mapDepth = _mapDimensions;
        _dungeonGridMap = new int[_mapWidth, _mapDepth];
        _corridors = new List<Vector2>();
        _root = new Leaf(0, 0, _mapWidth, _mapDepth);

        _gridMap = GetNode<GridMap>(_gridMapPath);

        // Dirty Flag
        if (_debugOn)
        {
            GenerateNewDungeon();
        }
    }

    public static void SaveRoomsData(List<Vector2I> roomData)
    {
        _potentialRooms.Add(roomData);
    }

    public static void ClearRoomsData()
    {
        _potentialRooms.Clear();
    }

    private void GenerateNewDungeon()
    {
        InitializeGridMap();
        Bsp(_root, _treeDepth); // Carve out rooms
        AddCorridors();
        FillInGridMapTiles();
        PlaceDownEnemies();
    }

    private void InitializeGridMap()
    {
        for (var i = 0; i < _mapDimensions; i++)
        {
            for (var j = 0; j < _mapDimensions; j++)
            {
                _dungeonGridMap[i, j] = 0;
            }
        }
    }

    private void Bsp(Leaf root, int treeDepth)
    {
        if (root == null) return;

        if (treeDepth <= 0)
        {
            Vector2 leafCenter = root.CalculateCenter(root.XPos, root.ZPos, root.Depth, root.Width);
            _corridors.Add(leafCenter);
            root.CarveOutRoom(_dungeonGridMap, _wallSize);
            return;
        }

        bool canBeSplit = root.Split();
        if (canBeSplit)
        {
            Bsp(root.LeftChild, treeDepth - 1);
            Bsp(root.RightChild, treeDepth - 1);
        }
        else
        {
            Vector2 leafCenter = root.CalculateCenter(root.XPos, root.ZPos, root.Depth, root.Width);
            _corridors.Add(leafCenter);
            root.CarveOutRoom(_dungeonGridMap, _wallSize);
        }
    }

    private void AddCorridors()
    {
        for (var i = 1; i < _corridors.Count; i++)
        {
            // we mark corridors only if our solutions to leaf midpoints are either horizontal or vertical
            if ((int) _corridors[i].X == (int) _corridors[i - 1].X ||
                (int) _corridors[i].Y == (int) _corridors[i - 1].Y)
            {
                MarkCorridorPaths((int) _corridors[i].X, (int) _corridors[i].Y, (int) _corridors[i - 1].X,
                    (int) _corridors[i - 1].Y);
            }
            else
            {
                // TODO: Convert this diagonal correction line into own function
                MarkCorridorPaths((int) _corridors[i].X, (int) _corridors[i].Y, (int) _corridors[i].X,
                    (int) _corridors[i - 1].Y);
                MarkCorridorPaths((int) _corridors[i].X, (int) _corridors[i].Y, (int) _corridors[i - 1].X,
                    (int) _corridors[i].Y);
            }
        }
    }

    private void AddRandomCorridors(int numOfCorridors)
    {
        var random = new Random();
        for (var i = 0; i < numOfCorridors; i++)
        {
            int startX = random.Next(5, _mapWidth - 5);
            int startZ = random.Next(5, _mapDepth - 5);
            int length = random.Next(5, _mapWidth);

            if (random.Next(0, 100) < 50)
            {
                MarkCorridorPaths(startX, startZ, length, startZ);
            }
            else
            {
                MarkCorridorPaths(startX, startZ, startX, length);
            }
        }
    }

    private void MarkCorridorPaths(int x, int y, int x2, int y2)
    {
        int w = x2 - x;
        int h = y2 - y;
        int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
        if (w < 0) dx1 = -1;
        else if (w > 0) dx1 = 1;
        if (h < 0) dy1 = -1;
        else if (h > 0) dy1 = 1;
        if (w < 0) dx2 = -1;
        else if (w > 0) dx2 = 1;
        int longest = Mathf.Abs(w);
        int shortest = Mathf.Abs(h);
        if (!(longest > shortest))
        {
            longest = Mathf.Abs(h);
            shortest = Mathf.Abs(w);
            if (h < 0) dy2 = -1;
            else if (h > 0) dy2 = 1;
            dx2 = 0;
        }

        int numerator = longest >> 1;
        for (int i = 0; i <= longest; i++)
        {
            _dungeonGridMap[x, y] = 1;
            numerator += shortest;
            if (!(numerator < longest))
            {
                numerator -= longest;
                x += dx1;
                y += dy1;
            }
            else
            {
                x += dx2;
                y += dy2;
            }
        }
    }

    // TODO: Look into potential breaking this up into a calculation and get method
    private List<int> GetDijkstraMap(Vector2I targetPosition)
    {
        List<int> dijkstraMap = new();
        Godot.Collections.Dictionary<Vector2I, int> distanceLookup = new();
        Queue<Vector2I> frontier = new Queue<Vector2I>();
        Array<int> horizontalNeighbours = new Array<int>() {-1, 1, 0, 0};
        Array<int> verticalNeighbours = new Array<int>() {0, 0, 1, -1};

        frontier.Enqueue(targetPosition);

        while (frontier.Any())
        {
            var current = frontier.Dequeue();
            for (var i = 0; i < 4; i++)
            {
                var neighbourCoordinates =
                    new Vector2I(current.X + horizontalNeighbours[i], current.Y + verticalNeighbours[i]);
                var currentNeighbour =
                    new Vector2I(neighbourCoordinates.X, neighbourCoordinates.Y);

                if (distanceLookup.ContainsKey(currentNeighbour) &&
                    _dungeonGridMap[neighbourCoordinates.X, neighbourCoordinates.Y] !=
                    0)
                {
                    distanceLookup[currentNeighbour] += 1;
                }
                else
                {
                    frontier.Enqueue(currentNeighbour);
                    distanceLookup.Add(currentNeighbour, 0);
                }
            }
        }


        foreach (var costValue in distanceLookup)
        {
            //dijkstraMap.Add(costValue);
        }


        return dijkstraMap;
    }

    private void AssignRooms()
    {
        PlaceDownPlayer();
        for (var i = 1; i < _potentialRooms.Count; i++)
        {
            var room = _potentialRooms[i];
            // Assign function
        }
    }

    private void FillInGridMapTiles()
    {
        for (var i = 0; i < _mapWidth; i++)
        {
            for (var j = 0; j < _mapDepth; j++)
            {
                int tileId = _dungeonGridMap[i, j];
                switch (tileId)
                {
                    case 0:
                        _gridMap.SetCellItem(new Vector3I(i, 0, j), 0); // Wall Tile
                        break;
                    case 1:
                        _gridMap.SetCellItem(new Vector3I(i, 0, j), 1); // Floor Title
                        break;
                }
            }
        }
    }

    private void PlaceDownPlayer()
    {
        List<Vector2I> playerSpawnRoom = _potentialRooms.First();
        Vector2 playerSpawnCoordinates = Utils.CalculateCenter(1, 2, 3, 4);

        var player = _playerPackedScene.Instantiate<PlayerController>();
        player.GlobalPosition = new Vector3(playerSpawnCoordinates.X, 0, playerSpawnCoordinates.Y);

        GetTree().Root.GetNode<Node3D>("Level").AddChild(player);
    }

    private void PlaceDownEnemies()
    {
    }

    private void PlaceDownExit()
    {
    }
}