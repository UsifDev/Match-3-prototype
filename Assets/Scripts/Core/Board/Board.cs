using UnityEngine;

namespace Core
{
    public struct Board
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Tile[,] Tiles { get; private set; }
        public Board(int width, int height, int itemPrefabs)
        {
            Width = width;
            Height = height;
            Tiles = new Tile[width, height];
        }
    }
}
