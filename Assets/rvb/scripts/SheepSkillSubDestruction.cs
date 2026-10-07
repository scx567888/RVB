using System;
using System.Collections.Generic;

namespace rvb.scripts {
    public sealed class SheepSkillSubDestruction {
        public int id;
        public string name = string.Empty;

        // 自爆子弹 id (伤害 / 范围 / 爆炸特效都由 SheepBullet 配置决定)
        // 注意: 该子弹的 animId 必须在场景的 bulletRenderConfigs 里配过, 否则渲染层会抛异常
        public int bulletId;

        // 触发自爆的索敌格子半径 (0 表示不索敌, 一直冲到场地尽头)
        public int triggerFindR;

        // 第几帧生成自爆子弹
        public int atkFrame;

        // 自爆动画结束帧 (之后转 Dead)
        public int endFrame;

        /// <summary>
        /// 冲刺 (Spurt) 阶段的技能判定: 前方出现敌人就自爆, 否则继续无视碰撞地往前冲.
        /// </summary>
        public void tick(SheepMgr sheepMgr, PetView e) {
            if (sheepMgr.hasEnemyNear(e, triggerFindR)) {
                castStart(e);
                return;
            }

            // 没碰到敌人: 继续朝敌方 boss 方向推进 (moveTar 传 null 时冲锋类才会自己定向,
            // 所以这里显式用 boss 当目标, 保证一直往最前方走)
            sheepMgr.moveTar(e, sheepMgr.getBackBoss(e.camp));
        }

        public void castStart(PetView e) {
            e.state = SheepRoleState.Destruction;
            e.subState = SheepRoleSubState.Destruction;
            // animType 的 setter 会把 animFrame 归 0
            e.animType = SheepRoleAnimType.Boom;
            e.readySkillId = this.id;
            e.castWaveCnt = 0;
            e.isLock = true;
        }

        public static IReadOnlyList<SheepSkillSubDestruction> List => SheepSkillSubDestructions.All;

        public static SheepSkillSubDestruction getById(int id) {
            return SheepSkillSubDestructions.GetById(id);
        }

        public static bool TryGetById(int id, out SheepSkillSubDestruction config) {
            return SheepSkillSubDestructions.TryGetById(id, out config);
        }
    }

    public static class SheepSkillSubDestructions {
        // 小兵自爆 (原有配置)
        // ⚠️ bulletId = 14 的 animId 是 11, 而 Sheep.unity 的 bulletRenderConfigs 没有配 11,
        //    直接拿这条配置去用会在渲染子弹时抛 KeyNotFoundException. 新单位请用 config_120002.
        public static readonly SheepSkillSubDestruction config_120001 = new() {
            id = 120001,
            name = "小兵自爆",
            bulletId = 14,
            triggerFindR = 1,
            atkFrame = 1,
            endFrame = 7
        };

        // 自爆兵 (bulletId = 114 的 animId = 19, 在 Sheep.unity 里已配置)
        // 帧数对齐 animId=104 的 Boom 动画
        public static readonly SheepSkillSubDestruction config_120002 = new() {
            id = 120002,
            name = "自爆兵自爆",
            bulletId = 114,
            triggerFindR = 1,
            atkFrame = 2,
            endFrame = 10
        };

        public static readonly SheepSkillSubDestruction[] All = {
            config_120001,
            config_120002,
        };

        private static readonly Dictionary<int, SheepSkillSubDestruction> Map = BuildMap();

        public static SheepSkillSubDestruction GetById(int id) {
            if (!Map.TryGetValue(id, out SheepSkillSubDestruction config)) {
                throw new KeyNotFoundException($"不存在 SheepSkillSubDestruction 配置，ID: {id}");
            }

            return config;
        }

        public static bool TryGetById(int id, out SheepSkillSubDestruction config) {
            return Map.TryGetValue(id, out config);
        }

        private static Dictionary<int, SheepSkillSubDestruction> BuildMap() {
            var map = new Dictionary<int, SheepSkillSubDestruction>(All.Length);

            foreach (SheepSkillSubDestruction config in All) {
                if (!map.TryAdd(config.id, config)) {
                    throw new InvalidOperationException($"SheepSkillSubDestruction 存在重复 ID: {config.id}");
                }
            }

            return map;
        }
    }
}