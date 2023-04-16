using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct PlayerInputKeys : IComponentData
{
    public KeyCode upKey;
    public KeyCode downKey;
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode jump;
}
