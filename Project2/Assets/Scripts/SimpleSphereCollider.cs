using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSphereCollider {

    public float x;
    public float y;
    public float z;
    public float radius;
    public int level;
    public int vitalLevel;
    public Vector3 position;

    public SimpleSphereCollider(float x, float y, float z, float radius, int level, int vitalLevel) {
        this.radius = radius;
        this.x = x;
        this.y = y;
        this.z = z;
        this.level = level;
        this.vitalLevel = vitalLevel; 
    }
    public SimpleSphereCollider(Vector3 location, float radius) {
        this.radius = radius;
        x = location.x;
        y = location.y;
        z = location.z;
    }
    public void Update(Vector3 pos) {
        position = new Vector3(pos.x + x, pos.y + y, pos.z + z);
    }
    public bool CheckCollision(SimpleSphereCollider other) {
        

        return Mathf.Pow(position.x - other.position.x, 2) +
               Mathf.Pow(position.y - other.position.y, 2) +
               Mathf.Pow(position.z - other.position.z, 2) <
               Mathf.Pow(radius + other.radius, 2);
    }
    public override string ToString() {
        return position.ToString() + "\nradius: " + radius + "\nlevel: " + level;
    }
    public SimpleSphereCollider DeepCopy() {
        return new SimpleSphereCollider(x, y, z, radius, level, vitalLevel);
    }
    

}
