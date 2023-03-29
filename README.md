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

## Trex

A carniverous agent that will wander around and hunt smaller creatures (NYI)

### Wander

**Objective:** Agent wanders around randomly

#### Steering Behaviors

- Moves forward for a bit then changes to a random direction
   - tracks nearby agents and can change to move away or chase behaviours
- Obstacles - NYI
- Seperation - currently seperates from any other agent with the "Carnivore" tag
   
#### State Transistions

- state is transitioned to after seperating, chasing, or being idle
   - When this agent gets far enough away from other avoided agents it will default to wander
   - When this agent has idled for a period of time it will transiton back to wandering
   
### Chase

**Objective:** moves towards a target agent

#### Steering Behaviors

- Needs to be updated
- Obstacles - NYI
- Seperation - Not implemented in this state, supposedly 2 of the same agent could chase a single target causing them to be close to eachother, however seperation check comes before any other state transition checks therefore 2 seperating agents should never be close enough to hunt the same agent
   
#### State Transistions

- Getting close enough to a target agent type, possibly will add a game timer or randomness to have this agent hunt the player

## SmallDino

A small herbivore dino, faster and more nimble than the TRex

### Wander

**Objective:** Same as Trex but speed and turn timings are different

#### Steering Behaviors

- Moves forward for a bit then changes to a random direction
   - tracks nearby agents and can change to run away behavior
- Obstacles - NYI
- Seperation - Any agent of the same type, will implement running from carnivores
   
#### State Transistions

- state is transitioned to after seperating, chasing, or being idle
   - When this agent gets far enough away from other avoided agents it will default to wander
   - When this agent has idled for a period of time it will transiton back to wandering
   
### NYI

**Objective:** NYI

#### Steering Behaviors

- NYI
- NYI
- Seperation - NYI
   
#### State Transistions

- NYI

## Sources

-   https://assetstore.unity.com/packages/tools/terrain/gaia-2021-terrain-scene-generator-193509
-   https://assetstore.unity.com/packages/vfx/shaders/free-skybox-extended-shader-107400
-   https://assetstore.unity.com/packages/3d/characters/animals/low-poly-animated-dinosaurs-110313
	- I stripped out all of the code and colliders from the dinosaurs, removing all of their behaviors
	- This has been replaced with my own sphere collider setup and agent script as described in the project outline

## Make it Your Own

-3D game with terrain generated using unity asset store generator
	- all colliders have been removed, only thing used from this is grabbing the height of the terrain from the terrain object and using that as ground
	- Physics object child script has been added to all of the trees
-Extended player interactions
	- eventually plan to create a small game where you can go around and hunt dinosaurs with an RPG, and maybe the dinos come to eat you, might play around with adding extra objectives depending on time allowances

## Known Issues

Tree hitbox is bugged

### Requirements not completed

_If you did not complete a project requirement, notate that here_

