using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "Blast/Level Data", order = 0)]
    public class LevelData : ScriptableObject
    {
        [Header("Grid Dimensions")]
        [Range(2, 10)]
        public int M_rows = 5;
        [Range(2, 10)]
        public int N_columns = 5;
        [Range(1,6)]
        public int K_numberOfColors = 6;

        [Header("Group Size Thresholds")]
        public int A_firstThreshold = 5;
        public int B_secondThreshold = 8;
        public int C_thirdThreshold = 10;
        
         public int[,] GridValues => ReadGridValuesFromFile();

        public void InitializeGridValues()
        {

        }
        
        private int[,] ReadGridValuesFromFile()
        {
            // TODO: use following path to reach the file from another device ( e.g Android device) - Also consider writing operation in the editor!
            // string[] lines = Resources.Load<TextAsset>(filePath).text.Split('\n');
            // TODO: Use the following path to reach the file from the persistent data path - Also consider writing operation in the editor!
            // string[] lines = File.ReadAllLines(Path.Combine(Application.persistentDataPath, $"{LevelManager.Instance.CurrentLevelData.name}.txt"));
            
            string[] lines = File.ReadAllLines($"Assets/_Game/Data/{LevelManager.Instance.CurrentLevelData.name}.txt");
            int[,] gridValues = new int[M_rows,N_columns];

            for (int y = 0; y < lines.Length; y++)
            {
                string[] values = lines[y].Split(',');
                for (int x = 0; x < values.Length; x++)
                {
                    gridValues[y, x] = int.Parse(values[x]);
                }
            }

            return gridValues;
        }
        
    }
}