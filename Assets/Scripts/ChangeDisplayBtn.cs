using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeDisplayBtn : MonoBehaviour {

    // Add references to objects to display/hide
    public GameObject[] MainDisplay;
    public GameObject[] ViewDisplay;
    public Text buttonText;
    private bool _inView;
    public bool inView {
        get { return _inView; }
        set { _inView = value; refreshDisplay(); }
    }

    public void toggleDisplay() {
        inView = !inView; // Toggle inView flag.
        refreshDisplay();
    }

    public void refreshDisplay() {
        for (int i = 0; i < MainDisplay.Length; ++i) {
            MainDisplay[i].SetActive(!inView);
        }
        for (int i = 0; i < ViewDisplay.Length; ++i) {
            ViewDisplay[i].SetActive(inView);
        }
        if (inView) {
            buttonText.text = "My View";
        } else {
            buttonText.text = "Enemy View";
        }
        GameManager.instance.toggleOpponentSendMapData(inView); //getOpponentGameState().sendMapData = inView;
    }

    // Use this for initialization
    void Start () {
        inView = false;
    }
    
    // Update is called once per frame
    void Update () {
        
    }
}
