using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameController : MonoBehaviour {

    public string buildNumber = "0.0.3.1";                                                      //build number to display on screen to make checking build pushes easier
    public float DEFAULT_GRAVITY = 10f;                                                         //default gravity value

    public int numChunks = 64;                                                                  //number of chunks of the map
    public int startingPoint = -1024;                                                           //starting x,y point, terrain's corner
    public int multi;
    [SerializeField] private TextAsset agentDataJson;                                           //reference to the JSON file containing the agent data
    public List<GameObject> agentPrefabs;                                                       //list of all available agent prefabs to instantiate
    public int dinoIndex = 2;                                                                   //tracker for which dinosaur to spawn
    public Chunk[][] chunks = new Chunk[8][];                                                   //holds every chunk 
    private float updateTimer, updateTime = 5f;                                                 //update variables for updating chunks
    private Terrain terrain;                                                                    //reference to the terrain object
    public bool agentsAvoidObj = true;                                                          //whether or not agent obstacle avoidance is enabled
    public float gravityAmount;                                                                 //the current gravity amount
    // Start is called before the first frame update
    void Start() {
        //tell the MyJsonUtility static class to parse all of the json data from the file
        MyJsonUtility.SetJsonText(agentDataJson.text);
        MyJsonUtility.ParseAllJson();
        //grab terrain object and set gravity default
        terrain = GameObject.FindWithTag("Ground").GetComponent<Terrain>();
        gravityAmount = DEFAULT_GRAVITY;

        //create new chunk arrays
        for (int x = 0; x < chunks.Length; x++) {
            chunks[x] = new Chunk[chunks.Length];
        }
        //get a list of all the trees in the scene to pass to the chunks
        var treeList = GameObject.FindGameObjectsWithTag("Synty Tree").ToList();

        //create new chunks and call their update methods
        int numRows = (int)Mathf.Sqrt(numChunks);
        multi = startingPoint * -2 / numRows;
        for (int x = 0; x < chunks.Length; x++) {
            for (int z = 0; z < chunks[x].Length; z++) {
                chunks[x][z] = new Chunk(this, x, z);
                chunks[x][z].Update();
            }
        }
        //gets the chunk of each tree and add 
        foreach (var tree in treeList) {
            if (tree != null) {
                GetChunk(tree.transform.position).AddTree(tree.GetComponent<TreeObject>());
            }
        }

       // agents = new();
    }


    // Update is called once per frame
    void Update() {

        UpdateChunks();

        //toggle on or off movement logic for all agents
        if (Input.GetKeyDown(KeyCode.F1)) {
            foreach (var chunkRow in chunks) {
                foreach (var chunk in chunkRow) {
                    foreach (var agent in chunk.agents) {
                        agent.ToggleAI();
                    }
                }
            }
        }
        //start spawn agent coroutine to spawn a bunch of agents
        //also increase the gravity by 5 times to have the agents drop quickly
        if (Input.GetKeyDown(KeyCode.F2)) {
            StartCoroutine(SpawnAgents());
         //   gravityAmount *= 5f;
            
            UpdateChunks();
        }
        //toggle obstacle avoidance for all agents
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
        //toggles on the hunting function for trexs
        //its off by default to allow agents time to drop and spread out before starting pursuit logic
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
        //change the selected dinosaur to spawn
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            dinoIndex++;
            if (dinoIndex >= agentPrefabs.Count) { dinoIndex = 0; }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            dinoIndex--;
            if (dinoIndex < 0) { dinoIndex = agentPrefabs.Count - 1; }
        }

    }
    //drop gravity back to default after seconds
    private IEnumerator ResetGravityAfterSeconds(int seconds) {
        for (int x = 0; x < seconds; x++) {
            yield return new WaitForSeconds(1);
        }
        gravityAmount = DEFAULT_GRAVITY;
    }
    //Spawn 150 agents of the currently selected type
    private IEnumerator SpawnAgents() {
        int numAgentsToSpawn = 150;
        for (int x = 0; x < numAgentsToSpawn; x++) {
            //SpawnAgent(x % agentPrefabs.Count);
            SpawnAgent(dinoIndex);
            yield return new WaitForEndOfFrame();
        }
      //  StartCoroutine(ResetGravityAfterSeconds(10));
    }
    //Spawns a single agent of type agentNum from the prefab list
    //at a random location within the circle defined by radius
    private void SpawnAgent(int agentNum) {
        
        float radius = 700;

        float xLoc = UnityEngine.Random.Range(-radius, radius);
        float zLoc = UnityEngine.Random.Range(-radius, radius);

        float minY = 5, maxY = 10;

        if (agentNum == 2) {
            minY += 35;
            maxY += 90;
        }

        float yLoc = terrain.SampleHeight(new Vector3(xLoc, 0f, zLoc)) + UnityEngine.Random.Range(minY, maxY);
        GameObject agent = Instantiate(agentPrefabs[agentNum], new Vector3(xLoc, yLoc, zLoc), Quaternion.identity);
    }
    //calls every chunk's update method every updateTime seconds
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
    //returns the chunk that the given vector position is located in
    public Chunk GetChunk(Vector3 pos) {
        int x = ((int)pos.x - startingPoint) / multi;
        int z = ((int)pos.z - startingPoint) / multi;
        if (x < 0 || z < 0 || x > chunks.Length || z > chunks[0].Length) {
            throw new ArgumentException();
            
        }
        return chunks[x][z];
    }

}
