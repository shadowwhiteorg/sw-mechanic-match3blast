using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Game
{
    public class GridHandler : MonoBehaviour
    {
        [SerializeField] private float fillDelay;
        private BlockBase[,] grid;
        public BlockBase[,] Grid => grid;
        public int Rows => LevelManager.Instance.CurrentLevelData.M_rows;
        public int Columns => LevelManager.Instance.CurrentLevelData.N_columns;

        public static GridHandler Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            EventHandler.OnBlocksSettled += CheckAnyObstaclesLeft;
        }

        private void OnDisable()
        {
            EventHandler.OnBlocksSettled -= CheckAnyObstaclesLeft;
        }

        public void Initialize()
        {
            grid = new BlockBase[Rows * 2, Columns];
        }

        public Vector3 GetWorldPosition(int row, int col)
        {
            return new Vector3(col, -row, 0); 
        }

        public void SetCell(int row, int col, BlockBase block)
        {
            int adjustedRow = row + Rows; 

            if (IsValidCell(adjustedRow, col))
            {
                grid[adjustedRow, col] = block;
                if (block != null)
                    block.Position = new Vector2Int(col, row); 
            }
        }

        public BlockBase GetCell(int row, int col)
        {
            int adjustedRow = row + Rows; 
            return IsValidCell(adjustedRow, col) ? grid[adjustedRow, col] : null;
        }

        public bool IsCellEmpty(int row, int col)
        {
            int adjustedRow = row + Rows; 
            return IsValidCell(adjustedRow, col) && grid[adjustedRow, col] == null;
        }

        public List<BlockBase> GetNeighbors(int row, int column)
        {
            var neighbors = new List<BlockBase>();

            AddNeighbor(neighbors, row - 1, column); // Check up
            AddNeighbor(neighbors, row + 1, column); // Check down
            AddNeighbor(neighbors, row, column - 1); // Check left
            AddNeighbor(neighbors, row, column + 1); // Check right

            return neighbors;
        }

        private void AddNeighbor(List<BlockBase> neighbors, int row, int col)
        {
            var neighbor = GetCell(row, col);
            if (neighbor != null)
                neighbors.Add(neighbor);
        }

        private bool IsValidCell(int row, int col)
        {
            return row >= 0 && row < grid.GetLength(0) && col >= 0 && col < grid.GetLength(1);
        }

        private void CheckAnyObstaclesLeft()
        {
            if (!IsObstacleBlockPresent())
            {
                EventHandler.FireOnLevelCompleted();
            }
        }

        private bool IsObstacleBlockPresent()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (GetCell(row, col) is ObstacleBlock)
                        return true;
                }
            }
            return false;
        }
    }
}