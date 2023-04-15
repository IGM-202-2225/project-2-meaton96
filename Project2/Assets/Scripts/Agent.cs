using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Agent : PhysicsObject {
    //public GameObject ground;                                           //temporary reference to ground object, will change to terrain later
    protected Animator animator;                                        //pointer to animator to change animations
    protected float wanderTimer, wanderSwitchDirCooldown;               //timers for tracking wandering around
    [SerializeField] protected float movingPower;                       //how strong in Newtons? to call ApplyForce
    protected float turnSpeed, turnTimer;                               //timers to track turning, turning takes place over timeSpeed time in seconds
    protected Quaternion previousRotation;                              //tracking for using linear interpolation to turn the agent
    protected Quaternion nextRotation;
    protected Transform target;                                         //current target, agent will run towards this if in chasing state
   // protected List<Agent> avoidAgents, targetAgents;                    //current world agents to avoid and target depending on tags Carnivore, herbivore ect..
    protected float runningSpeedMultiplier;                             //how much faster to run when chasing or running away then when wandering
    protected State nextState;                                          //holder to move to another state after a turn is completed
    //protected float runningSpeed = 3f;                                  
    protected float maxSpeed;                                           //speed cap, velocity magnitude will not exceed this value
    //protected Transform runningFrom;                                      //an initial position to run away from, this may be a sound (gunshot or something from player) 
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

    public float runAwayTimer, runAwayTime;

    protected const float CIRCLE_DISTANCE = 1f, CIRCLE_RADIUS = 2f, ANGLE_CHANGE = 0.275f, SHORE_CHECK_RADIUS = 10f, SHORE_CHECK_ANGLE = .1f;
    protected float wanderAngle = 0f;
    protected float stayInBoundsPower = 10f;
    protected float seperationDistance = 150f, minSeperationDistance = 100f;
    protected float pursuePredictTime = 2f;
    protected const float SEA_LEVEL = 27f;

    protected Chunk chunk;

    public bool alive = true;
    public int _id;
    public static int id = 0;
    //behaviour states for agent
    public enum State {
        idle,
        wandering,
        fleeing,
        chasing,
        hurt,
        turning,
        pursuing
    }

    public State state;

    //set target if necessary
    public void SetTarget(Transform target) {
        this.target = target;
    }


    protected override void Awake() {
        _id = id++;
        //init lists and arrays
        base.Awake();
       // avoidAgents = new();
        //targetAgents = new();
        //assign game controller pointer
        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        animator = GetComponent<Animator>();
        //force scan all actors when created to populate avoid and target lists
        //UpdateAgentLists();
        //activate movement logic
        isActive = true;
        alive = true;
        frictionEnabled = true;

    }
    protected virtual void Pursue() { }
    protected override void UpdateSphereCollider() {
        foreach (List<SimpleSphereCollider> colliderLists in colliders) {
            colliderLists.ForEach(collider => collider.Update(transform.position));
        }

    }
    //keeps the agent from going into the water by preventing it from going below sea level
    protected void StayInBounds() {
        if (transform.position.y <= SEA_LEVEL) {
            ApplyForce(stayInBoundsPower * movingPower * GetVectorPerpToShore());
        }
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
    //seperates from all nearby (seperationDistance) agents by apply a force to eachother
    //force is proportional to the distance with maximum force applied nearest to the agent
    void Seperate() {
        List<PhysicsObject> agentsInRange = chunk.GetAgentsOfType(
            new []{ tag }).
            FindAll(
            agent => Vector3.Distance(agent.transform.position, transform.position) < seperationDistance);
        
        foreach(Agent agent in agentsInRange) {
            Vector3 dir = agent.transform.position - transform.position;
            dir = dir.normalized * -1;
            float distance = Vector3.Distance(agent.transform.position, transform.position);
            float percentForce = distance / (seperationDistance - minSeperationDistance);
            ApplyForce(percentForce * movingPower * 2 * dir);
        }
    }
    public override void Update() {
        //call physics object update
        base.Update();
        if (alive) {

            UpdateSphereCollider();
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
                        Wander();
                        Seperate();
                        break;
                    case State.fleeing:
                        Flee();
                        break;
                    case State.chasing:
                        //ChaseTarget();
                        break;
                    case State.turning:
                        //turns the agent
                        if (turnTimer > turnSpeed) {
                            turnTimer = 0f;
                            state = nextState;
                        }
                        else {
                            transform.rotation = Quaternion.Slerp(previousRotation, nextRotation, turnTimer * turnSpeed);
                            turnTimer += Time.deltaTime;
                        }

                        break;

                }
                StayInBounds();
                
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
    
    protected virtual void Flee() {
        if (target == null) {
            state = State.wandering;
            return;
        }
        Vector3 desiredVelocity;

        float xV = transform.position.x - target.position.x;
        float zV = transform.position.z - target.position.z;
        desiredVelocity = new Vector3(xV, velocity.y, zV).normalized * 10;
        


        ApplyForce((desiredVelocity - velocity) * movingPower);
        
    }


    //assign class data fields that determine movement logic based on the passed in class name
    protected void AssignClassData(string className) {
        for (int x = 0; x < colliders.Length; x++) {
            colliders[x] = new();
        }

        AgentData ad = MyJsonUtility.classData[className];

        runningSpeedMultiplier = ad.runningSpeedMultiplier;
        wanderSwitchDirCooldown = ad.wanderSwitchDirCooldown;
        turnSpeed = ad.turnSpeed;
        maxSpeed = ad.maxSpeed;
        maxRunAwayDistance = ad.maxRunAwayDistance;
        avoidanceDistance = ad.avoidanceDistance;
        mass = ad.mass;
        movingPower = ad.movementPower;
        gravityAmount = ad.gravityAmt;

        frictionEnabled = false;
        frictionAmount = 2000f;
        //assign each of the colliders in the collider list for this class 
        //by copying them from the JsonUtilty list
        MyJsonUtility.colliderList[className].ForEach(collider => {
            colliders[collider.level].Add(collider.DeepCopy());
        });
        agentPollRate = 5f;
        state = State.wandering;
        stateSwitchTimer = 13f;
        runAwayTime = 7f;
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
        this.chunk = chunk;
    }
    //increments the state
    //only for testing
    public void NextState() {
        state++;
        if (state == State.turning) {
            state = 0;
        }
    }
    public void StartChase(Transform target) {
        this.target = target;
        nextState = State.chasing;
        TurnTo(Quaternion.LookRotation(target.position));

    }
    //will move forward and continually rotate to look at the taret transform
    //private void ChaseTarget() {
    //    transform.LookAt(target, Vector3.up);
    //    MoveForward(movingPower * 2);
    //}
    //run away from the runAwayFrom position
    //will turn directly away from and set the state to run away
    //public void RunAwayFrom(Transform runAwayFrom) {
        
    //    runningFrom = runAwayFrom;
    //    TurnTo(Quaternion.LookRotation(new Vector3(-runAwayFrom.position.x, 0f, -runAwayFrom.position.z)));
    //    nextState = State.fleeing;
    //}
    //move forawrd by the specified multiplier
    
    //wanders around
    //changes direction randomly every so often
    //protected virtual void Wander() {


    //    //50/50 to start idling or continue wandering
    //    if (stateTimer > stateSwitchTimer) {
    //        stateTimer = 0f;
    //        if (UnityEngine.Random.Range(0, 2) == 0) {
    //            state = State.idle;
    //        }
    //    }
    //    else
    //        stateTimer += Time.deltaTime;

    //    //randomly change direction
    //    if (wanderTimer >= wanderSwitchDirCooldown) {
    //        Vector3 currentRotation = transform.rotation.eulerAngles;
    //        wanderTimer = 0;
    //        nextState = State.wandering;
    //        TurnTo(
    //            Quaternion.Euler(
    //                new Vector3(
    //                    currentRotation.x,
    //                    UnityEngine.Random.Range(0f, 360f),
    //                    currentRotation.z)));
    //    }
    //    wanderTimer += Time.deltaTime;

    //    MoveForward(movingPower);




    //}
    protected virtual void Wander() {
        
        Vector3 circleCenter;
        circleCenter = new Vector3(velocity.x, velocity.y, velocity.z).normalized * CIRCLE_DISTANCE;

        Vector3 displacement = Vector3.down * CIRCLE_RADIUS;

        displacement.x = Mathf.Cos(wanderAngle) * displacement.magnitude;
        displacement.z = Mathf.Sin(wanderAngle) * displacement.magnitude;

        wanderAngle += UnityEngine.Random.Range(-ANGLE_CHANGE, ANGLE_CHANGE);

        Vector3 wanderForce = displacement + circleCenter;

        ApplyForce(wanderForce * movingPower);

    }
    public virtual void Seek() {
        Vector3 desiredVelocity = (target.position - transform.position).normalized * maxSpeed;
        ApplyForce((desiredVelocity - velocity) * movingPower);

    }

    //sets up agent turning to direction rotation
    protected virtual void TurnTo(Quaternion direction) {

        previousRotation = transform.rotation;
        nextRotation = direction;
        velocity = Vector3.zero;
        state = State.turning;
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
