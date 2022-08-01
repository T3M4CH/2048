using TMPro;
using System;
using UnityEngine;

namespace Game.Tiles
{
    [Serializable]
    public class TileViewSettings
    {
        [field: SerializeField]
        public SkinnedMeshRenderer MeshRenderer
        {
            get;
            private set;
        }

        [field: SerializeField]
        public TextMeshPro ValueText
        {
            get;
            private set;
        }

        [field: SerializeField]
        public TrailRenderer Trail
        {
            get;
            private set;
        }
    }
}
