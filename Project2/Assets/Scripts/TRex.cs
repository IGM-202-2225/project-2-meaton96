using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TRex : Agent {

    public float huntDistance = 500f;
    private const float DISTANCE_TO_STOP_PREDICT = 4f;
    public bool isHunting = false;
    protected override void Awake() {

        AssignClassData("trex");
        targetTags = new(new string[] { "SmallHerbivore", "LargeHerbivore", "SmallCarnivore" });
        base.Awake();
    }

    protected override void Wander() {
        if (isHunting) {
            if (pollTimer > agentPollRate) {
                pollTimer = 0;
                var targetAgents = chunk.
                    GetAgentsOfType(targetTags);


                var nearbyTargetAgents = targetAgents.Where(agent => Vector3.Distance(transform.position, agent.transform.position) < huntDistance).ToList();

                if (nearbyTargetAgents.Any()) {
                    int index = Random.Range(0, nearbyTargetAgents.Count());
                    target = nearbyTargetAgents[index].transform;
                    state = State.pursuing;
                }
            }
            else {
                pollTimer += Time.deltaTime;
            }
        }
        base.Wander();
    }
    //pursues the target transform 
    protected override void Pursue() {
        if (target == null) {
            state = State.wandering;
            return;
        }

        float distance = Vector3.Distance(transform.position, target.position);
        Vector3 pursuePos = target.position + target.gameObject.GetComponent<PhysicsObject>().velocity *

            (pursuePredictTime * (-(4 - distance) / distance));

        Vector3 desiredVelocity = (pursuePos - transform.position).normalized * maxSpeed;


        ApplyForce((desiredVelocity - velocity) * movingPower);
    }





}
