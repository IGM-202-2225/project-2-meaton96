using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public const float WIDTH = 64f;
    public Vector3 bottomLeft;
    public List<TreeObject> trees;

    public Chunk (int x, int z) {
        bottomLeft = new Vector3(x, 0, z);
        trees = new();
    }

    public bool IsInChunk(Vector3 pos) {
        return pos.x > bottomLeft.x && pos.x < bottomLeft.x + WIDTH
            && pos.z > bottomLeft.z && pos.y < bottomLeft.y + WIDTH;
    }

}
