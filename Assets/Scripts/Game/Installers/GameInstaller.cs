using Game.Board;
using Game.Board.Interfaces;
using Game.Settings;
using Game.Tiles;
using Zenject;
using UnityEngine;

namespace Game.Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private SerializableColors colorSettings;
        [SerializeField] private SerializableAudioSettings audioSettings;
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
                .BindInterfacesTo<CellService>()
                .FromComponentsInHierarchy()
                .AsSingle();
        }

    }
}