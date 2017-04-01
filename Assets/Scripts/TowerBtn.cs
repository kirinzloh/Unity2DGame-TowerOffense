using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBtn : MonoBehaviour {
	public GameObject towerPrefab;
	public Sprite sprite;
	public int price;

	public void OnMouseDown() {
		if (MainGameScript.Instance.tileSel && MainGameScript.Instance.selectedTile!=null && MainGameScript.Instance.gold>=price) {
			MainGameScript.Instance.gold -= price;
			MainGameScript.Instance.PlaceTower (MainGameScript.Instance.selectedTile, towerPrefab);
		}
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
