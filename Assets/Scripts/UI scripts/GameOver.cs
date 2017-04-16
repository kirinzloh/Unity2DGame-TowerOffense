using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour {
    
    public Text results;

    // Use this for initialization
    void Start () {
        PlayerGameState ownGS = GameManager.instance.getOwnGameState();
        PlayerGameState opponentGS = GameManager.instance.getOpponentGameState();
        if (!ownGS.winner && !opponentGS.winner) {
            results.text = "Draw!";
            SoundManager.instance.PlayLose();
        } else if (ownGS.winner) {
            results.text = "You Win!";
            SoundManager.instance.PlayWin();
        } else {
            results.text = "You Lose!";
            SoundManager.instance.PlayLose();
        }
        results.text += "\n";
        if (GameManager.instance.timeout) {
            results.text += "(Timed out)";
        }
        results.text += "\n";
        results.text += "Your HP: " + ownGS.hp + "\n";
        results.text += "Your Gold: " + ownGS.gold + "\n";
        results.text += "Your Gold spent on monsters: " + ownGS.monsterGoldSpent + "\n";
        results.text += "\n";
        results.text += "Opponent HP: " + opponentGS.hp + "\n";
        results.text += "Opponent Gold: " + opponentGS.gold + "\n";
        results.text += "Opponent Gold spent on monsters: " + opponentGS.monsterGoldSpent + "\n";
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
