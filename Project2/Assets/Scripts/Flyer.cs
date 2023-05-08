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



    protected override Vector3 StayInBounds() {
        var distanceToCenter = Vector3.Distance(transform.position, new Vector3(0, transform.position.y, 0));

        if (distanceToCenter > minBoundsDistance) {
            return stayInBoundsPower * 
                (distanceToCenter - minBoundsDistance) / (maxBoundsDistance - minBoundsDistance) *
                new Vector3(-transform.position.x, 0f, -transform.position.z);
        }
        return Vector3.zero;

    }
    protected override Vector3 AvoidTrees() { return Vector3.zero; }


}
