using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildMap : MonoBehaviour {

    public int numRows;
    public int numCols;
    // scaling and spacing should generally be the same number. Unity inspector overrides, remember to set in unity inspector.
    public float scaling = 1; // Scales size of tiles only. (default tile sprite should be 1 game unit width)
    public float spacing = 1; // Spacing in game units.
    // cached resources
    public BuildTile tileScript;
    public Text instructions;
    public Button readyButton;

    // Data/reference attributes for buildpath.
    private List<BuildTile> path;
    private BuildTile[,] grid;
    private BuildTile[] validTiles = new BuildTile[3]; // Index: 0: Up, 1: Right, 2: Down. (Clockwise)
    private bool disabled = false;
    
    BuildTile getTile(int i, int j) {
        if (i < 0 || j < 0 || i >= grid.GetLength(0) || j >= grid.GetLength(1)) {
            return null;
        }
        return grid[i, j];
    }

    public void onTileClick(BuildTile tile) {
        if (disabled) { return; }
        // Check for start tile.
        if (path.Count == 0) {
            if (tile.coord.col == 0) {
                tile.setSprite(TSprites.startTile);
                tile.state = TileData.State.PATH;
                tile.startDirection = TileData.Direction.LEFT;
                tile.endDirection = TileData.Direction.RIGHT;
                path.Add(tile);
                validTiles[0] = null;
                validTiles[1] = getTile(tile.coord.row, tile.coord.col + 1);
                validTiles[2] = null;
                validTiles[1].highlight();
                instructions.text = "Build path for monsters attacking you";
            }
            return;
        }

        // Check for subsequent tiles
        BuildTile prev = path[path.Count - 1];
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
            tile.startDirection = TileData.Direction.UNSET;
            tile.setSprite(TSprites.emptyTile);
            tile.state = TileData.State.EMPTY;
            path.RemoveAt(path.Count - 1);
            readyButton.interactable = false;
            if (path.Count == 0) {
                instructions.text = "Pick your starting point";
                return;
            }
            tile = path[path.Count - 1];
            if (path.Count == 1) {
                validTiles[0] = null;
                validTiles[1] = getTile(tile.coord.row, tile.coord.col + 1);
                validTiles[2] = null;
                validTiles[1].highlight();
                instructions.text = "Build path for monsters attacking you";
                return;
            } else {
                tile.endDirection = TileData.Direction.UNSET;
            }
        } else { // Adding tile.
            // Set directions.
            switch (valid) {
                case 0: // Going up.
                    prev.endDirection = TileData.Direction.UP;
                    tile.startDirection = TileData.Direction.DOWN;
                    break;
                case 1: // Going right.
                    prev.endDirection = TileData.Direction.RIGHT;
                    tile.startDirection = TileData.Direction.LEFT;
                    break;
                case 2: // Going down.
                    prev.endDirection = TileData.Direction.DOWN;
                    tile.startDirection = TileData.Direction.UP;
                    break;
            }

            // Set the prev tile sprite. (2nd tile ownwards)
            if (path.Count >= 2) {
                Sprite prev_s = TSprites.decideSprite(prev.startDirection, prev.endDirection);
                if (prev_s != null) {
                    prev.setSprite(prev_s);
                }
            }

            tile.state = TileData.State.PATH;
            path.Add(tile);

            // Check if is last tile.
            if (tile.coord.col + 1 >= numCols) {
                tile.setSprite(TSprites.endTile);
                instructions.text = "Path is complete, are you ready?";
                readyButton.interactable = true;
                return;
            }
        }

        // Set sprite
        Sprite s = TSprites.decideSprite(tile.startDirection, tile.endDirection);
        if (s != null) {
            tile.setSprite(s);
        }

        // Set valid tiles
        int cur_row = tile.coord.row;
        int cur_col = tile.coord.col;
        // Up
        BuildTile v_tile = getTile(cur_row - 1, cur_col);
        if (v_tile != null && !path.Contains(v_tile)) {
            v_tile.highlight();
            validTiles[0] = v_tile;
        }
        // Right
        v_tile = getTile(cur_row, cur_col + 1);
        if (v_tile != null && !path.Contains(v_tile)) {
            v_tile.highlight();
            validTiles[1] = v_tile;
        }
        // Down
        v_tile = getTile(cur_row + 1, cur_col);
        if (v_tile != null && !path.Contains(v_tile)) {
            v_tile.highlight();
            validTiles[2] = v_tile;
        }
    }
    
    public void ready() {
        foreach (Button btn in GameObject.FindObjectsOfType<Button>()) {
            btn.interactable = false;
        };
        instructions.text = "Please wait for other player...";
        MapData map = new MapData(numRows, numCols);
        map.setPath(path);
        map.setGrid(grid);
        // DEBUG MODE. DELETE WHEN DONE!
        System.IO.FileStream x = System.IO.File.Create("map.dat");
        byte[] mapbyte = map.serializeNew();
        x.Write(mapbyte, 0, mapbyte.Length);
        x.Close();
        Debug.Log("Map data written to: " + System.IO.Path.GetFullPath(x.Name));
        // DEBUG END
        GameManager gm = GameManager.instance;
        gm.getOwnGameState().map = map;
        gm.buildReady();
    }

    public void reset() {
        path.Clear();
        for (int i = 0; i < validTiles.Length; ++i) {
            validTiles[i] = null;
        }
        for (int i = 0; i < numRows; i++) {
            for (int j = 0; j < numCols; j++) {
                BuildTile tile = grid[i, j];
                tile.setSprite(TSprites.emptyTile);
                tile.state = TileData.State.EMPTY;
                tile.startDirection = TileData.Direction.UNSET;
                tile.endDirection = TileData.Direction.UNSET;
                tile.unhighlight();
            }
        }
        readyButton.interactable = false;
        instructions.text = "Pick your starting point";
    }

    public void cancel() {
        GameManager.instance.Cancel();
    }

    void Awake() {
        path = new List<BuildTile>();
        grid = new BuildTile[numRows, numCols];

        float topEdge = (float)(spacing * (numRows / 2.0 - 0.5));
        float leftEdge = (float)-(spacing * (numCols / 2.0 - 0.5));
        for (int i = 0; i < numRows; ++i) {
            GameObject row = new GameObject("row");
            row.transform.parent = gameObject.transform;
            row.transform.localPosition = new Vector3(0, topEdge - (spacing * i), 0);
            for (int j = 0; j < numCols; ++j) {
                BuildTile tile = Instantiate(tileScript);
                tile.transform.parent = row.transform;
                tile.transform.localPosition = new Vector3(leftEdge + (spacing * j), 0, 0);
                tile.transform.localScale = new Vector3(scaling, scaling, 0);
                tile.tileData.coord = new Coord(i, j);
                tile.mapScript = this;
                grid[i, j] = tile;
            }
        }
    }

    void Start() {
        readyButton.interactable = false;
        instructions.text = "Pick your starting point";
    }
}
