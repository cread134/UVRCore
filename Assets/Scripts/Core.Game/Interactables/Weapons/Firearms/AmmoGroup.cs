using UnityEditor;
using UnityEngine;

namespace Core.Game.Interactables.Weapons.Firearms
{
    public class AmmoGroup : MonoBehaviour, IAmmunitionSource
    {
        public AmmoGroupConfiguration Configuration;
        public int Quantity { get => quantity; set => quantity = value; }

        AmmoGroupConfiguration IAmmunitionSource.Configuration => Configuration;

        int quantity = 0;

        private void Awake()
        {
            Quantity = Configuration.MaxQuantity;
        }

        public bool Take()
        {
            if (Quantity > 0)
            {
                Quantity--;
                return true;
            }
            return false;
        }

        public bool Peek()
        {
            return Quantity > 0;
        }

        public bool Put()
        {
            if (Quantity < Configuration.MaxQuantity)
            {
                Quantity++;
                return true;
            }
            return false;
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
