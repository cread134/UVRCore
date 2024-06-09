using Core.Service;
using Core.Service.DependencyManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Service.Application
{
    internal class AppSettings : MonoBehaviour, IAppSettings
    {
        [Inject]
        public void Inject()
        {
        }
    }
}
