using System;
using Game.Settings.Interfaces;
using UnityEngine;

namespace Game.Settings
{
    [Serializable]
    public class SerializableColors : IColorSettings
    {
        [field: SerializeField]
        public Color[] Colors
        {
            get;
            private set;
        }
    }
}
