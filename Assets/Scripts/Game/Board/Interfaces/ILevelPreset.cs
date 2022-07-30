using UnityEngine;

namespace Game.Board
{
    public interface ILevelPreset
    {
        public int[,] Grid
        {
            get;
        }
    }
}