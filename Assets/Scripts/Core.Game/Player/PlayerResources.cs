using UnityEngine;
using Unity.Netcode;
using Core.Game.Integration;
using Core.Service.ObjectManagement;

namespace Core.Game
{
    public class PlayerResources : NetworkBehaviour, ISpawnable, IDamageable
    {
        NetworkVariable<int> playerHealth = new NetworkVariable<int>(100);

        public void Spawned()
        {
            playerHealth.OnValueChanged += OnHealthChanged;
        }

        public void TakeDamage(int damage)
        {
            TakeDamageServerRpc(damage);
        }

        [ServerRpc]
        void TakeDamageServerRpc(int damage)
        {
            playerHealth.Value -= damage;
        }

        private void OnHealthChanged(int previousValue, int newValue)
        {
            Debug.Log($"Health changed from {previousValue} to {newValue}");
        }
    }
}
