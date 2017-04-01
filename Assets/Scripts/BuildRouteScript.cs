using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class BuildRouteScript {

    static Sprite emptyTile = Resources.Load<Sprite>("Tiles/unoccupied");
    static Sprite startTile = Resources.Load<Sprite>("Tiles/start-tile");
    static Sprite horiTile = Resources.Load<Sprite>("Tiles/horizontal");
    static Sprite vertTile = Resources.Load<Sprite>("Tiles/vertical");
    static Sprite ULTile = Resources.Load<Sprite>("Tiles/up-left");
    static Sprite URTile = Resources.Load<Sprite>("Tiles/up-right");
    static Sprite DLTile = Resources.Load<Sprite>("Tiles/down-left");
    static Sprite DRTile = Resources.Load<Sprite>("Tiles/down-right");
    static Sprite endTile = Resources.Load<Sprite>("Tiles/end-tile");

    public static void clickTile(MapScript map, Tile tile, Tile[] validTiles) {
        List<Tile> path = map.getPath();
        // Check for start tile.
        if (path.Count == 0) {
            if (tile.coord.col == 0) {
                tile.setSprite(startTile);
                tile.state = Tile.State.PATH;
                tile.startDirection = Tile.Direction.LEFT;
                tile.endDirection = Tile.Direction.RIGHT;
                path.Add(tile);
                validTiles[0] = null;
                validTiles[1] = map.getTile(tile.coord.row, tile.coord.col + 1);
                validTiles[2] = null;
                validTiles[1].highlight();
                GameObject.Find("Instructions").GetComponent<Text>().text = "Build path for monsters attacking you";
            }
            return;
        }

        // Check for subsequent tiles
        Tile prev = path[path.Count - 1];
        int valid = Array.IndexOf(validTiles, tile);
        if (valid < 0 && !tile.Equals(prev)) { return; }

        // Clear valid tiles.
        for (int i = 0; i < validTiles.Length; ++i) {
            if (validTiles[i] != null) {
                validTiles[i].unhighlight();
                validTiles[i] = null;
            }
        }

        // check if is undo.
        if (tile.Equals(prev)) { // Undo tile.
            tile.startDirection = Tile.Direction.UNSET;
            tile.setSprite(emptyTile);
            tile.state = Tile.State.EMPTY;
            path.RemoveAt(path.Count - 1);
            GameObject.Find("Ready Button").GetComponent<Button>().interactable = false;
            if (path.Count == 0) {
                GameObject.Find("Instructions").GetComponent<Text>().text = "Pick your starting point";
                return;
            }
            tile = path[path.Count - 1];
            if (path.Count == 1) {
                validTiles[0] = null;
                validTiles[1] = map.getTile(tile.coord.row, tile.coord.col + 1);
                validTiles[2] = null;
                validTiles[1].highlight();
                GameObject.Find("Instructions").GetComponent<Text>().text = "Build path for monsters attacking you";
                return;
            } else {
                tile.endDirection = Tile.Direction.UNSET;
            }
        } else { // Adding tile.
            // Set directions.
            switch (valid) {
                case 0: // Going up.
                    prev.endDirection = Tile.Direction.UP;
                    tile.startDirection = Tile.Direction.DOWN;
                    break;
                case 1: // Going right.
                    prev.endDirection = Tile.Direction.RIGHT;
                    tile.startDirection = Tile.Direction.LEFT;
                    break;
                case 2: // Going down.
                    prev.endDirection = Tile.Direction.DOWN;
                    tile.startDirection = Tile.Direction.UP;
                    break;
            }

            // Set the prev tile sprite. (2nd tile ownwards)
            if (path.Count >= 2) {
                Sprite prev_s = decideSprite(prev.startDirection, prev.endDirection);
                if (prev_s != null) {
                    prev.setSprite(prev_s);
                }
            }

            tile.state = Tile.State.PATH;
            path.Add(tile);

            // Check if is last tile.
            if (tile.coord.col+1 >= map.numCols) {
                tile.setSprite(endTile);
                GameObject.Find("Instructions").GetComponent<Text>().text = "Path is complete, are you ready?";
                GameObject.Find("Ready Button").GetComponent<Button>().interactable = true;
                return;
            }
        }

        // Set sprite
        Sprite s = decideSprite(tile.startDirection, tile.endDirection);
        if (s != null) {
            tile.setSprite(s);
        }

        // Set valid tiles
        int cur_row = tile.coord.row;
        int cur_col = tile.coord.col;
        // Up
        Tile v_tile = map.getTile(cur_row - 1, cur_col);
        if (v_tile != null && !path.Contains(v_tile)) {
            v_tile.highlight();
            validTiles[0] = v_tile;
        }
        // Right
        v_tile = map.getTile(cur_row, cur_col + 1);
        if (v_tile != null && !path.Contains(v_tile)) {
            v_tile.highlight();
            validTiles[1] = v_tile;
        }
        // Down
        v_tile = map.getTile(cur_row + 1, cur_col);
        if (v_tile != null && !path.Contains(v_tile)) {
            v_tile.highlight();
            validTiles[2] = v_tile;
        }
    }

    static Sprite decideSprite(Tile.Direction start, Tile.Direction end) { // ^&#$@ hardcoded
        Sprite sprite = null;
        if (start == Tile.Direction.UP) {
            switch (end) {
                case Tile.Direction.UNSET:
                case Tile.Direction.DOWN:
                    sprite = vertTile;
                    break;
                case Tile.Direction.LEFT:
                    sprite = ULTile;
                    break;
                case Tile.Direction.RIGHT:
                    sprite = URTile;
                    break;
            }
        } else if (start == Tile.Direction.DOWN) {
            switch (end) {
                case Tile.Direction.UNSET:
                case Tile.Direction.UP:
                    sprite = vertTile;
                    break;
                case Tile.Direction.LEFT:
                    sprite = DLTile;
                    break;
                case Tile.Direction.RIGHT:
                    sprite = DRTile;
                    break;
            }
        } else if (start == Tile.Direction.LEFT) {
            switch (end) {
                case Tile.Direction.UP:
                    sprite = ULTile;
                    break;
                case Tile.Direction.DOWN:
                    sprite = DLTile;
                    break;
                case Tile.Direction.UNSET:
                case Tile.Direction.RIGHT:
                    sprite = horiTile;
                    break;
            }
        } else if (start == Tile.Direction.RIGHT) {
            switch (end) {
                case Tile.Direction.UP:
                    sprite = URTile;
                    break;
                case Tile.Direction.DOWN:
                    sprite = DRTile;
                    break;
                case Tile.Direction.UNSET:
                case Tile.Direction.LEFT:
                    sprite = horiTile;
                    break;
            }
        }
        return sprite;
    }

    public static void reset(MapScript map, Tile[] validTiles) {
        List<Tile> path = map.getPath();
        path.Clear();
        for (int i = 0; i < validTiles.Length; ++i) {
            validTiles[i] = null;
        }
        for (int i = 0; i < map.numRows; i++) {
            for (int j = 0; j < map.numCols; j++) {
                Tile tile = map.getTile(i, j);
                tile.setSprite(emptyTile);
                tile.state = Tile.State.EMPTY;
                tile.startDirection = Tile.Direction.UNSET;
                tile.endDirection = Tile.Direction.UNSET;
                tile.unhighlight();
            }
        }
        GameObject.Find("Ready Button").GetComponent<Button>().interactable = false;
        GameObject.Find("Instructions").GetComponent<Text>().text = "Pick your starting point";
    }
}
