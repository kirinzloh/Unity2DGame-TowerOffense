using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour {

	public MonsterButton monsterButtonScript;
	public int monsterButtonNum = 3;
	public int towerButtonNum = 4;
	public MonsterButton[] monsterButtons;
	public float scaling = 0.25f; // Scales size of buttons only.
    public float spacing = 3f; // Spacing in game units.
    public float x = -6f;
    public float y = -4.8f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void onClick(MonsterButton monsterButton) {
		if (monsterButton.tag == "Monster0") {

		}
		else if (monsterButton.tag == "Monster1") {

		}
		else if (monsterButton.tag == "Monster2") {
			
		}
	}
}
