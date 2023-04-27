using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Player : PhysicsObject {

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameController gameController;
    protected float movingPower = 1000;
    protected float turningSpeed = .1f;
    [SerializeField] private float speed;
    public Transform targetObject;

    private bool freeCam;
    private Vector3 playerLocation;
    private Quaternion playerRotation;

    [SerializeField] private GameObject hudImage, crossHair;

    [SerializeField] protected float jumpingPower;
    [SerializeField] protected float frictionPercent;
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
        frictionAmount = movingPower * frictionPercent;
        gravityEnabled = true;
        mass = 50;
        speed = 20;
        jumpingPower = 35000f;
        Cursor.lockState = CursorLockMode.Locked;
        freeCam = false;
        
        //gravityAmount = 30f;
    }
    public override void Update() {

        if (!freeCam) {
            HandleInput();
            
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

            if (headBob) {
                if (velocity.magnitude > 0f) {
                    //headbob
                }
            }
            base.Update();
            
           

        } else {
            CameraKeyboardMovement();
        }

        if (Input.GetKeyDown(KeyCode.Tab)) {
            velocity = Vector3.zero;    
            hudImage.SetActive(freeCam);
            crossHair.SetActive(freeCam);
            freeCam = !freeCam;
            
            if (freeCam) {
                playerLocation = transform.position;
                playerRotation = transform.rotation;
            }
            else {
                transform.position = playerLocation;
                transform.rotation = playerRotation;
            }
        }

        mousePos.x += Input.GetAxis("Mouse X") * sensitivity;
        mousePos.y += Input.GetAxis("Mouse Y") * sensitivity;

        transform.localRotation = Quaternion.Euler(-mousePos.y, mousePos.x, 0f);

        //CameraKeyboardMovement();

        //ZoomAmount += Input.GetAxis("Mouse ScrollWheel");
        //  ZoomAmount = Mathf.Clamp(ZoomAmount, -MaxToClamp, MaxToClamp);
        //var translate = Mathf.Min(Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")), MaxToClamp - Mathf.Abs(ZoomAmount));
        //  gameObject.transform.Translate(0, 0, translate * ROTSpeed * Mathf.Sign(Input.GetAxis("Mouse ScrollWheel")));
        
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
          //  ApplyFriction(2);
        }
        if (CheckForGround()) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                ApplyForce(jumpingPower * transform.up);
            }
        }
        if (Input.GetMouseButtonDown(0)) {
            Bullet bullet = Instantiate(bulletPrefab, Camera.main.transform.position, Quaternion.identity).GetComponent<Bullet>();
            bullet.Fire(Camera.main.transform.forward, gameController);
        }


    }

    void CameraKeyboardMovement() {
        if (Input.GetKey(KeyCode.W)) {
            transform.Translate(speed * Time.deltaTime * Vector3.up);
        }
        if (Input.GetKey(KeyCode.A)) {
            transform.Translate(speed * Time.deltaTime * Vector3.left);
        }
        if (Input.GetKey(KeyCode.S)) {
            transform.Translate(speed * Time.deltaTime * Vector3.down);
        }
        if (Input.GetKey(KeyCode.D)) {
            transform.Translate(speed * Time.deltaTime * Vector3.right);
        }
        if (Input.GetKey(KeyCode.LeftShift)) {
            transform.Translate(0, 0, speed * 2 * Time.deltaTime);
        }
    }
    private void OnDrawGizmos() {
        Gizmos.DrawLine(transform.position, transform.forward);
    }
    

}
