using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System;
using Game.Settings.Interfaces;

namespace Game.Settings
{
    [Serializable]
    public class SerializableAudioSettings : IInitializable, IAudioSettings
    {
        [SerializeField] private List<SerializablePair<string, AudioClip>> audioStorage;
        
        public void Initialize()
        {
            if (AudioStorage.Count > 0) return;
            foreach (var clip in audioStorage)
            {
                AudioStorage.Add(clip.Key, clip.Value);
            }
        }

        public Dictionary<string, AudioClip> AudioStorage { get; private set; } = new();
        
        [field: SerializeField]
        public AudioSource AudioSource
        {
            get;
            private set;
        }
    }
}
