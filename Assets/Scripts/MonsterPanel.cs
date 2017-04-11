using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterPanel : MonoBehaviour {

	public GameObject MonsterButton;

	// Use this for initialization
	void Start () {
		Debug.Log ("Here");
		PlayMap playMap  = Object.FindObjectOfType<PlayMap>();
		Transform grid = transform.GetChild(0);
		foreach (Monster monsterPrefab in MonsterR.getBaseMonsters()) {
			Debug.Log ("COUNT");
			GameObject monsterbtn = Instantiate(MonsterButton);
			monsterbtn.name = monsterPrefab.name + " button";
			monsterbtn.transform.SetParent(grid, false);
			monsterbtn.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
			monsterbtn.transform.Translate (Vector3.up * 10000);
			monsterbtn.GetComponent<Image>().sprite = monsterPrefab.GetComponent<SpriteRenderer>().sprite;
			monsterbtn.GetComponent<Button>().onClick.AddListener(delegate { playMap.onMonsterBtnClick(monsterPrefab); });
			monsterbtn.transform.GetChild(0).GetComponent<Text>().text = "$" + monsterPrefab.price;
		}
	}
}
