using Unity.Entities;

namespace DapperDino.Targeting
{
    public struct HasTarget : IComponentData
    {
        public Entity targetEntity;
    }
}
