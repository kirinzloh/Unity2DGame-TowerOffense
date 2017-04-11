using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayMap : ViewMap {

    // Data/reference attributes for playing.
    public PlayTile selectedTile;

    // UI stuff
    public Text gold;
    public Text upgradeCost;
    public Text sellPrice;
    public GameObject upgradePanel;

    // To Deprecate
    public ProjectilePool projectilePool { get; set; }

    // When empty tiles are clicked, they are highlighted so that activity can be done.
    public void onTileClick(PlayTile tile) {
        if (selectedTile==null) {
            switch (tile.state) {
                case TileData.State.TOWER:
                    DisplayUpgradePanel(tile.tower);
                    selectedTile = tile;
                    tile.highlight();
                    break;
                case TileData.State.EMPTY:
                    selectedTile = tile;
                    tile.highlight();
                    break;
                default:
                    break;
            }
        } else if (tile.Equals(selectedTile)) {
            selectedTile = null;
            tile.unhighlight();
            HideUpgradePanel();
        }
    }
    
    // Called when tower button is clicked
    public void onTowerBtnClick(Tower towerPrefab) {
        if (selectedTile == null || selectedTile.state == TileData.State.TOWER) {
            // Display info here.
            // NOT IMPLEMENTED YET. // DEBUG
            // Deselect tile if is a tower.
            if (selectedTile != null) {
                selectedTile = null;
                selectedTile.unhighlight();
            }
        } else {
            // Try to build tower
            if (towerPrefab.price > GameState.gold) { return; }
			GameState.gold -= towerPrefab.price;
            Tower tower = Instantiate(towerPrefab, selectedTile.transform.position, Quaternion.identity);
            //tower.GetComponent<SpriteRenderer>().sortingOrder = selectedTile.coord.row;
            selectedTile.setTower(tower);
            selectedTile.unhighlight();
            selectedTile = null;
        }
    }

    // Called when monster button is clicked (on opponent side)
    /*
<<<<<<< PREVIOUS
	public void spawnMonster() {
		if (ClickedMtrBtn.price > GameState.gold) { return; }
		GameState.gold -= ClickedMtrBtn.price;
		//Debug.Log (path[0].transform.position);
		GameObject monster = (GameObject)Instantiate(ClickedMtrBtn.monsterPrefab, path[0].transform.position, Quaternion.identity);
		Monster monster_mtr = monster.GetComponent<Monster> ();
		monster_mtr.playMap = this;
		monster.GetComponent<SpriteRenderer>().sortingOrder = path[0].coord.row;
    }
=======
	public void onMonsterBtnClick(Monster monsterPrefab) {
		if (monsterPrefab.price > ownGameState.gold) { return; }
		Monster monster = Instantiate (monsterPrefab, path [0].transform.position, Quaternion.identity);
		monster.playMap = this;
	}
>>>>>>> master (NEW)
    */

    // To toggle display of upgrade panel when a tower is clicked
    public void DisplayUpgradePanel(Tower tower)
    {
        // INCOMPLETE
        if (tower.upgradeCost > 0) {
            upgradeCost.text = "$" + tower.upgradeCost;
        }
        sellPrice.text = "$" + (int) tower.price / 2;
        upgradePanel.SetActive(true);
    }

    public void HideUpgradePanel() {
        upgradePanel.SetActive(false);
    }

    // To initalize the projectile pool
    private void Awake()
    {
        projectilePool = GetComponent<ProjectilePool>();
    }

    // Use this for initialization
    void Start () {
        GameState = GameManager.instance.getOwnGameState();
        initMap();
    }
    
    // Update is called once per frame
    void Update () {
        health.text = GameState.hp.ToString();
        gold.text = "$" + GameState.gold.ToString();
    }
}
