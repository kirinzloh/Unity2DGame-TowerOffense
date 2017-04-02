using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateTester : MonoBehaviour {

    public MapData map;

    void Awake() {
        // This is far more tedious than I expected.
        map = new MapData(8,12);
    }

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
