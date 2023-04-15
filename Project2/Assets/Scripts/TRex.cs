using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TRex : Agent {

    public float huntDistance = 100f;

    protected override void Awake() {

        AssignClassData("trex");
        targetTags = new(new string[] { "SmallHerbivore", "LargeHerbivore", "SmallCarnivore", "Player" });
        base.Awake();
    }

    protected override void Wander() {
        
        if (pollTimer > agentPollRate) {
            pollTimer = 0;
            var nearbyTargetAgents = chunk.
                GetAgentsOfType(targetTags).
                Where(agent => Vector3.Distance(transform.position, agent.transform.position) < huntDistance).ToList();

            if (nearbyTargetAgents.Any()) {
                int index = Random.Range(0, nearbyTargetAgents.Count());
                target = nearbyTargetAgents[index].transform;
                state = State.pursuing;
            }
        }
        else {
            pollTimer += Time.deltaTime;
        }
        base.Wander();
    }
    //pursues the target transform 
    protected override void Pursue() {
        if (target == null) {
            state = State.wandering;
            return;
        }
        
        Vector3 pursuePos = transform.position + transform.gameObject.GetComponent<PhysicsObject>().velocity * pursuePredictTime;
        Vector3 desiredVelocity = (pursuePos - transform.position).normalized * maxSpeed;
        ApplyForce((desiredVelocity - velocity) * movingPower);
    }





}
