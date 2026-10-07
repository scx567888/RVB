using System;
using scx.SpriteRenderer;
using UnityEngine;

namespace rvb.scripts {
    
    /// 单位数据定义
    public class PetView {
        // 唯一 id
        public int id = 0;
        // 是否活跃 (用于 SheepMgr 使用)
        public bool isActive = false;
        // 是否死亡
        public bool isDie = false;
        // 阵营
        public SheepCamp camp = SheepCamp.Red;
        // 静态配置
        public SheepRoleTypeInfo conf;
        // 
        public int roleId = 0;
        // 主状态
        public SheepRoleState state = SheepRoleState.In;
        // 子状态
        public SheepRoleSubState subState = SheepRoleSubState.None;
        
        // 逻辑位置相关
        public float posBefX = 0;
        public float posBefY = 0;
        public float posX = 0;
        public float posY = 0;
        public float tarPosX = 0;
        public float tarPosY = 0;
        public float dirX = 0;
        public float dirY = 0;
        public float impulseX = 0;
        public float impulseY = 0;
        public int frame = 0;
        
        // 动画位置相关
        public float animX = 0;
        public float animY = 0;
        public float animZ = 0;
        private SheepRoleAnimType _animType = 0;
        public int animFrame = 0;

        // 当前 血量
        public float curHp = 0;
        public float curAtkBuff = 0;
        public int curAckFrame = 0;
        public float curAckCd = 0;
        public bool isHeavyAtk = false;
        public bool isNotConn = false;
        public bool isBoom = false;
        
        public bool isLock = false;
        
        public int readySkillId = 0;
        public int energy = 0;

        // ******************** 冰冻 (Freeze) 相关 ********************

        // 剩余 完全冰冻 帧数 (期间完全静止)
        public int freezeFrame = 0;

        // 剩余 解冻 帧数 (期间行动能力线性恢复)
        public int thawFrame = 0;

        // 解冻 总帧数 (用于计算解冻进度)
        public int thawTotalFrame = 0;

        // 完全恢复之后的 冰冻免疫 剩余帧数 (防止被无限连冻)
        public int freezeImmuneFrame = 0;

        // 待生效的免疫帧数, 在解冻完成的那一帧写入 freezeImmuneFrame
        private int pendingFreezeImmuneFrame = 0;

        // 本次冰冻施法是否已经生效 (施法者使用)
        public bool freezeCasted = false;

        // 本次施法已经进行的波次 (轰炸用来数投弹波次; 自爆 / 支援用来保证只生效一次)
        public int castWaveCnt = 0;

        // 渲染层使用: 当前是否处于 "被染成冰色" 的状态, 避免每帧重复写入颜色
        public bool freezeTinted = false;

        public BuffTimeAttacher attacher;

        // 渲染器句柄
        public ScxSpriteRenderUnit renderUnit;

        public PetView() {
            
        }

        public SheepRoleAnimType animType {
            get { return _animType; }

            set {
                _animType = value;
                animFrame = 0;
                animFrameAcc = 0f;
            }
        }

        // 是否处于 完全冰冻 (不能行动)
        public bool isFrozen => freezeFrame > 0;

        // 是否处于 解冻过程中
        public bool isThawing => freezeFrame <= 0 && thawFrame > 0;

        /// 行动速率: 0 (完全冰冻) ~ 1 (正常).
        /// 解冻过程中线性恢复, 用于缩放 移动速度 / 攻击 cd / 动画播放速度.
        public float freezeActionScale {
            get {
                // 死亡后不再受冰冻影响: 否则 updateAnimFrame() 拿到 0 会让死亡动画卡住,
                // role_logic 里 "animFrame >= 动画总帧数 - 1" 的回收条件永远不成立, 尸体留在场上.
                if (isDie) {
                    return 1f;
                }

                if (freezeFrame > 0) {
                    return 0f;
                }

                if (thawFrame <= 0 || thawTotalFrame <= 0) {
                    return 1f;
                }

                return 1f - (float)thawFrame / thawTotalFrame;
            }
        }

        /// 冰冻视觉强度: 1 (完全冰冻) ~ 0 (完全解冻). 渲染层据此上色.
        public float freezeViewRatio {
            get {
                if (freezeFrame > 0) {
                    return 1f;
                }

                if (thawFrame <= 0 || thawTotalFrame <= 0) {
                    return 0f;
                }

                return (float)thawFrame / thawTotalFrame;
            }
        }

        /// 施加冰冻. 返回 是否成功施加.
        /// freezeFrames 完全冰冻帧数, thawFrames 解冻帧数, immuneFrames 解冻后的额外免疫帧数.
        public bool applyFreeze(int freezeFrames, int thawFrames, int immuneFrames) {
            // 死亡单位 / BOSS 免疫
            if (isDie || curHp <= 0) {
                return false;
            }

            if (conf == null || conf.roleType == SheepRoleType.BOSS) {
                return false;
            }

            // 只有"完全恢复后的免疫期"内才拒绝.
            // 还在冰冻/解冻中时允许被下一次大招刷新 (取较大值), 不会出现大招打空的情况.
            if (freezeImmuneFrame > 0) {
                return false;
            }

            // 取较大值, 避免短时间冰冻覆盖掉长时间冰冻
            if (freezeFrames > freezeFrame) {
                freezeFrame = freezeFrames;
            }

            if (thawFrames > thawFrame) {
                thawFrame = thawFrames;
                thawTotalFrame = thawFrames;
            }

            // 免疫从"完全解冻"那一刻才开始计时
            if (immuneFrames > pendingFreezeImmuneFrame) {
                pendingFreezeImmuneFrame = immuneFrames;
            }

            // 冻住的单位不会被击退
            impulseX = 0;
            impulseY = 0;

            return true;
        }

        /// 立即清除冰冻状态 (驱散用). 不清除已经生效的免疫.
        public void clearFreeze() {
            freezeFrame = 0;
            thawFrame = 0;
            thawTotalFrame = 0;
            pendingFreezeImmuneFrame = 0;
        }

        /// 推进冰冻计时. 由 SheepMgr.role_logic 每 "逻辑帧" 调用一次.
        /// 注意: 不能放在 action() 里 —— action() 会按 logic_counts 每逻辑帧跑 1~2 次
        /// (攻速翻倍 buff 生效时为 2), 那样带 buff 的一方会提前一半时间解冻.
        /// 冰冻是控制效果, 时长应该是真实时间, 不被目标自身的加速影响.
        public void updateFreezeTimer() {
            if (freezeImmuneFrame > 0) {
                freezeImmuneFrame -= 1;
            }

            if (freezeFrame > 0) {
                freezeFrame -= 1;
                return;
            }

            if (thawFrame > 0) {
                thawFrame -= 1;
                if (thawFrame == 0) {
                    thawTotalFrame = 0;
                    // 完全恢复, 免疫期开始
                    freezeImmuneFrame = pendingFreezeImmuneFrame;
                    pendingFreezeImmuneFrame = 0;
                }
            }
        }

        public float subCurHp(int t) {
            var old = curHp;
            curHp -= t;
            return old;
        }
        
        public float subAtkCd(float deltaTime) {
            float i = curAckCd;
            if (i != 0f) {
                i -= deltaTime;
                if (i < 0f) {
                    i = 0f;
                }
                curAckCd = i;
            }

            return i;
        }
        
        // 是否处于攻击 cd
        public bool isAtkCd() {
            return curAckCd > 0f;
        }

        // 重置 攻击 cd
        public void resetAtkCd( float t) {
            curAckCd = t;
        }

        public void logicMove(float x, float y) {
            posBefX = posX;
            posBefY = posY;

            posX = x;
            posY = y;
        }

        public virtual void action(SheepMgr sheepMgr, float fixedDeltaTime) {
            // 冰冻: 完全冰冻期间不行动 (计时由 SheepMgr.role_logic 按逻辑帧推进)
            if (this.isFrozen && !this.isDie) {
                // 完全冰冻: 钉在原地, 不跑状态机, 不推进动画帧和逻辑帧
                this.posBefX = this.posX;
                this.posBefY = this.posY;
                this.animX = this.posX;
                this.animY = this.posY;
                this.impulseX = 0;
                this.impulseY = 0;
                return;
            }

            var bbb = this.update_frame(sheepMgr);
            var petIsDie = this.isDie;
            if (!petIsDie) {
                this.update_role_state(bbb,sheepMgr,fixedDeltaTime);
            }

            this.updateAnimFrame();
            
            // 增加逻辑帧
            frame += 1;
        }
        
        private bool update_frame(SheepMgr sheepMgr) {
            var frame = this.frame;
            var loopFrame = sheepMgr.sheepConfig.loopFrame;
            var i = frame % loopFrame == loopFrame - 1;
            var posBefX = this.posBefX;
            var posBefY = this.posBefY;
            var posX = this.posX;
            var posY = this.posY;
            if (!this.isDie) {
                this.animX = posBefX + (posX - posBefX) * (frame % loopFrame) / loopFrame;
                this.animY = posBefY + (posY - posBefY) * (frame % loopFrame) / loopFrame;
            }

            
            if (!this.isDie && i) {
                this.logicMove(posX, posY);
            }

            return i;
        }
        
        private void update_role_state(bool isLogicFrame, SheepMgr sheepMgr, float fixedDeltaTime) {
            // 解冻过程中 攻击 cd 恢复也会变慢
            var actionScale = this.freezeActionScale;
            this.subAtkCd(actionScale >= 1f ? fixedDeltaTime : fixedDeltaTime * actionScale);

            PetLogic petLogic;
            switch (state) {
                case SheepRoleState.Start:
                    petLogic = PetLogicStart.Instance;
                    break;
                case SheepRoleState.In:
                    petLogic = PetLogicIn.Instance;
                    break;
                case SheepRoleState.Spurt:
                    petLogic = PetLogicSpurt.Instance;
                    break;
                case SheepRoleState.Charge:
                    petLogic = PetLogicCharge.Instance;
                    break;
                case SheepRoleState.ChargePlus:
                    petLogic = PetLogicChargePlus.Instance;
                    break;
                case SheepRoleState.SpinSpurt:
                    petLogic = PetLogicSpinSpurt.Instance;
                    break;
                case SheepRoleState.Move:
                    petLogic = PetLogicMove.Instance;
                    break;
                case SheepRoleState.Attack:
                    petLogic = PetLogicAttack.Instance;
                    break;
                case SheepRoleState.Killer:
                    petLogic = PetLogicKiller.Instance;
                    break;
                case SheepRoleState.Boom:
                    petLogic = PetLogicBoom.Instance;
                    break;
                case SheepRoleState.Invincible:
                    petLogic = PetLogicInvincible.Instance;
                    break;
                case SheepRoleState.Bladestorm:
                    petLogic = PetLogicBladestorm.Instance;
                    break;
                case SheepRoleState.Palm:
                    petLogic = PetLogicPalm.Instance;
                    break;
                case SheepRoleState.CallBullets:
                    petLogic = PetLogicCallBullets.Instance;
                    break;
                case SheepRoleState.Buff:
                    petLogic = PetLogicBuff.Instance;
                    break;
                case SheepRoleState.Rigidity:
                    petLogic = PetLogicRigidity.Instance;
                    break;
                case SheepRoleState.Freeze:
                    petLogic = PetLogicFreeze.Instance;
                    break;
                case SheepRoleState.Destruction:
                    petLogic = PetLogicDestruction.Instance;
                    break;
                case SheepRoleState.Bombard:
                    petLogic = PetLogicBombard.Instance;
                    break;
                case SheepRoleState.Support:
                    petLogic = PetLogicSupport.Instance;
                    break;
                case SheepRoleState.SpinAtk:
                    petLogic = PetLogicSpinAtk.Instance;
                    break;
                case SheepRoleState.Dead:
                    petLogic = PetLogicDead.Instance;
                    break;
                case SheepRoleState.Merge:
                    petLogic = PetLogicMerge.Instance;
                    break;
                case SheepRoleState.Res:
                    petLogic = PetLogicRes.Instance;
                    break;
                case SheepRoleState.SkillBullet:
                    petLogic = PetLogicSkillBullet.Instance;
                    break;
                case SheepRoleState.Up:
                    petLogic = PetLogicUp.Instance;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            petLogic.tick(this, sheepMgr);
            
            if (impulseX != 0 || impulseY != 0) {
                if (!isDie && curHp > 0) {
                    var t1 = impulseX;
                    var i1 = impulseY;
                    logicMove(animX + t1, posY + i1);
                }

                impulseX = 0;
                impulseY = 0;
            }
        }    
        
        
        // 动画帧累加器 (解冻过程中动画播放速度小于 1 时使用)
        private float animFrameAcc = 0f;

        // 每逻辑帧调用一次
        public void updateAnimFrame() {
            // 注意: freezeActionScale 在 isDie 时固定返回 1, 所以死亡动画永远按正常速度播放
            var actionScale = this.freezeActionScale;

            if (actionScale >= 1f) {
                animFrame += 1;
                return;
            }

            // 解冻过程中 动画逐渐变快
            animFrameAcc += actionScale;
            if (animFrameAcc >= 1f) {
                animFrameAcc -= 1f;
                animFrame += 1;
            }
        }
        
    }
}