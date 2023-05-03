using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerInputAuthoring : MonoBehaviour
{
    public KeyCode upKey;
    public KeyCode downKey;
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode jump;
}

public class PlayerInputBaker : Baker<PlayerInputAuthoring>
{
    public override void Bake(PlayerInputAuthoring authoring)
    {
        Entity thisEntity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(thisEntity, new PlayerInputKeys {
            upKey = authoring.upKey,
            downKey = authoring.downKey,
            leftKey = authoring.leftKey,
            rightKey = authoring.rightKey,
            jump = authoring.jump,
        });
        AddComponent<PlayerInputData>(thisEntity);
    }
}