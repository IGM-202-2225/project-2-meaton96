using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IntroBehaviour : MonoBehaviour {
    [SerializeField] private float speed = 75f;
    [SerializeField] private GameController gameController;
    [SerializeField] private GameObject startButton;
    [SerializeField] private TextMeshProUGUI introText;
    [SerializeField] private GameObject introTextObject;
   // [SerializeField] private GameObject GaiaController;
    private bool isFlying = false;
    private bool isShowingText = false;
    private int index = 0;

    private readonly string[] introStrings = {
        "You are about to be dropped onto an island infested with dinosaurs...",
        "Thankfully you are armed with an infinite ammo rocket launcher",
        "WASD to move around, Space to Jump, Left click to fire a rocket",
        "Try to survive..."
    };


    [SerializeField] private float fadeInTime = 1f, onScreenTime = 5f, fadeSteps = 20;

    // Start is called before the first frame update
    void Start() {
        Time.timeScale = 0f;
    }

    // Update is called once per frame
    void Update() {
        if (isFlying) {
            Vector3 pos = transform.position;
            pos.x += speed * Time.deltaTime;
            transform.position = pos;
            
            if (!isShowingText && index < introStrings.Length) {
                isShowingText = true;
                StartCoroutine(ShowNextText()); 
            }
            
        }
        if (transform.position.x > 0 ) {
            gameController.Drop();
            GetComponent<AudioSource>().volume = 0;

       //    GaiaController.SetActive(true);
            
        }
        else if (transform.position.x > 5000) {
            Destroy(gameObject);

        }
    }

    public void BeginFlight() {
        isFlying = true;
        startButton.SetActive(false);
        introText.gameObject.SetActive(true);
        Color color = introText.color;
        color.a = 0;
        introText.color = color;
        Time.timeScale = 1f;    
        
    }
    public void ByPassFlight() {
        startButton.SetActive(false);
        transform.position = new Vector3(1f, 500f, 0f);
        isFlying = true;
        Time.timeScale = 1f;
    }
    private IEnumerator ShowNextText() {
        introText.text = introStrings[index];
        Color color;
        for (int x = 0; x < fadeInTime; x++) {
            color = introText.color;
            color.a += 255 / fadeSteps;
            introText.color = color;
            yield return new WaitForSeconds(fadeInTime/fadeSteps);
        }
        yield return new WaitForSeconds(onScreenTime);
        index++;
        isShowingText = false;
        color = introText.color;
        color.a = 0;
        introText.color = color;
    }

}
