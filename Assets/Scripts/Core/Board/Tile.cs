using Core.Constants.Tiles;

namespace Core
{
    public struct Tile
    {
        public Background background;
        public Item item;
        public Foreground foreground;
        public Tile(Background backgroundType, Item itemType, Foreground foregroundType)
        {
            background = backgroundType;
            item = itemType;
            foreground = foregroundType;
        }
    }
}