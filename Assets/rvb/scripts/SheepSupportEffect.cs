namespace rvb.scripts {
    /// <summary>
    /// 全局支援技能的效果类型.
    /// </summary>
    public enum SheepSupportEffect {
        // 解冻: 清除我方全部单位的冰冻/解冻状态, 立刻恢复战斗
        Unfreeze = 1,

        // 技能重置: 让我方全部单位重新回到出场状态, 再放一次自己的出场技能
        SkillReset = 2
    }
}
