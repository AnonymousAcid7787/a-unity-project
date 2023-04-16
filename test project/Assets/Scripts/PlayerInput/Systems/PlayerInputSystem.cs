using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial class PlayerInputSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref PlayerMovementData data, in PlayerInputKeys keys) => {
            int xDirection = Convert.ToInt32(Input.GetKey(keys.rightKey)) - Convert.ToInt32(Input.GetKey(keys.leftKey));
            int zDirection = Convert.ToInt32(Input.GetKey(keys.upKey)) - Convert.ToInt32(Input.GetKey(keys.downKey));
            data.movementDirection.x = xDirection;
            data.movementDirection.z = zDirection;

            data.jump = Input.GetKey(keys.jump);
        }).WithoutBurst().Run();
    }
}