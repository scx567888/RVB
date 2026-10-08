using UnityEngine;

namespace rvb.scripts {
    /// <summary>
    /// 自爆 施法者状态逻辑.
    /// 进入该状态后播放 Boom 动画, 在 atkFrame 帧生成自爆子弹 (伤害由子弹配置结算), 然后自身死亡.
    /// </summary>
    public class PetLogicDestruction : PetLogic {
        public static readonly PetLogicDestruction Instance = new();

        public void tick(PetView pet, SheepMgr sheepMgr) {
            var skill = SheepSkill.getById(pet.readySkillId);
            if (skill == null || !SheepSkillSubDestruction.TryGetById(skill.id, out var conf)) {
                Debug.LogError("自爆技能配置缺失, readySkillId = " + pet.readySkillId);
                pet.isLock = false;
                pet.state = SheepRoleState.Move;
                pet.subState = SheepRoleSubState.MoveBoss;
                pet.animType = SheepRoleAnimType.Idle;
                return;
            }

            var animFrame = pet.animFrame;

            // 自爆生效帧 (castWaveCnt 复用为 "是否已经炸过" 的标记, 保证只炸一次)
            if (pet.castWaveCnt == 0 && animFrame >= conf.atkFrame) {
                pet.castWaveCnt = 1;
                sheepMgr.createBullet(new BullteCreate() {
                    view_pet = pet,
                    bulletId = conf.bulletId
                });
            }

            if (animFrame < conf.endFrame) {
                return;
            }

            // 自爆单位必然死亡
            pet.isLock = false;
            pet.isDie = true;
            pet.state = SheepRoleState.Dead;
            pet.subState = SheepRoleSubState.Dead;
            pet.animType = SheepRoleAnimType.Dead;
        }
    }
}
