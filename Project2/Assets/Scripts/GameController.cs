using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour {
    public int numChunks = 64;
    public int startingPoint = -1024;
    public int multi;
    public TRex Trex1;
    public List<PhysicsObject> agents;
    [SerializeField] private TextAsset agentDataJson;
    public List<GameObject> agentPrefabs;
    [SerializeField] private GameObject bulletPrefab;
    public int dinoIndex = 0;
    List<TreeObject> trees = new();
    public List<Chunk> chunks = new();
    // Start is called before the first frame update
    void Start() {

        MyJsonUtility.SetJsonText(agentDataJson.text);
        MyJsonUtility.ParseAllJson();

        var treeList = GameObject.FindGameObjectsWithTag("Synty Tree").ToList();
        
        int numRows = (int)Mathf.Sqrt(numChunks);
        multi = startingPoint * -2 / numRows;
        for (int x = startingPoint; x < numRows; x++) {
            for (int z = startingPoint; z < numRows; z++) {
                chunks.Add(new Chunk(x * multi, z * multi));
            }
        }
        foreach (var tree in treeList) {
            if (tree != null) {
                GetChunk(tree.transform.position).trees.Add(tree.GetComponent<TreeObject>());
            }
        }
        //crashes unity
        //trees.ForEach(tree => {
        //    foreach (var chunk in chunks) {
        //        if (chunk.IsInChunk(tree.transform.position)) {
        //            chunk.trees.Add(tree);
        //            trees.Remove(tree);
        //            continue;
        //        }
        //    }
        //});
        agents = new();



    }

    // Update is called once per frame
    void Update() {
        foreach (Agent agent in agents.Cast<Agent>()) {
            if (!agent.alive) {
                agents.Remove(agent);
                break;
            }
        }
        if (Input.GetKeyDown(KeyCode.F1)) {
            agents.ForEach(agent => ((Agent)agent).ToggleAI());
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            Bullet bullet = Instantiate(bulletPrefab, Camera.main.transform.position, Quaternion.identity).GetComponent<Bullet>();
            bullet.Fire(Camera.main.transform.forward, agents, this);
        }
        if (Input.GetKeyDown(KeyCode.F2)) {
            GameObject spawnedAgent = Instantiate(agentPrefabs[dinoIndex],
                Camera.main.transform.position + Camera.main.transform.forward.normalized * 10, Quaternion.identity);
            agents.Add(spawnedAgent.GetComponent<PhysicsObject>());
        }
        if (Input.GetKeyDown(KeyCode.F3)) {
            foreach (PhysicsObject agent in agents) {
                foreach (List<SimpleSphereCollider> colliderList in ((Agent)agent).colliders) {
                    colliderList.ForEach(collider => Debug.Log(collider.ToString()));
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
        // Debug.Log(Trex1.state.ToString());  
        
    }
    public Chunk GetChunk(Vector3 pos) {
        int x = ((int) pos.x - startingPoint) / multi;
        int z = ((int) pos.z - startingPoint) / multi;
        
        return chunks[x + z];
    }

}
