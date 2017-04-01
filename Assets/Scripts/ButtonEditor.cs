using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ButtonEditor : MonoBehaviour {

	public ButtonScript buttonScript;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		// destroy previous buttons
		List<GameObject> prev_buttons = new List<GameObject>();
        foreach (Transform child in gameObject.transform){
            prev_buttons.Add(child.gameObject);
        }
		foreach (GameObject child in prev_buttons) {
            GameObject.DestroyImmediate(child);
        }

		buttonScript.monsterButtons = new MonsterButton[buttonScript.monsterButtonNum];
		for (int j = 0; j < buttonScript.monsterButtonNum; ++j)
		{
			MonsterButton monster_button = Instantiate(buttonScript.monsterButtonScript);
			monster_button.transform.parent = gameObject.transform;
			monster_button.tag = j;
			monster_button.transform.localPosition = new Vector3((buttonScript.spacing * j), 0, 0);
			monster_button.transform.localScale = new Vector3(buttonScript.scaling, buttonScript.scaling, 0);
			monster_button.buttonScript = buttonScript;
		}
	}
}
