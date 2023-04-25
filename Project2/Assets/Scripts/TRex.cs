using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TRex : Agent {

    public float huntDistance = 500f;
    public bool isHunting = false;
    protected override void Awake() {

        AssignClassData("trex");
        targetTags = new(new string[] { "SmallHerbivore", "LargeHerbivore", "SmallCarnivore" });
        base.Awake();
    }

    protected override Vector3 Wander() {
        if (isHunting) {
            if (pollTimer > agentPollRate) {
                pollTimer = 0;

                var nearbyTargetAgents = FilterAgentsByRangeAndTag(huntDistance, targetTags);

                if (nearbyTargetAgents.Any()) {
                    int index = Random.Range(0, nearbyTargetAgents.Count());
                    target = nearbyTargetAgents[index].transform;
                    state = State.pursuing;
                    return Vector3.zero;
                }
            }
            else {
                pollTimer += Time.deltaTime;
            }
        }
        return base.Wander();
    }
    //pursues the target transform 
    protected override Vector3 Pursue() {
        if (target == null) {
            state = State.wandering;
            return Vector3.zero;
        }

        float distance = Vector3.Distance(transform.position, target.position);
        Vector3 pursuePos = target.position + target.gameObject.GetComponent<PhysicsObject>().velocity *

            (pursuePredictTime * (-(4 - distance) / distance));

        Vector3 desiredVelocity = (pursuePos - transform.position).normalized * maxSpeed;


        return desiredVelocity - velocity;
    }





}
