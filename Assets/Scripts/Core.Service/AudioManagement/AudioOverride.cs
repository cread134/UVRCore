namespace Core.Service.AudioManagement
{
    public struct AudioOverride
    {
        public static AudioOverride Default => new AudioOverride()
        {
            VolumeMultiplier = 1.0f,
            PitchMultiplier = 1.0f,
        };
        public float VolumeMultiplier;
        public float PitchMultiplier;
    }
}
