using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour {

	public MonsterButton monsterButtonScript;
	public int monsterButtonNum = 3;
	public int towerButtonNum = 4;
	public MonsterButton[] monsterButtons;
	public float scaling = 0.1; // Scales size of buttons only.
    public float spacing = 10; // Spacing in game units.

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void onClick(MonsterButton monsterButton)
	{
	}
}
