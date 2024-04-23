using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Service.Physics
{
    [CreateAssetMenu]
    public class LayerMaskConfiguration : ScriptableObject
    {
        public string Key;
        public LayerMask LayerMask;
    }
}
