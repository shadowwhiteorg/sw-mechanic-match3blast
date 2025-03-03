using UnityEngine;

namespace _Game
{
    public class ObstacleBlock : BlockBase
    {
        [SerializeField] private int healthPoint = 2;
        public int HealthPoint => healthPoint;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            Type = BlockType.Obstacle;
        }

        public override void OnDestroyed()
        {
            if (--healthPoint <= 0)
            {
                Destroy(gameObject);
            }
            else
            {
                SetSprite(LevelManager.Instance.ObstacleSprites.DamagedSprite);
            }
        }

        public void SetSprite(Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
        }
    }
}