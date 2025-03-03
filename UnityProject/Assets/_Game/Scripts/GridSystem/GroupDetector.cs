using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Game
{
    public class GroupDetector : MonoBehaviour
    {
        private BlockBase[,] _blocks;
        public LevelData LevelData { get; private set; }
        public bool[,] Visited { get; private set; }

        public static GroupDetector Instance;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            EventHandler.OnBlocksSettled += CheckBlastableGroup;
        }

        private void OnDisable()
        {
            EventHandler.OnBlocksSettled -= CheckBlastableGroup;
        }

        public GroupDetector(RegularBlock[,] blocks, LevelData levelData)
        {
            _blocks = blocks;
            LevelData = levelData;
            Visited = new bool[levelData.M_rows, levelData.N_columns];
        }

        public List<BlockBase> DetectGroup(BlockBase startBlock)
        {
            if (startBlock.IsFalling) return null;

            var group = new List<BlockBase>();
            var toCheck = new Queue<BlockBase>();
            var visited = new HashSet<BlockBase>();

            toCheck.Enqueue(startBlock);
            visited.Add(startBlock);

            while (toCheck.Count > 0)
            {
                var current = toCheck.Dequeue();
                group.Add(current);

                foreach (var neighbor in GridHandler.Instance.GetNeighbors(current.Position.y, current.Position.x))
                {
                    if (neighbor is RegularBlock && neighbor.Color == startBlock.Color && !neighbor.IsFalling && !visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        toCheck.Enqueue(neighbor);
                    }
                }
            }

            return group;
        }

        private void ExploreGroup(int row, int col, int targetColor, List<BlockBase> group)
        {
            if (!IsValidCell(row, col) || Visited[row, col]) return;

            var block = GridHandler.Instance.Grid[row, col];
            if (block is RegularBlock regularBlock && regularBlock.GetColor() == targetColor)
            {
                Visited[row, col] = true;
                group.Add(block);

                ExploreGroup(row + 1, col, targetColor, group);
                ExploreGroup(row - 1, col, targetColor, group);
                ExploreGroup(row, col + 1, targetColor, group);
                ExploreGroup(row, col - 1, targetColor, group);
            }
        }

        public void AssignGroupSprites(List<BlockBase> group)
        {
            foreach (var block in group)
            {
                if (block is RegularBlock regularBlock)
                {
                    var sprite = LevelManager.Instance.RegularBlockSpritesList[regularBlock.GetColor()].DefaultSprite;
                    if (group.Count > LevelManager.Instance.CurrentLevelData.C_thirdThreshold - 1)
                        sprite = LevelManager.Instance.RegularBlockSpritesList[regularBlock.GetColor()].ThirdThresholdSprite;
                    else if (group.Count > LevelManager.Instance.CurrentLevelData.B_secondThreshold - 1)
                        sprite = LevelManager.Instance.RegularBlockSpritesList[regularBlock.GetColor()].SecondThresholdSprite;
                    else if (group.Count > LevelManager.Instance.CurrentLevelData.A_firstThreshold - 1)
                        sprite = LevelManager.Instance.RegularBlockSpritesList[regularBlock.GetColor()].FirstThresholdSprite;

                    regularBlock.SetSprite(sprite);
                }
            }
        }

        public void UpdateAllGroups()
        {
            int rows = LevelManager.Instance.CurrentLevelData.M_rows;
            int cols = LevelManager.Instance.CurrentLevelData.N_columns;
            Visited = new bool[rows, cols];

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (!Visited[row, col])
                    {
                        var block = GridHandler.Instance.GetCell(row, col);
                        if (block is RegularBlock regularBlock && !regularBlock.IsFalling)
                        {
                            var group = DetectGroup(regularBlock);
                            if (group != null && group.Count > 0)
                            {
                                AssignGroupSprites(group);
                                foreach (var groupBlock in group)
                                    Visited[groupBlock.Position.y, groupBlock.Position.x] = true;
                            }
                        }
                    }
                }
            }
        }

        public void CheckBlastableGroup()
        {
            if (!AnyBlastableGroupExists())
                EventHandler.FireOnNoBlastableFound();
        }

        public bool AnyBlastableGroupExists(int? minSize = null)
        {
            int requiredSize = minSize ?? LevelManager.Instance.CurrentLevelData.A_firstThreshold;
            int rows = LevelManager.Instance.CurrentLevelData.M_rows;
            int cols = LevelManager.Instance.CurrentLevelData.N_columns;
            var localVisited = new bool[rows, cols];

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (!localVisited[row, col])
                    {
                        var block = GridHandler.Instance.GetCell(row, col);
                        if (block is RegularBlock regBlock && !regBlock.IsFalling)
                        {
                            var group = DetectGroup(block);
                            if (group != null && group.Count >= requiredSize)
                                return true;

                            if (group != null)
                            {
                                foreach (var gBlock in group)
                                    localVisited[gBlock.Position.y, gBlock.Position.x] = true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private bool IsValidCell(int row, int col)
        {
            return row >= 0 && col >= 0 && row < LevelManager.Instance.CurrentLevelData.M_rows && col < LevelManager.Instance.CurrentLevelData.N_columns;
        }
    }
}