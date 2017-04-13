using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterPanel : MonoBehaviour {

	public GameObject MonsterButton;

	// Use this for initialization
	void Start () {
		PlayMap playMap  = Object.FindObjectOfType<PlayMap>();
		Transform grid = transform.GetChild(0);
		foreach (Monster monsterPrefab in MonsterR.getAllMonsters()) {
			GameObject monsterbtn = Instantiate(MonsterButton);
			monsterbtn.name = monsterPrefab.name + " button";
			monsterbtn.transform.SetParent(grid, false);
			monsterbtn.transform.localScale = new Vector3(1,1,1);
			monsterbtn.GetComponent<Image>().sprite = monsterPrefab.GetComponent<SpriteRenderer>().sprite;
			monsterbtn.GetComponent<Image>().color = monsterPrefab.GetComponent<SpriteRenderer>().color;
			monsterbtn.GetComponent<Button>().onClick.AddListener(delegate { GameManager.instance.sendMonster(monsterPrefab); });
			monsterbtn.transform.GetChild(0).GetComponent<Text>().text = "$" + monsterPrefab.price;
		}
	}
}
