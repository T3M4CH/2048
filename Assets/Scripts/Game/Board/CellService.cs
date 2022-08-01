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
        private readonly int _height;
        private readonly int _width;
        private readonly TileStorage _tileStorage;
        private ILevelPreset _levelPreset;
        private List<MonoTileController> _blocks;

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
                var i = tile.x;
                _grid[i, tile.z] = 0;
                while (i >= 0 && i <= _height - 1)
                {
                    i -= direction.y;
                    if (i < 0 || i > _height - 1 || (_grid[i, tile.z] != 0 && _grid[i, tile.z] != tile.size))
                    {
                        i += direction.y;
                        break;
                    }

                    if (_grid[i, tile.z] != tile.size) continue;
                    if (i > 0 && i < _height - 1)
                    {
                        i -= direction.y;
                        if (_grid[i, tile.z] == tile.size + 1)
                        {
                            var targetBlock = _blocks.Find(x => x.x == i && x.z == tile.z && !x.combineElement);
                            var consumerBlock = _blocks.Find(x => x.x == i + direction.y && x.z == tile.z);
                            MergeThrough(EAxis.Vertical, direction.y, consumerBlock, tile, targetBlock);
                            tile.SetVertical(i, direction.y);
                            return;
                        }

                        i += direction.y;
                    }


                    var blockPref = _blocks.Find(x => x.x == i && x.z == tile.z);
                    tile.x = i;
                    if (blockPref == null || tile.size != blockPref.size)
                    {
                        i += direction.y;
                        _grid[i, tile.z] = tile.size;
                        tile.x = i;
                        tile.SetVertical(i, direction.y);
                        return;
                    }


                    if (tile.combineElement) continue;
                    tile.combineElement = true;
                    tile.ChangeSize(tile.size);
                    blockPref.ChangeSize(blockPref.size + 1);

                    _grid[i, tile.z] = blockPref.size;
                    tile.SetVertical(i, direction.y);
                    return;
                }

                if (tile.combineElement) RemoveTile(tile);

                tile.x = i;
                _grid[i, tile.z] = tile.size;
                tile.SetVertical(i, direction.y);
            }

            if (direction.x != 0)
            {
                var i = tile.z;
                _grid[tile.x, i] = 0;
                while (i >= 0 && i <= _width - 1)
                {
                    i += direction.x;
                    if (i < 0 || i > _width - 1 || (_grid[tile.x, i] != 0 && _grid[tile.x, i] != tile.size))
                    {
                        i -= direction.x;
                        break;
                    }

                    if (_grid[tile.x, i] != tile.size) continue;
                    if (i > 0 && i < _width - 1)
                    {
                        i += direction.x;
                        if (_grid[tile.x, i] == tile.size + 1)
                        {
                            var targetBlock = _blocks.Find(x => x.z == i && x.x == tile.x && !x.combineElement);
                            var consumerBlock = _blocks.Find(x => x.z == i - direction.x && x.x == tile.x);
                            MergeThrough(EAxis.Horizontal, direction.x, consumerBlock, tile, targetBlock);
                            tile.SetHorizontal(i, direction.x);
                            return;
                        }

                        i -= direction.x;
                    }


                    var blockRef = _blocks.Find(x => x.z == i && x.x == tile.x);
                    tile.z = i;
                    if (blockRef == null || tile.size != blockRef.size)
                    {
                        i -= direction.x;
                        _grid[tile.x, i] = tile.size;
                        tile.z = i;
                        tile.SetHorizontal(i, direction.x);
                        return;
                    }


                    if (tile.combineElement) continue;
                    tile.combineElement = true;
                    tile.ChangeSize(tile.size);
                    blockRef.ChangeSize(blockRef.size + 1);


                    _grid[tile.x, i] = blockRef.size;
                    tile.SetHorizontal(i, direction.x);
                    return;
                }

                if (tile.combineElement) RemoveTile(tile);

                _grid[tile.x, i] = tile.size;
                tile.SetHorizontal(i, direction.x);
            }
        }

        private void MergeThrough(EAxis eAxis, int directionValue, MonoTileController combinationTile,
            MonoTileController currentTile, MonoTileController targetTile)
        {
            if (eAxis == EAxis.Horizontal)
            {
                var targetPoint = targetTile.z;
                targetTile.ChangeSize(targetTile.size + 1);
                RemoveTile(currentTile);
                RemoveTile(combinationTile);
                combinationTile.SetHorizontal(targetPoint, directionValue);
                currentTile.SetHorizontal(targetPoint, directionValue);
                targetTile.SetHorizontal(targetPoint, directionValue);
                _grid[combinationTile.x, targetPoint - directionValue] = 0;
                _grid[currentTile.x, targetPoint] = targetTile.size;
            }
            else
            {
                var targetPoint = targetTile.x;
                targetTile.ChangeSize(targetTile.size + 1);
                RemoveTile(currentTile);
                RemoveTile(combinationTile);
                combinationTile.SetVertical(targetPoint, directionValue);
                currentTile.SetVertical(targetPoint, directionValue);
                targetTile.SetVertical(targetPoint, directionValue);
                _grid[targetPoint + directionValue, combinationTile.z] = 0;
                _grid[targetPoint, combinationTile.z] = targetTile.size;
            }
        }

        private void RemoveTile(MonoTileController tile)
        {
            _blocks.Remove(tile);
            _tileStorage.Tiles = _blocks;
            tile.Combine();
        }

        private void Initialize()
        {
            _blocks = _tileStorage.Tiles.ToList();
            foreach (var block in _blocks)
            {
                _grid[block.x, block.z] = block.size;
            }
        }
    }
}