using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    public enum State { EMPTY, TOWER, PATH, OBSTACLE}

    public Coord coord;
    public MapScript mapScript;
    public State state;

    void OnMouseDown() {
        mapScript.onClick(this);
    }

    void Awake() {
        state = State.EMPTY;
    }

    // Use this for initialization
    void Start () {
    }
    
    // Update is called once per frame
    void Update () {
    }
}
