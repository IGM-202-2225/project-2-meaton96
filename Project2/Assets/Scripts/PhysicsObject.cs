using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour {
    public Vector3 direction;
    public Vector3 velocity;
    public Vector3 acceleration;
    public float mass;
    public bool frictionEnabled;
    public float frictionAmount;
    public bool gravityEnabled;
    public float gravityAmount = 5;
    public float bounceAmount;
    public bool collisionEnabled = true;
    public float radius;
    public SimpleSphereCollider sCollider;

    public Terrain terrain;
    public void EnableCollision() {
        collisionEnabled = true;
    }
    public void DisableCollision() {
        collisionEnabled = false;
    }
    protected virtual void Awake() {
        terrain = GameObject.FindWithTag("Ground").GetComponent<Terrain>();
        sCollider = new SimpleSphereCollider(Vector3.zero, radius);
    }
    // Update is called once per frame
    public virtual void Update() {

        if (CheckForGround()) {
            if (frictionEnabled)
                ApplyFriction();
            velocity.y = 0;
            Vector3 pos = transform.position;
            pos.y = terrain.SampleHeight(new Vector3(transform.position.x, 0f, transform.position.z));
            transform.position = pos;
        }
        else if (gravityEnabled) {
            ApplyGravity();
        }
        UpdateSphereCollider();
        velocity += acceleration * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;

        direction = new Vector3(velocity.x, 0f, velocity.z);
        acceleration = Vector3.zero;

    }
    protected virtual void UpdateSphereCollider() {
        sCollider.Update(transform.position);
    }
    public virtual bool CheckForGround() {
        return terrain.SampleHeight(new Vector3(transform.position.x, 0f, transform.position.z)) >= transform.position.y;
    }
    public void ToggleCollision() {
        collisionEnabled = !collisionEnabled;
    }



    //check for circle bounds collision with other physics object
    public virtual bool CheckCollision(SimpleSphereCollider other) {
        return false;
    }


    public void ApplyForce(Vector3 force) {
        acceleration += force / mass;
    }
    public void DisableFriction() {
        frictionEnabled = false;
    }
    public void EnableFriction() {
        frictionEnabled = true;
    }
    public void SetFrictionCoeff(float coeff) {
        frictionAmount = coeff;
    }
    public void EnableGravity() {
        gravityEnabled = true;
    }
    public void DisableGravity() {
        gravityEnabled = false;
    }
    public void SetGravityAmount(float amount) {
        gravityAmount = amount;
    }
    public void SetBounceAmount(float amount) {
        bounceAmount = amount;
    }
    public void Init(bool fric, float fricAmt, bool gravity, float gravityAmt, float bounceAmt, float mass) {
        frictionEnabled = fric;
        frictionAmount = fricAmt;
        gravityEnabled = gravity;
        gravityAmount = gravityAmt;
        bounceAmount = bounceAmt;
        this.mass = mass;
    }
    public void SetMass(float mass) {
        this.mass = mass;
    }

    public void ApplyFriction() {
        Vector3 friction = -velocity;
        friction = frictionAmount * friction.normalized;
        ApplyForce(friction);
    }
    public void ApplyGravity() {
        
        acceleration += new Vector3(0f, -gravityAmount, 0f);
    }
}
