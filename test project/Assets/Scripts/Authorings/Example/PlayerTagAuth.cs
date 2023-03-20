using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerTagAuth : MonoBehaviour
{
    
}

public class PlayerTagBaker : Baker<PlayerTagAuth>
{
    public override void Bake(PlayerTagAuth authoring)
    {
        AddComponent(new PlayerTag());
    }
}
