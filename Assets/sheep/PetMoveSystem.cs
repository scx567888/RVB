using UnityEngine;

namespace sheep {
    /// 移动系统
    public class PetMoveSystem {
        // 计算自主移动位移 
        // x 当前位置
        // y 当前位置
        // 返回需要移动的距离
        public Vector2 calculateSelfMove(PetMoveIntent petMoveIntent) {
            var moveIntent = petMoveIntent;
            var pet = petMoveIntent.entityId;
            var x = pet.x;
            var y = pet.y;

            return moveIntent.moveMode switch {
                // NONE 不移动
                PetMoveMode.NONE => Vector2.zero,
                // 方向模式
                PetMoveMode.DIRECTION => calculateDirectionMove(moveIntent),
                // 目标模式
                PetMoveMode.TARGET => calculateTargetMove(moveIntent, x, y),
                // 目标模式
                PetMoveMode.TELEPORT => calculateTeleportMove(moveIntent, x, y),
                // 默认兜底
                _ => Vector2.zero
            };
        }

        public static float calculateMoveDistance(PetMoveIntent moveIntent) {
            // 固定逻辑帧下, moveSpeed 就是每帧移动距离
            return moveIntent.moveSpeed;
        }

        // 方向模式
        public static Vector2 calculateDirectionMove(PetMoveIntent moveIntent) {
            // 这里 directionX, directionY 已经归一化
            var direction = new Vector2(moveIntent.directionX, moveIntent.directionY);
            var moveDistance = calculateMoveDistance(moveIntent);
            return direction * moveDistance;
        }

        public static Vector2 calculateTargetMove(PetMoveIntent moveIntent, float x, float y) {
            var offset = new Vector2(moveIntent.targetX - x, moveIntent.targetY - y);

            var distance = offset.magnitude;

            var moveDistance = calculateMoveDistance(moveIntent);

            // 已到达, 或者这一帧可以直接到达
            if (distance <= moveDistance) {
                return offset;
            }

            var direction = offset / distance;
            return direction * moveDistance;
        }

        public static Vector2 calculateTeleportMove(PetMoveIntent moveIntent, float x, float y) {
            var offset = new Vector2(moveIntent.targetX - x, moveIntent.targetY - y);
            return offset;
        }
    }
}