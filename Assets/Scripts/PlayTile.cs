using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayTile : MonoBehaviour {

    public TileData tileData = new TileData(); // This will be a Direct reference to TileData in PlayerGameState.
    public PlayMap mapScript;
    public GameObject tower; // Should probably subclass a tower class
    private SpriteRenderer spriteR;

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

    public void setSprite(Sprite sprite) {
        spriteR.sprite = sprite;
    }

    public void highlight() {
        spriteR.color = Color.green; // new Color32 (96, 255, 90, 255); for nicer green if you want.
    }

    public void unhighlight() {
        spriteR.color = Color.white;
    }
    #endregion

    void OnMouseDown() {
        mapScript.onTileClick(this);
    }

    void Awake() {
        spriteR = GetComponent<SpriteRenderer>();
    }
}
