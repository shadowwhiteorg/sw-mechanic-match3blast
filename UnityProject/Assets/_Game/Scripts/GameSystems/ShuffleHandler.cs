using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game
{
    public class ShuffleHandler : MonoBehaviour
    {
        public static ShuffleHandler Instance { get; private set; }

        [SerializeField] private float shuffleDuration = 0.5f;
        [SerializeField] private RegularBlock regularBlockPrefab;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        public void ShuffleGrid()
        {
            var gridHandler = GridHandler.Instance;
            var levelData = LevelManager.Instance.CurrentLevelData;
            int rows = levelData.M_rows;
            int cols = levelData.N_columns;
            int minGroupSize = levelData.A_firstThreshold;

            List<RegularBlock> allRegularBlocks = new List<RegularBlock>();
            List<Vector2Int> allPositions = new List<Vector2Int>();

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    BlockBase block = gridHandler.GetCell(r, c);
                    if (block is RegularBlock rb)
                    {
                        allRegularBlocks.Add(rb);
                        allPositions.Add(new Vector2Int(r, c));
                    }
                }
            }

            if (allRegularBlocks.Count < minGroupSize)
            {
                return;
            }

            Dictionary<RegularBlock, Vector2Int> mapping = new Dictionary<RegularBlock, Vector2Int>();
            if (!ForceGroup(mapping, allRegularBlocks, allPositions, rows, cols, minGroupSize))
            {
                return;
            }

            PlaceRemainingBlocks(mapping, allRegularBlocks, ShuffleList(allPositions));
            StartCoroutine(ShuffleRoutine(mapping));
        }

        private IEnumerator ShuffleRoutine(Dictionary<RegularBlock, Vector2Int> mapping)
        {
            var gridHandler = GridHandler.Instance;

            foreach (var pair in mapping)
            {
                RegularBlock block = pair.Key;
                Vector2Int newCell = pair.Value;
                Vector3 targetWorldPos = GridBuilder.Instance.GetWorldPosition(newCell.x, newCell.y);
                StartCoroutine(MoveBlock(block, targetWorldPos, shuffleDuration));
                block.Position = newCell;
            }

            yield return new WaitForSeconds(shuffleDuration);

            foreach (var pair in mapping)
            {
                RegularBlock block = pair.Key;
                Vector2Int finalPos = block.Position;
                gridHandler.SetCell(finalPos.x, finalPos.y, block);
            }

            GroupDetector.Instance.UpdateAllGroups();
            FillEmptyCellsWithNewBlocks();
            GroupDetector.Instance.UpdateAllGroups();
        }

        private IEnumerator MoveBlock(BlockBase block, Vector3 targetWorldPos, float duration)
        {
            Vector3 startPos = block.transform.position;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                block.transform.position = Vector3.Lerp(startPos, targetWorldPos, Mathf.Clamp01(elapsed / duration));
                yield return null;
            }

            block.transform.position = targetWorldPos;
        }

        private bool ForceGroup(Dictionary<RegularBlock, Vector2Int> mapping, List<RegularBlock> allBlocks, List<Vector2Int> allPositions, int totalRows, int totalCols, int minGroupSize)
        {

            var colorGroups = new Dictionary<int, List<RegularBlock>>();
            foreach (var block in allBlocks)
            {
                int color = (int)block.Color;
                if (!colorGroups.ContainsKey(color))
                    colorGroups[color] = new List<RegularBlock>();
                colorGroups[color].Add(block);
            }

            int chosenColor = -1;
            foreach (var kvp in colorGroups)
            {
                if (kvp.Value.Count >= minGroupSize)
                {
                    chosenColor = kvp.Key;
                    break;
                }
            }

            if (chosenColor == -1)
            {
                return false;
            }

            var forcedBlocks = colorGroups[chosenColor].GetRange(0, minGroupSize);
            System.Random rng = new System.Random();

            for (int attempt = 0; attempt < 30; attempt++)
            {
                int row = rng.Next(0, totalRows);
                if (totalCols < minGroupSize) break;
                int col = rng.Next(0, totalCols - (minGroupSize - 1));

                bool validPlacement = true;
                var candidatePositions = new List<Vector2Int>();
                for (int i = 0; i < minGroupSize; i++)
                {
                    var checkPos = new Vector2Int(row, col + i);
                    if (!allPositions.Contains(checkPos))
                    {
                        validPlacement = false;
                        break;
                    }
                    candidatePositions.Add(checkPos);
                }

                if (validPlacement)
                {
                    for (int i = 0; i < minGroupSize; i++)
                    {
                        Vector2Int targetPos = candidatePositions[i];
                        mapping[forcedBlocks[i]] = targetPos;
                        allPositions.Remove(targetPos);
                        allBlocks.Remove(forcedBlocks[i]);
                    }
                    return true;
                }
            }

            return false;
        }

        private void PlaceRemainingBlocks(Dictionary<RegularBlock, Vector2Int> mapping, List<RegularBlock> remainingBlocks, List<Vector2Int> availablePositions)
        {
            if (remainingBlocks.Count > availablePositions.Count)
            {
                return;
            }

            for (int i = 0; i < remainingBlocks.Count; i++)
            {
                mapping[remainingBlocks[i]] = availablePositions[i];
            }
        }

        private List<T> ShuffleList<T>(List<T> list)
        {
            System.Random rng = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
            return list;
        }

        private void FillEmptyCellsWithNewBlocks()
        {
            var gridHandler = GridHandler.Instance;
            var builder = GridBuilder.Instance;
            var levelData = LevelManager.Instance.CurrentLevelData;
            int rows = levelData.M_rows;
            int cols = levelData.N_columns;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (gridHandler.GetCell(r, c) == null)
                    {
                        RegularBlock newBlock = Instantiate(regularBlockPrefab,GridBuilder.Instance.transform);
                        newBlock.transform.position = builder.GetWorldPosition(r, c);
                        newBlock.Position = new Vector2Int(r, c);
                        newBlock.Color = (BlockColor)Random.Range(0, levelData.K_numberOfColors);
                        gridHandler.SetCell(r, c, newBlock);
                    }
                }
            }
        }
    }
}