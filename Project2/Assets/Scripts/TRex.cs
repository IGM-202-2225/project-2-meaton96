using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRex : Agent {

    protected override void Awake() {

        AssignClassData("trex");
        targetTags = new(new string[] { "SmallHerbivore", "LargeHerbivore", "SmallCarnivore", "Player" });
        base.Awake();
    }





}
