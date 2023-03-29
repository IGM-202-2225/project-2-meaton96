using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeObject : StaticObject
{

    
    void Start()
    {
        
    }
    protected override void Awake() {
        frictionAmount = 2000;
        mass = 1000;
        radius = .5f;
        base.Awake();
    }


}
