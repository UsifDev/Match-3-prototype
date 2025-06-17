using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardState
{
    public int Width { get; }
    public int Height { get; }

    public int[,] TileTypes { get; private set; }
    
    private int _itemPrefabsCount;

    public BoardState(int width, int height, int itemPrefabs)
    {
        Width = width;
        Height = height;
        _itemPrefabsCount = itemPrefabs;
        TileTypes = new int[width, height];
    }

    public static BoardState CreateBoard(int width, int height, int itemPrefabs)
    {
        var board = new BoardState(width, height, itemPrefabs);
        board.GenerateBoard();
        return board;
    }

    private void GenerateBoard()
    {
        for (int x = 0; x < Width; x++)
        for (int y = 0; y < Height; y++)
        TileTypes[x, y] = GetNonMatchingTileType(x, y);
    }

    private int GetNonMatchingTileType(int x, int y)
    {
        var validTypes = Enumerable.Range(0, _itemPrefabsCount)
            .Where(type => !CreatesMatch(x, y, type))
            .ToList();

        return validTypes.Count > 0
            ? validTypes[Random.Range(0, validTypes.Count)]
            : Random.Range(0, _itemPrefabsCount);
    }

    private bool CreatesMatch(int x, int y, int newType)
    {
        bool ans = false;
        if (x >= 2)         ans = TileTypes[x - 1, y] == TileTypes[x - 2, y] && TileTypes[x - 1, y] == newType;
        if (!ans && y >= 2) ans = TileTypes[x, y - 1] == TileTypes[x, y - 2] && TileTypes[x, y - 1] == newType;
        return ans;
    }

    public void SwapTiles(Vector2Int pos1, Vector2Int pos2)
    {
        (TileTypes[pos1.x, pos1.y], TileTypes[pos2.x, pos2.y]) = (TileTypes[pos2.x, pos2.y], TileTypes[pos1.x, pos1.y]);
    }

    public IEnumerable<Vector2Int> FindMatches()
    {
        var matches = new HashSet<Vector2Int>();

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                // Horizontal matches
                if (x < Width - 2 &&
                    TileTypes[x, y] == TileTypes[x + 1, y] &&
                    TileTypes[x, y] == TileTypes[x + 2, y])
                {
                    matches.UnionWith(new[]
                    {
                        new Vector2Int(x, y),
                        new Vector2Int(x+1, y),
                        new Vector2Int(x+2, y)
                    });
                }

                // Vertical matches
                if (y < Height - 2 &&
                    TileTypes[x, y] == TileTypes[x, y + 1] &&
                    TileTypes[x, y] == TileTypes[x, y + 2])
                {
                    matches.UnionWith(new[]
                    {
                        new Vector2Int(x, y),
                        new Vector2Int(x, y+1),
                        new Vector2Int(x, y+2)
                    });
                }
            }
        }

        return matches;
    }
}