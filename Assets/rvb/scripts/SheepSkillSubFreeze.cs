using System;
using System.Collections.Generic;

namespace rvb.scripts {
    /// <summary>
    /// 全局冰冻技能 子配置 (寒冰菇式大招).
    ///
    /// 释放流程:
    ///   1. 单位出场进入 Spurt 状态, 第一次逻辑判定就立即进入 <see cref="SheepRoleState.Freeze"/> 施法状态
    ///      (不需要索敌, 不需要走到前线, 因为是全场效果)
    ///   2. 施法动画第 atkFrame 帧: 冰冻敌方阵营的 "全部" 单位
    ///   3. 施法动画第 endFrame 帧: 施法者按 endState 结束 (默认 Dead, 即一次性消耗掉)
    ///
    /// 被冰冻单位:
    ///   前 freezeFrame 帧 完全不可动, 后 thawFrame 帧 逐渐解冻恢复战斗.
    /// </summary>
    public sealed class SheepSkillSubFreeze {
        public int id;
        public string name = string.Empty;

        // 是否播放专门的技能动画 (0: 用普通攻击动画, 1: 用 CallBullets 动画)
        // 置 1 前请确认兵种图集里存在 CallBullets 动画, 否则渲染层找不到帧会抛异常
        public int isAnim;

        // 施法动画第几帧生效 (冰冻全场)
        public int atkFrame;

        // 冰冻瞬间附带的伤害倍率 (0 表示纯控制, 不造成伤害)
        public float atkBet;

        // 完全冰冻帧数 (逻辑帧, 30 帧 = 1 秒). 这段时间内目标完全不可动
        public int freezeFrame;

        // 解冻帧数 (逻辑帧). 这段时间内目标的移动/攻击/动画速度从 0 线性恢复到 1
        public int thawFrame;

        // 完全恢复之后的免疫帧数, 0 表示不免疫 (即可以被下一次大招立刻再冻)
        public int immuneFrame;

        // 施法动画结束帧
        public int endFrame;

        // 施法结束后施法者切换到的状态.
        // Dead = 一次性消耗掉 (寒冰菇式); Move = 放完继续战斗
        public int endState;

        /// <summary>
        /// 冲刺 (Spurt) 阶段的技能判定.
        /// 全场技能不依赖目标和位置, 所以出场即释放.
        /// </summary>
        public void tick(SheepMgr sheepMgr, PetView e) {
            castStart(e);
        }

        /// <summary>
        /// 进入冰冻施法状态.
        /// </summary>
        public void castStart(PetView e) {
            e.state = SheepRoleState.Freeze;
            e.subState = SheepRoleSubState.Freeze;
            // animType 的 setter 会把 animFrame 归 0, 保证施法动画从头播
            e.animType = isAnim != 0 ? SheepRoleAnimType.CallBullets : SheepRoleAnimType.Attack;
            e.readySkillId = this.id;
            // 本次施法还没有真正生效
            e.freezeCasted = false;
            // 施法期间不被其它逻辑抢走
            e.isLock = true;
        }

        public static IReadOnlyList<SheepSkillSubFreeze> List => SheepSkillSubFreezes.All;

        public static SheepSkillSubFreeze getById(int id) {
            return SheepSkillSubFreezes.GetById(id);
        }

        public static bool TryGetById(int id, out SheepSkillSubFreeze config) {
            return SheepSkillSubFreezes.TryGetById(id, out config);
        }
    }

    public static class SheepSkillSubFreezes {
        // 全场冰冻 (寒冰菇): 全场冻 4 秒 = 完全冰冻 2 秒 + 逐渐解冻 2 秒, 施法者放完即消耗
        // 帧数对齐 animId=102 的 Attack 动画 (共 16 帧, 弓箭手原本的出手帧是 6)
        public static readonly SheepSkillSubFreeze config_170001 = new() {
            id = 170001,
            name = "全场冰冻",
            isAnim = 0,
            atkFrame = 6,
            atkBet = 0f,
            freezeFrame = 60,
            thawFrame = 60,
            immuneFrame = 0,
            endFrame = 16,
            endState = (int)SheepRoleState.Dead
        };

        public static readonly SheepSkillSubFreeze[] All = {
            config_170001,
        };

        private static readonly Dictionary<int, SheepSkillSubFreeze> Map = BuildMap();

        public static SheepSkillSubFreeze GetById(int id) {
            if (!Map.TryGetValue(id, out SheepSkillSubFreeze config)) {
                throw new KeyNotFoundException($"不存在 SheepSkillSubFreeze 配置，ID: {id}");
            }

            return config;
        }

        public static bool TryGetById(int id, out SheepSkillSubFreeze config) {
            return Map.TryGetValue(id, out config);
        }

        private static Dictionary<int, SheepSkillSubFreeze> BuildMap() {
            var map = new Dictionary<int, SheepSkillSubFreeze>(All.Length);

            foreach (SheepSkillSubFreeze config in All) {
                if (!map.TryAdd(config.id, config)) {
                    throw new InvalidOperationException($"SheepSkillSubFreeze 存在重复 ID: {config.id}");
                }
            }

            return map;
        }
    }
}
