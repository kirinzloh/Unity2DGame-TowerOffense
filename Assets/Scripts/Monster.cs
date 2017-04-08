using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour {

	private string monsterType;


	// Use this for initialization
	void Start () {
		this.name = monsterType;
	}

	// Update is called once per frame
	void Update () {

	}

	void OnMouseDown()
	{
        Object.FindObjectOfType<PlayMap>().DisplayUpgradePanel(monsterType);
	}
}
