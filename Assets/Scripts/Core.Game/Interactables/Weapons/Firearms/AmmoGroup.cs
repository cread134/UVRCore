using UnityEditor;
using UnityEngine;

namespace Core.Game.Interactables.Weapons.Firearms
{
    public class AmmoGroup : MonoBehaviour
    {
        public AmmoGroupConfiguration Configuration;
        public int Quantity { get; private set; }

        private void Awake()
        {
            Quantity = Configuration.MaxQuantity;
        }

        #region Debug
        private void OnDrawGizmos()
        {
            if (Configuration == null) return;
            Vector3 position = transform.position + Vector3.up * 0.01f;
            Handles.Label(position, $"Ammo: {Quantity}/{Configuration.MaxQuantity}");
        }
        #endregion
    }
}
