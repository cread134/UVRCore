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
        private void Awake()
        {
            physicsObject = GetComponent<PhysicsObject>();
        }

        public void ShootInput(HandType handType, GrabPoint grabPoint)
        {
            Debug.Log($"Gun shoot input received GrabPoint: {grabPoint.name} HandType: {handType}", this);
            if (grabPoint == null)
                return;
            if (grabPoint.IsGrabbed == false)
                return;

            shootInputDown = true;
            Shoot();
        }

        public void ShootInputUp(HandType handType, GrabPoint grabPoint)
        {
            shootInputDown = false;
        }

        void Shoot()
        {
            Debug.Log("Gun shoot performed", this);
            DoRecoil();
            PlayShootSound();
        }

        void DoRecoil()
        {
            var recoil = gunConfiguration.PositionalRecoil;
            var angularRecoil = barrelReference.right * -gunConfiguration.AngularRecoil.x;
            physicsObject.AddForce(recoil, ForceMode.Impulse);
            physicsObject.AddTorque(angularRecoil, ForceMode.Impulse);
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
