using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayMap : ViewMap {

    // Data/reference attributes for playing.
    public PlayTile selectedTile;

    // UI stuff
    public Text gold;
    public Text towerName;
    public Text towerStats;
    public GameObject upgradePanel;
    public GameObject towerInfo;
    public Button upgradeTowerBtn;
    public Text upgradeCost;
    public Button sellTowerBtn;
    public Text sellPrice;

    // To Deprecate
    public ProjectilePool projectilePool { get; set; }

    // Tile click selects tiles. Click again to unselect.
    public void onTileClick(PlayTile tile) {
        PlayTile prev = selectedTile;
        DeselectTile();
        if (tile.Equals(prev)) { return; }
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
    }
    
    // Called when tower button is clicked
    public void onTowerBtnClick(Tower towerPrefab) {
        if (selectedTile == null || selectedTile.state == TileData.State.TOWER) {
            DisplayTowerInfo(towerPrefab);
            if (selectedTile != null) {
                DeselectTile();
            }
        } else {
            // Try to build tower
            if (towerPrefab.price > GameState.gold) { return; }
            GameState.gold -= towerPrefab.price;
            Tower tower = Instantiate(towerPrefab);
            selectedTile.setTower(tower);
            selectedTile.unhighlight();
            selectedTile = null;
        }
    }

    // To toggle display of upgrade panel when a tower is clicked
    public void DisplayUpgradePanel(Tower tower)
    {
        // INCOMPLETE
        if (tower.upgradeCost > 0) {
            upgradeCost.text = "$" + tower.upgradeCost;
            upgradeTowerBtn.interactable = true;
        }
        else if (tower.upgradeCost <= 0)
        {
            upgradeCost.text = "MAX";
            upgradeTowerBtn.interactable = false;
        }
        sellPrice.text = "$" + (tower.price / 2);
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
                          "ATK SPEED: " + (1 / tower.delay);
        towerInfo.SetActive(true);
    }
    public void HideTowerInfo()
    {
        towerInfo.SetActive(false);
    }

    public void DeselectTile() {
        if (selectedTile == null) { return; }
        selectedTile.unhighlight();
        selectedTile = null;
        HideUpgradePanel();
        HideTowerInfo();
    }

    // Upgrade towers
    public void UpgradeTower()
    {
        if (selectedTile.tower.upgradeCost > GameState.gold || selectedTile.tower.upgradeCost<=0) { return; }
        GameState.gold -= selectedTile.tower.upgradeCost;
        int currentTowerID = selectedTile.tower.towerId;
        int upgradedTowerID = currentTowerID + 1;
        Tower upgradedTower = TowerR.getById(upgradedTowerID);
        Tower tower = Instantiate(upgradedTower);
        selectedTile.setTower(tower);
        // Refresh display.
        DisplayUpgradePanel(tower);
        DisplayTowerInfo(tower);
    }

    // Sell towers (not working yet)
    public void SellTower()
    {
        GameState.gold += selectedTile.tower.price/2;
        selectedTile.removeTower();
        DeselectTile();
    }

    // To initalize the projectile pool
    private void Awake()
    {
        projectilePool = GetComponent<ProjectilePool>();
    }

    // Use this for initialization
    void Start () {
        GameState = GameManager.instance.getOwnGameState();
        GameState.viewMapRef = this;
        initMap();
    }
    
    // Update is called once per frame
    void Update () {
        health.text = GameState.hp.ToString();
        gold.text = "$" + GameState.gold.ToString();
    }
}
