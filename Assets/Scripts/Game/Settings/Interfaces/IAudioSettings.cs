using System.Collections.Generic;
using UnityEngine;

namespace Game.Settings.Interfaces
{
    public interface IAudioSettings
    {
        public Dictionary<string, AudioClip> AudioStorage
        {
            get;
        }

        public AudioSource AudioSource
        {
            get;
        }
    }
}