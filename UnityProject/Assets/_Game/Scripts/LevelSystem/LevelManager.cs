using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game
{
    public class LevelManager : MonoBehaviour
    {
        public List<LevelData> GridRulesList = new List<LevelData>();
        // Set current level mod of grid rules list
        
        public int CurrentLevel => PlayerPrefs.GetInt(PlayerPrefsData.CurrentLevelKey,0)%GridRulesList.Count;
        public LevelData CurrentLevelData => GridRulesList[CurrentLevel];
        public List<RegularBlockSprites> RegularBlockSpritesList = new List<RegularBlockSprites>();
        public ObstacleSprites ObstacleSprites;
        public static LevelManager Instance { get; private set; }
        
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

        private void Start()
        {
            GridBuilder.Instance.BuildGrid(CurrentLevelData);
        }

        public void LoadNextLevel()
        {
            PlayerPrefs.SetInt(PlayerPrefsData.CurrentLevelKey, CurrentLevel + 1);
            SceneSystem.Instance.ResetScene();
        }
    }
    
    [Serializable]
    public class RegularBlockSprites
    {
        public BlockColor BlockColor;
        public Sprite DefaultSprite;
        public Sprite FirstThresholdSprite;
        public Sprite SecondThresholdSprite;
        public Sprite ThirdThresholdSprite;
    }
    
    [Serializable]
    public class ObstacleSprites
    {
        public Sprite DefaultSprite;
        public Sprite DamagedSprite;
    }
}