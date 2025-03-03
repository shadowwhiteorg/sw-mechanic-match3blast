using System;
using UniRx;
using UnityEngine;

namespace _Game
{
    public class EventHandler : MonoBehaviour
    {
        public static event Action OnGridBuilt;
        public static event Action<BlockBase> OnReachedTargetPosition; 
        public static event Action OnBlocksSettled;
        public static event Action OnLevelCompleted;

        public static event Action OnNoBlastableFound; 
        
        private Subject<(int, int)> _onGridBuiltRx = new Subject<(int, int)>();

        public static void FireOnGridBuilt()
        {
            OnGridBuilt?.Invoke();
        }
        
        public static void FireOnBlocksSettled()
        {
            OnBlocksSettled?.Invoke();
        }
        
        public static void FireOnReachedTargetPosition(BlockBase block)
        {
            OnReachedTargetPosition?.Invoke(block);
        }
        
        public static void FireOnLevelCompleted()
        {
            OnLevelCompleted?.Invoke();
        }
        
        public static void FireOnNoBlastableFound()
        {
            OnNoBlastableFound?.Invoke();
        }
        
        private void RxEventTest()
        {
            _onGridBuiltRx.Subscribe(x => Debug.Log(x));
            _onGridBuiltRx.OnNext((1, 2));
            
            _onGridBuiltRx.OnNext((10, 20));

            _onGridBuiltRx.Subscribe(gridParams =>
            {
                int param1 = gridParams.Item1;
                int param2 = gridParams.Item2;
                Debug.Log($"Grid built with parameters: {param1}, {param2}");
            });
        }
    }
}