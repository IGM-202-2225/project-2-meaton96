using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour {
    [SerializeField] private float speed;
    public Transform targetObject;

    float ZoomAmount = 0; //With Positive and negative values
    float MaxToClamp = 10;
    float ROTSpeed = 10;

    Vector2 mousePos;
    public float sensitivity = 1f;
    private void Start() {
        
        Cursor.lockState = CursorLockMode.Locked;
    }
    // Update is called once per frame
    void Update() {

        mousePos.x += Input.GetAxis("Mouse X") * sensitivity;
        mousePos.y += Input.GetAxis("Mouse Y") * sensitivity;

        transform.localRotation = Quaternion.Euler(-mousePos.y, mousePos.x, 0f);

        CameraKeyboardMovement();

        ZoomAmount += Input.GetAxis("Mouse ScrollWheel");
        ZoomAmount = Mathf.Clamp(ZoomAmount, -MaxToClamp, MaxToClamp);
        var translate = Mathf.Min(Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")), MaxToClamp - Mathf.Abs(ZoomAmount));
        gameObject.transform.Translate(0, 0, translate * ROTSpeed * Mathf.Sign(Input.GetAxis("Mouse ScrollWheel")));

    }


    void CameraKeyboardMovement() {
        if (Input.GetKey(KeyCode.RightArrow)) {
            transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.LeftArrow)) {
            transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.DownArrow)) {
            transform.Translate(new Vector3(0, -speed * Time.deltaTime, 0));
        }
        if (Input.GetKey(KeyCode.UpArrow)) {
            transform.Translate(new Vector3(0, speed * Time.deltaTime, 0));
        }
    }
}
