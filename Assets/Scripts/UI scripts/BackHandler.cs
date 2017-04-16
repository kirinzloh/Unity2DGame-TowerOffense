using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackHandler : MonoBehaviour {

    void Update () {
        if (Input.GetKey(KeyCode.Escape)) {
            if (SceneManager.GetActiveScene().buildIndex == 0) {
                Application.Quit();
            } else {
                SceneManager.LoadScene(0);
            }
        }
    }
}
