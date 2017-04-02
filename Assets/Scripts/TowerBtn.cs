using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBtn : MonoBehaviour {
	public GameObject towerPrefab;
    public PlayMap playMap;
	public Sprite sprite;
	public int price;

	public void OnMouseDown() {
        playMap.buildTower(towerPrefab, price);
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
