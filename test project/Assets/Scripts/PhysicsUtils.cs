using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

public struct PhysicsUtils
{
    public unsafe static void ColliderCastAll(
        ref NativeList<ColliderCastHit> castHits,
        in PhysicsWorldSingleton physicsWorldSingleton,
        in float3 fromPos, in float3 toPos, in PhysicsCollider collider) 
    {
        unsafe {
            physicsWorldSingleton.CastCollider(new ColliderCastInput {
                Start = fromPos,
                End = toPos,
                Collider = collider.ColliderPtr
            }, ref castHits); 
        }
    }

    public unsafe static NativeList<DistanceHit> ColliderDistanceAll(
        in PhysicsCollider collider,
        float maxDistance,
        in RigidTransform transform,
        in PhysicsWorldSingleton physicsWorldSingleton,
        Entity ignore,
        Allocator allocator = Allocator.TempJob)
    {
        ColliderDistanceInput input = new ColliderDistanceInput {
            Collider = collider.ColliderPtr,
            MaxDistance = maxDistance,
            Transform = transform
        };

        NativeList<DistanceHit> allDistances = new NativeList<DistanceHit>(allocator);

        if(physicsWorldSingleton.CalculateDistance(input, ref allDistances)) {
            TrimByEntity(ref allDistances, ignore);
        }
        return allDistances;
    }

    //Remove an entity if it is in the castResults list
    public static void TrimByEntity<T>(ref NativeList<T> castResults, Entity ignore) where T : unmanaged, IQueryResult {
        if (ignore == Entity.Null) {
            return;
        }

        for (int i = (castResults.Length - 1); i >= 0; --i) {
            if (ignore == castResults[i].Entity) {
                castResults.RemoveAt(i);
            }
        }
    }
    
    public unsafe static bool ColliderDistance(
        out ColliderCastHit nearestHit,
        in PhysicsCollider collider,
        float3 from,
        float3 to,
        ref PhysicsWorldSingleton physicsWorldSingleton,
        Entity ignore,
        CollisionFilter? filter = null,
        EntityManager? manager = null,
        ComponentLookup<PhysicsCollider>? colliderData = null,
        Allocator allocator = Allocator.TempJob)
    {
        nearestHit = new ColliderCastHit();
        NativeList<ColliderCastHit> allHits = new NativeList<ColliderCastHit>(Allocator.Temp);
            ColliderCastAll(
                ref allHits,
                in physicsWorldSingleton,
                from, to,
                in collider
            );
        

        if (filter.HasValue) {
            if (manager.HasValue)
                TrimByFilter(ref allHits, manager.Value, filter.Value);
            
            else if (colliderData.HasValue)
                TrimByFilter(ref allHits, colliderData.Value, filter.Value);
        }

        GetSmallestFractional(ref allHits, out nearestHit);
        allHits.Dispose();

        return true;
    }

    public unsafe static void TrimByFilter<T>(ref NativeList<T> castResults, ComponentLookup<PhysicsCollider> colliderData, CollisionFilter filter) where T : unmanaged, IQueryResult {
        for (int i = (castResults.Length - 1); i >= 0; --i) {
            if (colliderData.HasComponent(castResults[i].Entity)) {
                PhysicsCollider collider = colliderData[castResults[i].Entity];

                if (CollisionFilter.IsCollisionEnabled(filter, collider.Value.Value.GetCollisionFilter()))
                    continue;
            }

            castResults.RemoveAt(i);
        }
    }

    public unsafe static void TrimByFilter<T>(ref NativeList<T> castResults, EntityManager entityManager, CollisionFilter filter) where T : unmanaged, IQueryResult {
        for (int i = (castResults.Length - 1); i >= 0; --i) {
            if (entityManager.HasComponent<PhysicsCollider>(castResults[i].Entity)) {
                PhysicsCollider collider = entityManager.GetComponentData<PhysicsCollider>(castResults[i].Entity);

                if (CollisionFilter.IsCollisionEnabled(filter, collider.Value.Value.GetCollisionFilter()))
                    continue;
            }

            castResults.RemoveAt(i);
        }
    }

    public static bool GetSmallestFractional<T>(ref NativeList<T> castResults, out T nearest) where T : unmanaged, IQueryResult {
        nearest = default(T);

        if (castResults.Length == 0)
            return false;

        float smallest = float.MaxValue;

        for (int i = 0; i < castResults.Length; ++i) {
            if (castResults[i].Fraction < smallest) {
                smallest = castResults[i].Fraction;
                nearest = castResults[i];
            }
        }

        return true;
    }
}
