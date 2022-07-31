using System.Collections.Generic;
using Game.Board.Interfaces;
using System.Linq;
using UnityEngine;
using Game.Enums;
using Game.Tiles;

namespace Game.Board
{
    public class CellService : ICellService
    {
        private readonly int[,] _grid;
        private readonly TileStorage _tileStorage;
        private ILevelPreset _levelPreset;
        private List<MonoTileController> _blocks;
        private int _height;
        private int _width;
        
        private CellService(TileStorage tileStorage, ILevelPreset levelPreset)
        {
            _grid = levelPreset.Grid;
            _width = _grid.GetLength(1);
            _height = _grid.GetLength(0);
            _tileStorage = tileStorage;
            levelPreset.OnLevelBuilt += Initialize;
        }
        
        public void GetSuitableCell(MonoTileController tile, Vector2Int direction)
        {
            if (direction.y != 0)
            {
                var i = tile.X;
                _grid[i, tile.Z] = 0;
                while (i >= 0 &&  i <= _height - 1)
                {
                    i -= direction.y;
                    if (i < 0 || i > _height - 1 || (_grid[i, tile.Z] != 0 && _grid[i, tile.Z] != tile.size))
                    {
                        i += direction.y;
                        break;
                    }

                    if (_grid[i, tile.Z] != tile.size) continue;
                    if (i > 0 && i < _height - 1)
                    {
                        i -= direction.y;
                        if (_grid[i, tile.Z] == tile.size + 1)
                        {
                            var targetBlock = _blocks.Find(x => x.X == i && x.Z == tile.Z && !x.victim);
                            var consumerBlock = _blocks.Find(x => x.X == i + direction.y && x.Z == tile.Z);
                            MergeThrough(EAxis.Vertical, direction.y, consumerBlock, tile, targetBlock);
                            tile.SetVertical(i);
                            return;
                        }

                        i += direction.y;
                    }


                    var blockPref = _blocks.Find(x => x.X == i && x.Z == tile.Z);
                    tile.X = i;
                    if (blockPref == null || tile.size != blockPref.size)
                    {
                        i += direction.y;
                        _grid[i, tile.Z] = tile.size;
                        tile.X = i;
                        tile.SetVertical(i);
                        return;
                    }


                    if (tile.victim) continue;
                    tile.victim = true;
                    tile.ChangeSize(tile.size);
                    blockPref.ChangeSize(blockPref.size + 1);

                    _grid[i, tile.Z] = blockPref.size;
                    tile.SetVertical(i);
                    return;
                }

                if (tile.victim) RemoveTile(tile);

                tile.X = i;
                _grid[i, tile.Z] = tile.size;
                tile.SetVertical(i);
            }

            if (direction.x != 0)
            {
                var i = tile.Z;
                _grid[tile.X, i] = 0;
                while (i >= 0 && i <= _width - 1)
                {
                    i += direction.x;
                    if (i < 0 || i > _width - 1 || (_grid[tile.X, i] != 0 && _grid[tile.X, i] != tile.size))
                    {
                        i -= direction.x;
                        break;
                    }

                    if (_grid[tile.X, i] != tile.size) continue;
                    if (i > 0  && i < _width - 1)
                    {
                        i += direction.x;
                        if (_grid[tile.X, i] == tile.size + 1)
                        {
                            var targetBlock = _blocks.Find(x => x.Z == i && x.X == tile.X && !x.victim);
                            var consumerBlock = _blocks.Find(x => x.Z == i - direction.x && x.X == tile.X);
                            MergeThrough(EAxis.Horizontal, direction.x, consumerBlock, tile, targetBlock);
                            tile.SetHorizontal(i);
                            return;
                        }

                        i -= direction.x;
                    }


                    var blockPref = _blocks.Find(x => x.Z == i && x.X == tile.X);
                    tile.Z = i;
                    if (blockPref == null || tile.size != blockPref.size)
                    {
                        i -= direction.x;
                        _grid[tile.X, i] = tile.size;
                        tile.Z = i;
                        tile.SetHorizontal(i);
                        return;
                    }


                    if (tile.victim) continue;
                    tile.victim = true;
                    tile.ChangeSize(tile.size);
                    blockPref.ChangeSize(blockPref.size + 1);


                    _grid[tile.X, i] = blockPref.size;
                    tile.SetHorizontal(i);
                    return;
                }

                if (tile.victim) RemoveTile(tile);

                _grid[tile.X, i] = tile.size;
                tile.SetHorizontal(i);
            }
        }

        private void MergeThrough(EAxis eAxis, int directionValue, MonoTileController victimMonoTileController,
            MonoTileController currentMonoTileController, MonoTileController targetMonoTileController)
        {
            if (eAxis == EAxis.Horizontal)
            {
                var targetPoint = targetMonoTileController.Z;
                targetMonoTileController.ChangeSize(targetMonoTileController.size + 1);
                RemoveTile(currentMonoTileController);
                RemoveTile(victimMonoTileController);
                //victimTile.MoveHorizontal(targetPoint);
                victimMonoTileController.SetHorizontal(targetPoint);
                currentMonoTileController.SetHorizontal(targetPoint);
                targetMonoTileController.SetHorizontal(targetPoint);
                _grid[victimMonoTileController.X, targetPoint - directionValue] = 0;
                _grid[currentMonoTileController.X, targetPoint] = targetMonoTileController.size;
            }
            else
            {
                var targetPoint = targetMonoTileController.X;
                targetMonoTileController.ChangeSize(targetMonoTileController.size + 1);
                RemoveTile(currentMonoTileController);
                RemoveTile(victimMonoTileController);
                //victimTile.MoveVertical(targetPoint);
                victimMonoTileController.SetVertical(targetPoint);
                currentMonoTileController.SetVertical(targetPoint);
                targetMonoTileController.SetVertical(targetPoint);
                _grid[targetPoint + directionValue, victimMonoTileController.Z] = 0;
                _grid[targetPoint, victimMonoTileController.Z] = targetMonoTileController.size;
            }
        }

        private void RemoveTile(MonoTileController monoTileController)
        {
            _blocks.Remove(monoTileController);
            _tileStorage.Tiles = _blocks;
            monoTileController.Sacrifice();
        }

        private void Initialize()
        {
            _blocks = _tileStorage.Tiles.ToList();
            foreach (var block in _blocks)
            {
                _grid[block.X, block.Z] = block.size;
            }
        }
    }
}