using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Time = UnityEngine.Time;

namespace DapperDino.Targeting
{
    public class SpawnerSystem : ComponentSystem
    {
        private Random random;

        protected override void OnCreate() => random = new Random(1234);

        protected override void OnUpdate()
        {
            float deltaTime = Time.deltaTime;

            Entities.ForEach((ref Spawner spawner, ref LocalToWorld localToWorld) =>
            {
                spawner.secondsToNextSpawn -= deltaTime;

                if (spawner.secondsToNextSpawn < 0)
                {
                    spawner.secondsToNextSpawn += spawner.secondsBetweenSpawns;

                    Entity instance = EntityManager.Instantiate(spawner.prefab);
                    EntityManager.SetComponentData(instance, new Translation
                    {
                        Value = localToWorld.Position + random.NextFloat3Direction() * random.NextFloat() * spawner.maxDistanceFromSpawner
                    });
                }
            });
        }
    }
}
