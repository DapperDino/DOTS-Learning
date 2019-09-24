using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

namespace DapperCoding
{
    public class VelocitySystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            float deltaTime = Time.deltaTime;
            return Entities.ForEach((ref Translation translation, in Velocity velocity) =>
            {
                translation.Value += velocity.Value * deltaTime;
            }).Schedule(inputDeps);
        }
    }
}
