using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI DinoText;
    [SerializeField] private TextMeshProUGUI avoidText;
    [SerializeField] private TextMeshProUGUI buildText;
    [SerializeField] private GameController gameController;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DinoText.text = gameController.agentPrefabs[gameController.dinoIndex].name.ToString();
        avoidText.text = "Avoiding Obstacles:\n" + gameController.agentsAvoidObj;
        buildText.text = "Build Number: " + gameController.buildNumber;
    }
}
