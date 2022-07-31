using Game.Board;
using Game.Settings;
using Game.Tiles;
using Zenject;
using UnityEngine;

namespace Game.Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private SerializableAudioSettings audioSettings;
        [SerializeField] private SerializableColors colorSettings;
        [SerializeField] private ParticleSystem mergeEffect;
        [SerializeField] private GameObject tilePrefab;

        public override void InstallBindings()
        {
            Container
                .BindInterfacesTo<SerializableColors>()
                .FromInstance(colorSettings)
                .AsSingle();

            Container
                .BindInterfacesTo<SerializableAudioSettings>()
                .FromInstance(audioSettings)
                .AsSingle();

            Container
                .Bind<MonoTileController>()
                .FromComponentOn(tilePrefab)
                .AsSingle();

            Container
                .BindInterfacesTo<LevelBuilder>()
                .AsSingle()
                .NonLazy();

            Container
                .Bind<TileStorage>()
                .AsSingle();

            Container
                .BindInterfacesTo<CellService>()
                .AsSingle();

            Container
                .BindMemoryPool<ParticleSystem, MemoryPool<ParticleSystem>>()
                .WithInitialSize(10)
                .FromComponentInNewPrefab(mergeEffect)
                .UnderTransformGroup("Effects");
        }
    }
}