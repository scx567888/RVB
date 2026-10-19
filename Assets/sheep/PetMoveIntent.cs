namespace sheep {
    /// 自主移动意图
    public class PetMoveIntent : Intent{
        
        public Pet entityId;
        
        // ********************** 自主移动相关 ***********************

        // 自主移动模式
        public PetMoveMode moveMode;

        // 自主移动速度 (每帧/像素)
        public float moveSpeed;

        // 自主移动向量, 归一化方向 两个分量范围是 [-1, 1], 整体长度为 1.
        // 用于 moveMode 为 DIRECTION 时
        public float directionX;
        public float directionY;

        // 目标位置
        // 用于 moveMode 为 TARGET 和 TELEPORT 时 
        public float targetX;
        public float targetY;
    }
}