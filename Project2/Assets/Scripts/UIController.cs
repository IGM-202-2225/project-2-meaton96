using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI controlsText;
    [SerializeField] private TextMeshProUGUI dinoText, dinoTextTwo, dinoTextThree;
    [SerializeField] private GameController gameController;
    [SerializeField] private Player player;

    // Start is called before the first frame update
    string freeCamText =
        "WASD - translate camera\n" +
        "Shift - Move forward\n" +
        "F1 - Toggle Dino AI\n" +
        "F2 - Set all Dinos pursuing player\n" +
        "F3 - Toggle all agent obstacle avoidance\n" +
        "F5 - Spawn agent at camera\n" +
        "Up/Down Arrow - Change which dino to spawn\n" + 
        "Tab - Toggle Free Camera";

    string playingText = 
        "WASD to move Space to Jump\n" + 
        "Left Click to fire rocket\n" +
        "Tab - Toggle Free Camera";
    void Start() {
     //   Time.timeScale = 0f;
       // buildText.text = "Build Number: " + gameController.buildNumber;

    }
    void Update() {
        dinoText.enabled = player.freeCam;
        dinoTextTwo.enabled = player.freeCam;
        if (player.freeCam) {
            controlsText.text = freeCamText;
        } else {
            controlsText.text = playingText;
        }
        dinoTextThree.text = gameController.agentPrefabs[gameController.dinoIndex].name;
    }

    
    
    

}
