using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    public enum State { EMPTY, TOWER, PATH, OBSTACLE,SELECTED}

    public Coord coord;
    public MapScript mapScript;
    public State state;
	public SpriteRenderer spriteRenderer;

    void OnMouseDown() {
        mapScript.onClick(this);
    }

    void Awake() {
        state = State.EMPTY;
    }

    // Use this for initialization
    void Start () {
		spriteRenderer = GetComponent<SpriteRenderer> ();
    }
    
    // Update is called once per frame
    void Update () {
    }

	public void ColorTile(Color newColor) {
		spriteRenderer.color = newColor;
	}
}
