using System;
using Game.Settings.Interfaces;
using TMPro;
using UnityEngine;
using Zenject;

namespace Game.Tiles
{
    public class MonoTileController : MonoBehaviour
    {
        public event Action OnActionComplete = () => { };
        public bool combineElement;
        public int size = 1;
        public int x;
        public int z;

        [SerializeField] private TileViewSettings tileViewSettings;

        private MonoTileView _tileView;

        [Inject]
        private void Construct(IAudioSettings audioSettings, IColorSettings colorSettings,
            MemoryPool<ParticleSystem> effects)
        {
            _tileView = new MonoTileView(audioSettings, colorSettings, effects, tileViewSettings, this);
            _tileView.OnMoveComplete += () => OnActionComplete.Invoke();
            _tileView.OnDisappeared += () => Destroy(gameObject);
        }

        public void Initialize(int sizeValue, int xPos, int zPos)
        {
            x = xPos;
            z = zPos;
            size = sizeValue;
        }

        public void ChangeSize(int value)
        {
            size = value;
            _tileView.ChangeSize(value);
        }

        public void SetVertical(int value, int directionValue)
        {
            x = value;
            _tileView.MoveVertical(value, directionValue);
        }

        public void SetHorizontal(int value, int directionValue)
        {
            z = value;
            _tileView.MoveHorizontal(value, directionValue);
        }

        public void Combine()
        {
            OnActionComplete += () => { gameObject.SetActive(false); };
            _tileView.Disappear();
        }
    }
}