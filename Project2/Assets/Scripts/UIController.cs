using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI DinoText;
    [SerializeField] private TextMeshProUGUI avoidText;
    [SerializeField] private TextMeshProUGUI buildText;
    [SerializeField] private GameController gameController;

    [SerializeField] private List<GameObject> uiComponents = new();
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private List<GameObject> numDinos;
    
    // Start is called before the first frame update
    void Start() {
        Time.timeScale = 0f;
    }

    // Update is called once per frame
    void Update() {
        if (gameController.mainMenu) {
            
        }
        else {
            DinoText.text = gameController.agentPrefabs[gameController.dinoIndex].name.ToString();
            avoidText.text = "Avoiding Obstacles:\n" + gameController.agentsAvoidObj;
            buildText.text = "Build Number: " + gameController.buildNumber;
        }
    }
    
    

}
