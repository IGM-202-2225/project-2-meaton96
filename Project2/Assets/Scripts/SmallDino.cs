using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallDino : Agent {

    protected override void Awake() {

        AssignClassData("sdino");
        targetTags = new();
        base.Awake();
    }





}
