using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameScript : Singleton<MainGameScript> {
	private Color32 emptyColor = new Color32 (96, 255, 90, 255);
	public bool tileSel = false;
	public Tile selectedTile;
	public int gold;
	public int health;
	public void clickTile(MapScript mapScript, Tile tile) {
		if (tile.state==Tile.State.EMPTY && !tileSel) {
			selectedTile = tile;
			tile.state = Tile.State.SELECTED;
			tile.ColorTile (emptyColor);
			tileSel = true;
		}
		if (tile.state == Tile.State.SELECTED && tileSel) {
			selectedTile = null;
			tile.state = Tile.State.EMPTY;
			tile.ColorTile (Color.white);
			tileSel = false;
		}
	}
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void PlaceTower(Tile tile, GameObject towerPrefab) {
		GameObject tower = (GameObject)Instantiate (towerPrefab, tile.transform.position, Quaternion.identity);
		tower.GetComponent<SpriteRenderer> ().sortingOrder = tile.coord.row;
		tower.transform.SetParent (tile);
		tile.state = Tile.State.TOWER;
		tile.ColorTile (Color.white);
	}
}
