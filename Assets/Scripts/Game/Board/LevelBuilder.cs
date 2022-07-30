using UnityEngine;
using Game.Tiles;
using Zenject;

namespace Game.Board
{
    public class LevelBuilder : IInitializable, ILevelPreset

    {
        public LevelBuilder(DiContainer diContainer, MonoTileController tileController)
        {
            _diContainer = diContainer;
            _tileController = tileController;
        }

        private readonly DiContainer _diContainer;
        private readonly MonoTileController _tileController;

        private static readonly int[,] Board =
        {
            { 1, 1, 1, 1, 0, -1 },
            { -1, 0, -1, -1, 0, -1 },
            { -1, 0, 2, 2, 2, 2 },
            { -1, 0, -1, -1, 0, -1 },
            { 1, 1, 1, 1, 0, -1 }
        };

        public void Initialize()
        {
            for (var i = 0; i < 5; i++)
            {
                for (var j = 0; j < 6; j++)
                {
                    if (Board[i, j] <= 0) continue;
                    var instance = _diContainer.InstantiatePrefab(_tileController);
                    instance.transform.position = new Vector3(i, 0, j);
                    instance.GetComponent<MonoTileController>().Initialize(Board[i, j]);
                }
            }
        }

        public int[,] Grid { get; private set; } = Board;
    }
}