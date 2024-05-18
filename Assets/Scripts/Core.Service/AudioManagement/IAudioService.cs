using Core.Service.DependencyManagement;
using UnityEngine;

namespace Core.Service.AudioManagement
{
    public interface IAudioService : IGameService
    {
        AudioInstance PlaySound(GameSound gameSound, Vector3? position = null, Quaternion? rotation = null, bool doLoop = false, AudioOverride? audioOverride = null);
    }
}