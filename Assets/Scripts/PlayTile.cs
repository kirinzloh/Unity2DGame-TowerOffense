using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayTile : ViewTile {

    //public TileData tileData = new TileData(); // This will be a Direct reference to TileData in PlayerGameState.

    #region public api
<<<<<<< HEAD
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

    public void setSprite(Sprite sprite) {
        spriteR.sprite = sprite;
    }

    public void setTower(Tower tower) {
        this.tower = tower;
        tower.transform.SetParent(transform);
        state = TileData.State.TOWER;
        tileData.towerType = tower.towerId;
    }

    // NOT WORKING (It only destroys the tower script attached, game object still exists)
    /*public void destroyTower()
    {
        Destroy(this.tower);
        state = TileData.State.EMPTY;
        tileData.towerType = 0;
    }*/

=======
>>>>>>> refs/remotes/origin/master
    public void highlight() {
        spriteR.color = Color.green; // new Color32 (96, 255, 90, 255); for nicer green if you want.
        if (tower != null) {
            tower.ShowRange();
        }
    }

    public void unhighlight() {
        spriteR.color = Color.white;
        if (tower != null) {
            tower.HideRange();
        }
    }
    #endregion

    void OnMouseDown() {
        ((PlayMap) mapScript).onTileClick(this);
    }

    void Awake() {
        spriteR = GetComponent<SpriteRenderer>();
    }
}
