using Game.Settings.Interfaces;
using Game.Constants;
using UnityEngine;
using DG.Tweening;
using Zenject;
using System;
using TMPro;

namespace Game.Tiles
{
    public class TileView
    {
        public TileView
        (
            TileViewSettings settings,
            IAudioSettings audioSettings,
            IColorSettings colorSettings,
            MemoryPool<ParticleSystem> effects,
            MonoTileController tileController
        )
        {
            _meshRenderer = settings.MeshRenderer;
            _valueText = settings.ValueText;
            _trail = settings.Trail;
            _transform = tileController.transform;
            _source = audioSettings.AudioSource;
            _clip = audioSettings.AudioStorage[AudioConstants.Merge];
            _colors = colorSettings.Colors;
            _mergeEffects = effects;

            _controller = tileController;
            var size = tileController.size;
            ChangeColor(size);
            ChangeText(size);
        }

        public event Action OnMoveComplete = () => { };
        public event Action OnDisappeared = () => { };

        private SkinnedMeshRenderer _meshRenderer;
        private TextMeshPro _valueText;
        private TrailRenderer _trail;

        private bool _isPlaying;
        private Transform _transform;
        private readonly Color[] _colors;
        private readonly AudioClip _clip;
        private readonly AudioSource _source;
        private readonly MonoTileController _controller;
        private readonly IMemoryPool<ParticleSystem> _mergeEffects;

        public void ChangeSize(int size)
        {
            ChangeColor(size);
            ChangeText(size);
        }

        public void MoveVertical(int endValue, int directionValue)
        {
            PlaySound();
            if (Mathf.Abs(endValue - _transform.position.x) <= 0.2f && !_isPlaying)
            {
                _isPlaying = true;
                var offset = directionValue;
                _transform.DOMoveX(_transform.position.x - offset, 0.25f).SetEase(Ease.OutBack)
                    .OnComplete(() =>
                    {
                        _isPlaying = false;
                        _transform.DOMoveX(_transform.position.x + offset, 0.25f).SetEase(Ease.InBack);
                        OnMoveComplete.Invoke();
                    });
                return;
            }

            _transform.DOMoveX(_transform.position.x + (endValue - _transform.position.x), 0.5f)
                .SetEase(Ease.OutBack, 1, 0.5f)
                .OnComplete(() => OnMoveComplete.Invoke());
        }

        public void MoveHorizontal(int endValue, int directionValue)
        {
            PlaySound();
            if (Mathf.Abs(endValue - _transform.position.z) <= 0.2f && !_isPlaying)
            {
                _isPlaying = true;
                var offset = directionValue;
                _transform.DOMoveZ(_transform.position.z + offset, 0.25f).SetEase(Ease.OutBack)
                    .OnComplete(() =>
                    {
                        _isPlaying = false;
                        _transform.DOMoveZ(_transform.position.z - offset, 0.25f).SetEase(Ease.InBack);
                        OnMoveComplete.Invoke();
                    });
                return;
            }

            _transform.DOMoveZ(_transform.position.z + (endValue - _transform.position.z), 0.5f)
                .SetEase(Ease.OutBack, 1, 0.5f)
                .OnComplete(() => OnMoveComplete.Invoke());
        }

        public void Disappear()
        {
            PlaySound();
            CreateEffect(out ParticleSystem effect);
            _valueText.gameObject.SetActive(false);
            _transform.DOScale(0, 3f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                effect.gameObject.SetActive(false);
                _mergeEffects.Despawn(effect);
                OnDisappeared.Invoke();
            });
        }

        private void ChangeText(int size)
        {
            _valueText.text = size.ToString();
        }

        private void ChangeColor(int size)
        {
            var color = _colors[size - 1];
            _meshRenderer.material.DOColor(color, 0.2f);
            _trail.startColor = color;
            _trail.endColor = color;
        }

        private void CreateEffect(out ParticleSystem effect)
        {
            effect = _mergeEffects.Spawn();
            effect.transform.position = _transform.position;
            var direction = (new Vector3(_controller.x, 0, _controller.z) + _transform.position) / 2;
            effect.transform.LookAt(direction);
            var settings = effect.main;
            settings.startColor = _colors[_controller.size];
            effect.gameObject.SetActive(true);
        }

        private void PlaySound()
        {
            if (!_source.isPlaying)
            {
                _source.PlayOneShot(_clip);
            }
        }

        public class Factory : PlaceholderFactory<TileViewSettings, MonoTileController,TileView>
        {
            
        }
    }
}