using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : PhysicsObject {
    public float firePower;
    //public List<PhysicsObject> objects;
    public List<TreeObject> trees;
    private GameController gameController;
    [SerializeField] private GameObject dinoHitExplosion, groundHitExplosion, smokeTrail;
    [SerializeField] private Terrain ground;
    [SerializeField] private AudioSource explosionSound;
    protected override void Awake() {
        base.Awake();
        firePower = 1900;
        mass = 2f;
        radius = .45f;
        explosionSound = GetComponent<AudioSource>();
        ground = GameObject.FindWithTag("Ground").GetComponent<Terrain>();
    }
    public void Fire(Vector3 spawnPos, GameController gameController) {
        this.gameController = gameController;
        ApplyForce(firePower * mass * spawnPos);
    }
    public override void Update() {
        transform.localRotation = Quaternion.LookRotation(direction);
        transform.Rotate(Vector3.right * 90f);

        if (CheckForGround()) {
            Instantiate(groundHitExplosion, transform.position, Quaternion.identity);
            explosionSound.Play();
            Destroy(gameObject);

        }

        ResolveCollision();

        base.Update();
    }
    public override bool CheckForGround() {

        return transform.position.y <= terrain.SampleHeight(transform.position);


    }
    public override bool CheckCollision(SimpleSphereCollider other) {
        return sCollider.CheckCollision(other);
    }


    public void ResolveCollision() {
        try {
            foreach (Agent obj in gameController.GetChunk(transform.position).agents) {
                if (obj.CheckCollision(sCollider)) {
                    obj.velocity = Vector3.zero;
                    obj.ApplyForce(firePower * mass * direction);
                    
                    obj.alive = false;
                    obj.isActive = false;

                    Instantiate(dinoHitExplosion, transform.position, Quaternion.identity);
                    explosionSound.Play();

                    //gameController.GetChunk(transform.position).Update();
                    Destroy(gameObject);
                    break;
                }
            }
        }
        catch (Exception) {
            Destroy(gameObject);
        }
    }


}
