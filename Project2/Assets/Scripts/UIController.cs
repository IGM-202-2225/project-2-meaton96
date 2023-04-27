using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour {
    // [SerializeField] private TextMeshProUGUI DinoText;
    //[SerializeField] private TextMeshProUGUI avoidText;
    [SerializeField] private TextMeshProUGUI buildText;
    //[SerializeField] private GameController gameController;


    // [SerializeField] private GameObject mainMenu;
    [SerializeField] private NumDinoScript numDinoScript;
    [SerializeField] private List<TextMeshProUGUI> dinoInputFields = new();


   


    // Start is called before the first frame update
    void Start() {
        //Time.timeScale = 0f;
        buildText.text = "0.1";
    }

    // Update is called once per frame
    void Update() {

    }
    public void StartSimulation() {
        //   Time.timeScale = 1f;

        //int count = 0;
        //numDinos.ForEach(dinoField => {
        //    int spawnNum = int.Parse(dinoField.GetComponent<InputField>().textComponent.text);
        //    StartCoroutine(gameController.SpawnAgents(count, spawnNum, FlagDoneSpawning));
        //    count++;
        //});

        //for (int x = 0; x < dinoInputFields.Count; x++) {
        //  //  Debug.Log(x + ", " + int.TryParse(dinoInputFields[x].text, out numDinoScript.numDinos[x]));
        //   // Debug.Log(dinoInputFields[x].text);


        //}
        SceneManager.LoadScene(1);

    }



}
