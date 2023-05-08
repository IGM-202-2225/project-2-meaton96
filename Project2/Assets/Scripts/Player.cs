using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Player : MonoBehaviour {

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameController gameController;
    protected float movingPower = 700;
    protected float turningSpeed = .1f;
    [SerializeField] private float speed;
    public Transform targetObject;

    public bool freeCam;
    private Vector3 playerLocation;
    private Quaternion playerRotation;
    private float shootSoundRadius = 50f;

    private bool paused = false;

    [SerializeField] private GameObject hudImage, crossHair;

    protected float jumpingPower;
    protected float frictionPercent = .5f;
    protected float maxSpeed = 10f;
    protected float maxSpeedSprint = 20f;
    private Rigidbody rb;
    private PhysicsObject po;
    
    // [SerializeField] protected GameObject playerCharacter;

    protected float playerHeight = 3.5f;

    Vector2 mousePos;
    public float sensitivity = 1f;

    private AudioSource gunShotSound;
    // Start is called before the first frame update
    void Start() {
        gunShotSound = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>(); 
        po = GetComponent<PhysicsObject>();
    }
    public void InitPlayer() {
        speed = 15;
        jumpingPower = 25000f;
        Cursor.lockState = CursorLockMode.Locked;
        freeCam = false;
        po.gravityEnabled = false;
    }
    public void Update() {
        if (gameController.mainMenu) {

        }
        else {
            if (freeCam) {
                CameraKeyboardMovement();
            }
            else {
                HandleInput();
                po.velocity = rb.velocity;
            }
            if (Input.GetKeyDown(KeyCode.Tab)) {
                hudImage.SetActive(freeCam);
                crossHair.SetActive(freeCam);

                freeCam = !freeCam;

                if (freeCam) {
                    playerLocation = transform.position;
                    playerRotation = transform.rotation;
                }
                else {
                    transform.SetPositionAndRotation(playerLocation, playerRotation);
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape)) {

                paused = !paused;
            }
            if (!paused) {
                mousePos.x += Input.GetAxis("Mouse X") * sensitivity;
                mousePos.y += Input.GetAxis("Mouse Y") * sensitivity;

                transform.localRotation = Quaternion.Euler(-mousePos.y, mousePos.x, 0f);
            }
        }

    }
    protected void HandleInput() {
        if (Input.GetKey(KeyCode.W)) {
            rb.velocity += maxSpeed * Time.deltaTime * transform.forward;
            //ApplyForce(movingPower * transform.forward);
        }
        if (Input.GetKey(KeyCode.S)) {
            rb.velocity += -maxSpeed * Time.deltaTime * transform.forward;
            //ApplyForce(movingPower * -.5f * transform.forward);

        }
        if (Input.GetKey(KeyCode.D)) {
            rb.velocity += maxSpeed * Time.deltaTime * transform.right;
            //ApplyForce(movingPower * transform.right);
        }
        if (Input.GetKey(KeyCode.A)) {
            rb.velocity += -maxSpeed * Time.deltaTime * transform.right;
            //ApplyForce(movingPower * -transform.right);
        }
        if (Input.GetMouseButtonDown(0)) {
            FireBullet();

        }


    }
    private void FireBullet() {
        Bullet bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity).GetComponent<Bullet>();
        bullet.Fire(transform.forward, gameController);
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



}
