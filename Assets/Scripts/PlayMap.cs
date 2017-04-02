using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMap : MonoBehaviour {

    PlayerGameState ownGameState;

    public int numRows;
    public int numCols;
    // scaling and spacing should generally be the same number. Unity inspector overrides, remember to set in unity inspector.
    public float scaling = 1; // Scales size of tiles only. (default tile sprite should be 1 game unit width)
    public float spacing = 1; // Spacing in game units.
    // cached resources
    public PlayTile tileScript;

    // Data/reference attributes for playing.
    private List<PlayTile> path;
    private PlayTile[,] grid;
    public PlayTile selectedTile;

    public void onTileClick(PlayTile tile) {
        if (tile.state == TileData.State.EMPTY && selectedTile==null) {
            selectedTile = tile;
            tile.highlight();
        } else if (tile.Equals(selectedTile)) {
            selectedTile = null;
            tile.unhighlight();
        }
    }

    public void buildTower(GameObject towerPrefab, int price) { // Should probably subclass a tower class
        if (price > ownGameState.gold) { return; }
        ownGameState.gold -= price;
        GameObject tower = Instantiate(towerPrefab, selectedTile.transform.position, Quaternion.identity);
        tower.GetComponent<SpriteRenderer>().sortingOrder = selectedTile.coord.row; // Probably better to set the tower prefab to a higher layer itself.
        tower.transform.SetParent(selectedTile.transform);
        selectedTile.state = TileData.State.TOWER;
        selectedTile.unhighlight();
        selectedTile = null;
    }

    // Use this for initialization
    void Start () {
        ownGameState = GameManager.instance.getOwnGameState();
        MapData map = ownGameState.map;
        /*if (!PhotonNetwork.connected) { // DEBUG MODE
            map = ownGameState.GetComponent<GameStateTester>().map;
        }*/

        numRows = map.numRows;
        numCols = map.numCols;
        path = new List<PlayTile>();
        grid = new PlayTile[numRows, numCols];
        selectedTile = null;

        // Generate tiles
        float topEdge = (float)(spacing * (numRows / 2.0 - 0.5));
        float leftEdge = (float)-(spacing * (numCols / 2.0 - 0.5));
        for (int i = 0; i < numRows; ++i) {
            GameObject row = new GameObject("row");
            row.transform.parent = gameObject.transform;
            row.transform.localPosition = new Vector3(0, topEdge - (spacing * i), 0);
            for (int j = 0; j < numCols; ++j) {
                PlayTile tile = Instantiate(tileScript);
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
        for (int i = 1; i < map_path.Count-1; ++i) {
            td = map_path[i];
            grid[td.coord.row, td.coord.col].setSprite(TSprites.decideSprite(td.startDirection,td.endDirection));
            path.Add(grid[td.coord.row, td.coord.col]);
        }
        td = map_path[map_path.Count-1];
        grid[td.coord.row, td.coord.col].setSprite(TSprites.endTile);
        path.Add(grid[td.coord.row, td.coord.col]);
    }
    
    // Update is called once per frame
    void Update () {
        
    }
}
