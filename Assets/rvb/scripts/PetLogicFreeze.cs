using UnityEngine;

namespace rvb.scripts {
    /// <summary>
    /// 全局冰冻技能 施法者状态逻辑.
    /// </summary>
    public class PetLogicFreeze : PetLogic {
        public static readonly PetLogicFreeze Instance = new();

        public void tick(PetView pet, SheepMgr sheepMgr) {
            var skill = SheepSkill.getById(pet.readySkillId);
            if (skill == null || !SheepSkillSubFreeze.TryGetById(skill.id, out var conf)) {
                Debug.LogError("冰冻技能配置缺失, readySkillId = " + pet.readySkillId);
                backToMove(pet);
                return;
            }

            var animFrame = pet.animFrame;

            // 施法生效帧: 冰冻敌方全部单位 (freezeCasted 保证一次施法只生效一次)
            if (!pet.freezeCasted && animFrame >= conf.atkFrame) {
                pet.freezeCasted = true;
                sheepMgr.freezeEnemyAll(
                    pet.camp,
                    conf.freezeFrame,
                    conf.thawFrame,
                    conf.immuneFrame,
                    pet,
                    conf.atkBet
                );
            }

            if (animFrame < conf.endFrame) {
                return;
            }

            pet.isLock = false;

            // 一次性消耗掉施法者 (寒冰菇式)
            if (conf.endState == (int)SheepRoleState.Dead) {
                pet.isDie = true;
                pet.state = SheepRoleState.Dead;
                pet.subState = SheepRoleSubState.Dead;
                pet.animType = SheepRoleAnimType.Dead;
                return;
            }

            if (conf.endState != (int)SheepRoleState.Move) {
                Debug.LogError("冰冻技能 endState 错误: " + conf.endState);
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
