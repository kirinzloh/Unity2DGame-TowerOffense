using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MapEditor : MonoBehaviour {

    public MapScript mapScript;
    
    void Start () {
    }

    // DISABLE SCRIPT IF YOU DON'T WANT TO AUTO-UPDATE THE MAP.
    // VERY IMPORTANT: DISABLE THIS SCRIPT IN PLAY MODE AS WELL OR IT WILL RESET THE MAP.
    // Update is called whenever anything on the gameObject changes.
    // May make the editor slow if you edit other stuff on map because it rebuilds the map repeatedly.
    // It's possible to make it rebuild only when a relevant property changes, but that takes too much time.
    void Update () {
        // Destroy previous map fiirst
        List<GameObject> prev_map = new List<GameObject>();
        foreach (Transform child in gameObject.transform){
            prev_map.Add(child.gameObject);
        }
        foreach (GameObject child in prev_map) {
            GameObject.DestroyImmediate(child);
        }

        // Make new map
        mapScript.grid = new Tile[mapScript.numRows, mapScript.numCols];

        float topEdge = (float)(mapScript.spacing * (mapScript.numRows / 2.0 - 0.5));
        float leftEdge = (float)-(mapScript.spacing * (mapScript.numCols / 2.0 - 0.5));
        for (int i = 0; i < mapScript.numRows; ++i)
        {
            GameObject row = new GameObject("row");
            row.transform.parent = gameObject.transform;
            row.transform.localPosition = new Vector3(0, topEdge - (mapScript.spacing * i), 0);
            for (int j = 0; j < mapScript.numCols; ++j)
            {
                Tile tile = Instantiate(mapScript.tileScript);
                tile.transform.parent = row.transform;
                tile.transform.localPosition = new Vector3(leftEdge + (mapScript.spacing * j), 0, 0);
                tile.transform.localScale = new Vector3(mapScript.scaling, mapScript.scaling, 0);
                tile.coord = new Coord(i, j);
                tile.mapScript = mapScript;
                mapScript.grid[i, j] = tile;
            }
        }
    }
}
