using System.Collections.Generic;

namespace sheep {
    public class Pet : Entity {

        // 唯一真实位置 X
        public float x;

        // 唯一真实位置 Y
        public float y;

        // 核心逻辑帧
        public int frame;

        // 移动意图
        public float moveSpeed;
        
        public PetMoveMode moveMode;

        // 碰撞意图
        public PetCollideIntent collideIntent;

        // ********************* 渲染器挂载相关 **********************

        // 渲染器句柄 (逻辑层不应使用此字段)
        public PetRenderHandle renderHandle;
        
        // 是否闪光
        public bool flash = false;

        public override IEnumerable<Intent> tick(SheepWorld world)
        {
            // 1, 执行单位自身逻辑, 更新各类意图和逻辑状态
            var intents = PetLogic.INSTANCE.tick(this, world);

            // 2, 更新逻辑帧
            frame++;
            return intents;
        }
    }
}