using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Game
{
    public class UIManager : MonoBehaviour
    {
        
        [SerializeField] private Button shuffleButton;
        [SerializeField] private Button nextLevelButton;

        private void Start()
        {
           shuffleButton.onClick.AddListener(ShuffleHandler.Instance.ShuffleGrid);
           shuffleButton.onClick.AddListener(DisableShuffleButton);
           shuffleButton.gameObject.SetActive(false);
           
           nextLevelButton.onClick.AddListener(LevelManager.Instance.LoadNextLevel);
           nextLevelButton.gameObject.SetActive(false);
           
        }

        private void OnEnable()
        {
            EventHandler.OnLevelCompleted += EnableNextLevelButton;
            EventHandler.OnNoBlastableFound += EnableShuffleButton;
        }

        private void OnDisable()
        {
            EventHandler.OnLevelCompleted -= EnableNextLevelButton;
            EventHandler.OnNoBlastableFound -= EnableShuffleButton;
        }


        private void DisableShuffleButton()
        {
            shuffleButton?.gameObject.SetActive(false);
        }private void EnableShuffleButton()
        {
            if(shuffleButton)
                shuffleButton.gameObject.SetActive(true);
        }

        private void EnableNextLevelButton()
        {
            if(nextLevelButton)
                nextLevelButton.gameObject.SetActive(true);
        }
    }
    
    
}