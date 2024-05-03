using Core.Service;
using Core.Service.DependencyManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Service.Application
{
    internal class AppSettings : SingletonClass<AppSettings>, IAppSettings
    {
        public static bool IsEditor => Instance._isEditor;
        bool _isEditor;

        protected override void OnCreated()
        {
            _isEditor = UnityEngine.Application.isEditor;
        }
    }
}
