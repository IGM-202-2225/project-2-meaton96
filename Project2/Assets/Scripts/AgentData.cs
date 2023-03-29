
using UnityEngine;

public class AgentData {

    public float runningSpeedMultiplier;
    public float wanderSwitchDirCooldown;
    public float turnSpeed;
    public float maxSpeed;
    public float maxRunAwayDistance;
    public float avoidanceDistance;
    public float mass;
    public float movementPower;
    public float gravityAmt;

    public override string ToString() {
        return "runningSpeedMultiplier " + runningSpeedMultiplier + "\n" +
            "wanderSwitchDirCooldown " + wanderSwitchDirCooldown + "\n" +
            "turnSpeed " + turnSpeed + "\n" +
            "maxSpeed " + maxSpeed + "\n" +
            "maxRunAwayDistance " + maxRunAwayDistance + "\n" +
            "avoidanceDistance " + avoidanceDistance;
    }
}
