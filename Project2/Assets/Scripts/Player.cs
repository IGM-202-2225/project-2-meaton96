using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : PhysicsObject {

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameController gameController;
    protected float movingPower = 500;
    protected float turningSpeed = .1f;
    [SerializeField] private float speed;
    public Transform targetObject;

    

    [SerializeField] protected float jumpingPower;

    bool headBob = true;
    protected float maxSpeed = 10f;
    protected float maxSpeedSprint = 20f;
    bool isSprinting;
    [SerializeField] protected GameObject playerCharacter;

    Vector2 mousePos;
    public float sensitivity = 1.5f;
    // Start is called before the first frame update
    void Start() {
        frictionEnabled = true;
        frictionAmount = movingPower / 2f;
        gravityEnabled = true;
        mass = 50;
        jumpingPower = 500f;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public override void Update() {
        HandleInput();
        mousePos.x += Input.GetAxis("Mouse X") * sensitivity;
        mousePos.y += Input.GetAxis("Mouse Y") * sensitivity;

        transform.localRotation = Quaternion.Euler(-mousePos.y, mousePos.x, 0f);

        isSprinting = !Input.GetKey(KeyCode.LeftShift);

        if (isSprinting) {
            if (velocity.magnitude > maxSpeedSprint) {
                velocity = velocity.normalized * maxSpeedSprint;
            }
        }
        else {
            if (velocity.magnitude > maxSpeed) {
                velocity = velocity.normalized * maxSpeed;
            }
        }

        if (headBob ) {
            if (velocity.magnitude > 0f) {

            }
        }

        //CameraKeyboardMovement();

        //ZoomAmount += Input.GetAxis("Mouse ScrollWheel");
      //  ZoomAmount = Mathf.Clamp(ZoomAmount, -MaxToClamp, MaxToClamp);
        //var translate = Mathf.Min(Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")), MaxToClamp - Mathf.Abs(ZoomAmount));
      //  gameObject.transform.Translate(0, 0, translate * ROTSpeed * Mathf.Sign(Input.GetAxis("Mouse ScrollWheel")));
        base.Update();
    }
    protected void HandleInput() {
        if (Input.GetKey(KeyCode.W)) {
            ApplyForce(movingPower * transform.forward);
        }
        if (Input.GetKey(KeyCode.S)) {
            ApplyForce(movingPower * -.5f * transform.forward);

        }
        if (Input.GetKey(KeyCode.D)) {
            ApplyForce(movingPower * transform.right);
        }
        if (Input.GetKey(KeyCode.A)) {
            ApplyForce(movingPower * -transform.right);
        }
        else {
            //velocity = new Vector3(0f, velocity.y, 0f);
            ApplyFriction(2);
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            ApplyForce(jumpingPower * transform.up);
        }
        if (Input.GetMouseButtonDown(0)) {
            Bullet bullet = Instantiate(bulletPrefab, Camera.main.transform.position, Quaternion.identity).GetComponent<Bullet>();
            bullet.transform.localScale = new Vector3(2, 2, 2);
            bullet.Fire(Camera.main.transform.forward, gameController);
        }


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
