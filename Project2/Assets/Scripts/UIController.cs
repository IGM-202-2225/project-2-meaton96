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
    private bool[] finishedSpawning = { false, false, false, false };
    // Start is called before the first frame update
    void Start() {
        Time.timeScale = 0f;
    }

    // Update is called once per frame
    void Update() {
        if (gameController.mainMenu) {
            if (AllSpawned()) {
                mainMenu.SetActive(false);
                uiComponents.ForEach(comp => comp.SetActive(true));
                gameController.mainMenu = false;
            }
        }
        else {
            DinoText.text = gameController.agentPrefabs[gameController.dinoIndex].name.ToString();
            avoidText.text = "Avoiding Obstacles:\n" + gameController.agentsAvoidObj;
            buildText.text = "Build Number: " + gameController.buildNumber;
        }
    }
    public void StartSimulation() {
        Time.timeScale = 1f;
        
        int count = 0;
        numDinos.ForEach(dinoField => {
            int spawnNum = int.Parse(dinoField.GetComponent<InputField>().textComponent.text);
            StartCoroutine(gameController.SpawnAgents(count, spawnNum, FlagDoneSpawning));
            count++;
        });


    }
    private void FlagDoneSpawning(int dinoNum) {
        finishedSpawning[dinoNum] = true;
    }
    private bool AllSpawned() {
        foreach (bool b in finishedSpawning) {
            if (!b)
                return false;
        }
        return true;
    }

}
