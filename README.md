# Project Dino Sim

[Markdown Cheatsheet](https://github.com/adam-p/markdown-here/wiki/Markdown-Here-Cheatsheet)



### Student Info

-   Name: Michael Eaton
-   Section: 01-02

## Simulation Design

Simulation of dinosaurs interacting with eachother. Player can run around and shoot a rocket launcher at them to kill them.
Player can explode trees to adjust dinosaur pathing behaviors

### Controls

-   Fly-cam current controls
    -   WASD to move camera, right click and drag to rotate camera
    -   F1 to toggle dinosaur AI, F2 to spawn dinosaur, F3 to print all current dinosaur infos to debug console
    -   Up/Down arrow to chose a dinosaur to spawn (currently 2)
    -   Space bar to fire a bullet
    -   Bullets can interact with dinosaurs and trees, bullets kills dinosaurs and also will knock trees over

## T-rex

A carniverous agent that will wander around and hunt smaller creatures

### Wander

**Objective:** Agent wanders around randomly

#### Steering Behaviors

*  Wander movement using the circle method with small wander angle changes
   *  tracks nearby agents to start pursuing
*  Obstacles - NYI
*  Seperation - Will seperate from any agent with the same tag
   
#### State Transistions

-  default state, will be transitioned to after exiting another state
   -  Will transition back to this state if the pursuit/seek/flee target becomes null
   
### Pursue

**Objective:** moves towards a target agent

#### Steering Behaviors

-  Steers towards a target agent by predicting its movement by up to 2 seconds. The time is reduced as it gets closer.
-  Obstacles - NYI
-  Seperation - Not implemented in this state. Agent will ignore everything except pursuing its target.
   
#### State Transistions

- Getting close enough to a target agent type, possibly will add a game timer or randomness to have this agent hunt the player

## SmallDino

A small herbivore dino, faster and more nimble than the TRex

### Wander

**Objective:** Move around the game world randomly

#### Steering Behaviors

- Wander movement using the circle method with small wander angle changes
- Obstacles - NYI
- Seperation - Any agent of the same type
   
#### State Transistions

- currently the only state for this agent
   
### Flee

**Objective:** Run away from a target agent

#### Steering Behaviors

- uses fleeing logic to move away from a target transform
- Obstacles - NYI
- Seperation - Any agent of the same type
   
#### State Transistions

- NYI

## Sources

-   https://assetstore.unity.com/packages/tools/terrain/gaia-2021-terrain-scene-generator-193509
-   https://assetstore.unity.com/packages/vfx/shaders/free-skybox-extended-shader-107400
-   https://assetstore.unity.com/packages/3d/characters/animals/low-poly-animated-dinosaurs-110313
	- I stripped out all of the code and colliders from the dinosaurs, removing all of their behaviors
	- This has been replaced with my own sphere collider setup and agent script as described in the project outline

## Make it Your Own

-  3D game with terrain generated using unity asset store generator
	-  all colliders have been removed, only thing used from this is grabbing the height of the terrain from the terrain object and using that as ground
	-  Physics object child script has been added to all of the trees
-  Extended player interactions
	-  eventually plan to create a small game where you can go around and hunt dinosaurs with an RPG, and maybe the dinos come to eat you, might play around with adding extra objectives depending on time allowances

## Known Issues

-  Tree hitbox is bugged and is disabled (v0.0.2)
-  T-rex successfully completing a hunt does not correctly play the death animation of the small dino agent (v0.0.2)

### Requirements not completed

_If you did not complete a project requirement, notate that here_

