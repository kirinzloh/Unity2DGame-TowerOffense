using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    public enum State { EMPTY, TOWER, PATH, OBSTACLE }
    public enum Direction { UNSET, UP, DOWN, LEFT, RIGHT }

    public MapScript mapScript;
    private SpriteRenderer spriteR;

    #region public api
    public Coord coord;
    public State state; // Default EMPTY
    public Direction startDirection; // Default UNSET
    public Direction endDirection; // Default UNSET
    
    
    public void setSprite(Sprite sprite) {
        spriteR.sprite = sprite;
    }

    public void highlight() {
        spriteR.color = Color.green;
    }

    public void unhighlight() {
        spriteR.color = Color.white;
    }
    #endregion

    void OnMouseDown() {
        mapScript.onClick(this);
    }

    void Awake() {
        spriteR = GetComponent<SpriteRenderer>();
    }

    // Use this for initialization
    void Start () {
    }
    
    // Update is called once per frame
    void Update () {
    }
}
