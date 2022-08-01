using System;
using System.Collections.Generic;
using Game.Board.Interfaces;
using UnityEngine;
using Game.Tiles;
using Zenject;

namespace Game.Board
{
    public class LevelBuilder : IInitializable, ILevelPreset
    {
        public LevelBuilder(DiContainer diContainer, MonoTileController tileController, TileStorage tileStorage)
        {
            _tileStorage = tileStorage;
            _diContainer = diContainer;
            _tileController = tileController;
        }

        private readonly DiContainer _diContainer;
        private readonly TileStorage _tileStorage;
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
            List<MonoTileController> tiles = new();
            for (var i = 0; i < 5; i++)
            {
                for (var j = 0; j < 6; j++)
                {
                    if (Board[i, j] <= 0) continue;
                    var instance = _diContainer.InstantiatePrefab(_tileController).GetComponent<MonoTileController>();
                    instance.transform.position = new Vector3(i, 0, j);
                    instance.Initialize(Board[i, j], i, j);
                    tiles.Add(instance);
                }
            }

            _tileStorage.Tiles = tiles;

            OnLevelBuilt.Invoke();
        }

        public event Action OnLevelBuilt = () => { };
        public int[,] Grid { get; } = Board;
    }
}