using UnityEngine;

namespace Core.Game.World
{
    [CreateAssetMenu(fileName = "EntityConfiguration", menuName = "Core/Game/World/EntityConfiguration")]
    public class EntityConfiguration : ScriptableObject
    {
        public GameObject Template;
    }
}
