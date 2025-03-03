using UnityEngine;

namespace _Game
{
    public class RegularBlock : BlockBase
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        private BlockColor _color;

        public void SetSprite(Sprite sprite)
        {
            spriteRenderer.sprite = sprite;
        }

        public int GetColor()
        {
            return (int)Color;
        }

        public override void OnDestroyed()
        {
            Destroy(gameObject);
        }
    }
}