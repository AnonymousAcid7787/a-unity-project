using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Physics;

[UpdateAfter(typeof(IsGroundedSystem))]
public partial struct KinematicGravitySystem : ISystem
{
    

    partial struct kinematicGravityJob : IJobEntity {
        public void Execute() {
            
        }
    }
}
