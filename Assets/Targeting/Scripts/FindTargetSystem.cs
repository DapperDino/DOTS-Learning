using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.InputSystem.XR.Haptics;

namespace DapperDino.Targeting
{
    public class FindTargetSystem : JobComponentSystem
    {
        private struct EntityWithPosition
        {
            public Entity entity;
            public float3 position;
        }

        [BurstCompile]
        [RequireComponentTag(typeof(Unit))]
        [ExcludeComponent(typeof(HasTarget))]
        private struct FindTargetJob : IJobForEachWithEntity<Translation>
        {
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<EntityWithPosition> targetArray;
            public NativeArray<Entity> closestTargetEntityArray;

            public void Execute(Entity entity, int index, [ReadOnly] ref Translation translation)
            {
                float3 unitPosition = translation.Value;

                var closestTargetEntity = Entity.Null;
                var closestTargetPosition = float3.zero;

                for (int i = 0; i < targetArray.Length; i++)
                {
                    EntityWithPosition targetEntityWithPosition = targetArray[i];

                    if (closestTargetEntity == Entity.Null)
                    {
                        closestTargetEntity = targetEntityWithPosition.entity;
                        closestTargetPosition = targetEntityWithPosition.position;
                    }
                    else
                    {
                        if (math.distance(unitPosition, targetEntityWithPosition.position) < math.distance(unitPosition, closestTargetPosition))
                        {
                            closestTargetEntity = targetEntityWithPosition.entity;
                            closestTargetPosition = targetEntityWithPosition.position;
                        }
                    }
                }

                closestTargetEntityArray[index] = closestTargetEntity;
            }
        }

        [RequireComponentTag(typeof(Unit))]
        [ExcludeComponent(typeof(HasTarget))]
        private struct AddComponentJob : IJobForEachWithEntity<Translation>
        {
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> closestTargetEntityArray;
            public EntityCommandBuffer.Concurrent entityCommandBuffer;

            public void Execute(Entity entity, int index, ref Translation translation)
            {
                if (closestTargetEntityArray[index] != Entity.Null)
                {
                    entityCommandBuffer.AddComponent(index, entity, new HasTarget { targetEntity = closestTargetEntityArray[index] });
                }
            }
        }

        private EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem;

        protected override void OnCreate() => endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            EntityQuery targetQuery = GetEntityQuery(typeof(Target), ComponentType.ReadOnly<Translation>());

            NativeArray<Entity> targetEntityArray = targetQuery.ToEntityArray(Allocator.TempJob);
            NativeArray<Translation> targetTranslationArray = targetQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

            var targetArray = new NativeArray<EntityWithPosition>(targetTranslationArray.Length, Allocator.TempJob);

            for (int i = 0; i < targetEntityArray.Length; i++)
            {
                targetArray[i] = new EntityWithPosition
                {
                    entity = targetEntityArray[i],
                    position = targetTranslationArray[i].Value,
                };
            }

            targetEntityArray.Dispose();
            targetTranslationArray.Dispose();

            EntityQuery unitQuery = GetEntityQuery(typeof(Unit), ComponentType.Exclude<HasTarget>());
            var closestTargetEntityArray = new NativeArray<Entity>(unitQuery.CalculateEntityCount(), Allocator.TempJob);

            var findTargetJob = new FindTargetJob
            {
                targetArray = targetArray,
                closestTargetEntityArray = closestTargetEntityArray,
            };

            JobHandle jobHandle = findTargetJob.Schedule(this, inputDeps);

            var addComponentJob = new AddComponentJob
            {
                closestTargetEntityArray = closestTargetEntityArray,
                entityCommandBuffer = endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
            };

            jobHandle = addComponentJob.Schedule(this, jobHandle);

            endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);

            return jobHandle;
        }
    }
}
