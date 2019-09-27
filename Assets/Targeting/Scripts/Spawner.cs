using Unity.Entities;

namespace DapperDino.Targeting
{
    public struct Spawner : IComponentData
    {
        public Entity prefab;
        public float maxDistanceFromSpawner;
        public float secondsBetweenSpawns;
        public float secondsToNextSpawn;
    }
}
