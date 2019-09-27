using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DapperDino.Targeting
{
    public class UnitMoveToTargetSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity unitEntity, ref HasTarget hasTarget, ref Translation unitTranslation) =>
            {
                if (World.Active.EntityManager.Exists(hasTarget.targetEntity))
                {
                    var targetTranslation = World.Active.EntityManager.GetComponentData<Translation>(hasTarget.targetEntity);

                    float3 targetDirection = math.normalize(targetTranslation.Value - unitTranslation.Value);
                    float moveSpeed = 10f;
                    unitTranslation.Value += targetDirection * moveSpeed * Time.deltaTime;

                    if (math.distance(unitTranslation.Value, targetTranslation.Value) < 0.2f)
                    {
                        PostUpdateCommands.DestroyEntity(hasTarget.targetEntity);
                        PostUpdateCommands.RemoveComponent(unitEntity, typeof(HasTarget));
                    }
                }
                else
                {
                    PostUpdateCommands.RemoveComponent(unitEntity, typeof(HasTarget));
                }
            });
        }
    }
}
