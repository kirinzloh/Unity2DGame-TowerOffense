using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapScript : MonoBehaviour {

    public int numRows;
    public int numCols;
    // scaling and spacing should generally be the same number. Unity inspector overrides, remember to set in unity inspector.
    public float scaling = 1; // Scales size of tiles only. (default tile sprite should be 1 game unit width)
    public float spacing = 1; // Spacing in game units.
    
    // Data/reference attributes
    public Tile tileScript;
    private List<Tile> path;
	private List<Monster> monsters;
    public Tile[,] grid; // Public for editor to work, try not to access directly.

    public Tile getTile(int i, int j) {
        return grid[i, j];
    }

    public List<Tile> getPath() {
        return path;
    }

    public void onClick(Tile tile) {
        switch (SceneManager.GetActiveScene().name) {
            case "BuildRoute":
                BuildRouteScript.clickTile(this, tile);
                break;
        }
    }

    void Awake () {
        DontDestroyOnLoad(gameObject);
        path = new List<Tile>();
		monsters = new List<Monster>();
        /* // This is done by Map Editor instead.
        grid = new Tile[numRows, numCols];
        
        float topEdge = (float) (spacing * (numRows / 2.0 - 0.5));
        float leftEdge = (float) -(spacing * (numCols / 2.0 - 0.5));
        for (int i = 0; i < numRows; ++i) {
            GameObject row = new GameObject("row");
            row.transform.parent = gameObject.transform;
            row.transform.localPosition = new Vector3(0, topEdge - (spacing * i), 0);
            for (int j = 0; j < numCols; ++j)
            {
                Tile tile = Instantiate(tileScript);
                tile.transform.parent = row.transform;
                tile.transform.localPosition = new Vector3(leftEdge + (spacing * j), 0, 0);
                tile.transform.localScale = new Vector3(scaling, scaling, 0);
                tile.coord = new Coord(i,j);
                tile.mapScript = this;
                grid[i, j] = tile;
            }
        }
        */
    }

    void Start() {
        
    }
    
    void Update () {
        
    }
}
