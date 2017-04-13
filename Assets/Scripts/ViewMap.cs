using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewMap : MonoBehaviour {

    public PlayerGameState GameState;
    public int numRows;
    public int numCols;
    // scaling and spacing should generally be the same number. Unity inspector overrides, remember to set in unity inspector.
    public float scaling = 1; // Scales size of tiles only. (default tile sprite should be 1 game unit width)
    public float spacing = 1; // Spacing in game units.
    // cached resources
    public ViewTile tileScript;

    // Data/reference attributes for playing.
    protected List<ViewTile> path;
    protected ViewTile[,] grid;

    public Text health;

    public List<ViewTile> getPath() {
        return path;
    }

    // Use this for initialization
    void Start () {
        GameState = GameManager.instance.getOpponentGameState();
        GameState.viewMapRef = this;
        initMap();
    }

    protected void initMap() {
        MapData map = GameState.map;

        numRows = map.numRows;
        numCols = map.numCols;
        path = new List<ViewTile>();
        grid = new ViewTile[numRows, numCols];

        // Generate tiles
        float topEdge = (float)(spacing * (numRows / 2.0 - 0.5));
        float leftEdge = (float)-(spacing * (numCols / 2.0 - 0.5));
        for (int i = 0; i < numRows; ++i) {
            GameObject row = new GameObject("row");
            row.transform.parent = gameObject.transform;
            row.transform.localPosition = new Vector3(0, topEdge - (spacing * i), 0);
            for (int j = 0; j < numCols; ++j) {
                ViewTile tile = Instantiate(tileScript);
                tile.transform.parent = row.transform;
                tile.transform.localPosition = new Vector3(leftEdge + (spacing * j), 0, 0);
                tile.transform.localScale = new Vector3(scaling, scaling, 0);
                tile.tileData.coord = new Coord(i, j);
                tile.mapScript = this;
                grid[i, j] = tile;
            }
        }
        // Set tile data
        for (int i = 0; i < numRows; ++i) {
            for (int j = 0; j < numCols; ++j) {
                grid[i, j].tileData = map.getTileData(i, j);
            }
        }

        // Set path
        TileData td;
        List<TileData> map_path = map.getPath();
        td = map_path[0];
        grid[td.coord.row, td.coord.col].setSprite(TSprites.startTile);
        path.Add(grid[td.coord.row, td.coord.col]);
        for (int i = 1; i < map_path.Count - 1; ++i) {
            td = map_path[i];
            grid[td.coord.row, td.coord.col].setSprite(TSprites.decideSprite(td.startDirection, td.endDirection));
            path.Add(grid[td.coord.row, td.coord.col]);
        }
        td = map_path[map_path.Count - 1];
        grid[td.coord.row, td.coord.col].setSprite(TSprites.endTile);
        path.Add(grid[td.coord.row, td.coord.col]);
    }
    
    // Update is called once per frame
    void Update () {
        health.text = GameState.hp.ToString();
        if (!GameState.sendMapData) { return; }
        for (int i = 0; i < numRows; ++i) {
            for (int j = 0; j < numCols; ++j) {
                int towerid = grid[i, j].tower == null ? 0 : grid[i, j].tower.towerId;
                if (grid[i, j].tileData.towerType != towerid) { // has tower and id same, or both no tower.
                    Tower tower = Instantiate(TowerR.getById(grid[i, j].tileData.towerType), grid[i, j].transform.position, Quaternion.identity);
                    grid[i, j].setTower(tower);
                };
            }
        }
    }
}
