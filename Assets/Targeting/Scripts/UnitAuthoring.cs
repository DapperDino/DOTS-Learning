using UnityEngine;
using Unity.Entities;

namespace DapperDino.Targeting
{
    public class UnitAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Unit());
        }
    }
}
