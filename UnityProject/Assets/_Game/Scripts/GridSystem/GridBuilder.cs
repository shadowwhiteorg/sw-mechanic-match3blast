using UnityEngine;

namespace _Game
{
    public class GridBuilder : MonoBehaviour
    {
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private BlockBase regularBlockPrefab;
        [SerializeField] private BlockBase obstacleBlockPrefab;
        [SerializeField] private GameObject backgroundPrefab;
        [SerializeField] private GameObject blockMask;

        private LevelData _levelData;

        public static GridBuilder Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        public void BuildGrid(LevelData levelData)
        {
            _levelData = levelData;
            GridHandler.Instance.Initialize();

            for (int row = 0; row < _levelData.M_rows; row++)
            {
                for (int col = 0; col < _levelData.N_columns; col++)
                {
                    var prefab = _levelData.GridValues[row, col] == 1 ? obstacleBlockPrefab : regularBlockPrefab;
                    SpawnBlock(row, col, prefab);
                    Instantiate(backgroundPrefab, GetWorldPosition(row, col), Quaternion.identity, transform);
                    GroupDetector.Instance.UpdateAllGroups();
                }
            }
            CameraManager.Instance.SetCameraSize(levelData.N_columns);
            if(blockMask)
                blockMask.transform.position = new Vector3(0, _levelData.M_rows *2.9f, 0);
        }

        private void SpawnBlock(int row, int col, BlockBase blockPrefab)
        {
            Vector3 worldPosition = GetWorldPosition(row, col);
            BlockBase block = Instantiate(blockPrefab, worldPosition, Quaternion.identity, transform);
            BlockColor blockColor = (BlockColor)Random.Range(0, _levelData.K_numberOfColors);

            block.Initialize(BlockType.Regular, block is RegularBlock ? blockColor : default, new Vector2Int(col, row));
            GridHandler.Instance.SetCell(row, col, block);
        }

        public Vector3 GetWorldPosition(int row, int col)
        {
            float offsetX = (_levelData.N_columns - 1) * cellSize / 2f;
            float offsetY = (_levelData.M_rows - 1) * cellSize / 2f;
            return new Vector3(col * cellSize - offsetX, -row * cellSize + offsetY, 0);
        }
    }
}