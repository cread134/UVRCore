using Core.Service.DependencyManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Service.AudioManagement
{
    public static class AudioExtensions
    {
        public static AudioInstance PlaySound(this GameSound gameSound, Vector3? position = null, Quaternion? rotation = null, bool doLoop = false)
        {
            var audioService = ObjectFactory.ResolveService<IAudioService>();
            return audioService.PlaySound(gameSound, position: position, rotation: rotation, doLoop: doLoop);
        }
    }
}
