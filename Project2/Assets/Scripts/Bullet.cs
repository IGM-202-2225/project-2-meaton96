using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : PhysicsObject {
    public float firePower;
    public List<PhysicsObject> objects;
    public List<TreeObject> trees;
    private GameController gameController;
    
    protected override void Awake() {
        base.Awake();
        objects = new();
        firePower = 2500;
        mass = .5f;
        radius = .45f;

        
    }
    public void Fire(Vector3 spawnPos, List<PhysicsObject> otherObjects, GameController gameController) {
        objects = otherObjects;
        this.gameController = gameController;
        ApplyForce(spawnPos * firePower);
    }
    public override void Update() {
        transform.localRotation = Quaternion.LookRotation(direction);
        transform.Rotate(Vector3.right * 90f);

        if (CheckForGround())
            Destroy(gameObject);

        foreach (var obj in objects) {
            if (obj == null) {
                objects.Remove(obj);
                break;
            }
        }
        
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
        foreach (Agent obj in objects) {
            if (obj.CheckCollision(sCollider)) {
                obj.ApplyForce(firePower / 5f * direction);
                obj.alive = false;
                Destroy(gameObject);
            }
        }
        //not going to work
        //foreach (TreeObject tree in gameController.GetChunk(transform.position).trees) {
        //    if (tree.CheckCollision(sCollider)) {
        //        tree.ApplyForce(1000 * firePower * direction);
        //        Destroy(gameObject);
        //    }
        //}
    }


}
