using UnityEngine;

public static class TSprites {
    public static readonly Sprite emptyTile = Resources.Load<Sprite>("Tiles/unoccupied");
    public static readonly Sprite startTile = Resources.Load<Sprite>("Tiles/start-tile");
    public static readonly Sprite horiTile = Resources.Load<Sprite>("Tiles/horizontal");
    public static readonly Sprite vertTile = Resources.Load<Sprite>("Tiles/vertical");
    public static readonly Sprite ULTile = Resources.Load<Sprite>("Tiles/up-left");
    public static readonly Sprite URTile = Resources.Load<Sprite>("Tiles/up-right");
    public static readonly Sprite DLTile = Resources.Load<Sprite>("Tiles/down-left");
    public static readonly Sprite DRTile = Resources.Load<Sprite>("Tiles/down-right");
    public static readonly Sprite endTile = Resources.Load<Sprite>("Tiles/end-tile");

    public static Sprite decideSprite(TileData.Direction start, TileData.Direction end) { // hardcoded
        Sprite sprite = null;
        if (start == TileData.Direction.UP) {
            switch (end) {
                case TileData.Direction.UNSET:
                case TileData.Direction.DOWN:
                    sprite = TSprites.vertTile;
                    break;
                case TileData.Direction.LEFT:
                    sprite = TSprites.ULTile;
                    break;
                case TileData.Direction.RIGHT:
                    sprite = TSprites.URTile;
                    break;
            }
        } else if (start == TileData.Direction.DOWN) {
            switch (end) {
                case TileData.Direction.UNSET:
                case TileData.Direction.UP:
                    sprite = TSprites.vertTile;
                    break;
                case TileData.Direction.LEFT:
                    sprite = TSprites.DLTile;
                    break;
                case TileData.Direction.RIGHT:
                    sprite = TSprites.DRTile;
                    break;
            }
        } else if (start == TileData.Direction.LEFT) {
            switch (end) {
                case TileData.Direction.UP:
                    sprite = TSprites.ULTile;
                    break;
                case TileData.Direction.DOWN:
                    sprite = TSprites.DLTile;
                    break;
                case TileData.Direction.UNSET:
                case TileData.Direction.RIGHT:
                    sprite = TSprites.horiTile;
                    break;
            }
        } else if (start == TileData.Direction.RIGHT) {
            switch (end) {
                case TileData.Direction.UP:
                    sprite = TSprites.URTile;
                    break;
                case TileData.Direction.DOWN:
                    sprite = TSprites.DRTile;
                    break;
                case TileData.Direction.UNSET:
                case TileData.Direction.LEFT:
                    sprite = TSprites.horiTile;
                    break;
            }
        }
        return sprite;
    }
}