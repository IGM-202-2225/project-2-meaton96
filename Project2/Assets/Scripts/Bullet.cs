using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : PhysicsObject {
    public float firePower;
    //public List<PhysicsObject> objects;
    public List<TreeObject> trees;
    private GameController gameController;

    protected override void Awake() {
        base.Awake();
        firePower = 2500;
        mass = 2f;
        radius = .45f;


    }
    public void Fire(Vector3 spawnPos, GameController gameController) {
        this.gameController = gameController;
        ApplyForce(firePower * mass * spawnPos);
    }
    public override void Update() {
        transform.localRotation = Quaternion.LookRotation(direction);
        transform.Rotate(Vector3.right * 90f);

        if (CheckForGround())
            Destroy(gameObject);



        ResolveCollision();

        base.Update();
    }
    public override bool CheckForGround() {

        return transform.position.y <= -5f;


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
