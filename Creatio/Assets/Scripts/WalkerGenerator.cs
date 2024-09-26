using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WalkerGenerator : MonoBehaviour
{
    public enum Grid
    {
        GRASS,
        WATER,
        EMPTY
    }

    public Grid[,] gridHandler;
    public List<WalkerObject> Walkers;
    public Tilemap tileMap;
    public Tile GRASS;
    public Tile WATER;

    public int MapWidth = 4096;
    public int MapHeight = 4096;

    public int MaximumWalkers = 10;
    public int TileCount = default;
    public float FillPercent = 0.8f;
    public float Wait = 0.01f;

    void Start()
    {
        InitializeGrid();
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
        tileMap.SetTile(TileCenter, GRASS);
        Walkers.Add(curWalker);

        TileCount++;

        StartCoroutine(CreateFloors());
    }

    IEnumerator CreateFloors()
    {
        while ((float)TileCount / (float)gridHandler.Length < FillPercent)
        {
            bool hasFloor = false;
            foreach (WalkerObject curWalker in Walkers) {
                Vector3Int curPos = new Vector3Int((int)curWalker.Position.x, (int)curWalker.Position.y, 0);

                if (gridHandler[curPos.x, curPos.y] == Grid.EMPTY)
                {
                    tileMap.SetTile(curPos, GRASS);
                    gridHandler[curPos.x, curPos.y] = Grid.GRASS;
                    TileCount++;
                    hasFloor = true;
                }
            }

            ChanceToRemove();
            ChanceToRedirect();
            ChanceToCreate();
            UpdatePosition();

            if (hasFloor) {
                yield return new WaitForSeconds(Wait);
            }

            StartCoroutine(CreateWalls());
        }
    }

    IEnumerator CreateWalls()
    {
        for (int x = 0; x < gridHandler.GetLength(0); x++)
        {
            for (int y = 0; y < gridHandler.GetLength(1); y++)
            {
                if (gridHandler[x, y] == Grid.GRASS)
                {
                    bool hasWall = false;

                    if (gridHandler[x + 1, y] == Grid.EMPTY)
                    {
                        tileMap.SetTile(new Vector3Int(x + 1, y, 0), WATER);
                        gridHandler[x + 1, y] = Grid.WATER;
                        hasWall = true;
                    }
                    if (gridHandler[x - 1, y] == Grid.EMPTY)
                    {
                        tileMap.SetTile(new Vector3Int(x - 1, y, 0), WATER);
                        gridHandler[x - 1, y] = Grid.WATER;
                        hasWall = true;
                    }
                    if (gridHandler[x, y + 1] == Grid.EMPTY)
                    {
                        tileMap.SetTile(new Vector3Int(x, y + 1), WATER);
                        gridHandler[x, y + 1] = Grid.WATER;
                        hasWall = true;
                    }
                    if (gridHandler[x, y - 1] == Grid.EMPTY)
                    {
                        tileMap.SetTile(new Vector3Int(x, y - 1), WATER);
                        gridHandler[x, y - 1] = Grid.WATER;
                        hasWall = true;
                    }

                    if (hasWall)
                    {
                        yield return new WaitForSeconds(Wait);
                    }
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