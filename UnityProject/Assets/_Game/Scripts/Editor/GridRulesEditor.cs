using System.IO;
using UnityEditor;
using UnityEngine;

namespace _Game
{
    [CustomEditor(typeof(LevelData))]
    public class GridRulesEditor : Editor
    {
        private LevelData _levelData;
        private int[,] _editorGridValues;
        private bool _isInitialized;

        private void InitGrid()
        {
            _levelData = (LevelData)target;
            _isInitialized = true;
            if (_editorGridValues == null || _editorGridValues.Length == 0)
            {
                BuildGrid();
            }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Obstacle Placement", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            if (GUILayout.Button("Initialize Grid"))
            {
                InitGrid();
            }
            if (GUILayout.Button("Save Grid"))
            {
                SaveValues();
            }
            // if (GUILayout.Button("Reset Grid"))
            // {
            //     BuildGrid();
            // }
            if(!_isInitialized)
                return;
            DrawGrid();
            
        }

        private void BuildGrid()
        {
            Debug.Log("BuildGrid called");
            _editorGridValues = new int[_levelData.M_rows, _levelData.N_columns];
        }

        private void DrawGrid()
        {
            _levelData = (LevelData)target;
            for (int y = 0; y < _levelData.M_rows; y++)
            {
                GUILayout.BeginHorizontal();
                for (int x = 0; x < _levelData.N_columns; x++)
                {
                    if (GUILayout.Button(_editorGridValues[y, x].ToString(), GUILayout.Width(30), GUILayout.Height(30)))
                    {
                        _editorGridValues[y, x] = _editorGridValues[y, x] == 0 ? 1 : 0;
                    }
                }
                GUILayout.EndHorizontal();
            }
        }
        
        private void SaveValues()
        {
            _levelData.InitializeGridValues();
            // TODO: use following path to read the file from another device ( e.g Android device) - Also consider reading operation in the editor!
            // string filePath = Path.Combine("Resources", $"{_levelData.name}.txt");
            // TODO: Use the following path to write the file into the persistent data path - Also consider reading operation in the editor!
            // string filePath = Path.Combine(Application.persistentDataPath, $"{_levelData.name}.txt");

            string filePath = $"Assets/_Game/Data/{_levelData.name}.txt";
            WriteGridValuesToFile(filePath);
        }
        
        private void WriteGridValuesToFile(string filePath)
        {
            using StreamWriter writer = new StreamWriter(filePath);
            for (int y = 0; y < _levelData.M_rows; y++)
            {
                for (int x = 0; x < _levelData.N_columns; x++)
                {
                    writer.Write(_editorGridValues[y, x]);
                    if (x < _levelData.N_columns - 1)
                    {
                        writer.Write(",");
                    }
                }
                writer.WriteLine();
            }
        }
        
    }
}