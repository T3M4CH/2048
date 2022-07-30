using System.Collections.Generic;
using System.Linq;
using Game.Board.Interfaces;
using Game.Enums;
using Game.Tiles;
using UnityEngine;

namespace Game.Board
{
    public class CellService : MonoBehaviour, ICellService
    {
        [SerializeField] private List<MonoTileController> blocks;

        private readonly int[,] _grid =
        {
            { 0, 0, 0, 0, 0, -1 },
            { -1, 0, -1, -1, 0, -1 },
            { -1, 0, 0, 0, 0, 0 },
            { -1, 0, -1, -1, 0, -1 },
            { 0, 0, 0, 0, 0, -1 }
        };

        public void GetSuitableCell(MonoTileController tile, Vector2Int direction)
        {
            //Vertical
            if (direction.y != 0)
            {
                var i = tile.X;
                _grid[i, tile.Z] = 0;
                while (i is >= 0 and <= 4)
                {
                    i -= direction.y;
                    if (i is < 0 or > 4 || (_grid[i, tile.Z] != 0 && _grid[i, tile.Z] != tile.size))
                    {
                        i += direction.y;
                        break;
                    }

                    if (_grid[i, tile.Z] != tile.size) continue;
                    if (i is > 0 and < 4)
                    {
                        i -= direction.y;
                        if (_grid[i, tile.Z] == tile.size + 1)
                        {
                            var targetBlock = blocks.Find(x => x.X == i && x.Z == tile.Z && !x.victim);
                            var consumerBlock = blocks.Find(x => x.X == i + direction.y && x.Z == tile.Z);
                            MergeThrough(EAxis.Vertical, direction.y, consumerBlock, tile, targetBlock);
                            tile.SetVertical(i);
                            return;
                        }
                
                        i += direction.y;
                    }


                    var blockPref = blocks.Find(x => x.X == i && x.Z == tile.Z);
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

            //HORIZONTAL
            if (direction.x != 0)
            {
                var i = tile.Z;
                _grid[tile.X, i] = 0;
                while (i is >= 0 and <= 5)
                {
                    i += direction.x;
                    if (i is < 0 or > 5 || (_grid[tile.X, i] != 0 && _grid[tile.X, i] != tile.size))
                    {
                        i -= direction.x;
                        break;
                    }

                    if (_grid[tile.X, i] != tile.size) continue;
                    if (i is > 0 and < 5)
                    {
                        i += direction.x;
                        if (_grid[tile.X, i] == tile.size + 1)
                        {
                            var targetBlock = blocks.Find(x => x.Z == i && x.X == tile.X && !x.victim);
                            var consumerBlock = blocks.Find(x => x.Z == i - direction.x && x.X == tile.X);
                            MergeThrough(EAxis.Horizontal, direction.x, consumerBlock, tile, targetBlock);
                            tile.SetHorizontal(i);
                            return;
                        }
                
                        i -= direction.x;
                    }


                    var blockPref = blocks.Find(x => x.Z == i && x.X == tile.X);
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

        private void MergeThrough(EAxis eAxis, int directionValue, MonoTileController victimMonoTileController, MonoTileController currentMonoTileController, MonoTileController targetMonoTileController)
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
            blocks.Remove(monoTileController);
            TileQuery.Tiles = blocks;
            monoTileController.Sacrifice();
        }

        private void Start()
        {
            blocks = FindObjectsOfType<MonoTileController>().ToList();
            foreach (var block in blocks)
            {
                var blockPos = block.transform.position;
                _grid[block.X, block.Z] = block.size;
            }
            
            TileQuery.Tiles = blocks;
        }
    }
}