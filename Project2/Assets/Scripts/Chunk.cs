using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Chunk {
    public const float WIDTH = 256f;
    public Vector3 bottomLeft;
    public List<TreeObject> trees;
    public List<Agent> agents;
    private GameController gameController;

    public int x, z;
    public Chunk(GameController gc, int x, int z) {
        gameController = gc;
        bottomLeft = new Vector3(
            gameController.startingPoint + x * gameController.multi,
            0,
            gameController.startingPoint + z * gameController.multi);
        trees = new();
        agents = new();
        this.x = x;
        this.z = z;
    }

    public bool IsInChunk(Vector3 pos) {
        return pos.x > bottomLeft.x && pos.x < bottomLeft.x + WIDTH
            && pos.z > bottomLeft.z && pos.y < bottomLeft.y + WIDTH;
    }
    public void Update() {
     //   agents = new();
       // agents = gameController.agents.Where(agent => IsInChunk(agent.transform.position)).Cast<Agent>().ToList();
     //   agents.ForEach(agent => agent.UpdateChunk(this));
        foreach (Agent agent in agents) {
            if (!agent.alive) {
                RemoveAgentFromChunk(agent);
            }
        }

    }
    public void RemoveAgentFromChunk(Agent agent) {
        agents.Remove(agent);
    }
    public void AddAgent(Agent agent) {
        agents.Add(agent);
    }
    public void AddTree(TreeObject tree) {
        trees.Add(tree);
    }
    public List<PhysicsObject> GetAgentsOfType(IEnumerable<string> tags) {



        List<PhysicsObject> allAgents = new();
        foreach (string t in tags) {

            
            allAgents.AddRange(agents.Where(a => a.CompareTag(t)));


        }
        
        return allAgents;

    }
    public List<PhysicsObject> FilterAgentsByTagAndDistance(Vector3 pos, float radius, IEnumerable<string> tags) {
        return GetAgentsOfType(tags).Where(agent => Vector3.Distance(pos, agent.transform.position) < radius).ToList();
    }
    public List<Agent> GetAgentsInsideCircle(Vector3 center, float radius) {
        return agents.Where(agent => Vector3.Distance(center, agent.transform.position) < radius).ToList();
    }
    public List<Chunk> GetAdjacentChunks() {
        List<Chunk> adjacentChunks = new();
        for (int i = x - 1; i <= 1; i++) {
            for (int j = z - 1; j <= 1; j++) {
                try {
                    adjacentChunks.Add(gameController.chunks[i][j]);
                }
                catch (IndexOutOfRangeException) {
                    continue;
                }
            }
        }
        Debug.Log(adjacentChunks.Count);
        return adjacentChunks;
    }


}
