using UnityEngine;

namespace rvb.scripts {
    /// <summary>
    /// 全局支援技能 施法者状态逻辑 (解冻 / 技能重置共用).
    /// </summary>
    public class PetLogicSupport : PetLogic {
        public static readonly PetLogicSupport Instance = new();

        public void tick(PetView pet, SheepMgr sheepMgr) {
            var skill = SheepSkill.getById(pet.readySkillId);
            if (skill == null || !SheepSkillSubSupport.TryGetById(skill.id, out var conf)) {
                Debug.LogError("支援技能配置缺失, readySkillId = " + pet.readySkillId);
                backToMove(pet);
                return;
            }

            var animFrame = pet.animFrame;

            // 效果生效帧 (castWaveCnt 复用为 "是否已经生效" 的标记, 保证只生效一次)
            if (pet.castWaveCnt == 0 && animFrame >= conf.atkFrame) {
                pet.castWaveCnt = 1;

                if (conf.effect == SheepSupportEffect.Unfreeze) {
                    sheepMgr.unfreezeAlly(pet.camp);
                }
                else if (conf.effect == SheepSupportEffect.SkillReset) {
                    sheepMgr.resetAllySkill(pet.camp, pet);
                }
                else {
                    Debug.LogError("未知的支援技能效果: " + conf.effect);
                }
            }

            if (animFrame < conf.endFrame) {
                return;
            }

            pet.isLock = false;

            if (conf.endState == (int)SheepRoleState.Dead) {
                pet.isDie = true;
                pet.state = SheepRoleState.Dead;
                pet.subState = SheepRoleSubState.Dead;
                pet.animType = SheepRoleAnimType.Dead;
                return;
            }

            if (conf.endState != (int)SheepRoleState.Move) {
                Debug.LogError("支援技能 endState 错误: " + conf.endState);
            }

            backToMove(pet);
        }

        private static void backToMove(PetView pet) {
            pet.isLock = false;
            pet.state = SheepRoleState.Move;
            pet.subState = SheepRoleSubState.MoveBoss;
            pet.animType = SheepRoleAnimType.Idle;
        }
    }
}
