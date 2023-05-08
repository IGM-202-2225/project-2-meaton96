# Project Dino Sim

[Markdown Cheatsheet](https://github.com/adam-p/markdown-here/wiki/Markdown-Here-Cheatsheet)



### Student Info

-   Name: Michael Eaton
-   Section: 01-02

## Simulation Design

Simulation of dinosaurs interacting with eachother. Player can run around and shoot a rocket launcher at them to kill them.
Player can explode trees to adjust dinosaur pathing behaviors

### Controls

-   Displayed on screen

## T-rex

A carniverous agent that will wander around and hunt smaller creatures

### Wander

**Objective:** Agent wanders around randomly

#### Steering Behaviors

*  Wander movement using the circle method with small wander angle changes
   *  tracks nearby agents to start pursuing
*  Obstacles - Avoids all tree obstacles
*  Seperation - Will seperate from any agent with the same tag
   
#### State Transistions

-  default state, will be transitioned to after exiting another state
   -  Will transition back to this state if the pursuit/seek/flee target becomes null
   
### Pursue

**Objective:** moves towards a target agent

#### Steering Behaviors

-  Steers towards a target agent by predicting its movement by up to 2 seconds. The time is reduced as it gets closer.
-  Obstacles - avoids all tree obstacles
-  Seperation - Agents of the same type
   
#### State Transistions

-  Getting close enough to a target agent type
-  Every 10 seconds a random dinosaur will begin to pursue the player

## SmallDino

A small herbivore dino, faster and more nimble than the TRex

### Wander

**Objective:** Move around the game world randomly

#### Steering Behaviors

- Wander movement using the circle method with small wander angle changes
- Obstacles - Avoids all tree obstacles
- Seperation - Any agent of the same type
   
#### State Transistions

- Default state
   
### Flee

**Objective:** Run away from a target agent

#### Steering Behaviors

- uses fleeing logic to move away from a target transform
- Obstacles - Avoids all tree obstacles
- Seperation - Any agent of the same type
   
#### State Transistions

- All dinosaurs will flee the player for 10 seconds if they are near enough when the player shoots their gun (unless this agent is currently in pursuit of the player)

## Flyer
A large slow flying dinosaur

### Wander
Default and only current state. The agent wanders around the sky randomly

#### Steering Behaviours

- Wander movement using the circle method with small wander angle changes
- Obstacles - None
- Seperation - Any agent of the same type

#### State Transitions
- None

## Velociraptor
A small carniverous dinosaur that travels around in a pack

### Wander

**Objective:** Agent wanders around randomly

#### Steering Behaviors

*  Wander movement using the circle method with small wander angle changes
   *  tracks nearby agents to start pursuing
*  Obstacles - Avoids all tree obstacles
*  Seperation - Will seperate from any agent with the same tag
   
#### State Transistions

-  default state, will be transitioned to after exiting another state
   -  Will transition back to this state if the pursuit/seek/flee target becomes null
   
### Pursue

**Objective:** moves towards a target agent

#### Steering Behaviors

-  Steers towards a target agent by predicting its movement by up to 2 seconds. The time is reduced as it gets closer.
-  Obstacles - avoids all tree obstacles
-  Seperation - Agents of the same type
   
#### State Transistions

-  Getting close enough to a target agent type
-  Every 10 seconds a random dinosaur will begin to pursue the player

### Flee

**Objective:** Run away from a target agent

#### Steering Behaviors

- uses fleeing logic to move away from a target transform
- Obstacles - Avoids all tree obstacles
- Seperation - Any agent of the same type

### Flock

**Objective:** Move around in a pack of similar agents

#### Steering Behaviors
-  uses flocking logic to move with a group of agents
-  flocks with any units inside a specified radius
-  sperates with the agents its flocking with to prevent collision
-  avoids all tree obstacles

#### State Transitions
-  None, this agent will always perform flocking behaviour.
	-  this means if one unit decides to perform a pursuit action (i.e. is randomly chosen to pursue player) then all agents in its flock will move with it in its pursuit

## Sources

-   https://assetstore.unity.com/packages/tools/terrain/gaia-2021-terrain-scene-generator-193509
-   https://assetstore.unity.com/packages/3d/characters/animals/low-poly-animated-dinosaurs-110313
	- I stripped out all of the code and colliders from the dinosaurs, removing all of their behaviors
	- This has been replaced with my own sphere collider setup and agent script as described in the project outline
-  https://assetstore.unity.com/packages/vfx/particles/fire-explosions/3d-fire-and-explosions-176981
-  https://assetstore.unity.com/packages/3d/airplane-42229

## Make it Your Own

-  3D game with terrain generated using unity asset store generator
	-  all colliders have been removed, only thing used from this is grabbing the height of the terrain from the terrain object and using that as ground
	-  Physics object child script has been added to all of the trees
-  Extended player interactions
	-  eventually plan to create a small game where you can go around and hunt dinosaurs with an RPG, and maybe the dinos come to eat you, might play around with adding extra objectives depending on time allowances

## Known Issues

-  Tree Hitboxes are bugged and have been disabled
-  There is currently no collision resolution between players and dinosaurs
-  Shooting has odd behaviour

## Documentation

*For Grading:* Most movement behaviours can be found in the Agent class which derives from PhysicsObject and is the base class for all agent types [View Agent.cs on Github](Project2/Assets/Scripts/Agent.cs)

Pursuit can be viewed in the Trex class [View TRex.cs on Github](Project2/Assets/Scripts/TRex.cs)

-  0.0.3 (Checkpoint 2)
	-  Fixed bug with friction preventing flying dinosaurs from actually moving
	-  Removed some old variables and updated the JSON to read in new movement variables
	-  Added obstacle avoidance for the two ground dinosaurs
		-  This can be enabled or disabled by pressing F3, default=enabled
	-  Added free camera mode so you can move around more freely and view agents' movement behaviour more easily
		-  Toggle free camera by pressing Tab
- 0.0.3.1
	-  Updated all movement logic to return steering vectors instead of apply their own forces. All forces are summed and finally multiplied by a movingPower scalar. This should be more efficient.
	-  Seperation now applies a force on itself away from all other agents instead of applying a force to other agents--
- 0.0.4 (Checkpoint 3)
	-  Fixed Seperation bug that was causing it to not function properly
-  0.1 
	-  Fixed issue causing camera shake with player when on the ground
	-  Fixed issue causing dinosaurs to fly into the air during pursuit
	-  Fixed bounds issue causing flying dinosaurs to move outside of the chunk system resulting in NullPointer
	-  Added dinosaur pursuit of player
		-  every 10 seconds a random carnivourous dinosaur will decide to pursue the player
	-  Added Velociraptor (See above section)
-  0.2
      -  Fixed issue preventing free cam from working
      -  Fixed issue preventing the game from being paused
	-  Added Shooting sound effect
	-  Fixed an issue with wander which was outputing a steering force with a negative y value
	-  No post processing on the openGL web build makes the scene look bad
-  0.3
	-  Fixed gravity
-  0.4 
	-  Fixed Controls text
	-  Fixed bug with flying dino's StayInBounds 

### Requirements not completed

_If you did not complete a project requirement, notate that here_

