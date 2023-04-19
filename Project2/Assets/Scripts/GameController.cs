using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameController : MonoBehaviour {

    public string buildNumber = "0.0.3.1";

    public int numChunks = 64;
    public int startingPoint = -1024;
    public int multi;
    public TRex Trex1;
    public List<PhysicsObject> agents;
    [SerializeField] private TextAsset agentDataJson;
    public List<GameObject> agentPrefabs;
    [SerializeField] private GameObject bulletPrefab;
    public int dinoIndex = 2;
    List<TreeObject> trees = new();
    public Chunk[][] chunks = new Chunk[8][];
    private float updateTimer, updateTime = 5f;
    private Terrain terrain;
    public bool agentsAvoidObj = true;
    // Start is called before the first frame update
    void Start() {

        MyJsonUtility.SetJsonText(agentDataJson.text);
        MyJsonUtility.ParseAllJson();
        terrain = GameObject.FindWithTag("Ground").GetComponent<Terrain>();


        for (int x = 0; x < chunks.Length; x++) {
            chunks[x] = new Chunk[chunks.Length];
        }

        var treeList = GameObject.FindGameObjectsWithTag("Synty Tree").ToList();

        int numRows = (int)Mathf.Sqrt(numChunks);
        multi = startingPoint * -2 / numRows;
        for (int x = 0; x < chunks.Length; x++) {
            for (int z = 0; z < chunks[x].Length; z++) {
                chunks[x][z] = new Chunk(this, x, z);
                chunks[x][z].Update();
            }
        }
        foreach (var tree in treeList) {
            if (tree != null) {
                GetChunk(tree.transform.position).trees.Add(tree.GetComponent<TreeObject>());
            }
        }

        agents = new();
    }


    // Update is called once per frame
    void Update() {

        UpdateChunks();
        //causing exception chunks[x][y] is null
        foreach (Agent agent in agents) {
            if (!agent.alive) {
                agents.Remove(agent);
                break;
            }
        }
        if (Input.GetKeyDown(KeyCode.F1)) {
            agents.ForEach(agent => ((Agent)agent).ToggleAI());
        }
        
        if (Input.GetKeyDown(KeyCode.F2)) {
            StartCoroutine(SpawnAgents());
            UpdateChunks();
        }
        if (Input.GetKeyDown(KeyCode.F3)) {
            foreach (var chunkRow in chunks) {
                foreach (var chunk in chunkRow) {
                    foreach (var agent in chunk.agents) {
                        agent.avoidingObstacles = !agent.avoidingObstacles;
                        agentsAvoidObj = !agentsAvoidObj;
                    }
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.F4)) {
            foreach (Chunk[] chunkRow in chunks) {
                foreach (Chunk chunk in chunkRow) {
                    foreach(Agent agent in chunk.agents) {
                        if (agent is TRex rex) {
                            rex.isHunting = true;
                        }
                    }
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            dinoIndex++;
            if (dinoIndex >= agentPrefabs.Count) { dinoIndex = 0; }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            dinoIndex--;
            if (dinoIndex < 0) { dinoIndex = agentPrefabs.Count - 1; }
        }

    }

    private IEnumerator SpawnAgents() {
        int numAgentsToSpawn = 150;
        for (int x = 0; x < numAgentsToSpawn; x++) {
            //SpawnAgent(x % agentPrefabs.Count);
            SpawnAgent(dinoIndex);
            yield return new WaitForEndOfFrame();
        }
    }
    private void SpawnAgent(int agentNum) {

        float radius = 700;

        float xLoc = UnityEngine.Random.Range(-radius, radius);
        float zLoc = UnityEngine.Random.Range(-radius, radius);
        float yLoc = terrain.SampleHeight(new Vector3(xLoc, 0f, zLoc)) + UnityEngine.Random.Range(50, 150);
        GameObject agent = Instantiate(agentPrefabs[agentNum], new Vector3(xLoc, yLoc, zLoc), Quaternion.identity);
    }

    public void UpdateChunks() {
        if (updateTime >= updateTimer) {
            updateTimer = 0;
            for (int x = 0; x < chunks.Length; x++) {
                for (int y = 0; y < chunks[0].Length; y++) {
                    //Debug.Log(chunks[x][y]);
                    chunks[x][y].Update();
                }
            }

        }
        else {
            updateTime += Time.deltaTime;
        }
    }

    public Chunk GetChunk(Vector3 pos) {
        int x = ((int)pos.x - startingPoint) / multi;
        int z = ((int)pos.z - startingPoint) / multi;
        if (x < 0 || z < 0 || x > chunks.Length || z > chunks[0].Length) {
            throw new ArgumentException();
            
        }
        return chunks[x][z];
    }

}
