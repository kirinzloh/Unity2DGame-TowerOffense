using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpText : MonoBehaviour {

    public bool opponent;
    public Text hpText;
    private PlayerGameState gameState;

    // Use this for initialization
    void Start () {
        if (!opponent) {
            gameState = GameManager.instance.getOpponentGameState();
        } else {
            gameState = GameManager.instance.getOwnGameState();
        }
    }
    
    // Update is called once per frame
    void Update () {
        hpText.text = gameState.hp.ToString();
    }
}
