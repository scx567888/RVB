using System;
using System.Collections.Generic;

namespace rvb.scripts {
    /// <summary>
    /// 天降轰炸 子配置.
    ///
    /// 施法者进入 <see cref="SheepRoleState.Bombard"/> 状态, 在持续时间内每隔 frameStep 帧
    /// 投下 frameCnt 发炮弹. 每一波的落点中心是 "随机抽到的一个敌方单位", 再在 scatterR
    /// 半径内随机散布, 所以是 "随机从天上掉炮弹轰炸敌方".
    ///
    /// 炮弹本体 (bulletId) 从 startZ 高度垂直落下, 落地那一帧结算范围伤害并生成爆炸特效,
    /// 这些都由 SheepBullet 配置驱动 (atkFrames / atkR / createBulletID).
    /// </summary>
    public sealed class SheepSkillSubBombard {
        public int id;
        public string name = string.Empty;

        // 是否播放专门的技能动画 (0: 用普通攻击动画)
        public int isAnim;

        // 第几帧开始投弹
        public int startFrame;

        // 投弹间隔 (逻辑帧). 每隔这么多帧投一波
        public int frameStep;

        // 每波投几发
        public int frameCnt;

        // 一共投几波 (投完就停, 不等 endFrame)
        public int waveCnt;

        // 炮弹 id (SheepBullet)
        public int bulletId;

        // 炮弹起始高度
        public int startZ;

        // 落点散布半径 (以随机抽到的敌方单位为中心)
        public int scatterR;

        // 施法动画结束帧
        public int endFrame;

        // 施法结束后施法者切换到的状态 (Dead = 一次性消耗, Move = 继续战斗)
        public int endState;

        /// <summary>
        /// 冲刺 (Spurt) 阶段的技能判定. 场上有敌人就开始轰炸, 否则继续前压.
        /// </summary>
        public void tick(SheepMgr sheepMgr, PetView e) {
            // 场上没有敌人就不浪费技能, 继续前压
            if (!sheepMgr.hasEnemyPet(e.camp)) {
                var result = sheepMgr.findTar(e);
                if (result.moveTar != null) {
                    sheepMgr.moveTar(e, result.moveTar);
                }
                else if (result.moveBoss != null) {
                    sheepMgr.moveTar(e, result.moveBoss);
                }
                else {
                    sheepMgr.moveTar(e, null);
                }

                return;
            }

            castStart(e);
        }

        public void castStart(PetView e) {
            e.state = SheepRoleState.Bombard;
            e.subState = SheepRoleSubState.Bombard;
            // animType 的 setter 会把 animFrame 归 0, 保证施法动画从头播
            e.animType = isAnim != 0 ? SheepRoleAnimType.CallBullets : SheepRoleAnimType.Attack;
            e.readySkillId = this.id;
            e.castWaveCnt = 0;
            e.isLock = true;
        }

        public static IReadOnlyList<SheepSkillSubBombard> List => SheepSkillSubBombards.All;

        public static SheepSkillSubBombard getById(int id) {
            return SheepSkillSubBombards.GetById(id);
        }

        public static bool TryGetById(int id, out SheepSkillSubBombard config) {
            return SheepSkillSubBombards.TryGetById(id, out config);
        }
    }

    public static class SheepSkillSubBombards {
        // 炮火覆盖: 从第 6 帧起每 4 帧投 3 发, 共 8 波 = 24 发
        // 帧数对齐 animId=106 的 Attack 动画 (炮车, 共 37 帧):
        //   最后一波在第 6 + 7*4 = 34 帧, endFrame = 37 正好是一个完整动画循环
        // 施法者放完即消耗
        public static readonly SheepSkillSubBombard config_180001 = new() {
            id = 180001,
            name = "炮火覆盖",
            isAnim = 0,
            startFrame = 6,
            frameStep = 4,
            frameCnt = 3,
            waveCnt = 8,
            bulletId = 113,
            startZ = 1200,
            scatterR = 900,
            endFrame = 37,
            endState = (int)SheepRoleState.Dead
        };

        public static readonly SheepSkillSubBombard[] All = {
            config_180001,
        };

        private static readonly Dictionary<int, SheepSkillSubBombard> Map = BuildMap();

        public static SheepSkillSubBombard GetById(int id) {
            if (!Map.TryGetValue(id, out SheepSkillSubBombard config)) {
                throw new KeyNotFoundException($"不存在 SheepSkillSubBombard 配置，ID: {id}");
            }

            return config;
        }

        public static bool TryGetById(int id, out SheepSkillSubBombard config) {
            return Map.TryGetValue(id, out config);
        }

        private static Dictionary<int, SheepSkillSubBombard> BuildMap() {
            var map = new Dictionary<int, SheepSkillSubBombard>(All.Length);

            foreach (SheepSkillSubBombard config in All) {
                if (!map.TryAdd(config.id, config)) {
                    throw new InvalidOperationException($"SheepSkillSubBombard 存在重复 ID: {config.id}");
                }
            }

            return map;
        }
    }
}
