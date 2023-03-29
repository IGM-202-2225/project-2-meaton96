using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StaticObject : PhysicsObject
{
    float height;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    protected override void Awake() {
        terrain = GameObject.FindWithTag("Ground").GetComponent<Terrain>();
        
        base.Awake();
    }

    //something wrong with the collider
    public override bool CheckCollision(SimpleSphereCollider other) {
        if (other == null) return false;
        
        return Mathf.Pow(sCollider.position.x - other.position.x, 2) +
               Mathf.Pow(sCollider.position.y - other.position.y, 2) <
               Mathf.Pow(radius + other.radius, 2) && IsInHeightRange(other);
    }
    private bool IsInHeightRange(SimpleSphereCollider collider) {
        return collider.y >= sCollider.y && collider.y <= collider.y + height;
    }

}
