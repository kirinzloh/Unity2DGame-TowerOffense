using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public Text health;
    public Text gold;
    public Text upgradeCost;
    public Text sellPrice;
    public GameObject upgradePanel;

    public ProjectilePool projectilePool { get; set; }
    public TowerBtn ClickedBtn { get; private set; }
	public MonsterBtn ClickedMtrBtn { get; private set;}

    // When empty tiles are clicked, they are highlighted so that towers can be built on them
    public void onTileClick(PlayTile tile) {
        if (tile.state == TileData.State.EMPTY && selectedTile==null) {
            selectedTile = tile;
            tile.highlight();
        } else if (tile.Equals(selectedTile)) {
            selectedTile = null;
            tile.unhighlight();
        }
    }

    // Called when tower button is clicked
    public void buildTower() {
        if (selectedTile == null) { return; } //To prevent deduction of gold even when no tiles are highlighted
        if (ClickedBtn.price > ownGameState.gold) { return; }
        ownGameState.gold -= ClickedBtn.price;
        GameObject tower = (GameObject)Instantiate(ClickedBtn.towerPrefab, selectedTile.transform.position, Quaternion.identity);
        tower.GetComponent<SpriteRenderer>().sortingOrder = selectedTile.coord.row;
        tower.transform.SetParent(selectedTile.transform);
        selectedTile.state = TileData.State.TOWER;
        selectedTile.unhighlight();
        selectedTile = null;
    }

	// Called when monster button is clicked
	public void spawnMonster() {
		if (ClickedMtrBtn.price > ownGameState.gold) { return; }
		ownGameState.gold -= ClickedMtrBtn.price;
		GameObject monster = (GameObject)Instantiate(ClickedMtrBtn.monsterPrefab, path[0].transform.position, Quaternion.identity);
		monster.GetComponent<SpriteRenderer>().sortingOrder = path[0].coord.row;
		//monster.transform.SetParent ();
	}

    // To toggle display of upgrade panel when a tower is clicked
    public void DisplayUpgradePanel(string towerType)
    {
        if (upgradePanel.activeSelf)
        {
            upgradePanel.SetActive(false);
        }
        else
        {   // There must be a better way to do this, create new data structure for towers?
            //switch (towerType)
            //{
            //    case "ArrowTower":
            //        upgradeCost.text = "$2";
            //        sellPrice.text = "$0";
            //        break;
            //    case "AttackAuraTower":
            //        upgradeCost.text = "$2";
            //        sellPrice.text = "$0";
            //        break;
            //    case "BallistaTower":
            //        upgradeCost.text = "$2";
            //        sellPrice.text = "$0";
            //        break;
            //    case "CannonTower":
            //        upgradeCost.text = "$2";
            //        sellPrice.text = "$0";
            //        break;
            //    case "FrozeTower":
            //        upgradeCost.text = "$2";
            //        sellPrice.text = "$0";
            //        break;
            //    case "GoldTower":
            //        upgradeCost.text = "$2";
            //        sellPrice.text = "$0";
            //        break;
            //    case "InfernoTower":
            //        upgradeCost.text = "$2";
            //        sellPrice.text = "$0";
            //        break;
            //    case "MortarTower":
            //        upgradeCost.text = "$2";
            //        sellPrice.text = "$0";
            //        break;
            //    case "PoisonTower":
            //        upgradeCost.text = "$2";
            //        sellPrice.text = "$0";
            //        break;
            //    case "SpeedAuraTower":
            //        upgradeCost.text = "$2";
            //        sellPrice.text = "$0";
            //        break;
            //    case "StoneTower":
            //        upgradeCost.text = "$2";
            //        sellPrice.text = "$0";
            //        break;
            //    case "TeslaTower":
            //        upgradeCost.text = "$2";
            //        sellPrice.text = "$0";
            //        break;
            //    case "WizardTower":
            //        upgradeCost.text = "$2";
            //        sellPrice.text = "$0";
            //        break;

            //}
            upgradePanel.SetActive(true);
        }
    }

    // To determine which tower button was selected
    public void SelectTower(TowerBtn towerBtn)
    {
        this.ClickedBtn = towerBtn;
    }

	// To determine which monster button was selected
	public void SelectMonster(MonsterBtn monsterBtn)
	{
		this.ClickedMtrBtn = monsterBtn;
	}

    // To initalize the projectile pool
    private void Awake()
    {
        projectilePool = GetComponent<ProjectilePool>();
    }

    // Method used for releasing game objects such as projectiles
    public void ReleaseObject(GameObject gameObject)
    {
        gameObject.SetActive(false);
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
        health.text = ownGameState.hp.ToString();
        gold.text = "$" + ownGameState.gold.ToString();
    }
}
