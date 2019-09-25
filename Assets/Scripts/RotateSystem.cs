using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

namespace DapperDino
{
    public class RotateSystem : JobComponentSystem
    {
        private struct RotateJob : IJobForEach<RotationEulerXYZ, Rotate>
        {
            public float deltaTime;

            public void Execute(ref RotationEulerXYZ euler, [Unity.Collections.ReadOnly] ref Rotate rotate)
            {
                euler.Value.y += rotate.radiansPerSecond * deltaTime;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new RotateJob { deltaTime = Time.deltaTime };
            return job.Schedule(this, inputDeps);
        }
    }
}
