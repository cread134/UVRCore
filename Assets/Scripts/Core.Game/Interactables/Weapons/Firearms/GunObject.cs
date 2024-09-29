using Core.DevTools.Scripting;
using Core.Service.DependencyManagement;
using Core.Service.Logging;
using Core.XRFramework;
using Core.XRFramework.Interaction;
using Core.XRFramework.Physics;
using UnityEngine;

namespace Core.Game.Interactables.Weapons.Firearms
{
    [RequireComponent(typeof(PhysicsObject))]
    public class GunObject : MonoBehaviour
    {
        PhysicsObject physicsObject;

        [Header("Instance settings")]
        public GunConfiguration gunConfiguration;
        public Transform barrelReference;

        public ObjectInput ammunitionInput;

        bool shootInputDown = false;
        float lastShootTime = 0;

        IAmmunitionSource ammunitionSource;

        private void Awake()
        {
            physicsObject = GetComponent<PhysicsObject>();
        }

        private void Update()
        {
            if (shootInputDown)
            {
                TryShoot();
            }
        }

        public void ShootInput(HandType handType, GrabPoint grabPoint)
        {
            Debug.Log($"Gun shoot input received GrabPoint: {grabPoint.name} HandType: {handType}", this);
            if (grabPoint == null)
                return;
            if (grabPoint.IsGrabbed == false)
                return;

            shootInputDown = true;
            TryShoot();
        }

        public void ShootInputUp(HandType handType, GrabPoint grabPoint)
        {
            shootInputDown = false;
        }

        bool TryShoot()
        {
            if (CanShoot())
            {
                Shoot();
                return true;
            }
            return false;
        }

        bool CanShoot()
        {
            if (Time.time - lastShootTime < gunConfiguration.FireRate)
            {
                return false;
            }

            if (ammunitionSource == null)
            {
                return false;
            }

            if (!ammunitionSource.Peek())
            {
                return false;
            }

            return true;
        }

        void Shoot()
        {
            Debug.Log("Gun shoot performed", this);
            DoRecoil();
            PlayShootSound();
        }

        void DoRecoil()
        {
        }

        void PlayShootSound()
        {
        }

        public void ReleaseAmmunition()
        {
            if (ammunitionInput != null)
            {
                ammunitionInput.ReleaseInput();
            }
            OnUnloadAmmunitionSource();
        }

        public void OnLoadAmmunitionSource(IAmmunitionSource ammunitionSource)
        {
            if (ammunitionSource != null)
            {
                Debug.LogError("Cannot load ammunition when it is already loaded");
            }
            this.ammunitionSource = ammunitionSource;
        }

        public void OnUnloadAmmunitionSource()
        {
            this.ammunitionSource = null;
        }

        #region Debug
        private void OnDrawGizmos()
        {
            if (barrelReference != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(barrelReference.position, 0.005f);
                Gizmos.DrawLine(barrelReference.position, barrelReference.position + barrelReference.forward * 0.01f);
            }
        }
        #endregion
    }
}
