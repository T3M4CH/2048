using System;

namespace Game.Board.Interfaces
{
    public interface ILevelPreset
    {
        public event Action OnLevelBuilt;
        public int[,] Grid
        {
            get;
        }
    }
}