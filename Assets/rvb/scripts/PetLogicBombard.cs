using UnityEngine;

namespace rvb.scripts {
    /// <summary>
    /// 天降轰炸 施法者状态逻辑.
    /// </summary>
    public class PetLogicBombard : PetLogic {
        public static readonly PetLogicBombard Instance = new();

        public void tick(PetView pet, SheepMgr sheepMgr) {
            var skill = SheepSkill.getById(pet.readySkillId);
            if (skill == null || !SheepSkillSubBombard.TryGetById(skill.id, out var conf)) {
                Debug.LogError("轰炸技能配置缺失, readySkillId = " + pet.readySkillId);
                backToMove(pet);
                return;
            }

            var animFrame = pet.animFrame;

            // 投弹: 从 startFrame 起每 frameStep 帧一波, 共 waveCnt 波.
            // 这里用 "castWaveCnt 推出下一波的帧号" 而不是对 animFrame 取模 ——
            // 攻速翻倍 buff 生效时 action() 每逻辑帧跑两次, animFrame 会一次 +2,
            // 取模写法在 frameStep 为奇数时会整波漏掉.
            if (conf.frameStep > 0 && pet.castWaveCnt < conf.waveCnt) {
                var nextWaveFrame = conf.startFrame + pet.castWaveCnt * conf.frameStep;
                if (animFrame >= nextWaveFrame) {
                    pet.castWaveCnt += 1;
                    sheepMgr.dropBombs(pet, conf.bulletId[(int)pet.camp], conf.frameCnt, conf.startZ, conf.scatterR);
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
                Debug.LogError("轰炸技能 endState 错误: " + conf.endState);
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
