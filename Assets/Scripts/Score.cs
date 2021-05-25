using System;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {
    public PlayerMovement movement;
    public Text scoreText;
    
    private String prevText;

    void Update() {
        float distance = movement.reRootDistance + movement.body.position.z;
        String text = distance.ToString("N0") + "m";
        if (text != prevText) {
            prevText = text;
            scoreText.text = text;
        }
    }
}
