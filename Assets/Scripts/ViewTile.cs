using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewTile : MonoBehaviour {

    public TileData tileData = new TileData(); // This will be a Direct reference to TileData in PlayerGameState.
    public ViewMap mapScript;
    public Tower tower;
    protected SpriteRenderer spriteR;

    #region public api
    public Coord coord {
        set { tileData.coord = value; }
        get { return tileData.coord; }
    }
    public TileData.State state {
        set { tileData.state = value; }
        get { return tileData.state; }
    }
    public TileData.Direction startDirection {
        set { tileData.startDirection = value; }
        get { return tileData.startDirection; }
    }
    public TileData.Direction endDirection {
        set { tileData.endDirection = value; }
        get { return tileData.endDirection; }
    }

    public void setTower(Tower tower) {
        if (this.tower != null) {
            Destroy(this.tower.gameObject);
        }
        this.tower = tower;
        tower.transform.SetParent(transform, false);
        state = TileData.State.TOWER;
        tileData.towerType = tower.towerId;
    }

    public void removeTower() {
        Destroy(this.tower.gameObject);
        state = TileData.State.EMPTY;
        tileData.towerType = 0;
        this.tower = null;
    }

    public void setSprite(Sprite sprite) {
        spriteR.sprite = sprite;
    }
    #endregion

    void Awake() {
        spriteR = GetComponent<SpriteRenderer>();
    }
}
