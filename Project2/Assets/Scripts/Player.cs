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
    private float shootSoundRadius = 50f;

    //private bool paused = false;

    [SerializeField] private GameObject hudImage, crossHair;

    protected float jumpingPower;
    protected float frictionPercent = .5f;
    bool headBob = true;
    protected float maxSpeed = 10f;
    protected float maxSpeedSprint = 20f;
    bool isSprinting;
   // [SerializeField] protected GameObject playerCharacter;

    protected float playerHeight = 3.5f;

    Vector2 mousePos;
    public float sensitivity = 1.5f;

    private AudioSource gunShotSound;
    // Start is called before the first frame update
    void Start() {
        gravityEnabled = false;
        gunShotSound = GetComponent<AudioSource>();
        //gravityAmount = 30f;
    }
    public void InitPlayer() {
        frictionEnabled = true;
        frictionAmount = movingPower * frictionPercent;
        gravityEnabled = true;
        mass = 50;
        speed = 20;
        jumpingPower = 35000f;
        Cursor.lockState = CursorLockMode.Locked;
        freeCam = false;
    }
    public override void Update() {
        //Debug.Log(freeCam);
        if (gameController.mainMenu) {

        }
        else {
            if (freeCam) {
                
                CameraKeyboardMovement();
            }
            else {
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
                
                
            }

            if (Input.GetKeyDown(KeyCode.Tab)) {
                velocity = Vector3.zero;
                hudImage.SetActive(freeCam);
                crossHair.SetActive(freeCam);
                gravityEnabled = freeCam;
                
                freeCam = !freeCam;

                if (freeCam) {
                    playerLocation = transform.position;
                    playerRotation = transform.rotation;
                }
                else {
                    transform.SetPositionAndRotation(playerLocation, playerRotation);
                }
            }
            //if (Input.GetKeyDown(KeyCode.Escape)) {
            //    Time.timeScale = paused ? 1f : 0f;
            //    paused = !paused;
            //}

            mousePos.x += Input.GetAxis("Mouse X") * sensitivity;
            mousePos.y += Input.GetAxis("Mouse Y") * sensitivity;

            transform.localRotation = Quaternion.Euler(-mousePos.y, mousePos.x, 0f);
        }
        //CameraKeyboardMovement();

        //ZoomAmount += Input.GetAxis("Mouse ScrollWheel");
        //  ZoomAmount = Mathf.Clamp(ZoomAmount, -MaxToClamp, MaxToClamp);
        //var translate = Mathf.Min(Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")), MaxToClamp - Mathf.Abs(ZoomAmount));
        //  gameObject.transform.Translate(0, 0, translate * ROTSpeed * Mathf.Sign(Input.GetAxis("Mouse ScrollWheel")));

    }
    public override bool CheckForGround() {
        float total = 0;
        total += terrain.SampleHeight(new Vector3(transform.position.x - .5f, 0f, transform.position.z));
        total += terrain.SampleHeight(new Vector3(transform.position.x + .5f, 0f, transform.position.z));
        total += terrain.SampleHeight(new Vector3(transform.position.x, 0f, transform.position.z + .5f));
        total += terrain.SampleHeight(new Vector3(transform.position.x, 0f, transform.position.z - .5f));
        total /= 4.0f;
        if (transform.position.y <= total + playerHeight + 3.6) {
            Vector3 pos = transform.position;
            pos.y = total + playerHeight;
            velocity.y = 0f;
            //transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime);
            transform.position = pos;
            return true;
        }
        return false;
    }
    protected override void HandleGroundCollision() { }
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
            FireBullet();
            
        }


    }
    private void FireBullet() {
        Bullet bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity).GetComponent<Bullet>();
        bullet.Fire(transform.forward, gameController);
        Debug.Log(transform.forward);
        gunShotSound.Play();

        gameController.
            GetChunk(transform.position).
            GetAgentsInsideCircle(transform.position, shootSoundRadius).
            ForEach(agent => agent.FleeTarget(transform));

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
