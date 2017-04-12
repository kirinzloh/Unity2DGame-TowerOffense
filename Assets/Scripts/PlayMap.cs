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
    public Text towerName;
    public Text towerStats;
    public GameObject upgradePanel;
    public GameObject towerInfo;
    public Button upgradeTower;
    public Button sellTower;

    // To Deprecate
    public ProjectilePool projectilePool { get; set; }

    // When empty tiles are clicked, they are highlighted so that activity can be done.
    public void onTileClick(PlayTile tile) {
        if (selectedTile==null) {
            switch (tile.state) {
                case TileData.State.TOWER:
                    DisplayTowerInfo(tile.tower);
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
            HideTowerInfo();
        }
    }
    
    // Called when tower button is clicked
    public void onTowerBtnClick(Tower towerPrefab) {
        if (selectedTile == null || selectedTile.state == TileData.State.TOWER) {
            DisplayTowerInfo(towerPrefab);
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
            selectedTile.setTower(tower);
            selectedTile.unhighlight();
            selectedTile = null;
        }
    }

    
    public void spawnMonster(int monsterId) {

        /*GameObject monster = (GameObject)Instantiate(ClickedMtrBtn.monsterPrefab, path[0].transform.position, Quaternion.identity);
        Monster monster_mtr = monster.GetComponent<Monster>();
        monster_mtr.playMap = this;
        monster.GetComponent<SpriteRenderer>().sortingOrder = path[0].coord.row;*/
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

	public void onMonsterBtnClick(Monster monsterPrefab) {
		if (monsterPrefab.price > GameState.gold) { return; }
		Monster monster = Instantiate (monsterPrefab, path [0].transform.position, Quaternion.identity);
		monster.playMap = this;
	}
>>>>>>> master (NEW)
    */

    // To toggle display of upgrade panel when a tower is clicked
    public void DisplayUpgradePanel(Tower tower)
    {
        // INCOMPLETE
        if (tower.upgradeCost <= GameState.gold)
        {
            upgradeTower.onClick.AddListener(UpgradeTower);
        }
        if (tower.upgradeCost > 0) {
            upgradeCost.text = "$" + tower.upgradeCost;
        }
        else if (tower.upgradeCost <= 0)
        {
            upgradeCost.text = "MAX";
        }
        sellTower.onClick.AddListener(SellTower);
        sellPrice.text = "$" + tower.price / 2;
        upgradePanel.SetActive(true);
    }
    public void HideUpgradePanel()
    {
        upgradePanel.SetActive(false);
    }
    
    // To toggle display of tower info when a towerBtn/tower is clicked
    public void DisplayTowerInfo(Tower tower)
    {
        this.towerName.text = tower.towerName;
        towerStats.text = "DAMAGE: " + tower.damage + "\n" +
                          "RANGE: " + tower.range + "\n" +
                          "ATK SPEED: " + (1 / tower.delay) + "\n" +
                          "ATK TYPE: ";
                         
        towerInfo.SetActive(true);
    }
    public void HideTowerInfo()
    {
        towerInfo.SetActive(false);
    }

    // Upgrade towers
    public void UpgradeTower()
    {
        GameState.gold -= selectedTile.tower.upgradeCost;
        int currentTowerID = selectedTile.tower.towerId;
        int upgradedTowerID = currentTowerID + 1;
        Tower upgradedTower = TowerR.getById(upgradedTowerID);
        Tower tower = Instantiate(upgradedTower, selectedTile.transform.position, Quaternion.identity);
        selectedTile.setTower(tower);
    }

    // Sell towers (not working yet)
    public void SellTower()
    {
        GameState.gold += selectedTile.tower.price/2;
        //selectedTile.destroyTower(); // NOT WORKING
        HideUpgradePanel();
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
