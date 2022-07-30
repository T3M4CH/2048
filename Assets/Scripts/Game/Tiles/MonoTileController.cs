using System;
using UnityEngine;

namespace Game.Tiles
{
    public class MonoTileController : MonoBehaviour
    {
        public event Action OnActionComplete = () => {};
        public bool victim;

        private MonoTileView _monoTileView;

        public void Initialize(int sizeValue)
        {
            size = sizeValue;
            _monoTileView = GetComponent<MonoTileView>();
        }

        public void ChangeSize(int value)
        {
            size = value;
            _monoTileView.ChangeSize(value);
        }

        public void SetVertical(int value)
        {
            //TODO: VALIDATE via Board height in model
            X = value;
            _monoTileView.MoveVertical(value);
        }

        public void SetHorizontal(int value)
        {
            Z = value;
            _monoTileView.MoveHorizontal(value);
        }

        public void Sacrifice()
        {
            // _period = 2;
            // _amplitude = 1;
            OnActionComplete += () => { gameObject.SetActive(false); };
            _monoTileView.Disappear();
        }

        private void Start()
        {
            _monoTileView.Initialize(size);
            _monoTileView.OnMoveComplete += () => OnActionComplete.Invoke();
            _monoTileView.OnDisappeared += () => Destroy(gameObject);
            X = Mathf.RoundToInt(transform.position.x);
            Z = Mathf.RoundToInt(transform.position.z);
        }

        public int size = 1;

        public int X;
        public int Z;
    }
}