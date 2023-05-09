using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Random = Unity.Mathematics.Random;

public class RandomAuth : MonoBehaviour
{   
    [SerializeField]
    public OptionalField<int> randomSeed;
}

public class RandomBaker : Baker<RandomAuth>
{
    public override void Bake(RandomAuth authoring)
    {
        if(authoring.randomSeed.Enabled) {
            AddComponent(GetEntity(TransformUsageFlags.None), new RandomComponent {
                random = new Random((uint)authoring.randomSeed.Value),
            });
        }
        else {
            AddComponent(GetEntity(TransformUsageFlags.None), new RandomComponent {
                random = new Random((uint)System.DateTime.Now.Ticks)
            });
        }
    }
}
