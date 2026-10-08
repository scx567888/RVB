namespace rvb.scripts {
    public enum SheepSkillType {
        Boom = 1,          // 爆炸
        Killer = 2,        // 刺杀 / 处决
        Bullet = 3,        // 子弹攻击
        Bladestorm = 4,    // 剑刃风暴 / 旋风斩
        Invincible = 5,    // 无敌
        ForceState = 6,    // 强制状态切换
        Palm = 7,          // 掌击
        CallBullets = 8,   // 召唤子弹 / 批量生成子弹
        Buff = 9,          // 增益效果
        Charge = 10,       // 冲锋
        Destruction = 11,  // 毁灭 / 摧毁
        SpinAtk = 12,      // 旋转攻击
        SpinSpurt = 13,    // 旋转冲刺
        Rigidity = 14,     // 僵直 / 硬直
        ChargePlus = 15,   // 强化冲锋
        Freeze = 16,       // 冰冻 (控制 + 逐渐解冻)
        Bombard = 17,      // 天降轰炸 (随机落点炮击)
        Support = 18       // 全局支援 (解冻 / 技能重置)
    }
}