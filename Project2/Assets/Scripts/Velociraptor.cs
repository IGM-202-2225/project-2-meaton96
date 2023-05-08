using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Velociraptor : Agent {
    public float huntDistance = 300f;
    public bool isHunting = false;
    protected float flockRange = 50f;

    [Range(0f, 2f)]
    public float sepMulti = .2f;
    [Range(0f, 2f)]
    public float aliMulti = 1f;
    [Range(0f, 2f)]
    public float cohMulti = .5f;


    protected override void Awake() {
        AssignClassData("velociraptor");
        targetTags = new(new string[] { "SmallHerbivore" });
        base.Awake();
    }
    protected override Vector3 Wander() {
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

            (pursuePredictTime * (-(3 - distance) / distance));

        Vector3 desiredVelocity = (pursuePos - transform.position).normalized * maxSpeed;

        Vector3 steeringForce = desiredVelocity - velocity;
        steeringForce.y = 0;

        return steeringForce;
    }
    protected override Vector3 Flock() {

        return Seperate() * sepMulti +
                Align() * aliMulti +
                Cohesion() * cohMulti;
    }
    protected override Vector3 Seperate() {

        return base.Seperate() * .1f;
    }
    protected Vector3 Align() {
        Vector3 sum = Vector3.zero;

        List<string> tags = new() { tag };

        var agentsInRange = FilterAgentsByRangeAndTag(flockRange, tags);

        if (agentsInRange.Count == 0)
            return Vector3.zero;

        foreach (Agent agent in agentsInRange) {
            sum += agent.velocity;
        }
        sum /= agentsInRange.Count;


        Vector3 steer = (sum - velocity).normalized * maxSpeed;
        return steer;
    }
    protected Vector3 Cohesion() {
        Vector3 sum = Vector3.zero;

        List<string> tags = new() { tag };

        var agentsInRange = FilterAgentsByRangeAndTag(flockRange, tags);

        if (agentsInRange.Count == 0)
            return Vector3.zero;

        foreach (Agent agent in agentsInRange) {
            sum += agent.transform.position;
        }
        sum /= agentsInRange.Count;

        return Seek(sum);

    }

}
