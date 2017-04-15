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
                HideUpgradePanel();
                HideTowerInfo();
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
            if (selectedTile != null) {
                DeselectTile();
            }
            DisplayTowerInfo(towerPrefab);
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
    public void DisplayTowerInfo(Tower tower) {
        this.towerName.text = tower.towerName;
        List<string> info = new List<string>();
        if (tower.towerId / 10 == 8) { // gold towers
            info.Add(string.Format("EXTRA GOLD: {0}gold / {1}s", tower.damage, GameManager.instance.goldInterval));
        } else if (tower.isSupport) {
            info.Add(string.Format("MULTIPLIER: {0}", tower.supportMultiplier));
            info.Add(string.Format("RANGE: {0:f2}", tower.range));
        } else {
            info.Add(string.Format("DAMAGE: {0}", tower.damage));
            info.Add(string.Format("RANGE: {0:f2}", tower.range));
        }
        
        if (!tower.isSupport) {
            info.Add(string.Format("ATK SPEED: {0:f2}", (1 / tower.delay)));
        }
        if (tower.stunTime > 0) {
            info.Add(string.Format("STUN EFFECT: {0:f2}s", (tower.stunTime / 1000f)) );
        }
        if (tower.slowTime > 0) {
            info.Add(string.Format("SLOW EFFECT: {0:f2}s", (tower.slowTime / 1000f)));
        }
        if (tower.DOTdamage > 0) {
            info.Add("DOT DAMAGE: ");
            info.Add( string.Format("{0}dmg / {1}s", tower.DOTdamage, (tower.DOTduration / 1000f)) );
        }
        if (tower.splashRadius > 0) {
            info.Add(string.Format("SPLASH: {0:f2}", tower.splashRadius));
        }
        towerStats.text = string.Join("\n", info.ToArray());
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
