using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackHandler : MonoBehaviour {

    void Update () {
        if (Input.GetKey(KeyCode.Escape)) {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
}
