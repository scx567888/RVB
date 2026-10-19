using System;
using System.Collections.Generic;

namespace rvb.scripts {
    public sealed class SheepSkill {
        public int id;
        public string name = string.Empty;
        public SheepSkillType skillType;

        public static IReadOnlyList<SheepSkill> List => SheepSkills.All;

        public static SheepSkill getById(int id) {
            return SheepSkills.GetById(id);
        }

        public static bool TryGetById(int id, out SheepSkill skill) {
            return SheepSkills.TryGetById(id, out skill);
        }
    }

    public static class SheepSkills {
        // 爆炸技能
        public static readonly SheepSkill skill_10000 = new() {
            id = 10000,
            name = "爆炸技能",
            skillType = SheepSkillType.Boom
        };

        // 小河马冲锋爆炸
        public static readonly SheepSkill skill_10001 = new() {
            id = 10001,
            name = "小河马冲锋爆炸",
            skillType = SheepSkillType.Boom
        };

        // 河马冲锋爆炸
        public static readonly SheepSkill skill_10002 = new() {
            id = 10002,
            name = "河马冲锋爆炸",
            skillType = SheepSkillType.Boom
        };

        // 小老虎落地爆炸
        public static readonly SheepSkill skill_10003 = new() {
            id = 10003,
            name = "小老虎落地爆炸",
            skillType = SheepSkillType.Boom
        };

        // 老虎落地爆炸
        public static readonly SheepSkill skill_10004 = new() {
            id = 10004,
            name = "老虎落地爆炸",
            skillType = SheepSkillType.Boom
        };

        // 大河马冲锋爆炸
        public static readonly SheepSkill skill_10005 = new() {
            id = 10005,
            name = "大河马冲锋爆炸",
            skillType = SheepSkillType.Boom
        };

        // 大河马攻击爆炸
        public static readonly SheepSkill skill_10006 = new() {
            id = 10006,
            name = "大河马攻击爆炸",
            skillType = SheepSkillType.Boom
        };

        // 大河马攻击爆炸
        public static readonly SheepSkill skill_10007 = new() {
            id = 10007,
            name = "大河马攻击爆炸",
            skillType = SheepSkillType.Boom
        };

        // 骑兵冲刺爆炸技能
        public static readonly SheepSkill skill_10008 = new() {
            id = 10008,
            name = "骑兵冲刺爆炸技能",
            skillType = SheepSkillType.Boom
        };

        // 旋转攻击爆炸技能
        public static readonly SheepSkill skill_10009 = new() {
            id = 10009,
            name = "旋转攻击爆炸技能",
            skillType = SheepSkillType.Boom
        };

        // 麒麟冲刺爆炸技能
        public static readonly SheepSkill skill_10010 = new() {
            id = 10010,
            name = "麒麟冲刺爆炸技能",
            skillType = SheepSkillType.Boom
        };

        // 刺杀技能
        public static readonly SheepSkill skill_20000 = new() {
            id = 20000,
            name = "刺杀技能",
            skillType = SheepSkillType.Killer
        };

        // 小刺客冲刺刺杀
        public static readonly SheepSkill skill_20001 = new() {
            id = 20001,
            name = "小刺客冲刺刺杀",
            skillType = SheepSkillType.Killer
        };

        // 刺客冲刺刺杀
        public static readonly SheepSkill skill_20002 = new() {
            id = 20002,
            name = "刺客冲刺刺杀",
            skillType = SheepSkillType.Killer
        };

        // 大刺客冲刺刺杀
        public static readonly SheepSkill skill_20003 = new() {
            id = 20003,
            name = "大刺客冲刺刺杀",
            skillType = SheepSkillType.Killer
        };

        // 大刺客攻击刺杀
        public static readonly SheepSkill skill_20004 = new() {
            id = 20004,
            name = "大刺客攻击刺杀",
            skillType = SheepSkillType.Killer
        };

        // 大刺客攻击刺杀
        public static readonly SheepSkill skill_20005 = new() {
            id = 20005,
            name = "大刺客攻击刺杀",
            skillType = SheepSkillType.Killer
        };

        // 大刺客攻击刺杀
        public static readonly SheepSkill skill_20006 = new() {
            id = 20006,
            name = "大刺客攻击刺杀",
            skillType = SheepSkillType.Killer
        };

        // 子弹技能
        public static readonly SheepSkill skill_30000 = new() {
            id = 30000,
            name = "子弹技能",
            skillType = SheepSkillType.Bullet
        };

        // 弓箭手冲刺射箭
        public static readonly SheepSkill skill_30001 = new() {
            id = 30001,
            name = "弓箭手冲刺射箭",
            skillType = SheepSkillType.Bullet
        };

        // 金箍棒
        public static readonly SheepSkill skill_30002 = new() {
            id = 30002,
            name = "金箍棒",
            skillType = SheepSkillType.Bullet
        };

        // 剑刃风暴技能
        public static readonly SheepSkill skill_40000 = new() {
            id = 40000,
            name = "剑刃风暴技能",
            skillType = SheepSkillType.Bladestorm
        };

        // 大刺客剑刃风暴
        public static readonly SheepSkill skill_40001 = new() {
            id = 40001,
            name = "大刺客剑刃风暴",
            skillType = SheepSkillType.Bladestorm
        };

        // 格挡技能
        public static readonly SheepSkill skill_50000 = new() {
            id = 50000,
            name = "格挡技能",
            skillType = SheepSkillType.Invincible
        };

        // 大河马大招格挡
        public static readonly SheepSkill skill_50001 = new() {
            id = 50001,
            name = "大河马大招格挡",
            skillType = SheepSkillType.Invincible
        };

        // 重新回到in状态
        public static readonly SheepSkill skill_60001 = new() {
            id = 60001,
            name = "重新回到in状态",
            skillType = SheepSkillType.ForceState
        };

        // 如来神掌
        public static readonly SheepSkill skill_70001 = new() {
            id = 70001,
            name = "如来神掌",
            skillType = SheepSkillType.Palm
        };

        // 强力如来神掌
        public static readonly SheepSkill skill_70002 = new() {
            id = 70002,
            name = "强力如来神掌",
            skillType = SheepSkillType.Palm
        };

        // 召唤冰雹
        public static readonly SheepSkill skill_80001 = new() {
            id = 80001,
            name = "召唤冰雹",
            skillType = SheepSkillType.CallBullets
        };

        // 召唤剑雨
        public static readonly SheepSkill skill_80002 = new() {
            id = 80002,
            name = "召唤剑雨",
            skillType = SheepSkillType.CallBullets
        };

        // 召唤鬼火
        public static readonly SheepSkill skill_80003 = new() {
            id = 80003,
            name = "召唤鬼火",
            skillType = SheepSkillType.CallBullets
        };

        // 召唤导弹
        public static readonly SheepSkill skill_80004 = new() {
            id = 80004,
            name = "召唤导弹",
            skillType = SheepSkillType.CallBullets
        };

        // 召唤导弹
        public static readonly SheepSkill skill_80005 = new() {
            id = 80005,
            name = "召唤导弹",
            skillType = SheepSkillType.CallBullets
        };

        // 加buff
        public static readonly SheepSkill skill_90001 = new() {
            id = 90001,
            name = "加buff",
            skillType = SheepSkillType.Buff
        };

        // 骑兵冲刺技能
        public static readonly SheepSkill skill_110001 = new() {
            id = 110001,
            name = "骑兵冲刺技能",
            skillType = SheepSkillType.Charge
        };

        // 自爆
        public static readonly SheepSkill skill_120001 = new() {
            id = 120001,
            name = "自爆",
            skillType = SheepSkillType.Destruction
        };

        // 旋转攻击技能
        public static readonly SheepSkill skill_130001 = new() {
            id = 130001,
            name = "旋转攻击技能",
            skillType = SheepSkillType.SpinAtk
        };

        // 旋转冲刺技能
        public static readonly SheepSkill skill_140001 = new() {
            id = 140001,
            name = "旋转冲刺技能",
            skillType = SheepSkillType.SpinSpurt
        };

        // 僵直技能
        public static readonly SheepSkill skill_150001 = new() {
            id = 150001,
            name = "僵直技能",
            skillType = SheepSkillType.Rigidity
        };

        // 麒麟冲刺
        public static readonly SheepSkill skill_160001 = new() {
            id = 160001,
            name = "麒麟冲刺",
            skillType = SheepSkillType.ChargePlus
        };

        // 全场冰冻 (寒冰菇式大招)
        public static readonly SheepSkill skill_170001 = new() {
            id = 170001,
            name = "全场冰冻",
            skillType = SheepSkillType.Freeze
        };

        // 自爆兵自爆
        public static readonly SheepSkill skill_120002 = new() {
            id = 120002,
            name = "自爆兵自爆",
            skillType = SheepSkillType.Destruction
        };

        // 炮火覆盖 (天降轰炸)
        public static readonly SheepSkill skill_180001 = new() {
            id = 180001,
            name = "炮火覆盖",
            skillType = SheepSkillType.Bombard
        };

        // 寒冰消融 (全体解冻)
        public static readonly SheepSkill skill_190001 = new() {
            id = 190001,
            name = "寒冰消融",
            skillType = SheepSkillType.Support
        };

        // 全军号令 (全体技能重置)
        public static readonly SheepSkill skill_190002 = new() {
            id = 190002,
            name = "全军号令",
            skillType = SheepSkillType.Support
        };

        public static readonly SheepSkill[] All = {
            skill_10000,
            skill_10001,
            skill_10002,
            skill_10003,
            skill_10004,
            skill_10005,
            skill_10006,
            skill_10007,
            skill_10008,
            skill_10009,
            skill_10010,
            skill_20000,
            skill_20001,
            skill_20002,
            skill_20003,
            skill_20004,
            skill_20005,
            skill_20006,
            skill_30000,
            skill_30001,
            skill_30002,
            skill_40000,
            skill_40001,
            skill_50000,
            skill_50001,
            skill_60001,
            skill_70001,
            skill_70002,
            skill_80001,
            skill_80002,
            skill_80003,
            skill_80004,
            skill_80005,
            skill_90001,
            skill_110001,
            skill_120001,
            skill_130001,
            skill_140001,
            skill_150001,
            skill_160001,
            skill_170001,
            skill_120002,
            skill_180001,
            skill_190001,
            skill_190002,
        };

        private static readonly Dictionary<int, SheepSkill> Map = BuildMap();

        public static SheepSkill GetById(int id) {
            if (!Map.TryGetValue(id, out SheepSkill skill)) {
                return null;
            }

            return skill;
        }

        public static bool TryGetById(int id, out SheepSkill skill) {
            return Map.TryGetValue(id, out skill);
        }

        private static Dictionary<int, SheepSkill> BuildMap() {
            var map = new Dictionary<int, SheepSkill>(All.Length);

            foreach (SheepSkill skill in All) {
                if (!map.TryAdd(skill.id, skill)) {
                    throw new InvalidOperationException($"SheepSkill 存在重复 ID: {skill.id}");
                }
            }

            return map;
        }
    }
}