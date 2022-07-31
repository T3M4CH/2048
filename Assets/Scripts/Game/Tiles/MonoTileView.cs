using Game.Settings.Interfaces;
using Game.Constants;
using UnityEngine;
using DG.Tweening;
using Zenject;
using System;
using TMPro;

namespace Game.Tiles
{
    public class MonoTileView : MonoBehaviour
    {
        public event Action OnMoveComplete = () => { };
        public event Action OnDisappeared = () => { };

        [SerializeField] private SkinnedMeshRenderer meshRenderer;
        [SerializeField] private TextMeshPro valueText;
        [SerializeField] private TrailRenderer trail;

        private bool _combine;
        private bool _isPlaying;
        private Color[] _colors;
        private AudioClip _clip;
        private AudioSource _source;
        private MonoTileController _controller;
        private IMemoryPool<ParticleSystem> _mergeEffects;

        [Inject]
        public void Construct(IAudioSettings audioSettings, IColorSettings colorSettings,
            MemoryPool<ParticleSystem> effects)
        {
            _source = audioSettings.AudioSource;
            _clip = audioSettings.AudioStorage[AudioConstants.Merge];
            _colors = colorSettings.Colors;
            _mergeEffects = effects;
        }

        public void ChangeSize(int size)
        {
            ChangeColor(size);
            ChangeText(size);
        }

        public void MoveVertical(int endValue)
        {
            PlaySound();
            if (Mathf.Abs(endValue - transform.position.x) <= 0.2f && !_isPlaying)
            {
                _isPlaying = true;
                transform.DOMoveX(transform.position.x - 0.5f, 0.25f).SetEase(Ease.OutBack)
                    .OnComplete(() =>
                    {
                        _isPlaying = false;
                        transform.DOMoveX(transform.position.x + 0.5f, 0.25f).SetEase(Ease.InBack);
                        OnMoveComplete.Invoke();
                    });
                return;
            }

            transform.DOMoveX(transform.position.x + (endValue - transform.position.x), 0.5f)
                .SetEase(Ease.OutBack, 1, 0.5f)
                .OnComplete(() => OnMoveComplete.Invoke());
        }

        public void MoveHorizontal(int endValue)
        {
            PlaySound();
            if (Mathf.Abs(endValue - transform.position.z) <= 0.2f && !_isPlaying)
            {
                _isPlaying = true;
                transform.DOMoveZ(transform.position.z + 0.5f, 0.25f).SetEase(Ease.OutBack)
                    .OnComplete(() =>
                    {
                        _isPlaying = false;
                        transform.DOMoveZ(transform.position.z - 0.5f, 0.25f).SetEase(Ease.InBack);
                        OnMoveComplete.Invoke();
                    });
                return;
            }

            transform.DOMoveZ(transform.position.z + (endValue - transform.position.z), 0.5f)
                .SetEase(Ease.OutBack, 1, 0.5f)
                .OnComplete(() => OnMoveComplete.Invoke());
        }

        private void ChangeText(int size)
        {
            valueText.text = size.ToString();
        }

        private void ChangeColor(int size)
        {
            var color = _colors[size - 1];
            meshRenderer.material.DOColor(color, 0.2f);
            trail.startColor = color;
            trail.endColor = color;
        }

        private void CreateEffect(out ParticleSystem effect)
        {
            effect = _mergeEffects.Spawn();
            effect.transform.position = transform.position;
            var direction = (new Vector3(_controller.X, 0, _controller.Z) + transform.position) / 2;
            Debug.Log(direction);
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

        public void Disappear()
        {
            PlaySound();
            CreateEffect(out ParticleSystem effect);
            valueText.gameObject.SetActive(false);
            transform.DOScale(0, 3f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                effect.gameObject.SetActive(false);
                _mergeEffects.Despawn(effect);
                OnDisappeared.Invoke();
            });
        }

        public void Initialize(MonoTileController tileController)
        {
            _controller = tileController;
            var size = tileController.size;
            ChangeColor(size);
            ChangeText(size);
        }
    }
}