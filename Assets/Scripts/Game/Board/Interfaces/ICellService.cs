using Game.Tiles;
using UnityEngine;

namespace Game.Board.Interfaces
{
    public interface ICellService
    {
        public void GetSuitableCell(MonoTileController block, Vector2Int direction);
    }
}
