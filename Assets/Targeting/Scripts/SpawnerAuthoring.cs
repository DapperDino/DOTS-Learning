using UnityEngine;
using Unity.Entities;
using System.Collections.Generic;

namespace DapperDino.Targeting
{
    public class SpawnerAuthoring : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private float secondsBetweenSpawns;
        [SerializeField] private float maxDistanceFromSpawner;

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(prefab);
        }

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Spawner
            {
                prefab = conversionSystem.GetPrimaryEntity(prefab),
                maxDistanceFromSpawner = maxDistanceFromSpawner,
                secondsBetweenSpawns = secondsBetweenSpawns,
                secondsToNextSpawn = 0f,
            });
        }
    }
}
