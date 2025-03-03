using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Game
{
    public class SceneSystem : MonoBehaviour
    {
        public static SceneSystem Instance { get; private set; }

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
        
        public void ResetScene()
        {
            SceneManager.LoadScene(0);
        }
    }
}