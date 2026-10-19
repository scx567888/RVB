using System;
using System.Collections.Generic;

namespace rvb.scripts {
    /// <summary>
    /// 全局支援技能 子配置.
    ///
    /// 施法者进入 <see cref="SheepRoleState.Support"/> 状态, 在 atkFrame 帧对我方全体生效,
    /// 在 endFrame 帧按 endState 结束 (默认 Dead, 一次性消耗).
    /// 具体效果由 <see cref="effect"/> 决定.
    /// </summary>
    public sealed class SheepSkillSubSupport {
        public int id;
        public string name = string.Empty;

        // 效果类型
        public SheepSupportEffect effect;

        // 是否播放专门的技能动画 (0: 用普通攻击动画)
        public int isAnim;

        // 第几帧生效
        public int atkFrame;

        // 施法动画结束帧
        public int endFrame;

        // 施法结束后施法者切换到的状态 (Dead = 一次性消耗, Move = 继续战斗)
        public int endState;

        /// <summary>
        /// 冲刺 (Spurt) 阶段的技能判定. 支援技能不依赖目标和位置, 出场即释放.
        /// </summary>
        public void tick(SheepMgr sheepMgr, PetView e) {
            castStart(e);
        }

        public void castStart(PetView e) {
            e.state = SheepRoleState.Support;
            e.subState = SheepRoleSubState.Support;
            // animType 的 setter 会把 animFrame 归 0, 保证施法动画从头播
            e.animType = isAnim != 0 ? SheepRoleAnimType.Palm : SheepRoleAnimType.Attack;
            e.readySkillId = this.id;
            e.castWaveCnt = 0;
            e.isLock = true;
        }

        public static IReadOnlyList<SheepSkillSubSupport> List => SheepSkillSubSupports.All;

        public static SheepSkillSubSupport getById(int id) {
            return SheepSkillSubSupports.GetById(id);
        }

        public static bool TryGetById(int id, out SheepSkillSubSupport config) {
            return SheepSkillSubSupports.TryGetById(id, out config);
        }
    }

    public static class SheepSkillSubSupports {
        // 寒冰消融: 解除我方全体的冰冻/解冻状态, 立刻恢复战斗
        // 帧数对齐 animId=103 的 Attack 动画
        public static readonly SheepSkillSubSupport config_190001 = new() {
            id = 190001,
            name = "寒冰消融",
            effect = SheepSupportEffect.Unfreeze,
            isAnim = 0,
            atkFrame = 6,
            endFrame = 16,
            endState = (int)SheepRoleState.Dead
        };

        // 全军号令: 我方全体重新释放一次自己的出场技能
        // isAnim = 1 用 Palm 动画, 只有 animId=107 (羊神) 这类图集才有 Palm
        public static readonly SheepSkillSubSupport config_190002 = new() {
            id = 190002,
            name = "全军号令",
            effect = SheepSupportEffect.SkillReset,
            isAnim = 1,
            atkFrame = 20,
            endFrame = 40,
            endState = (int)SheepRoleState.Dead
        };

        public static readonly SheepSkillSubSupport[] All = {
            config_190001,
            config_190002,
        };

        private static readonly Dictionary<int, SheepSkillSubSupport> Map = BuildMap();

        public static SheepSkillSubSupport GetById(int id) {
            if (!Map.TryGetValue(id, out SheepSkillSubSupport config)) {
                throw new KeyNotFoundException($"不存在 SheepSkillSubSupport 配置，ID: {id}");
            }

            return config;
        }

        public static bool TryGetById(int id, out SheepSkillSubSupport config) {
            return Map.TryGetValue(id, out config);
        }

        private static Dictionary<int, SheepSkillSubSupport> BuildMap() {
            var map = new Dictionary<int, SheepSkillSubSupport>(All.Length);

            foreach (SheepSkillSubSupport config in All) {
                if (!map.TryAdd(config.id, config)) {
                    throw new InvalidOperationException($"SheepSkillSubSupport 存在重复 ID: {config.id}");
                }
            }

            return map;
        }
    }
}
