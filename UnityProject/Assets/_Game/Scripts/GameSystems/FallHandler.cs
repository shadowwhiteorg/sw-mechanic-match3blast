using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game
{
    public class FallHandler : MonoBehaviour
    {
        public static FallHandler Instance { get; private set; }

        [SerializeField] private float spawnOffsetY = 1f;
        public  GameObject blockPrefab;
        [SerializeField] private float fallSpeed = 5f;
        private Dictionary<int, float> lastSpawnHeights;

        private int activeFallingBlocks = 0;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            lastSpawnHeights = new Dictionary<int, float>();
        }

        public void HandleFallAndRefill(List<BlockBase> blastedBlocks)
        {
            StartCoroutine(FallAndRefillRoutine(blastedBlocks));
        }

        private IEnumerator FallAndRefillRoutine(List<BlockBase> blastedBlocks)
        {
            var emptyRowsByColumn = new Dictionary<int, List<int>>();

            foreach (var block in blastedBlocks)
            {
                int column = block.Position.x;
                if (!emptyRowsByColumn.ContainsKey(column))
                    emptyRowsByColumn[column] = GetEmptyRowsInColumn(column);

                GridHandler.Instance.SetCell(block.Position.y, block.Position.x, null);
                Destroy(block.gameObject);
            }

            foreach (var column in emptyRowsByColumn.Keys)
                SpawnAndAddBlocks(column, emptyRowsByColumn[column].Count);

            var columnCoroutines = new List<Coroutine>();
            foreach (var column in emptyRowsByColumn.Keys)
                columnCoroutines.Add(StartCoroutine(HandleColumnFall(column)));

            foreach (var coroutine in columnCoroutines)
                yield return coroutine;
            
            if (activeFallingBlocks == 0)
            {
                EventHandler.FireOnBlocksSettled();
                Debug.Log("Blocks Settled1");
            }
        }

        private IEnumerator HandleColumnFall(int column)
        {
            var fallAnimations = new List<IEnumerator>();

            for (int row = GridHandler.Instance.Rows + 5; row >= -5; row--)
            {
                var block = GridHandler.Instance.GetCell(row, column);
                if (block is RegularBlock)
                {
                    int targetRow = FindTargetRow(column, row);
                    if (targetRow != row)
                    {
                        GridHandler.Instance.SetCell(row, column, null);
                        GridHandler.Instance.SetCell(targetRow, column, block);

                        var targetPosition = GridBuilder.Instance.GetWorldPosition(targetRow, column);
                        fallAnimations.Add(AnimateFall(block, targetPosition));
                        block.Position = new Vector2Int(column, targetRow);
                    }
                }
            }

            foreach (var animation in fallAnimations)
                StartCoroutine(animation);

            yield return null;
        }

        private void SpawnAndAddBlocks(int column, int count)
        {
            lastSpawnHeights[column] = GridBuilder.Instance.GetWorldPosition(-1, column).y;

            for (int i = 0; i < count; i++)
            {
                float spawnY = lastSpawnHeights[column] + spawnOffsetY;
                var spawnPosition = new Vector3(GridBuilder.Instance.GetWorldPosition(0, column).x, spawnY, 0);

                var newBlockObj = Instantiate(blockPrefab, spawnPosition, Quaternion.identity);
                var newBlock = newBlockObj.GetComponent<BlockBase>();
                var blockColor = (BlockColor)UnityEngine.Random.Range(0, LevelManager.Instance.CurrentLevelData.K_numberOfColors);

                newBlock.Initialize(BlockType.Regular, blockColor, new Vector2Int(column, -i - 1));
                GridHandler.Instance.SetCell(-i - 1, column, newBlock);
                lastSpawnHeights[column] = spawnY;
            }
        }

        private IEnumerator AnimateFall(BlockBase block, Vector3 targetPosition)
        {
            // Increment the counter for falling blocks
            activeFallingBlocks++;

            block.CanBeClicked = false;
            block.IsFalling = true;
            var startPosition = block.transform.position;
            float distance = Vector3.Distance(startPosition, targetPosition);
            float duration = distance / fallSpeed;

            for (float elapsedTime = 0f; elapsedTime < duration; elapsedTime += Time.deltaTime)
            {
                block.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
                yield return null;
            }

            block.CanBeClicked = true;
            block.IsFalling = false;
            block.transform.position = targetPosition;
            GroupDetector.Instance.UpdateAllGroups();
            // Decrement the counter for falling blocks
            activeFallingBlocks--;

            // Fire the event if all blocks have settled
            if (activeFallingBlocks == 0)
            {
                EventHandler.FireOnBlocksSettled();
                Debug.Log("Blocks Settled");
            }
        }

        private int FindTargetRow(int column, int startingRow)
        {
            int targetRow = startingRow;

            for (int row = startingRow + 1; row < GridHandler.Instance.Rows; row++)
            {
                if (GridHandler.Instance.GetCell(row, column) != null)
                    break;

                targetRow = row;
            }

            return targetRow;
        }

        private List<int> GetEmptyRowsInColumn(int column)
        {
            var emptyRows = new List<int>();

            for (int row = GridHandler.Instance.Rows - 1; row >= 0; row--)
            {
                if (GridHandler.Instance.GetCell(row, column) == null)
                    emptyRows.Add(row);
            }

            return emptyRows;
        }
    }
}
