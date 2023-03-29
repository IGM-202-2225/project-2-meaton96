using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrossHairBehavior : MonoBehaviour {

    public Sprite sprite;
    public bool active = false;

    // Update is called once per frame
    void Update() {
        //not wokring
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            gameObject.SetActive(active);
            active = !active;
        }
    }
}
