using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace _Game
{
    public abstract class BlockBase : MonoBehaviour, IBlock
    {
        public BlockType Type { get; protected set; }
        public BlockColor Color { get;  set; }
        public Vector2Int Position { get; set; }
        public int Row { get; protected set; }
        public int Volumn { get; protected set; }
        private bool _isFalling = false;
        private bool _canBeClicked = true;

        public bool IsFalling
        {
            get => _isFalling;
            set => _isFalling = value;
        }
        public bool CanBeClicked
        {
            get => _canBeClicked;
            set => _canBeClicked = value;
        }
        
        public virtual void Initialize(BlockType type, BlockColor color, Vector2Int position)
        {
            Type = type;
            Color = color;
            Position = position;
            if(this is RegularBlock)
            {
                RegularBlock regularBlock = (RegularBlock)this;
                regularBlock.SetSprite(LevelManager.Instance.RegularBlockSpritesList[regularBlock.GetColor()].DefaultSprite);
            }
        }
        

        public abstract void OnDestroyed();
        
        public void FallTo(Vector3 targetPosition, float speed)
        {
            StartCoroutine(FallAnimation(targetPosition, speed));
        }
        
        public IEnumerator FallAnimation(Vector3 targetPosition, float speed)
        {
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * speed);
                yield return null;
            }
        
            transform.position = targetPosition;
            
            EventHandler.FireOnReachedTargetPosition(this);
            // OnReachedTargetPosition?.Invoke();
        }
        
        
    }
}