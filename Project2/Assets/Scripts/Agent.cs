using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Agent : PhysicsObject {
    protected Animator animator;                                        //pointer to animator to change animations
    [SerializeField] protected float movingPower;                       //how strong in Newtons? to call ApplyForce
    protected Transform target;                                         //current target, agent will run towards this if in chasing state
                                                                        // protected List<Agent> avoidAgents, targetAgents;                    
                                                                        // protected float runningSpeedMultiplier;                             
    protected float maxSpeed;                                           //speed cap, velocity magnitude will not exceed this value
    protected float maxSpeedWander;                                     //an initial position to run away from, this may be a sound (gunshot or something from player) 
                                                                        //or a sighting of another actor from the avoidAgents list
    protected float maxRunAwayDistance;                                 //how far away from the initial runningFrom position to get before going back to wandering
    protected float avoidanceDistance;                                  //how close to get to another agent before triggering a seperation 
    protected float stateTimer, stateSwitchTimer;                       //trackers to randomly swap between Idle and Wander states, 50/50 every time at the end of stateSiwtchTimer
    protected float agentPollRate;                                      //how often, in seconds, the agent will search for other agents around it to add to its list
    protected float pollTimer;
    protected GameController gameController;                            //pointer to game controller
    protected List<string> targetTags;                                  //which tags the agent will potentially target, should be moved to Carnivore class maybe
    public bool isActive;                                               //activates and deactivates all movement logic, physics logic is always active

    public List<SimpleSphereCollider>[] colliders = new List<SimpleSphereCollider>[3];   //contains lists of all colliders for the agent, up to 3 layers of collider logic

    protected Vector3 totalForces;

    protected const float SHORE_CHECK_RADIUS = 10f;
    protected const float SHORE_CHECK_ANGLE = .1f;

    protected float wanderCircleDistance, wanderCircleRadius, wanderAnglechange;
    protected float wanderAngle = 0f;
    protected float stayInBoundsPower = 10f;

    protected float pursuePredictTime = 2f;
    protected const float SEA_LEVEL = 27f;

    protected const float AVOID_DISTANCE = 25f, OBSTACLE_AVOID_POWER = .6f;

    public bool avoidingObstacles = true;

    protected Chunk chunk;

    public bool alive = true;
    public int _id;
    public static int id = 0;
    //behaviour states for agent
    public enum State {
        idle,
        wandering,
        fleeing,
        hurt,
        pursuing
    }

    public State state;

    //set target if necessary
    public void SetTarget(Transform target) {
        this.target = target;
    }


    protected override void Awake() {
        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        chunk = gameController.GetChunk(transform.position);
        _id = id++;
        base.Awake();

        animator = GetComponent<Animator>();

        //activate movement logic
        isActive = true;
        alive = true;
        frictionEnabled = true;

    }
    protected virtual Vector3 Pursue() { return Vector3.zero; }
    protected override void UpdateSphereCollider() {
        foreach (List<SimpleSphereCollider> colliderLists in colliders) {
            colliderLists.ForEach(collider => collider.Update(transform.position));
        }

    }
    //keeps the agent from going into the water by preventing it from going below sea level
    protected virtual Vector3 StayInBounds() {
        if (transform.position.y <= SEA_LEVEL) {
            return stayInBoundsPower * GetVectorPerpToShore();
        }
        return Vector3.zero;
    }
    //checks all around the agent in a radius of SHORE_CHECK_RADIUS
    //gets every point on the circle seperated by angle SHORE_CHECK_ANGLE
    //takes the highest point and returns a vector pointing from the agent to the highest point
    private Vector3 GetVectorPerpToShore() {
        float max = -1;
        float maxAngle = 0;
        for (float angle = 0; angle < Mathf.PI * 2; angle += SHORE_CHECK_ANGLE) {
            float x = transform.position.x + Mathf.Cos(angle) * SHORE_CHECK_RADIUS;
            float z = transform.position.z + Mathf.Sin(angle) * SHORE_CHECK_RADIUS;
            if (terrain.SampleHeight(new Vector3(x, 0, z)) > max) {
                max = terrain.SampleHeight(new Vector3(x, 0, z));
                maxAngle = angle;
            }
        }
        return new Vector3(Mathf.Cos(maxAngle), 0f, Mathf.Sin(maxAngle));

    }
    //Obstacle Avoidance
    protected virtual Vector3 AvoidTrees() {
        //testing all trees in the current chunk
        //this might cause a small issue if a tree is on the edge of a chunk and the agent is moving into that chunk it wont avoid the tree until it has swapped
        //to the same chunk as the tree
        Vector3 forces = Vector3.zero;
        foreach (TreeObject tree in chunk.trees) {
            Vector3 vecToTree = tree.transform.position - transform.position;
            //ignore if behind
            if (Vector3.Dot(vecToTree, transform.forward) < 0f) {
                continue;
            }
            //ignore if too far
            else if (Vector3.Distance(transform.position, tree.transform.position) > AVOID_DISTANCE) {
                continue;
            }
            else {
                //check for potential collision and steer right or left
                float radius = colliders[0][0].radius;
                float dotProd = Vector3.Dot(vecToTree, transform.right);
                if (Mathf.Abs(dotProd) < radius + tree.radius) {
                    forces += OBSTACLE_AVOID_POWER * ((dotProd < 0 ? 1 : -1) * transform.right);
                }
            }
        }
        return forces;
    }

    //currently working similar to flock woops
    protected Vector3 Seperate() {
        List<PhysicsObject> agentsInRange = chunk.GetAgentsOfType(
            new[] { tag }).
            FindAll(
            agent => Vector3.Distance(agent.transform.position, transform.position) < maxRunAwayDistance);
        Vector3 SeperateForce = Vector3.zero;
        foreach (Agent agent in agentsInRange.Cast<Agent>()) {
            Vector3 dir = agent.transform.position - transform.position;
            dir = -dir.normalized;
            //float distance = Vector3.Distance(agent.transform.position, transform.position);
            //float percentForce = distance / (maxRunAwayDistance - avoidanceDistance);
            SeperateForce = 1 * 4 * dir;
        }

        return SeperateForce;
    }


    public override void Update() {
        totalForces = Vector3.zero;
        //call physics object update
        base.Update();
        if (alive) {
            Chunk temp = gameController.GetChunk(transform.position);
            if (chunk != temp) {
                UpdateChunk(temp);
            }

            UpdateSphereCollider();
            CheckCollisionWithOtherAgents();
            if (isActive) {

                if (velocity.magnitude > 0) {
                    transform.rotation = Quaternion.LookRotation(direction);
                }


                //movement behaviour
                switch (state) {
                    case State.idle:
                        Idle();
                        break;
                    case State.wandering:
                        totalForces += Wander();
                        //Seperate();
                        totalForces += Seperate();
                        break;
                    case State.fleeing:
                        totalForces += Flee();
                        break;
                    case State.pursuing:
                        totalForces += Pursue();
                        break;
                }
                totalForces += StayInBounds();
                if (avoidingObstacles)
                    totalForces += AvoidTrees();

                ApplyForce(movingPower * totalForces);

            }


            //adjuist animator speed float to change between walking and running anims
            animator.SetFloat("speed", velocity.magnitude);
            if (velocity.magnitude > maxSpeed) {
                velocity = velocity.normalized * maxSpeed;
            }
        }
        else {

            animator.SetTrigger("Die");
        }

    }
    public void FleeTarget(Transform transform) {
        target = transform;
        state = State.fleeing;
    }
    public void ToggleAI() {
        isActive = !isActive;
        if (!isActive) {
            velocity = Vector3.zero;
        }
    }

    protected virtual Vector3 Flee() {
        if (target == null) {
            state = State.wandering;
            return Vector3.zero;
        }

        Vector3 desiredVelocity;

        float xV = transform.position.x - target.position.x;
        float zV = transform.position.z - target.position.z;
        desiredVelocity = new Vector3(xV, velocity.y, zV).normalized * 10;



        return desiredVelocity - velocity;

    }

    //assign class data fields that determine movement logic based on the passed in class name
    protected void AssignClassData(string className) {
        for (int x = 0; x < colliders.Length; x++) {
            colliders[x] = new();
        }

        AgentData ad = MyJsonUtility.classData[className];

        maxSpeed = ad.maxSpeed;
        maxSpeedWander = maxSpeed * ad.runningSpeedMultiplier;
        maxRunAwayDistance = ad.maxRunAwayDistance;
        avoidanceDistance = ad.avoidanceDistance;
        mass = ad.mass;
        movingPower = ad.movementPower;
        //gravityAmount = ad.gravityAmt;

        //frictionEnabled = false;
        frictionAmount = movingPower * .6f;
        //assign each of the colliders in the collider list for this class 
        //by copying them from the JsonUtilty list
        MyJsonUtility.colliderList[className].ForEach(collider => {
            colliders[collider.level].Add(collider.DeepCopy());
        });
        agentPollRate = 5f;
        state = State.wandering;
        //stateSwitchTimer = 13f;
        wanderCircleDistance = ad.circleDistance;
        wanderCircleRadius = ad.circleRadius;
        wanderAnglechange = ad.angleChange;

    }
    //returns true if the agent has a tag that is in the list of target tags this agent has
    protected bool IsATargetTag(Agent agent) {
        foreach (string targTag in targetTags) {
            if (agent.CompareTag(targTag))
                return true;
        }
        return false;
    }
    //updates the avoid agents list with all types of agents the agent is supposed to be avoiding
    //protected virtual void UpdateAgentLists() {
    //    avoidAgents = new();
    //    foreach (Agent agent in gameController.agents) {
    //        if (agent.Equals(this)) continue;

    //        if (agent.CompareTag(tag)) {
    //            avoidAgents.Add(agent);
    //        }

    //    }

    //}
    //do nothing, 50/50 chance every stateSwitchTimer to swap between idle and wander
    public void Idle() {

        velocity = Vector3.zero;
        if (stateTimer > stateSwitchTimer) {
            stateTimer = 0f;
            state = State.wandering;
        }
        else
            stateTimer += Time.deltaTime;
    }
    public void UpdateChunk(Chunk chunk) {
        this.chunk.RemoveAgentFromChunk(this);
        this.chunk = chunk;
        chunk.AddAgent(this);
    }


    protected virtual Vector3 Wander() {
        Vector3 circleCenter;
        circleCenter = new Vector3(velocity.x, velocity.y, velocity.z).normalized * wanderCircleDistance;

        Vector3 displacement = Vector3.down * wanderCircleRadius;

        displacement.x = Mathf.Cos(wanderAngle) * displacement.magnitude;
        displacement.z = Mathf.Sin(wanderAngle) * displacement.magnitude;

        wanderAngle += UnityEngine.Random.Range(-wanderAnglechange, wanderAnglechange);

        Vector3 wanderForce = displacement + circleCenter;

        return wanderForce;

    }
    public virtual Vector3 Seek() {
        Vector3 desiredVelocity = (target.position - transform.position).normalized * maxSpeed;
        return desiredVelocity - velocity;

    }

    //sets up agent turning to direction rotation

    protected void CheckCollisionWithOtherAgents() {
        foreach (Agent agent in chunk.agents) {
            if (agent == this) continue;
            if (agent.CheckCollisionByLevel(0, colliders[0][0])) {
                foreach (SimpleSphereCollider otherAgentCollider in agent.colliders[1]) {
                    if (CheckCollisionByLevel(1, otherAgentCollider)) {
                        if (agent.CompareTag(tag)) {
                            agent.ApplyForce(velocity * mass);
                        }
                        else {
                            if (IsATargetTag(agent)) {
                                velocity = Vector3.zero;
                                agent.velocity = Vector3.zero;
                                target = agent.transform;
                                animator.SetTrigger("Eat");
                                return;
                            }
                            else {
                                //??
                            }
                        }

                    }
                }
            }
        }
    }
    public void KillTarget() {
        Agent agent = target.GetComponent<Agent>();
        agent.animator.SetTrigger("Die");
        agent.alive = false;
        agent.isActive = false;
        chunk.RemoveAgentFromChunk(agent);
        target = null;

        state = State.idle;
    }
    //check each level of collision against other SphereCollider
    public override bool CheckCollision(SimpleSphereCollider other) {
        if (colliders[0] == null)
            return false;

        if (CheckCollisionByLevel(0, other)) {
            if (CheckCollisionByLevel(1, other)) {
                return CheckCollisionByLevel(2, other);
            }
        }

        return false;
    }
    //checks the agents collider list at the specified level
    //will return true if one of the colliders encountered a collision with other
    //false otherwise
    protected bool CheckCollisionByLevel(int level, SimpleSphereCollider other) {
        if (level > colliders.Length - 1 || level < 0) {
            throw new ArgumentException();
        }
        foreach (SimpleSphereCollider sphereCollider in colliders[level]) {
            if (sphereCollider.CheckCollision(other)) {
                return true;
            }
        }
        return false;
    }


}
