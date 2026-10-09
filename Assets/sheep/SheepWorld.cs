using System.Collections.Generic;
using scx.GridMap;

namespace sheep {
    public class SheepWorld {
        // 当前在场上的角色
        public List<Pet> pets = new();

        // 准备添加到下一帧的 角色
        public List<Pet> pre_pets = new();

        // 准备删除的 角色, 这里用 Set 保证查询速度
        public HashSet<Pet> del_pets = new();

        // 角色自增 id 
        public int petId = 0;

        // 格子空间, 用于加速索敌碰撞
        public GridMap<SheepCell> gridMap;

        // 世界随机数生成器
        public System.Random random;

        // 移动系统
        public PetMoveSystem moveSystem;

        // 碰撞系统
        public PetCollideSystem collideSystem;

        public SheepWorld() {
            // 初始化格子空间
            this.gridMap = new GridMap<SheepCell>(
                -50, -50,
                100, 100,
                4,
                (gridX, gridY, worldStartX, worldStartY, worldEndX, worldEndY) =>
                    new SheepCell(gridX, gridY, worldStartX, worldStartY, worldEndX, worldEndY)
            );
            // 初始化随机数器
            this.random = new(666);
            // 初始化移动系统
            this.moveSystem = new();
            // 初始化碰撞系统
            this.collideSystem = new();
        }

        // *********************** 单位添加删除相关 ***************************

        // 获取 petId
        public int getNextPetId() {
            return ++petId;
        }

        // 添加单位, 不要在逻辑帧循环中调用
        public void addPet(Pet pet) {
            pets.Add(pet);
        }

        // 删除单位, 不要在逻辑帧循环中调用
        public void delPet(Pet pet) {
            pets.Remove(pet);
        }

        // 添加单位 下一帧才会使用
        public void addPrePet(Pet pet) {
            pre_pets.Add(pet);
        }

        // 添加 删除单位.
        public void addDelPet(Pet pet) {
            del_pets.Add(pet);
        }

        // 将 pre_pets 应用到 pets 中
        public void applyPrePets() {
            foreach (var pet in pre_pets) {
                addPet(pet);
            }

            pre_pets.Clear();
        }

        // 从 pets 中 删除 del_pets 中的 单位
        public HashSet<Pet> applyDelPets() {
            // 复制一份方便 渲染层处理
            var copy = new HashSet<Pet>(del_pets);

            // 应用移除
            foreach (var pet in del_pets) {
                delPet(pet);
            }

            // 清空
            del_pets.Clear();

            return copy;
        }

        // ************************ 格子相关 ************************

        // 重建格子
        public void rebuildGridMap() {
            // 清空格子
            gridMap.forEachCell(cell => { cell.clearPets(); });

            // 重建格子
            foreach (var pet in pets) {
                var cell = gridMap.getCellByWorldPositionSafe(
                    pet.x,
                    pet.y
                );
                cell.addPet(pet);
            }
        }

        // ******************** 随机数相关 *************************
        public int randomInt(int minValue, int maxValue) {
            return random.Next(minValue, maxValue);
        }

        public float random01() {
            return (float)random.NextDouble();
        }

        public float randomFloat(float minValue, float maxValue) {
            return minValue + (maxValue - minValue) * (float)random.NextDouble();
        }

        // **************************** 逻辑相关 ***************************

        public void petsAction() {
            var intents = new List<Intent>();
            
            // 所有 pet 行动, 执行单位自身逻辑, 更新各类意图和逻辑状态
            foreach (var pet in pets)
            {
                var petIntent= pet.tick(this);
                foreach (var intent in petIntent)
                {
                    intents.Add(intent);
                }
            }

            // 更新 pet 实际位置 
            foreach (var intent in intents) {
                if (intent is PetMoveIntent  petMoveIntent)
                {
                    // 1. 计算自主位移
                    var selfMove = moveSystem.calculateSelfMove(petMoveIntent);    
                      
                    // 2. 根据碰撞修正最终位移
                    var finalMove = collideSystem.calculateCollisionMove(petMoveIntent.entityId, selfMove, this);
                    // 4. 应用最终位移
                    petMoveIntent.entityId.x += finalMove.x;
                    petMoveIntent.entityId.y += finalMove.y;
                }
            }
        }

        // Tick
        public HashSet<Pet> tick() {
            // 1, 应用 预添加数据
            applyPrePets();

            // 2, 重建格子
            rebuildGridMap();

            // 3, 执行动作
            this.petsAction();

            // 4, 应用移除
            var del_pets1 = applyDelPets();

            return del_pets1;
        }

        // ************* 测试 ******************

        public float bossX;
        public float bossY;

        public float boss1X;
        public float boss1Y;
    }
}