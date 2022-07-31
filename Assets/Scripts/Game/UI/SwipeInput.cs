using System.Linq;
using Game.Board.Interfaces;
using Game.Tiles;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Game.UI
{
    public class SwipeInput : MonoBehaviour, IBeginDragHandler, IDragHandler
    {
        [SerializeField] private Canvas canvas;
            
        private bool _isLocked;
        private Vector2 _startPosition;
        private TileStorage _tileStorage;
        private ICellService _cellService;
        
        [Inject]
        private void Construct(ICellService cellService, TileStorage tileStorage)
        {
            _tileStorage = tileStorage;
            _cellService = cellService;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_isLocked) return;
            var orderedBlocks = _tileStorage.Tiles;
            if (Mathf.Abs(eventData.delta.y) > Mathf.Abs(eventData.delta.x))
            {
                if(Mathf.Abs(eventData.delta.y) < 1f) return;
                if (eventData.delta.y > 0)
                {
                    orderedBlocks = orderedBlocks.OrderBy(x => x.X);
                    for (int i = 0; i < 3; i++)
                    {
                        foreach (var block in orderedBlocks)
                        {
                            _cellService.GetSuitableCell(block, Vector2Int.up);
                        }
                    }
                }
                else
                {
                    orderedBlocks = orderedBlocks.OrderByDescending(x => x.X);
                    for (int i = 0; i < 3; i++)
                    {
                        foreach (var block in orderedBlocks)
                        {
                            _cellService.GetSuitableCell(block, Vector2Int.down);
                        }
                    }
                }

                var lastBlock = orderedBlocks.Last();
                LockMovement(lastBlock);
            }

            if (!(Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y))) return;
            {
                if(Mathf.Abs(eventData.delta.x) < 1) return;
                if (eventData.delta.x > 0)
                {
                    orderedBlocks = orderedBlocks.OrderByDescending(x => x.Z);
                    for (int i = 0; i < 3; i++)
                    {
                        foreach (var block in orderedBlocks)
                        {
                            _cellService.GetSuitableCell(block, Vector2Int.right);
                        }
                    }
                }
                else
                {
                    orderedBlocks = orderedBlocks.OrderBy(x => x.Z);
                    for (int i = 0; i < 3; i++)
                    {
                        foreach (var block in orderedBlocks)
                        {
                            _cellService.GetSuitableCell(block, Vector2Int.left);
                        }
                    }
                }

                var lastBlock = orderedBlocks.Last();
                LockMovement(lastBlock);
            }
        }

        private void LockMovement(MonoTileController block)
        {
            _isLocked = true;
            block.OnActionComplete += UnlockMovement;
        }

        private void UnlockMovement()
        {
            _isLocked = false;
        }


        public void OnDrag(PointerEventData eventData)
        {
        }
    }
}