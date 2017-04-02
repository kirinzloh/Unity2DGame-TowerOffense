using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterButton : MonoBehaviour {

	public ButtonScript buttonScript;

	void onMouseDown() {
		buttonScript.onClick(this);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
