using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flyer : Agent {

    private float liftAmount;
    //private float liftChangeTimer, liftChangeTime = 3f, liftChangePercent = 0.1f;
    private float maxBoundsDistance = 1024f;
    private float minBoundsDistance = 850f;
    protected override void Awake() {
        AssignClassData("flyer");
        liftAmount = gravityAmount;
        targetTags = new();
        base.Awake();
        animator.SetBool("isFlapping", true);
        frictionEnabled = false;
        gravityEnabled = false;
    }
    protected override void TeleportToMiddle() {
        transform.position = new Vector3(0, Random.Range(70, 150), 0);
    }

    public override void Update() {
       // Lift();
        //Debug.Log(velocity.ToString());
        base.Update();
    }
    private void Lift() {
        
        acceleration += new Vector3(0f, liftAmount, 0f);
    }
    protected override Vector3 StayInBounds() {
        var distanceToCenterSquared = Mathf.Pow(transform.position.x, 2) + Mathf.Pow(transform.position.z, 2);

        
        if (distanceToCenterSquared > Mathf.Pow(minBoundsDistance, 2)) {
            return stayInBoundsPower * 
                (distanceToCenterSquared - Mathf.Pow(minBoundsDistance, 2)) /
                (maxBoundsDistance - minBoundsDistance) *
                new Vector3(-transform.position.x, 0f, -transform.position.z);
        }
        return Vector3.zero;
        
    }
    protected override Vector3 AvoidTrees() { return Vector3.zero; }


}
