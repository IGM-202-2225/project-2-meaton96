using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chunk {
    public const float WIDTH = 64f;
    public Vector3 bottomLeft;
    public List<TreeObject> trees;
    public List<Agent> agents;
    private GameController gameController;

    public int x, z;
    public Chunk(GameController gc, int x, int z) {
        bottomLeft = new Vector3(
            gameController.startingPoint + x * gameController.multi,
            0,
            gameController.startingPoint + z * gameController.multi);
        trees = new();

        this.x = x;
        this.z = z;
    }

    public bool IsInChunk(Vector3 pos) {
        return pos.x > bottomLeft.x && pos.x < bottomLeft.x + WIDTH
            && pos.z > bottomLeft.z && pos.y < bottomLeft.y + WIDTH;
    }
    public void Update() {

        agents = gameController.agents.Where(agent => IsInChunk(agent.transform.position)).Cast<Agent>().ToList();
        agents.ForEach(agent => agent.UpdateChunk(this));


    }
    public List<PhysicsObject> GetAgentsOfType(IEnumerable<string> tags) {




        List<PhysicsObject> agents = new();
        foreach (string tag in tags) {
            agents.AddRange(agents.Where(a => a.CompareTag(tag)));
        }
        return agents;

    }
    public List<Chunk> GetAdjacentChunks() {
        List<Chunk> adjacentChunks = new();
        for (int i = x - 1; i <= 1; i++) {
            for (int j = z - 1; j <= 1; j++) {
                try {
                    adjacentChunks.Add(gameController.chunks[i][j]);
                }
                catch (IndexOutOfRangeException) { }
            }
        }
        return adjacentChunks;
    }


}
