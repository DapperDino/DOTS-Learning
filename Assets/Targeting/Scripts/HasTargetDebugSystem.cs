using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace DapperDino.Targeting
{
    public class HasTargetDebugSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref Translation translation, ref HasTarget hasTarget) => {
                if (World.Active.EntityManager.Exists(hasTarget.targetEntity))
                {
                    Translation targetTranslation = World.Active.EntityManager.GetComponentData<Translation>(hasTarget.targetEntity);
                    Debug.DrawLine(translation.Value, targetTranslation.Value);
                }
            });
        }
    }
}
