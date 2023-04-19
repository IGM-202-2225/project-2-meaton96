using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flyer : Agent {

    private float liftAmount;
    private float liftChangeTimer, liftChangeTime = 3f, liftChangePercent = 0.1f;
    private float maxBoundsDistance = 1700f;
    private float minBoundsDistance = 1600f;
    protected override void Awake() {
        AssignClassData("flyer");
        liftAmount = gravityAmount;
        
        targetTags = new();
        base.Awake();
        animator.SetBool("isFlapping", true);
        frictionEnabled = false;
        gravityEnabled = false;
    }

    public override void Update() {
        Lift();
        base.Update();
    }
    private void Lift() {
        if (liftChangeTime < liftChangeTimer) {
          //  liftAmount = Random.Range(liftAmount * (1 - liftChangePercent),
          //      liftAmount * (1 + liftChangePercent));
            liftChangeTimer = 0;
        }
        else {
            liftChangeTimer += Time.deltaTime;
        }

        acceleration += new Vector3(0f, liftAmount, 0f);
    }
    protected override Vector3 StayInBounds() {
        var distanceToCenter = Vector3.Distance(
            new Vector3(transform.position.x, 0f, transform.position.z),
            Vector3.zero);
        
        if (distanceToCenter > minBoundsDistance) {
            return stayInBoundsPower * 
                (distanceToCenter - minBoundsDistance) / (maxBoundsDistance - minBoundsDistance) *
                new Vector3(-transform.position.x, 0f, -transform.position.z);
        }
        return Vector3.zero;
        
    }
    protected override Vector3 AvoidTrees() { return Vector3.zero; }


}
