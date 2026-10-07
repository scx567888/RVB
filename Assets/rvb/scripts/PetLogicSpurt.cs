using UnityEngine;

namespace rvb.scripts {
    public class PetLogicSpurt : PetLogic{
        private static readonly int LOOP_FRAME = 4;
        public static readonly PetLogicSpurt  Instance = new ();
        public void tick(PetView pet, SheepMgr sheepMgr) {
            // 每四个逻辑帧 (action) 执行一次
            var shouldExecute = pet.frame % LOOP_FRAME == LOOP_FRAME - 1;
            if (!shouldExecute) {
                return;
            }

            if (pet.conf.skillSpurt != 0) {
                var s = SheepSkill.getById(pet.conf.skillSpurt);
                if (s.skillType == SheepSkillType.Boom) {
                    var o = SheepSkillSubBoom.getById(s.id);
                    o.tick(sheepMgr, pet);
                }
                else if (s.skillType == SheepSkillType.Killer) {
                    var o = SheepSkillSubKiller.getById(s.id);
                    o.tick(sheepMgr, pet);
                }
                else if (s.skillType == SheepSkillType.Bullet) {
                    var o = SheepSkillSubBullet.getById(s.id);
                    o.tick(sheepMgr, pet);
                }
                else if (s.skillType == SheepSkillType.CallBullets) {
                    var o = SheepSkillSubCallBullets.getById(s.id);
                    o.tick(sheepMgr, pet);
                }
                else if (s.skillType == SheepSkillType.Freeze) {
                    var o = SheepSkillSubFreeze.getById(s.id);
                    o.tick(sheepMgr, pet);
                }
                else if (s.skillType == SheepSkillType.Destruction) {
                    var o = SheepSkillSubDestruction.getById(s.id);
                    o.tick(sheepMgr, pet);
                }
                else if (s.skillType == SheepSkillType.Bombard) {
                    var o = SheepSkillSubBombard.getById(s.id);
                    o.tick(sheepMgr, pet);
                }
                else if (s.skillType == SheepSkillType.Support) {
                    var o = SheepSkillSubSupport.getById(s.id);
                    o.tick(sheepMgr, pet);
                }
                else if (s.skillType == SheepSkillType.Buff) {
                    // 攻速翻倍: 进入 Buff 状态, role_logic 会在 buff 帧窗口内把本阵营的
                    // logic_counts 置为 2 (每逻辑帧跑两次 action)
                    var o = SheepSkillSubBuff.getById(s.id);
                    pet.state = SheepRoleState.Buff;
                    pet.subState = SheepRoleSubState.Buff;
                    pet.animType = SheepRoleAnimType.Idle;
                    pet.readySkillId = o.id;
                }
            }
            else {
                var fff = sheepMgr.findTar(pet);
                var s = fff.atkTar;
                var o = fff.moveTar;
                var l = fff.moveBoss;

                if (s != null) {
                    pet.state = SheepRoleState.Attack;
                    pet.subState = SheepRoleSubState.AttackAwait;
                    return;
                }

                if (o != null) {
                    pet.state = SheepRoleState.Move;
                    pet.subState = SheepRoleSubState.MoveTar;
                    sheepMgr.moveTar(pet, o);
                    return;
                }

                if (l != null) {
                    pet.state = SheepRoleState.Move;
                    pet.subState = SheepRoleSubState.MoveBoss;
                    sheepMgr.moveTar(pet, l);
                    return;
                }

                sheepMgr.moveTar(pet, null);
            }
        }
    }
}