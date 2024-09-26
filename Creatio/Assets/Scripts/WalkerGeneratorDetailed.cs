using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WalkerGeneratorDetailed : MonoBehaviour
{
    public enum Grid
    {
        GRASS,
        WATER,
        SAND,
        EMPTY
    }

    private Grid[,] gridHandler;
    private List<WalkerObject> Walkers;
    private Tilemap tileMap;
    private Tile Grass;
    private Tile Water;
    private Tile Sand;


    private int MapWidth = 32;
    private int MapHeight = 32;

    private int MaximumWalkers = 64;
    private int TileCount = default;
    private float FillPercent = 0.8f;
    private float WaitTime = 0.01f;

    void Start()
    {
        InitializeGrid();
    }

    Vector2 GetDirection()
    {
        int choice = Mathf.FloorToInt(UnityEngine.Random.value * 3.99f);

        switch (choice)
        {
            case 0:
                return Vector2.down;
            case 1:
                return Vector2.left;
            case 2:
                return Vector2.up;
            case 3:
                return Vector2.right;
            default:
                return Vector2.zero;

        }
    }

    void InitializeGrid()
    {
        gridHandler = new Grid[MapWidth, MapHeight];

        for (int x = 0; x < gridHandler.GetLength(0); x++)
        {
            for (int y = 0; y < gridHandler.GetLength(1); y++)
            {
                gridHandler[x, y] = Grid.EMPTY;
            }
        }

        Walkers = new List<WalkerObject>();

        Vector3Int TileCenter = new Vector3Int(gridHandler.GetLength(0) / 2, gridHandler.GetLength(1) / 2, 0);

        WalkerObject curWalker = new WalkerObject(new Vector2(TileCenter.x, TileCenter.y), GetDirection(), 0.5f);
        gridHandler[TileCenter.x, TileCenter.y] = Grid.GRASS;
        tileMap.SetTile(TileCenter, Grass);
        Walkers.Add(curWalker);

        TileCount++;

        StartCoroutine(CreateGrasss());
    }

    IEnumerator CreateGrasss()
    {
        while (((float)TileCount / (float)gridHandler.Length) < FillPercent)
        {
            bool hasGrass = false;
            foreach (WalkerObject curWalker in Walkers) {

                Vector3Int curPos = new Vector3Int((int)curWalker.Position.x, (int)curWalker.Position.y, 0);

                if (gridHandler[curPos.x, curPos.y] != Grid.GRASS)
                {
                    tileMap.SetTile(curPos, Grass);
                    TileCount++;
                    gridHandler[curPos.x, curPos.y] = Grid.GRASS; 
                    hasGrass = true;
                }
            }

            ChanceToRemove();
            ChanceToRedirect();
            ChanceToCreate();
            UpdatePosition();

            if (hasGrass) {
                yield return new WaitForSeconds(WaitTime);
            }
        }
        StartCoroutine(CreateWaters());
    }

    IEnumerator CreateWaters()
    {
        for (int x = 1; x < gridHandler.GetLength(0) - 1; x++)
        {
            for (int y = 1; y < gridHandler.GetLength(1) - 1; y++)
            {
                if (gridHandler[x, y] == Grid.GRASS)
                {
                    bool hasWater = false;

                    if (gridHandler[x + 1, y] == Grid.EMPTY)
                    {
                        tileMap.SetTile(new Vector3Int(x + 1, y, 0), Water);
                        gridHandler[x + 1, y] = Grid.WATER;
                        hasWater = true;
                    }
                    if (gridHandler[x - 1, y] == Grid.EMPTY)
                    {
                        tileMap.SetTile(new Vector3Int(x - 1, y, 0), Water);
                        gridHandler[x - 1, y] = Grid.WATER;
                        hasWater = true;
                    }
                    if (gridHandler[x, y + 1] == Grid.EMPTY)
                    {
                        tileMap.SetTile(new Vector3Int(x, y + 1), Water);
                        gridHandler[x, y + 1] = Grid.WATER;
                        hasWater = true;
                    }
                    if (gridHandler[x, y - 1] == Grid.EMPTY)
                    {
                        tileMap.SetTile(new Vector3Int(x, y - 1), Water);
                        gridHandler[x, y - 1] = Grid.WATER;
                        hasWater = true;
                    }

                    if (hasWater)
                    {
                        yield return new WaitForSeconds(WaitTime);
                    }
                }
            }
        }
        StartCoroutine(CreateSand());
    }

    IEnumerator CreateSand()
    {
        for (int x = 1; x < gridHandler.GetLength(0) - 1; x++)
        {
            for (int y = 1; y < gridHandler.GetLength(1) - 1; y++)
            {
                if (gridHandler[x, y] == Grid.WATER)
                {
                    bool hasSand = false;

                    if (gridHandler[x + 1, y] == Grid.GRASS)
                    {
                        tileMap.SetTile(new Vector3Int(x + 1, y, 0), Sand);
                        gridHandler[x + 1, y] = Grid.SAND;
                        hasSand = true;
                    }
                    if (gridHandler[x - 1, y] == Grid.GRASS)
                    {
                        tileMap.SetTile(new Vector3Int(x - 1, y, 0), Sand);
                        gridHandler[x - 1, y] = Grid.SAND;
                        hasSand = true;
                    }
                    if (gridHandler[x, y + 1] == Grid.GRASS)
                    {
                        tileMap.SetTile(new Vector3Int(x, y + 1), Sand);
                        gridHandler[x, y + 1] = Grid.SAND;
                        hasSand = true;
                    }
                    if (gridHandler[x, y - 1] == Grid.GRASS)
                    {
                        tileMap.SetTile(new Vector3Int(x, y - 1), Sand);
                        gridHandler[x, y - 1] = Grid.SAND;
                        hasSand = true;
                    }

                    if (hasSand)
                    {
                        yield return new WaitForSeconds(WaitTime);
                    }
                }
            }
        }
        StartCoroutine(FillWater());
    }

    IEnumerator FillWater()
    {
        
        for (int x = 0; x < gridHandler.GetLength(0); x++)
        {
            for (int y = 0; y < gridHandler.GetLength(1); y++)
            {
                bool waterFilled = false;
                if (gridHandler[x, y] == Grid.EMPTY) {
                    tileMap.SetTile(new Vector3Int(x, y, 0), Water);
                    waterFilled = true;
                }
                
                if (waterFilled)
                {
                    yield return new WaitForSeconds(WaitTime);
                }
            }
        }
    }

        void ChanceToRemove()
    {
        int UpdatedCount = Walkers.Count;
        for (int i = 0; i < UpdatedCount; i++)
        {
            if (UnityEngine.Random.value < Walkers[i].ChangeChance && Walkers.Count > 1)
            {
                Walkers.RemoveAt(i);
                UpdatedCount--;
            }
        }
    }
    void ChanceToRedirect()
    {
        for (int i = 0; i < Walkers.Count; i++)
        {
            if (UnityEngine.Random.value < Walkers[i].ChangeChance)
            {
                WalkerObject curWalker = Walkers[i];
                curWalker.Direction = GetDirection();
                Walkers[i] = curWalker;
            }
        }
    }
    void ChanceToCreate()
    {
        int updatedCount = Walkers.Count;
        for (int i = 0; i < updatedCount; i++) {
            if (UnityEngine.Random.value < Walkers[i].ChangeChance && Walkers.Count < MaximumWalkers)
            {
                Vector2 newDirection = GetDirection();
                Vector2 newPos = Walkers[i].Position;

                WalkerObject newWalker = new WalkerObject(newPos, newDirection, 0.5f);
                Walkers.Add(newWalker);
            }
        }
    }
    void UpdatePosition()
    {
            for (int i = 0; i < Walkers.Count; i++)
            {
                WalkerObject FoundWalker = Walkers[i];
                FoundWalker.Position += FoundWalker.Direction;
                FoundWalker.Position.x = Mathf.Clamp(FoundWalker.Position.x, 1, gridHandler.GetLength(0) - 2);
                FoundWalker.Position.y = Mathf.Clamp(FoundWalker.Position.y, 1, gridHandler.GetLength(1) - 2);
                Walkers[i] = FoundWalker;
            }
        }
    }