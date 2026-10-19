using System.Collections.Generic;

namespace sheep {
    // 测试
    public class PetLogic {
        public static PetLogic INSTANCE = new PetLogic();

        public IEnumerable<Intent> tick(Pet pet, SheepWorld sheepWorld) {
            var moveIntent=new PetMoveIntent();
            moveIntent.entityId = pet;
            moveIntent.moveSpeed=pet.moveSpeed;
            // 简化的 索敌逻辑 
            if (pet.id % 2 == 0) {
                moveIntent.moveMode = PetMoveMode.TELEPORT;
                moveIntent.targetX = sheepWorld.bossX;
                moveIntent.targetY = sheepWorld.bossY;
            }
            else {
                moveIntent.moveMode = PetMoveMode.TARGET;
                moveIntent.targetX = sheepWorld.boss1X;
                moveIntent.targetY = sheepWorld.boss1Y;
            }
            return new[] { moveIntent };
        }
    }
}