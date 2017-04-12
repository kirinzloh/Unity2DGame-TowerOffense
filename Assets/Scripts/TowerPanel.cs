using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerPanel : MonoBehaviour {

    public GameObject TowerButton;

    // Use this for initialization
    void Start () {
        PlayMap playMap  = Object.FindObjectOfType<PlayMap>();
        Transform grid = transform.GetChild(0);
        foreach (Tower towerPrefab in TowerR.getBaseTowers()) {
            GameObject towerbtn = Instantiate(TowerButton);
            towerbtn.name = towerPrefab.name + " button";
            towerbtn.transform.SetParent(grid, false);
            towerbtn.transform.localScale = new Vector3(1, 1, 1);
            towerbtn.GetComponent<Image>().sprite = towerPrefab.GetComponent<SpriteRenderer>().sprite;
            towerbtn.GetComponent<Image>().color = towerPrefab.GetComponent<SpriteRenderer>().color;
            towerbtn.GetComponent<Button>().onClick.AddListener(delegate { playMap.onTowerBtnClick(towerPrefab); });
            towerbtn.transform.GetChild(0).GetComponent<Text>().text = "$" + towerPrefab.price;
        }
    }
}
