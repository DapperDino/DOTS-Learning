using Unity.Entities;
using Unity.Mathematics;

namespace DapperCoding
{
    [GenerateAuthoringComponent]
    public struct Velocity : IComponentData
    {
        public float4 value;
    }
}
