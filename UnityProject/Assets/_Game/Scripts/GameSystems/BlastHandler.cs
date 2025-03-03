using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game
{
    public class BlastHandler : MonoBehaviour
    {
        public static BlastHandler Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Handles blasting of blocks and triggers fall and refill logic.
        /// </summary>
        /// <param name="blocksToBlast">List of blocks to be blasted.</param>
        public void HandleBlast(List<BlockBase> blocksToBlast)
        {
            if (blocksToBlast == null || blocksToBlast.Count == 0)
                return;

            // 1) Remove blasted blocks from the grid
            foreach (var block in blocksToBlast)
            {
                GridHandler.Instance.SetCell(block.Position.y, block.Position.x, null);
                Destroy(block.gameObject); // Destroy the block GameObject
            }

            // 2) For each blasted block, damage neighbor obstacles
            foreach (var block in blocksToBlast)
            {
                var neighbors = GridHandler.Instance.GetNeighbors(block.Position.y, block.Position.x);
                foreach (var neighbor in neighbors)
                {
                    if (neighbor is ObstacleBlock obstacle)
                    {
                        // Decrement HP via OnDestroyed
                        obstacle.OnDestroyed();
                
                        // If it's destroyed, remove from grid
                        if (obstacle.HealthPoint <= 0)
                        {
                            GridHandler.Instance.SetCell(obstacle.Position.y, obstacle.Position.x, null);
                            // The obstacle block is already destroyed inside OnDestroyed
                        }
                    }
                }
            }

            // 3) Trigger fall and refill logic
            FallHandler.Instance.HandleFallAndRefill(blocksToBlast);
        }

    }


}