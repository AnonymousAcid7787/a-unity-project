using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Entities;
using Unity.Mathematics;

public class RespawnButton : MonoBehaviour
{
    public Vector3 randomPositionFrom = new Vector3(-10f, -10f, -10f);
    public Vector3 randomPositionTo = new Vector3(10f, 10f, 10f);

    public TMP_Dropdown dropdown;

    public struct PrefabData {
        public string name;
        public PrefabType prefabType;
    }

    public List<PrefabData> prefabs = new List<PrefabData> {
        new PrefabData {
            name = "Cube",
            prefabType = PrefabType.Cube,
        }
    };

    private EntityArchetype respawnArchetype;

    public void Awake() {
        dropdown.ClearOptions();

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        foreach(PrefabData prefab in prefabs) {
            options.Add(new TMP_Dropdown.OptionData {
                text = prefab.name
            });
        }

        dropdown.AddOptions(options);
    }

    public void Start() {
        respawnArchetype = World.DefaultGameObjectInjectionWorld.EntityManager.CreateArchetype(typeof(RespawnPrefabComponent));
    }

    public void Respawn() {
        #region error checking
        if(World.DefaultGameObjectInjectionWorld == null || World.DefaultGameObjectInjectionWorld.EntityManager == null) {
            Debug.LogError("Can't find entity default world!");
            return;
        }

        if(dropdown.value >= prefabs.Count) {
            Debug.LogError("Can't find prefab type: " + dropdown.value + "!");
            return;
        }
        #endregion error checking

        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            Entity newEntity = entityManager.CreateEntity(respawnArchetype);

        entityManager.AddComponentData(newEntity, new RespawnPrefabComponent {
            position = new float3(
                UnityEngine.Random.Range(-5, 5),
                10,
                UnityEngine.Random.Range(-5, 5)
            ),

            prefabType = prefabs[dropdown.value].prefabType,
        });
    }
}
