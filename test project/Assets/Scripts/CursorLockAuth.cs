using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

#region authoring
public class CursorLockAuth : MonoBehaviour
{
    public CursorLockMode cursorLockMode = CursorLockMode.Locked;
}

public class CursorLockBaker : Baker<CursorLockAuth>
{
    public override void Bake(CursorLockAuth authoring)
    {
        AddComponent(
            GetEntity(TransformUsageFlags.None), 
            new CursorLockComponent {
                cursorLockMode = authoring.cursorLockMode,
            });
    }
}
#endregion authoring

#region component & system
public struct CursorLockComponent : IComponentData {
    public CursorLockMode cursorLockMode;
}

public partial class CursorLockSystem : SystemBase
{
    protected override void OnUpdate()
    {
        if(SystemAPI.HasSingleton<CursorLockComponent>()) {
            Cursor.lockState = SystemAPI.GetSingleton<CursorLockComponent>().cursorLockMode;
        }
    }
}
#endregion component & system