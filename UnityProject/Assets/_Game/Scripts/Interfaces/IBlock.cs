using UnityEngine;

namespace _Game
{
    public interface IBlock
    {
        BlockType Type { get; }
        BlockColor Color { get; }
        Vector2Int Position { get; set; }

        void Initialize(BlockType type, BlockColor color, Vector2Int position);
        void OnDestroyed();
    }
}