using Core.DevTools.Scripting;
using Core.Service.DependencyManagement;
using Core.Service.Logging;
using Core.XRFramework;
using Core.XRFramework.Interaction;
using Core.XRFramework.Physics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Game
{
    [RequireComponent(typeof(PhysicsObject))]
    public class GunObject : MonoBehaviour
    {
        LazyComponent<PhysicsObject> physicsObject;
        LazyService<ILoggingService> loggingService = new LazyService<ILoggingService>();

        [Header("Instance settings")]
        public GunConfiguration gunConfiguration;
        public Transform barrelReference;

        private void Awake()
        {
            physicsObject = new LazyComponent<PhysicsObject>(gameObject);
        }

        public void ShootInput(HandType handType, GrabPoint grabPoint)
        {
            loggingService.Value.Log($"Gun shoot input received GrabPoint: {grabPoint.name} HandType: {handType}", LogLevel.Info, this);
            if (grabPoint == null)
                return;
            if (grabPoint.IsGrabbed == false)
                return;
            Shoot();
        }

        void Shoot()
        {
            loggingService.Value.Log("Gun shoot performed", LogLevel.Info, this);
            DoRecoil();
            PlayShootSound();
        }

        void DoRecoil()
        {
            var recoil = gunConfiguration.PositionalRecoil;
            var angularRecoil = barrelReference.right * -gunConfiguration.AngularRecoil.x;
            physicsObject.Value.AddForce(recoil, ForceMode.Impulse);
            physicsObject.Value.AddTorque(angularRecoil, ForceMode.Impulse);
        }

        void PlayShootSound()
        {

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
