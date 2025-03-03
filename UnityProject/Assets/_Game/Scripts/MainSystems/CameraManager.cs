using UnityEngine;

namespace _Game
{
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance { get; private set; }
        
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
        
        [SerializeField] private float sizeMultiplier = 3f;

        public void SetCameraSize(int column)
        {
            if(Camera.main == null) return;
                Camera.main.orthographicSize = column * sizeMultiplier;
        }
    }
}