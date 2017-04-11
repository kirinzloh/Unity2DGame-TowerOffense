using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayTile : ViewTile {

    //public TileData tileData = new TileData(); // This will be a Direct reference to TileData in PlayerGameState.

    #region public api
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
