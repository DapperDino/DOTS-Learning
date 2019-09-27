using Unity.Entities;

namespace DapperDino.SpinningCubes
{
    public struct Rotate : IComponentData
    {
        public float radiansPerSecond;
    }
}
