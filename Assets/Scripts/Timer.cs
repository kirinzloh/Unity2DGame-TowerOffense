using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

    public Text display;
    int endTime;
    

    // Use this for initialization
    void Start () {
        endTime = GameManager.instance.startTime + GameManager.instance.gameTimeLimit;
    }
    
    // Update is called once per frame
    void Update () {
        int time = GameManager.instance.getTime();
        int timeleft = endTime - time;
        if (timeleft < 0) {
            timeleft = 0;
        }
        display.text = timeleft/60000 + ":" + (timeleft/1000)%60;
    }
}
