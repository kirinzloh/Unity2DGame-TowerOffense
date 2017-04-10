using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// DEPRECATED

public class TowerBtn : MonoBehaviour {
	public GameObject towerPrefab;
	public int price;
    public Text priceText;


	// Use this for initialization
	void Start () {
        priceText.text = "$" + price.ToString();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

}
