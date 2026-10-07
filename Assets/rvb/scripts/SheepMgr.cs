using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using scx.GridMap;
using UnityEngine;
using static rvb.scripts.BullteCreate;
using static rvb.scripts.SheepModes;
using static rvb.scripts.EventBus;

namespace rvb.scripts {
    public class SheepMgr {
        public static SheepMgr inc;


        // 固定 30 fps 
        public static readonly float FixedDeltaTime = 1f / 30f;


        // 自动出兵计时器
        public float autoTime = 0f;

        // 游戏模式 (外部会设置)
        public int gameMode = 0;

        // 时间模式 (外部会设置)
        public int timeMode = 2;

        // boss 血量 (外部会设置)
        public int loongHp = 10000;


        // 地块比例
        public float plotRatio = 0.5f;

        // 核心状态机
        public SheepRoomState state = SheepRoomState.Ready;

        public int gameIndex = 0;
        public float gameStartTimerForBuff = 0;

        public long endTime = 0;
        public List<int>[] preBuffs = new[] { new List<int>(), new List<int>() };
        public List<Buff>[] buffs = new[] { new List<Buff>(), new List<Buff>() };
        public int[] countNewBuffs = new[] { 0, 0 };
        public int[] countBuffs = new[] { 0, 0 };
        public int[] countShowBuffs = new[] { 0, 0 };

        // 反击时刻标识符 (防止多次触发反击时刻)
        public bool[] flagLongBuffs = new[] { false, false };

        public Dictionary<int, int>[] petStartCounts = { new Dictionary<int, int>(), new Dictionary<int, int>() };
        public List<PetView> god_view_pets = new List<PetView>();


        public long updateTime = 0;

        public int redBuffCount;
        public int blueBuffCount;


        public System.Random logicRandom = new System.Random(123);

        //********************************** 以下字段 待处理 **********************************************

        public int plotRatioIndex;


        // ********************** 确定字段 *******************************

        // 红蓝 boss
        public Boss[] bosses = { null, null };

        // 当前在场上的角色
        public List<PetView> pets = new();

        // 准备添加到下一帧的 角色
        public List<PetView> pre_pets = new();

        // 准备删除的 角色, 这里用 Set 保证查询速度
        public HashSet<PetView> del_pets = new();

        // 当前在场上的子弹
        public List<BulletView> bullets = new();

        // 准备添加到下一帧的 子弹
        public List<BulletView> pre_bullets = new();

        // 准备删除的 子弹, 这里用 Set 保证查询速度
        public HashSet<BulletView> del_bullets = new();

        // 角色自增 id 
        public int petId = 0;

        // 子弹自增 id
        public int bulletId = 0;

        // 场上角色数量, 按照 [阵营][角色类型] 存储
        public int[][] petCounts = new[] {
            new int[(int)SheepRoleType.Count],
            new int[(int)SheepRoleType.Count]
        };

        // 格子空间, 用于加速索敌碰撞
        public GridMap<SheepCell> gridMap;

        // 红方召唤池
        public Dictionary<int, SheepCallInfo> redCallInfos = new Dictionary<int, SheepCallInfo>();

        // 蓝方召唤池
        public Dictionary<int, SheepCallInfo> blueCallInfos = new Dictionary<int, SheepCallInfo>();

        // 红蓝双方 每次执行多少逻辑帧 
        public int[] logic_counts = { 1, 1 };

        // 是否自动出兵
        public bool isAutoCall = true;

        public SheepConfig sheepConfig;
        private SheepAnimFrameCountResolver animFrameCountResolver;

        public SheepMgr(
            SheepConfig sheepConfig,
            SheepAnimFrameCountResolver animFrameCountResolver,
            SheepCtl sheepCtl
        ) {
            this.sheepConfig = sheepConfig;
            this.animFrameCountResolver = animFrameCountResolver;
            // 自动出兵计时器
            this.autoTime = 0;

            // 游戏模式 (外部会设置)
            this.gameMode = 0;

            // 时间模式 (外部会设置)
            this.timeMode = 2;

            // boss 血量 (外部会设置)
            this.loongHp = 10000;

            // 红蓝 boss
            this.bosses = sheepCtl.boss;

            // 地块比例
            this.plotRatio = 0.5f;

            // 核心状态机
            this.state = SheepRoomState.Ready;

            this.gameIndex = 0;
            this.gameStartTimerForBuff = 0;

            this.endTime = 0;
            this.preBuffs = new[] { new List<int>(), new List<int>() };
            this.buffs = new[] { new List<Buff>(), new List<Buff>() };
            this.countNewBuffs = new[] { 0, 0 };
            this.countBuffs = new[] { 0, 0 };
            this.countShowBuffs = new[] { 0, 0 };

            // 反击时刻标识符 (防止多次触发反击时刻)
            this.flagLongBuffs = new[] { false, false };

            this.petStartCounts = new[] { new Dictionary<int, int>(), new Dictionary<int, int>() };
            this.god_view_pets = new List<PetView>();


            this.updateTime = 0;

            this.redCallInfos = new Dictionary<int, SheepCallInfo>();

            this.blueCallInfos = new Dictionary<int, SheepCallInfo>();


            // ************************ 以下待整理 **************************

            this.redBuffCount = 0;
            this.blueBuffCount = 0;


            // 绑定 system
            inc = this;


            gridMap = new GridMap<SheepCell>(
                -sheepConfig.w / 2f, -sheepConfig.h / 2f,
                sheepConfig.w, sheepConfig.h,
                sheepConfig.d,
                (gridX, gridY, worldStartX, worldStartY, worldEndX, worldEndY) =>
                    new SheepCell(gridX, gridY, worldStartX, worldStartY, worldEndX, worldEndY)
            );
        }

        // ************************* 生成相关 **************************

        // 获取 petId
        public int getNextPetId() {
            return ++petId;
        }

        // 获取 bulletId
        public int getNextBulletId() {
            return ++bulletId;
        }

        // 添加单位 下一帧才会使用
        public void addPrePet(PetView pet) {
            pre_pets.Add(pet);
        }

        // 添加子弹 下一帧才会使用
        public void addPreBullet(BulletView bullet) {
            pre_bullets.Add(bullet);
        }

        // 将 pre_pets 应用到 pets 中
        public void applyPrePets() {
            foreach (var pet in pre_pets) {
                addPet(pet);
            }

            pre_pets.Clear();
        }

        // 将 pre_bullets 应用到 bullets 中
        public void applyPreBullets() {
            foreach (var bullet in pre_bullets) {
                addBullet(bullet);
            }

            pre_bullets.Clear();
        }

        // ********************* 删除相关 **************************

        // 添加 删除单位.
        public void addDelPet(PetView pet) {
            del_pets.Add(pet);
        }

        // 添加 删除子弹.
        public void addDelBullet(BulletView bullet) {
            del_bullets.Add(bullet);
        }

        // 从 pets 中 删除 del_pets 中的 单位
        public HashSet<PetView> applyDelPets() {
            // 复制一份方便 渲染层处理
            var copy = new HashSet<PetView>(del_pets);

            // 应用移除

            foreach (var pet in del_pets) {
                delPet(pet);
            }

            // 清空
            del_pets.Clear();


            return copy;
        }

        // 从 bullets 中 删除 del_bullets 中的 对象
        public HashSet<BulletView> applyDelBullets() {
            // 复制一份方便 渲染层处理
            var copy = new HashSet<BulletView>(del_bullets);
            // 应用移除
            foreach (var bullet in del_bullets) {
                delBullet(bullet);
            }

            // 清空
            del_bullets.Clear();
            return copy;
        }


        // ****************** 场上单位相关 *************************

        // 添加单位, 不要在逻辑帧循环中调用
        public void addPet(PetView pet) {
            pets.Add(pet);
            petCounts[(int)pet.camp][(int)pet.conf.roleType] += 1;
        }

        // 添加子弹, 不要在逻辑帧循环中调用
        public void addBullet(BulletView bullet) {
            bullets.Add(bullet);
        }

        // 删除单位, 不要在逻辑帧循环中调用
        public void delPet(PetView pet) {
            pets.Remove(pet);
            petCounts[(int)pet.camp][(int)pet.conf.roleType] -= 1;
        }

        // 添加子弹, 不要在逻辑帧循环中调用
        public void delBullet(BulletView bullet) {
            bullets.Remove(bullet);
        }


        // ******************** 格子相关 **************************

        // 重建格子
        public void rebuildGridMap() {
            // 清空格子
            gridMap.forEachCell(cell => {
                cell.clearPets();
            });

            // 重建格子
            foreach (var pet in pets) {
                var cell = gridMap.getCellByWorldPositionSafe(
                    pet.posX,
                    pet.posY
                );
                cell.addPet(pet);
            }
        }

        // 根据阵营获取 pet 总数
        public long getPetCount(SheepCamp camp) {
            var count = 0;
            var p1 = petCounts[(int)camp];
            foreach (var i in p1) {
                count += i;
            }

            return count;
        }


        // ***************************** 旧方法 ************************************

        public void onGameStart() {
            this.gameStartTimerForBuff = 0;
            this.clearPets();
            this.clearCallPets();

            foreach (var e1 in this.petStartCounts) {
                e1.Clear();
            }

            this.preBuffs = new[] { new List<int>(), new List<int>() };
            this.buffs = new[] { new List<Buff>(), new List<Buff>() };
            this.countNewBuffs = new[] { 0, 0 };
            this.countBuffs = new[] { 0, 0 };
            this.countShowBuffs = new[] { 0, 0 };
            this.flagLongBuffs = new[] { false, false };
            var e2 = SheepCtl.instance;
            e2.comMatch.updateWinloops();
        }

        public void onGameRun() {
            foreach (var e in this.god_view_pets) {
                e.state = SheepRoleState.Palm;
                e.subState = SheepRoleSubState.Palm;
                e.animType = SheepRoleAnimType.Palm;
                e.readySkillId = 70002;
            }
        }

        public void onGameEnd() {
            this.god_view_pets = new List<PetView>();
        }

        public void setState(SheepRoomState e) {
            this.state = e;
            Debug.Log("房间状态改变: " + e);
            eventBus.emit(EventType.RoomState, (state = e));
        }

        public void clearPets() {
            this.pets.Clear();
        }

        public void clearPetViews() {
            pets.Clear();
        }


        public void clearViewBullets() {
            foreach (var viewElement in this.bullets) {
                if (viewElement != null) {
                    viewElement.clear();
                }
            }

            foreach (var viewElement in this.pre_bullets) {
                if (viewElement != null) {
                    viewElement.clear();
                }
            }
        }

        public (HashSet<PetView> del_pets, HashSet<BulletView> del_bullets) game_update(SheepCtl sheepCtl, float i) {
            // 处理召唤兵
            this.consume(i);

            // 执行主逻辑
            return this.role_logic(sheepCtl, i);
        }

        public bool updateBoss(SheepCtl sheepCtl, float dt, long c) {
            var isEnd = false;
            for (var i = 0; i < this.bosses.Length; i++) {
                var t = this.bosses[i];
                var index = i;


                var viewPet = this.bosses[index];
                var camp = viewPet.camp;
                var state = viewPet.state;

                if ((int)state == (int)SheepBossState.Ready) {
                    viewPet.curHp = t.curHp;
                    t.comProgress.setVue(t.curHp);
                    viewPet.state = (SheepRoleState)(int)SheepBossState.NomalRun;
                }
                else if ((int)state == (int)SheepBossState.AwakeAnim || (int)state == (int)SheepBossState.UnAwakeAnim) {
                    t.comProgress.setVue(t.comProgress._vue);
                }
                else if ((int)state == (int)SheepBossState.Dead) {
                }
                else {
                    var curHp = viewPet.curHp;
                    if (curHp <= 0) {
                        curHp = 0;
                    }

                    var d = t.comProgress._vue;
                    var _ = d - curHp;

                    if (_ != 0 && curHp != 0) {
                        var S = this.countBuffs[1 - (int)camp];
                        if (S > 0) {
                            var b = 1 + sheepConfig.buffDragonDamageIncreseRate * S;
                            b += 0;
                            _ = (float)Math.Floor(_ * b);
                            curHp = d - _;
                            viewPet.curHp = curHp;
                        }

                        var I = this.countBuffs[(int)camp];
                        if (I > 0) {
                            var B = Math.Pow(1 - sheepConfig.buffDragonReduceRate, I);
                            B -= 0;
                            if (B < 1 - sheepConfig.buffDragonMaxReduceRate) {
                                B = 1 - sheepConfig.buffDragonMaxReduceRate;
                            }

                            _ = (float)Math.Floor(_ * B);
                            curHp = d - _;
                            viewPet.curHp = curHp;
                        }
                    }

                    if (t.subShield(this) && _ > 1) {
                        curHp = d - 1;
                        if (curHp < 0) {
                            curHp = 0;
                        }

                        _ = 1;
                        viewPet.curHp = curHp;
                    }

                    if (curHp != d) {
                        t.comProgress.setVue(curHp);
                        t.curHp = curHp;
                        SheepAnims.showBossBlood(sheepCtl, t, _);
                        t.hitAnim();
                    }

                    var R = this.countShowBuffs[(int)camp];
                    var M = this.countBuffs[(int)camp];

                    if (!this.flagLongBuffs[(int)camp] && curHp < this.loongHp * sheepConfig.counterHpRatio) {
                        this.flagLongBuffs[(int)camp] = true;
                        t.backStateTime = c;
                        this.preBuffs[(int)camp].Add(0);
                        sheepCtl.comMatch.showDoubleAnim(camp);
                        sheepCtl.comUIAnim.backAnim(camp);
                        sheepCtl.cameraCtl.onShake(sheepConfig.shockBeginNumber);
                    }
                    else if (t.backStateTime != 0 && c - t.backStateTime > 12e4 && M - R == 0) {
                        t.backStateTime = 0;
                        sheepCtl.comMatch.hideDoubleAnim(camp);
                        sheepCtl.comUIAnim.backSuccessAnim(camp);
                        sheepCtl.cameraCtl.onShake(sheepConfig.shockEndNumber);
                    }

                    if (curHp <= 0) {
                        viewPet.state = (SheepRoleState)(int)SheepBossState.Dead;
                        viewPet.isDie = true;
                        t.curHp = 0;
                        eventBus.emit(EventType.RoomStateEnd);
                        isEnd = true;
                        continue;
                    }

                    var unuse = viewPet.curAckFrame;

                    var T = 0;
                    var D = this.plotRatio;

                    for (var A = 0; A < sheepConfig.loongStateSwitching.Length; A++) {
                        if (D <= sheepConfig.loongStateSwitching[A]) {
                            T = A;
                            break;
                        }
                    }

                    this.plotRatioIndex = T;
                    t.updateState(sheepCtl, this, T + 1);
                    t.updateStateJJL(sheepCtl, this, T + 1);
                }
            }


            return isEnd;
        }

        public void clear_pets() {
            foreach (var petView in del_pets) {
            }
        }

        public void del_bullet(BulletView e) {
            var bullet = e;
            bullet.id = 0;
            addDelBullet(bullet);
        }

        public void del_pet(PetView e) {
            e.isActive = false;
            e.isDie = true;
            e.id = 0;
            addDelPet(e);
        }

        public void clear_bullets() {
            foreach (var bulletView in del_bullets) {
            }
        }

// todo
        public void game_clear() {
            this.clearPetViews();

            this.clearViewBullets();
            this.clear_pets();
            this.clear_bullets();
            InitializeBossView(SheepCamp.Red);
            InitializeBossView(SheepCamp.Blue);
        }

// todo        
        private void InitializeBossView(SheepCamp camp) {
            int index = (int)camp;
            Boss view = new Boss(index);
            // view.clear();
            view.id = getNextPetId();
            view.isActive = true;
            view.isDie = false;
            view.camp = camp;
            view.roleId = 0;

            view.conf = SheepRoleTypeInfo.getById(0);
            view.state = (SheepRoleState)(int)SheepBossState.Ready;
            view.subState = SheepRoleSubState.None;
            view.animType = SheepRoleAnimType.Idle;
            view.curHp = loongHp;
            view.posX = camp == SheepCamp.Red ? -sheepMode.loongX : sheepMode.loongX;
            view.posY = 0f;
            view.posBefX = view.posX;
            view.posBefY = view.posY;
            view.animX = view.posX;
            view.animY = view.posY;

            view.dirX = camp == SheepCamp.Red ? 1f : -1f;
            view.dirY = 0f;
            
            bosses[(int)camp] = view;
        }

        public (HashSet<PetView> del_pets, HashSet<BulletView> del_bullets) role_logic(SheepCtl sheepCtl, float dt) {
            this.logic_counts[(int)SheepCamp.Red] = this.redBuffCount > 0 ? 2 : 1;
            this.logic_counts[(int)SheepCamp.Blue] = this.blueBuffCount > 0 ? 2 : 1;

            // 应用 预添加数据
            applyPrePets();
            applyPreBullets();
            
            // 重建格子映射
            rebuildGridMap();

            // 单位 行动
            this.bossesAction();
            this.petsAction();
            this.bulletsAction();

            var isEnd = false;

            var now = NowMs();


            if (this.endTime != 0 && this.endTime < NowMs()) {
                eventBus.emit(EventType.RoomStateEnd);
                isEnd = true;
                this.endTime = 0;
                return (new HashSet<PetView>(), new HashSet<BulletView>());
            }


            this.countNewBuffs = new[] { 0, 0 };
            this.countBuffs = new[] { 0, 0 };
            this.countShowBuffs = new[] { 0, 0 };

            // console.log(sheepMgr.buffs)
            for (var i = 0; i < this.buffs.Length; i++) {
                var r = this.buffs[i];
                var s = i;
                if (r.Count != 0 && r[0].time < this.gameStartTimerForBuff) {
                    r.RemoveAt(0);
                    this.buffs[s] = r;
                }

                foreach (var o in r) {
                    this.countBuffs[s] += o.count != 0 ? o.count : sheepConfig.counterBuffNumber;
                    this.countShowBuffs[s] += o.count;
                }
            }

            for (var i = 0; i < this.preBuffs.Length; i++) {
                var r = this.preBuffs[i];
                var s = i;

                if (r.Count == 0) {
                    continue;
                }

                var sum = 0;
                var hasZero = false;

                foreach (var f in r) {
                    if (0 == f) {
                        hasZero = true;
                    }

                    sum += f;
                }

                if (hasZero) {
                    this.buffs[s].Add(new Buff() {
                        time = (int)(this.gameStartTimerForBuff + 1000 * sheepConfig.counterTime),
                        count = 0
                    });

                    if (r.Count > 1) {
                        this.buffs[s].Add(new Buff() {
                            time = (int)(this.gameStartTimerForBuff + 1000 * sheepConfig.buffLastTime),
                            count = sum
                        });
                    }
                }
                else {
                    this.buffs[s].Add(new Buff() {
                        time = (int)(this.gameStartTimerForBuff + 1000 * sheepConfig.buffLastTime),
                        count = sum
                    });
                }

                this.preBuffs[s] = new List<int>();
                this.countNewBuffs[s] += sum;
            }

            isEnd = this.updateBoss(sheepCtl, dt, now);

            if (isEnd) {
                return (new HashSet<PetView>(), new HashSet<BulletView>());
            }

            var _redBuffCount = 0;
            var _blueBuffCount = 0;


            foreach (var y in this.pets) {
                // 冰冻计时按逻辑帧推进, 不受 logic_counts 的加速影响
                y.updateFreezeTimer();

                updateSkinPet(y, sheepCtl, this, this, dt);


                var D = y;
                var A = D.state;
                var P = D.animType;
                var W = D.animFrame;

                int M = animFrameCountResolver.resolve(y.camp, y.conf.animId, P);

                if (A == SheepRoleState.In && W >= M - 1) {
                    var E = SheepSkill.getById(D.readySkillId);
                    if (E != null) {
                        if (E.skillType == SheepSkillType.Boom) {
                            var F = SheepSkillSubBoom.getById(E.id);
                            D.state = SheepRoleState.Boom;
                            if (F.isAnim != 0) {
                                D.animType = SheepRoleAnimType.Boom;
                            }
                            else {
                                D.animType = SheepRoleAnimType.Idle;
                            }
                        }
                    }
                    else {
                        D.state = SheepRoleState.Move;
                        D.animType = SheepRoleAnimType.Idle;
                    }
                }
                else if (A == SheepRoleState.Dead && W >= M - 1) {
                    D.state = SheepRoleState.Res;
                    D.animType = SheepRoleAnimType.None;
                    del_pet(D);
                }
                else if (A == SheepRoleState.Up && W >= M - 1) {
                    D.state = SheepRoleState.In;
                    D.animType = SheepRoleAnimType.In;
                }
                else if (A == SheepRoleState.Buff) {
                    var V = SheepSkillSubBuff.getById(D.readySkillId);
                    var U = D.animFrame;
                    // 被冰冻的单位 action() 直接返回, animFrame 不推进, 会把自己卡在 buff
                    // 帧窗口里导致 buff 无限延长. 冰冻期间不提供 buff.
                    if (D.isFrozen) {
                    }
                    else if (U > V.buffStratFrame && U < V.buffEndFrame) {
                        if (y.camp == SheepCamp.Blue) {
                            _blueBuffCount += 1;
                        }
                        else {
                            _redBuffCount += 1;
                        }
                    }
                }
            }


            foreach (var X in bullets) {
                if (X.isDie) {
                    continue;
                }

                if (X.frame >= X.conf.endFrame) {
                    X.isDie = true;
                    this.del_bullet(X);
                }
                else {
                    var z = X.roleIndex.conf.splitN;

                    for (var O = -z; O <= z; ++O) {
                        for (var Q = -z; Q <= z; ++Q) {
                            var (gridX,gridY) = getXnYn(X.x + O, X.y + Q);
                            sheepCtl.addFrameBlockCamp(gridX,gridY, X.camp);
                        }
                    }
                }
            }


            

            this.redBuffCount = _redBuffCount;
            this.blueBuffCount = _blueBuffCount;


            var del_pets1 = applyDelPets();

            var del_bullets1 = applyDelBullets();

            return (del_pets1, del_bullets1);
        }
        
        // boss 行动
        public void bossesAction() {
            foreach (var boss in bosses) {
                if (!boss.isActive) {
                    continue;
                }
                boss.action(this,FixedDeltaTime);
            }
        }

        public void petsAction() {
            foreach (var pet in pets) {
                if (!pet.isActive) {
                    continue;
                }
                var logicCount = this.logic_counts[(int)pet.camp];
                for (var i = 0; i < logicCount; i++) {
                    pet.action(this,FixedDeltaTime);
                }
            }
        }

        public void bulletsAction() {
            foreach (var bullet in bullets) {
               bullet.action(this);
            }
        }

        public void produce_pets(int typeID, int count, SheepCamp camp) {
            // 根据阵营 获取 map
            var callInfos = camp == SheepCamp.Red ? this.redCallInfos : this.blueCallInfos;

            // 没有就创建一个 然后加进去
            if (!callInfos.TryGetValue(typeID, out SheepCallInfo sheepCallInfo)) {
                sheepCallInfo = new SheepCallInfo();
                sheepCallInfo.camp = camp;
                sheepCallInfo.type = typeID;
                sheepCallInfo.count = 0;
                sheepCallInfo.frame = 0;
                sheepCallInfo.count_line = 0;

                sheepCallInfo.items = new int[] { };
                sheepCallInfo.pets = new List<SheepCallInfoPet>();
                callInfos[typeID] = sheepCallInfo;
            }

            //  爆炸是什么意思? 用什么用?
            sheepCallInfo.count += count;
            sheepCallInfo.pets.Add(new SheepCallInfoPet() { camp = camp, count = count });
        }

        public void consume(float t) {
            var o = this;

            this.autoTime += t;

            if (this.isAutoCall && this.autoTime > sheepConfig.systemAutomaticTroopsIntervalTime) {
                this.autoTime = 0;
                if (this.pets.Count < sheepConfig.systemLongerAutomaticallyDispatch) {
                    foreach (var e in new SheepCamp[] { SheepCamp.Red, SheepCamp.Blue }) {
                        if (getPetCount(e) < sheepConfig.systemAutomaticallyMaxTroops) {
                            o.produce_pets(sheepConfig.WarmUpID, sheepConfig.systemAutomaticallyTroopsOneNumber, e);
                        }
                    }
                }
            }

            foreach (var t1 in new[] { this.redCallInfos, this.blueCallInfos }) {
                var n = t1 == o.redCallInfos ? SheepCamp.Red : SheepCamp.Blue;

                foreach (var ee in t1.ToArray()) {
                    var o1 = ee.Value;
                    var a = ee.Key;

                    if (o1.count <= 0) {
                        continue;
                    }

                    var c = SheepRoleTypeInfo.getById(a);
                    var formation = SheepRoleFormation.getById(c.formationId);

                    var u = petCounts[(int)n][(int)c.roleType];

                    if (c.roleType == SheepRoleType.XIAO_BING) {
                        if (u > 14500) {
                            continue;
                        }
                    }
                    else if (c.roleType == SheepRoleType.CI_KE && u > 9500) {
                        continue;
                    }

                    o1.frame += 1;

                    // 限制每帧生成的单位
                    if (o1.frame <= formation.frameItemX) {
                        continue;
                    }

                    o1.frame = 0;

                    if (formation.formationType == SheepRoleFormationType.RectangleTidy) {
                        var h = formation.itemNumY;
                        var m = formation.itemY;
                        var d = formation.itemYGapNum;
                        var g = formation.itemYGap;

                        var y = formation.startX + sheepMode.startAddX;
                        var S = n == SheepCamp.Red ? -y : y;

                        var pets = o1.pets;
                        var I = 0;

                        for (; pets.Count > 0 && I < h;) {
                            var T = Math.Floor((double)(I / 2.0));
                            float M = 0;

                            if (h % 2 == 0) {
                                if (I % 2 == 0) {
                                    M = (float)(m * T + m / 2.0 + Math.Floor(T / d + 1) * g);
                                }
                                else {
                                    M = (float)(-m * T - m / 2.0 - Math.Floor(T / d + 1) * g);
                                }
                            }
                            else {
                                if (I % 2 == 0) {
                                    M = (float)(m * Math.Floor(T) + Math.Floor(T / d) * g);
                                }
                                else {
                                    M = (float)(-m * Math.Floor(T + 1) - Math.Floor(T / d) * g);
                                }
                            }

                            var C = pets[0];
                            C.count -= 1;
                            o1.count -= 1;
                            I += 1;

                            var R = new Vector3(S, M, 0f);
                            if (C.booms != null && C.booms.Count > 0) {
                                createPetView(C.camp, a, h, R, C.booms.Pop());
                            }
                            else {
                                createPetView(C.camp, a, h, R, false);
                            }

                            if (C.count <= 0) {
                                pets.RemoveAt(0);
                            }
                        }

                        o1.count_line += 1;
                        if (o1.count_line >= formation.itemNumX) {
                            o1.frame -= formation.itemYGapFrame;
                            o1.count_line = 0;
                        }

                        if (o1.count <= 0) {
                            t1.Remove(a);
                        }
                    }
                    else if (formation.formationType == SheepRoleFormationType.AngleTidy) {
                        var k = 2 * formation.maxAngle - formation.minAngle;
                        double G = Math.Floor((double)(k / formation.startStepAngle));
                        var A = Math.Floor(1 * G);

                        var pets = o1.pets;
                        var b = 0;

                        for (; pets.Count > 0 && b < A;) {
                            var P = pets[0];
                            var w = P.count;

                            for (var D = 0; D < w && b < A; D++) {
                                P.count -= 1;
                                o1.count -= 1;
                                b += 1;

                                var _ = Math.Floor(b / G);
                                var N = formation.startR + sheepMode.startAddR + formation.startStepR * _;

                                var E = (b % 2 == 0 ? 1 : -1) *
                                        (Math.Floor(b % G / 2) * formation.startStepAngle + formation.minAngle);

                                var x = Math.Cos(E * Math.PI / 180) * N;
                                float F = (float)(Math.Sin(E * Math.PI / 180) * N);
                                var X = new Vector3();
                                var L = sheepMode.loongX;
                                X = n == SheepCamp.Red
                                    ? new Vector3((float)(L - x), F, 0)
                                    : new Vector3((float)(x - L), F, 0);
                                if (P.booms != null && P.booms.Count > 0) {
                                    createPetView(P.player, a, (int)G, X, P.booms.Pop());
                                }
                                else {
                                    createPetView(P.camp, a, (int)G, X, false);
                                }
                            }

                            if (P.count <= 0) {
                                pets.RemoveAt(0);
                            }
                        }
                    }
                }
            }
        }

        public Vector3 getPetStartEndPos(int petId, SheepCamp camp) {
            var petStartCount = this.petStartCounts[(int)camp];
            var a = petStartCount.GetValueOrDefault(petId, 0);
            petStartCount[petId] = a + 1;
            var sheepRoleTypeInfo = SheepRoleTypeInfo.getById(petId);
            if (sheepRoleTypeInfo == null) {
                Debug.LogError("SheepMgr.getPetStartEndPos roleId=" + petId + " not found");
            }

            var sheepRoleFormation = SheepRoleFormation.getById(sheepRoleTypeInfo.formationId);
            if (sheepRoleFormation == null) {
                Debug.LogError("SheepMgr.getPetStartEndPos formationId=" + sheepRoleTypeInfo.formationId +
                               " not found");
            }

            var c = sheepRoleFormation.preItemNumY;
            var l = sheepRoleFormation.preItemX;
            var u = sheepRoleFormation.preItemY;
            var h = sheepRoleFormation.preStartX + Math.Floor((double)(a / c)) * l;
            var m = Math.Floor((double)(a % c));
            var d = 0;
            if (c % 2 == 0) {
                if (m % 2 == 0) {
                    d = (int)(u * Math.Floor(m / 2) + u / 2);
                }
                else {
                    d = (int)(-u * Math.Floor(m / 2) - u / 2);
                }
            }
            else {
                if (m % 2 == 0) {
                    d = (int)(u * Math.Floor(m / 2));
                }
                else {
                    d = (int)(-u * Math.Floor(m / 2 + 1));
                }
            }

            if (camp == SheepCamp.Red) {
                h *= -1;
            }

            return new Vector3((float)h, d + RandomFloat(-1, 1), 0);
        }

        public void clearCallPets() {
            redCallInfos.Clear();
            blueCallInfos.Clear();
        }


        // 创建 单位, 下一帧才会生效
        public void createPetView(SheepCamp camp, int roleType, int a, Vector3 f, bool s) {
            if (this.state != SheepRoomState.Run && this.state != SheepRoomState.Start) {
                return;
            }

            var sheepRoleTypeInfo = SheepRoleTypeInfo.getById(roleType);

            var pet = new PetView();

            pet.id = this.getNextPetId();
            pet.isActive = true;
            pet.isDie = false;
            pet.conf = sheepRoleTypeInfo;
            pet.camp = camp;
            pet.roleId = roleType;
            pet.isDie = false;

            pet.isBoom = s; //  这里不能写死

            pet.attacher = new BuffTimeAttacher();

            var ppppp = new Vector3();

            var formation = SheepRoleFormation.getById(sheepRoleTypeInfo.formationId);

            if (formation.formationType == SheepRoleFormationType.AngleRandom) {
                var T = Math.Min((a / (float)formation.angleDensity + formation.baseTimes) * formation.startAngle,
                    formation.maxAngle);
                T = RandomFloat(-T, T);
                T += T > 0 ? formation.minAngle : -formation.minAngle;

                var A = formation.startR + sheepMode.startAddR;
                var H = Math.Cos(T * Math.PI / 180) * A;
                var P = Math.Sin(T * Math.PI / 180) * A;
                var M = sheepMode.loongX;

                if (pet.camp == SheepCamp.Red) {
                    var x = new Vector3((float)(M - H), (float)P, 0);
                    ppppp = x;
                }
                else {
                    var D = new Vector3((float)(H - M), (float)P, 0);
                    ppppp = D;
                }
            }
            else if (formation.formationType == SheepRoleFormationType.RectangleRandom) {
                var F = Math.Min((a / (float)formation.density + formation.baseTimes) * formation.startScope,
                    formation.maxScope);
                var N = RandomFloat(-F, F);
                if (formation.minScope != 0) {
                    N += N > 0 ? formation.minScope : -formation.minScope;
                }

                var W = 0f;
                var E = formation.startX + sheepMode.startAddX;
                W = camp == SheepCamp.Red ? -Math.Abs(E) : Math.Abs(E);
                var O = new Vector3(W, N, 0f);
                ppppp = O;
            }
            else {
                ppppp = f;
            }


            Vector3 p1 = ppppp;
            int x7 = Mathf.FloorToInt(p1.x);
            int y7 = Mathf.FloorToInt(p1.y);


            if (this.state == SheepRoomState.Start) {
                pet.state = SheepRoleState.Start;
                pet.subState = SheepRoleSubState.Start;
                pet.animType = SheepRoleAnimType.Idle;
                pet.animFrame = this.RandomInt(0, 10);
            }
            else if (pet.conf.skillIn != 0) {
                pet.state = SheepRoleState.In;
                pet.subState = SheepRoleSubState.In;
                pet.animType = SheepRoleAnimType.In;
                pet.animFrame = 0;
            }
            else if (pet.conf.startState == SheepRoleState.In) {
                pet.state = pet.conf.startState;
                pet.subState = SheepRoleSubState.In;
                pet.animType = SheepRoleAnimType.In;
                pet.animFrame = 0;
            }
            else if (pet.conf.startState == SheepRoleState.SpinSpurt) {
                pet.state = pet.conf.startState;
                pet.animType = SheepRoleAnimType.Attack;
                pet.animFrame = 0;
            }
            else {
                pet.state = pet.conf.startState;
                pet.subState = SheepRoleSubState.Spurt;

                if (pet.conf.isSpurtAnim) {
                    pet.animType = SheepRoleAnimType.Spurt;
                    pet.animFrame = this.RandomInt(0, 10);
                }
                else {
                    pet.animType = SheepRoleAnimType.Idle;
                    pet.animFrame = this.RandomInt(0, 10);
                }
            }


            pet.frame = 0;
            pet.posBefX = x7;
            pet.posBefY = y7;
            pet.animX = x7;
            pet.animY = y7;
            pet.posX = x7;
            pet.posY = y7;

            if (this.state == SheepRoomState.Start) {
                var m = this.getPetStartEndPos(pet.roleId, pet.camp);

                pet.tarPosX = m.x;
                pet.tarPosY = m.y;
                pet.animY = m.y;
                pet.posBefY = m.y;
                pet.posY = m.y;
            }

            var roleFormation = SheepRoleFormation.getById(pet.conf.formationId);
            float d7 = pet.camp == SheepCamp.Red ? 1 : -1;

            if (roleFormation.formationType == SheepRoleFormationType.RectangleTidy ||
                roleFormation.formationType == SheepRoleFormationType.RectangleRandom) {
                pet.dirX = d7;
                pet.dirY = 0;
            }
            else if (roleFormation.formationType == SheepRoleFormationType.AngleTidy ||
                     roleFormation.formationType == SheepRoleFormationType.AngleRandom) {
                Vector3 g = new Vector3(
                    d7 * sheepMode.loongX - x7,
                    0 - y7,
                    0
                ).normalized;

                pet.dirX = g.x;
                pet.dirY = g.y;
            }

            pet.curHp = pet.conf.hp;
            pet.curAtkBuff = 0;

            // 无视碰撞 (自爆兵这类需要径直穿过人群的单位)
            if (pet.conf.isNotCollide) {
                pet.isNotConn = true;
            }

            if (pet.isBoom) {
                pet.isNotConn = true;
                pet.isBoom = true;
            }
            else {
                pet.isBoom = false;
            }

            foreach (var b1 in this.buffs) {
                foreach (var b2 in b1) {
                    double time = (b2.time - this.gameStartTimerForBuff) / 1e3;

                    int r = b2.count;

                    addGeneralOrderBuff(pet, pet, time, r);
                }
            }

            if (this.state == SheepRoomState.Start && pet.conf.roleType == SheepRoleType.YANG_SHEN) {
                this.god_view_pets.Add(pet);
            }

            this.addPrePet(pet);
        }


        // 生成 子弹 下一帧才会生效
        public void createBullet(BullteCreate tttt) {
            int bulletId = tttt.bulletId;
            PetView view_pet = tttt.view_pet;
            PetView view_tar_pet = tttt.view_tar_pet;
            Info l = tttt.info;

            var n = SheepBullet.getById(bulletId);
            var r = view_pet != null ? view_pet.camp == SheepCamp.Red ? n.startOffsetX : -n.startOffsetX : 0;
            var preBullet = new BulletView();
            preBullet.id = getNextBulletId();
            preBullet.bulletId = bulletId;
            preBullet.roleUid = view_pet != null ? view_pet.id : 0;
            preBullet.roleIndex = view_pet;


            preBullet.camp = view_pet != null ? view_pet.camp : l.camp;
            if (view_tar_pet != null && 0 == view_tar_pet.roleId) {
                preBullet.tarRoleIndex = view_tar_pet;
            }
            else {
                preBullet.tarRoleIndex = null;
            }

            if (n.moveType == (int)SheepBulletMoveType.Fixed) {
                var t = l != null && l.startX != 0 ? l.startX :
                    view_pet != null && view_pet.posX != 0 ? view_pet.posX : 0;
                preBullet.x = t;
                var s = l != null && l.startY != 0 ? l.startY :
                    view_pet != null && view_pet.posY != 0 ? view_pet.posY : 0;
                preBullet.y = s;

                preBullet.startY = n.startOffsetY;
                preBullet.z = 0 + n.startOffsetZ;
                preBullet.dirX = 0;
                preBullet.dirY = 0;
                preBullet.dirZ = 1;
            }
            else if (n.moveType == (int)SheepBulletMoveType.LineDir) {
                preBullet.x = view_pet.posX + r;
                preBullet.y = view_pet.posY + n.startOffsetY;
                preBullet.z = 0 + n.startOffsetZ;
                preBullet.dirX = view_pet.dirX;
                preBullet.dirY = view_pet.dirY;
            }
            else if (n.moveType == (int)SheepBulletMoveType.CurvePosFrame) {
                var t = view_pet != null ? view_pet.posX : l.startX;
                var s = view_pet != null ? view_pet.posY : l.startY;
                var c = view_tar_pet != null ? view_tar_pet.posX : view_pet.tarPosX;
                var f = view_tar_pet != null ? view_tar_pet.posY : view_pet.tarPosY;
                preBullet.x = t + r;
                preBullet.y = s + n.startOffsetY;
                preBullet.z = 0 + n.startOffsetZ;
                preBullet.startX = t + r;
                preBullet.startY = s + n.startOffsetY;
                preBullet.startZ = 0 + n.startOffsetZ;
                preBullet.endX = c;
                preBullet.endY = f;
                preBullet.endZ = 0 + n.endOffsetZ;
                preBullet.dirX = 0;
                preBullet.dirY = 0;
                preBullet.z = 1;
            }
            else if (n.moveType == (int)SheepBulletMoveType.DirAngle) {
                preBullet.x = view_pet.posX + r;
                preBullet.y = view_pet.posY + n.startOffsetY;
                preBullet.z = 0 + n.startOffsetZ;
                preBullet.dirX = l.dirX;
                preBullet.dirY = l.dirY;
                preBullet.dirZ = l.dirZ;
            }
            else if (n.moveType == (int)SheepBulletMoveType.RadiusAngle) {
                preBullet.x = view_pet.posX + r;
                preBullet.y = view_pet.posY + n.startOffsetY;
                preBullet.z = 0 + n.startOffsetZ;
                preBullet.startX = view_pet.posX + r;
                preBullet.startY = view_pet.posY + n.startOffsetY;
                preBullet.startZ = 0 + n.startOffsetZ;
                preBullet.dirX = l.dirX;
                preBullet.dirY = l.dirY;
                preBullet.dirZ = l.dirZ;
                preBullet.angle = l.angle;
            }
            else if (n.moveType == (int)SheepBulletMoveType.LineDirEndPos) {
                preBullet.x = l.startX;
                preBullet.y = l.startY;
                preBullet.z = l.startZ;
                preBullet.startX = l.startX;
                preBullet.startY = l.startY;
                preBullet.startZ = l.startZ;
                preBullet.endX = l.endX;
                preBullet.endY = l.endY;
                preBullet.endZ = l.endZ;
                if (l.dirX != 0 || l.dirY != 0 || l.dirZ != 0) {
                    preBullet.dirX = l.dirX;
                    preBullet.dirY = l.dirY;
                    preBullet.dirZ = l.dirZ;
                }
                else {
                    var t = l.endX - l.startX;
                    var i = l.endY - l.startY;
                    var s = l.endZ - l.startZ;
                    var o = Math.Sqrt(t * t + i * i);
                    preBullet.dirX = (float)(t / o);
                    preBullet.dirY = (float)(i / o);
                    preBullet.dirZ = (float)(s / o);
                }
            }
            else if (n.moveType == (int)SheepBulletMoveType.LinePosFrame) {
                preBullet.x = l.startX;
                preBullet.y = l.startY;
                preBullet.z = l.startZ;
                preBullet.startX = l.startX;
                preBullet.startY = l.startY;
                preBullet.startZ = l.startZ;
                preBullet.endX = l.endX;
                preBullet.endY = l.endY;
                preBullet.endZ = l.endZ;
                var t = l.endX - l.startX;
                var i = l.endY - l.startY;
                var s = l.endZ - l.startZ;
                var o = Math.Sqrt(t * t + i * i);
                preBullet.dirX = (float)(t / o);
                preBullet.dirY = (float)(i / o);
                preBullet.dirZ = (float)(s / o);
            }
            else {
                preBullet.x = view_pet.posX + r;
                preBullet.y = view_pet.posY + n.startOffsetY;
                preBullet.z = 0 + n.startOffsetZ;
                preBullet.dirX = 0;
                preBullet.dirY = 0;
                preBullet.dirZ = 1;
            }

            preBullet.atkVue = view_pet != null ? view_pet.conf.atk : l.atk;
            preBullet.frame = 0;
            addPreBullet(preBullet);
        }


        public void updateSkinPet(PetView ppp, SheepCtl e, SheepMgr t, SheepMgr n, double o) {
            PetView a = ppp;
            PetView i = a;

            if (i.state == SheepRoleState.Merge) {
                return;
            }

            bool isDie = i.isDie;
            int blockIndex = getIndexByXY(i.posX, i.posY);
            var (gridX,gridY) = getXnYn(i.posX, i.posY);
            float curHp = i.curHp;

            if (isDie) {
                return;
            }

            if (curHp <= 0) {
                isDie = true;
                i.isDie = isDie;
                i.state = SheepRoleState.Dead;
            }

            if (isDie) {
                i.state = SheepRoleState.Dead;
                i.subState = SheepRoleSubState.Dead;

                if (i.conf.roleType != SheepRoleType.QI_LIN) {
                    i.animType = SheepRoleAnimType.Dead;
                }

                if (i.conf.deadAnimType != null && i.conf.deadAnimType.Length != 0) {
                    i.animType = (SheepRoleAnimType)arrOn(i.conf.deadAnimType);
                }

                if (i.conf.roleType == SheepRoleType.XIAO_BING) {
                    i.animFrame = RandomInt(0, 10);
                }

                ppp.isDie = true;
                // this.id = 0;
                ppp.attacher.clear();
            }

            if (!isDie) {
                int S = i.conf.detectCollideR;

                for (int y = -S; y <= S; ++y) {
                    for (int v = -S; v <= S; ++v) {
                        e.addFrameBlockCamp(gridX,gridY, ppp.camp);
                    }
                }

                // Vector3 B = new Vector3(i.animX, i.animY, 0);

                // a.position = B;
            }

            if (!isDie) {
                int countNewBuff = n.countNewBuffs[(int)ppp.camp];

                if (countNewBuff != 0) {
                    addGeneralOrderBuff(ppp, i, sheepConfig.buffLastTime, countNewBuff);
                }
            }

            ppp.attacher.updateTimer(o / 1e3);
        }

        public void addGeneralOrderBuff(PetView ppp, PetView e, double t, int n) {
            PetView o = ppp;

            ppp.attacher.addIndependBuff(
                BuffID.GeneralOrder,
                t,
                buff => {
                    int addHp = (int)Math.Floor(
                        o.conf.hp *
                        sheepConfig.buffHpIncreaseRate *
                        n
                    );

                    float addAtk =
                        n *
                        sheepConfig.buffAtkIncreaseRate *
                        100;

                    buff.arg = (
                        addHp: addHp,
                        addAtk: addAtk
                    );

                    var arg = ((int addHp, float addAtk))buff.arg;

                    e.curHp += arg.addHp;
                    e.curAtkBuff += arg.addAtk;
                },
                buff => {
                    var arg = ((int addHp, float addAtk))buff.arg;

                    e.curHp -= arg.addHp;
                    e.curAtkBuff -= arg.addAtk;
                }
            );
        }

        public static long NowMs() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        public int RandomInt(
            int minInclusive,
            int maxExclusive) {
            return logicRandom.Next(
                minInclusive,
                maxExclusive
            );
        }

        public float Random01() {
            return (float)logicRandom.NextDouble();
        }

        public float RandomFloat(
            float minInclusive,
            float maxExclusive) {
            return minInclusive +
                   (maxExclusive - minInclusive) *
                   (float)logicRandom.NextDouble();
        }

        public T arrOn<T>(T[] r) {
            return r[RandomInt(0, r.Length)];
        }


        public (int xn, int yn) getXnYnByIndex(int e) {
            return (
                e % sheepConfig.line_w,
                Mathf.FloorToInt((float)e / sheepConfig.line_w)
            );
        }

        // 根据 空间坐标 获取 格子坐标
        public (int xn, int yn) getXnYn(float x, float y) {
            return (
                Mathf.FloorToInt(x / sheepConfig.d + sheepConfig.h / sheepConfig.d / 2f),
                Mathf.FloorToInt(y / sheepConfig.d + sheepConfig.w / sheepConfig.d / 2f)
            );
        }

        // 根据格子坐标 获取 index
        // 具有边界保护
        public int getIndexByXnYn(int xn, int yn) {
            if (xn < 0) {
                xn = 0;
            }
            else if (xn >= sheepConfig.line_w) {
                xn = sheepConfig.line_w - 1;
            }

            if (yn < 0) {
                yn = 0;
            }
            else if (yn >= sheepConfig.line_h) {
                yn = sheepConfig.line_h - 1;
            }

            return xn * sheepConfig.line_w + yn;
        }

        // 根据 空间坐标 获取 索引 (只是组合方法)
        public int getIndexByXY(float x, float y) {
            (int xn, int yn) i = getXnYn(x, y);
            return getIndexByXnYn(i.xn, i.yn);
        }

        public bool isCanAckByRole(PetView e, PetView t, float i = 1f) {
            //判断单位是否死亡
            bool o = !t.isDie;
            if (!o) {
                return o;
            }

            SheepRoleState l = t.state;
            if (
                t.roleId != 0 &&
                (
                    l == SheepRoleState.In ||
                    l == SheepRoleState.Dead ||
                    l == SheepRoleState.Merge ||
                    l == SheepRoleState.Res ||
                    l == SheepRoleState.Killer
                )
            ) {
                return false;
            }

            // 阵营判断
            SheepCamp r = e.camp;
            SheepCamp a = t.camp;
            if (a == r) {
                return false;
            }

            // 越界判断
            if (a == SheepCamp.Red && t.posX < -sheepConfig.limitSearchBorderX) {
                return false;
            }

            if (a == SheepCamp.Blue && t.posX > sheepConfig.limitSearchBorderX) {
                return false;
            }

            //距离判断
            float f = e.posX;
            float h = e.posY;
            float p = t.posX - f;
            float u = t.posY - h;
            float d = p * p + u * u;
            float g = Mathf.Sqrt(d);

            //攻击范围判断
            return g < e.conf.atkR * i + e.conf.collideR + t.conf.collideR;
        }

        // 是否可以移动?
        public bool isCanMove(PetView petSkin, PetView targetPetSkin) {
            SheepCamp o = targetPetSkin.camp;
            return !(
                o == SheepCamp.Red && targetPetSkin.posX < -sheepConfig.limitSearchBorderX ||
                o == SheepCamp.Blue && targetPetSkin.posX > sheepConfig.limitSearchBorderX ||
                targetPetSkin.isDie ||
                targetPetSkin.camp == petSkin.camp
            );
        }

        // 设置 e 到 t 的方向向量
        public static void dirTar(PetView e, PetView t) {
            float i = e.posX;
            float s = e.posY;
            float o = t.posX - i;
            float l = t.posY - s;
            float r = Mathf.Sqrt(o * o + l * l);
            if (r == 0f) {
                r = 1f;
            }

            float a = o / r;
            float c = l / r;
            e.dirX = a;
            e.dirY = c;
        }

        // 设置 e 到指定 x,y 的方向向量
        public static float[] dirTarByPos(PetView e, float x, float y) {
            float s = x - e.posX;
            float o = y - e.posY;
            float l = Mathf.Sqrt(s * s + o * o);
            if (l == 0f) {
                l = 1f;
            }

            return new[] { s / l, o / l };
        }

        // 返回两点之间的距离
        public static float dis(float x, float y, float x1, float y1) {
            float o = x1 - x;
            float l = y1 - y;
            return Mathf.Sqrt(o * o + l * l);
        }

        // 返回两个单位之间的距离
        public static float disByRole(PetView e, PetView t) {
            float i = e.posX;
            float s = e.posY;
            float o = t.posX - i;
            float l = t.posY - s;
            return Mathf.Sqrt(o * o + l * l);
        }

        // 以 e / t 的概率返回 true
        public bool numToBool(float e, float t = 1000f) {
            return Random01() * t < e;
        }

        // 获取 BOSS
        public Boss getBackBoss(SheepCamp camp) {
            var view_boss_red = bosses[(int)SheepCamp.Red];
            var view_boss_blue = bosses[(int)SheepCamp.Blue];


            if (camp == SheepCamp.Red) {
                return view_boss_blue;
            }
            else {
                return view_boss_red;
            }
        }

        public void moveTar(PetView e, PetView t) {
            var i = FixedDeltaTime;

            // todo 当这两个任意不为 0 的时候 跳过 ? 什么作用?
            if (e.impulseX != 0f || e.impulseY != 0f) {
                return;
            }

            // 时间系数
            i *= 3f;

            // 当前位置
            Vector2 l = new Vector2(e.posX, e.posY);

            // 有目标
            if (t != null) {
                // 如果目标是 boss
                if (t.conf.roleType == SheepRoleType.BOSS) {
                    // 当红方在中线左侧的时候 逼着往中线推进 (不贴边)
                    if (e.camp == SheepCamp.Red && e.posX < 0f) {
                        e.dirX = 1f;
                        e.dirY = 0.02f * Random01() - 0.01f;
                    }
                    // 同理当 蓝方在中线右侧的时候  (不贴边)
                    else if (e.camp == SheepCamp.Blue && e.posX > 0f) {
                        e.dirX = -1f;
                        e.dirY = 0.02f * Random01() - 0.01f;
                    }
                    else {
                        // 否则正常 移动
                        dirTar(e, t);
                    }
                }
                else {
                    // 否则正常移动
                    dirTar(e, t);
                }
            }
            else {
                // 冲锋类
                if (
                    e.state == SheepRoleState.Charge ||
                    e.state == SheepRoleState.SpinSpurt ||
                    e.state == SheepRoleState.ChargePlus
                ) {
                    // 根据阵营 左冲右冲
                    if (e.camp == SheepCamp.Red) {
                        e.dirX = 1f;
                    }
                    else {
                        e.dirX = -1f;
                    }

                    // 纵向没有力
                    e.dirY = 0f;
                }
            }

            // 速度
            float n;

            // 根据状态不同 采取不用的速度
            if (
                e.state == SheepRoleState.Spurt ||
                e.state == SheepRoleState.Charge ||
                e.state == SheepRoleState.SpinSpurt ||
                e.state == SheepRoleState.SpinAtk ||
                e.state == SheepRoleState.ChargePlus
            ) {
                n = e.conf.runSpeed;
            }
            else {
                n = e.conf.walkSpeed;
            }

            // 冰冻: 解冻过程中 移动速度逐渐恢复 (完全冰冻时不会走到这里)
            var freezeScale = e.freezeActionScale;
            if (freezeScale < 1f) {
                n *= freezeScale;
            }

            // 计算不考虑碰撞的情况下 应该移动的向量
            Vector2 r = new Vector2((float)(e.dirX * n * i), (float)(e.dirY * n * i));

            // 获取原始对应的格子
            (int xn, int yn) block = getXnYn(l.x, l.y);
            int xn = block.xn;
            int yn = block.yn;

            // 处于 ChargePlus 状态
            if (e.state == SheepRoleState.ChargePlus) {
                // 强制移动
                Vector2 target = new Vector2(l.x + r.x, l.y + r.y);
                e.logicMove(target.x, target.y);
            }
            else if (e.state == SheepRoleState.Charge || e.state == SheepRoleState.SpinSpurt) {
                // 一样强制移动
                Vector2 target = new Vector2(l.x + r.x, l.y + r.y);
                e.logicMove(target.x, target.y);
            }
            else if (e.isBoom) {
                // 一样强制移动
                Vector2 target = new Vector2(l.x + r.x, l.y + r.y);
                e.logicMove(target.x, target.y);
            }
            else if (e.state == SheepRoleState.SpinAtk) {
                Vector2 target = new Vector2(l.x + r.x, l.y + r.y);

                // 限制在某个区域内?
                if (
                    target.x < sheepConfig.w / 2f &&
                    target.x > -sheepConfig.w / 2f &&
                    target.y < sheepConfig.h / 2f &&
                    target.y > -sheepConfig.h / 2f ||
                    l.x > sheepConfig.w / 2f ||
                    l.x < -sheepConfig.w / 2f ||
                    l.y > sheepConfig.h / 2f ||
                    l.y < -sheepConfig.h / 2f
                ) {
                    e.logicMove(target.x, target.y);
                }
            }
            else {
                Vector2 tCollide = Vector2.zero;

                // 碰撞了多少单位的 计数器
                int collideCount = 0;

                // 碰撞了多少非冲刺单位的 计数器
                int notSpurtCount = 0;

                if (!e.isNotConn) {
                    forfeachBlocksByCollView(e, xn, yn, e.conf.detectCollideR, s => {
                        if (collideCount >= 20) {
                            return;
                        }

                        if (s.isNotConn) {
                            return;
                        }

                        float nX = l.x - s.posX;
                        float rY = l.y - s.posY;

                        // 计算 当前 单位 位置和目标的距离
                        float a = Mathf.Sqrt(nX * nX + rY * rY);

                        // 如果太近了 还不是同一个人 (需要推开? )
                        if (a < e.conf.collideR + s.conf.collideR && e.id != s.id) {
                            // 不是完全重合, 可以计算推开的距离
                            if (a > 0f) {
                                float push = e.conf.collideR + s.conf.collideR - a;
                                tCollide.x += nX * push / (e.conf.collideR + s.conf.collideR);
                                tCollide.y += rY * push / (e.conf.collideR + s.conf.collideR);
                            }
                            else {
                                // 完全重合防止死锁 给予一个随机数
                                tCollide.x += 0.1f * Random01();
                                tCollide.y += 0.1f * Random01();
                            }

                            collideCount++;

                            // 非 冲刺单位计数器
                            if (s.state != SheepRoleState.Spurt) {
                                notSpurtCount++;
                            }
                        }
                    });
                }

                if (collideCount >= 1) {
                    // 按照 碰撞算法 最终要移动到的位置
                    Vector2 movePos = new Vector2(l.x, l.y);

                    // 大体逻辑
                    // 冲刺：
                    //     人少 → 全速
                    //     人多 → 慢挪
                    //
                    // 普通：
                    //     人少 → 慢挪
                    //     人多 → 不动
                    if (e.state == SheepRoleState.Spurt) {
                        // 前面有大于 3 个 "不在冲刺的普通单位" 堵着
                        if (notSpurtCount > 3) {
                            // 限制最大只能移动 碰撞半径的距离
                            if (r.x > e.conf.collideR) {
                                r.x = e.conf.collideR;
                            }
                            else if (r.x < -e.conf.collideR) {
                                r.x = -e.conf.collideR;
                            }

                            if (r.y > e.conf.collideR) {
                                r.y = e.conf.collideR;
                            }
                            else if (r.y < -e.conf.collideR) {
                                r.y = -e.conf.collideR;
                            }

                            // 乘以 碰撞移动缩放系数
                            movePos.x += e.conf.colliderMoveScale * r.x;
                            movePos.y += e.conf.colliderMoveScale * r.y;
                        }
                        else {
                            // 如果没有那么多人 直接无视阻挡.
                            movePos.x += r.x;
                            movePos.y += r.y;
                        }
                    }
                    else {
                        // 普通单位 , 如果小于 colliderNotMoveNum 的人阻挡 采用和上边相同的位移逻辑
                        if (collideCount < e.conf.colliderNotMoveNum) {
                            if (r.x > e.conf.collideR) {
                                r.x = e.conf.collideR;
                            }
                            else if (r.x < -e.conf.collideR) {
                                r.x = -e.conf.collideR;
                            }

                            if (r.y > e.conf.collideR) {
                                r.y = e.conf.collideR;
                            }
                            else if (r.y < -e.conf.collideR) {
                                r.y = -e.conf.collideR;
                            }

                            movePos.x += e.conf.colliderMoveScale * r.x;
                            movePos.y += e.conf.colliderMoveScale * r.y;
                        }
                        else {
                            // 否则 太多人挡着, 不做位移 (卡在原地不动)
                            r.x = 0f;
                            r.y = 0f;
                        }
                    }

                    // todo  聚拢效果?
                    if (e.camp == SheepCamp.Red && e.posX >= 0f) {
                        float centerY = 0f;
                        float targetX = 1200f - e.posX;
                        float targetY = centerY - e.posY;
                        float distance = Mathf.Sqrt(targetX * targetX + targetY * targetY);
                        float dirY = targetY / distance;
                        float dirX = targetX / distance;

                        if (e.posY > 0f && tCollide.y > 0f) {
                            movePos.x -= dirY;
                            movePos.y += dirX;
                        }
                        else if (e.posY < 0f && tCollide.y < 0f) {
                            movePos.x += dirY;
                            movePos.y -= dirX;
                        }
                    }
                    else if (e.camp == SheepCamp.Blue && e.posX <= 0f) {
                        float centerY = 0f;
                        float targetX = -1200f - e.posX;
                        float targetY = centerY - e.posY;
                        float distance = Mathf.Sqrt(targetX * targetX + targetY * targetY);
                        float dirY = targetY / distance;
                        float dirX = targetX / distance;

                        if (e.posY > 0f && tCollide.y > 0f) {
                            movePos.x += dirY;
                            movePos.y -= dirX;
                        }
                        else if (e.posY < 0f && tCollide.y < 0f) {
                            movePos.x -= dirY;
                            movePos.y += dirX;
                        }
                    }

                    // 修正碰撞
                    if (tCollide.x > e.conf.collideR) {
                        tCollide.x = e.conf.collideR;
                    }
                    else if (tCollide.x < -e.conf.collideR) {
                        tCollide.x = -e.conf.collideR;
                    }

                    if (tCollide.y > e.conf.collideR) {
                        tCollide.y = e.conf.collideR;
                    }
                    else if (tCollide.y < -e.conf.collideR) {
                        tCollide.y = -e.conf.collideR;
                    }

                    movePos.x += e.conf.colliderElasticityScale * tCollide.x;
                    movePos.y += e.conf.colliderElasticityScale * tCollide.y;
                    e.logicMove(movePos.x, movePos.y);
                }
                else {
                    // 没有任何碰撞, 直接移动
                    Vector2 target = new Vector2(l.x + r.x, l.y + r.y);
                    e.logicMove(target.x, target.y);
                }
            }
        }

        public void ackTar(PetView e, PetView t) {
            float i = e.conf.atk;

            if (e.curAtkBuff != 0) {
                i = Mathf.Floor(i * (1f + e.curAtkBuff / 100f));
            }

            if (isCanAckByRole(e, t)) {
                hurtByRole(e, t, i);
            }

            if (t.roleId != 0) {
                (int xn, int yn) block = getXnYn(t.posX, t.posY);

                forfeachBlocksByAckView(e.camp, block.xn, block.yn, e.conf.splitN, target => {
                    if (!target.isDie && target.roleId != 0 && target.camp == t.camp && target.id != t.id &&
                        target.curHp > 0) {
                        float o = t.posX - target.posX;
                        float l = t.posY - target.posY;
                        if (Mathf.Sqrt(o * o + l * l) <= t.conf.collideR + target.conf.collideR + e.conf.spiltR) {
                            hurtByRole(e, target, i);
                        }
                    }
                });
            }
        }

        public void ackMe(
            PetView e,
            float t = 1f,
            float i = 1f,
            int s = 10,
            float o = 0f,
            IList<SheepRoleType> l = null
        ) {
            float n = i;
            n *= e.conf.atk;

            if (e.curAtkBuff != 0) {
                n = Mathf.Floor(n * (1f + e.curAtkBuff / 100f));
            }

            if (l == null) {
                l = new SheepRoleType[0];
            }

            (int xn, int yn) block = getXnYn(e.posX, e.posY);
            forfeachBlocksByAckView(e.camp, block.xn, block.yn, s, target => {
                if (!l.Contains(target.conf.roleType) && target.curHp > 0) {
                    float targetX = e.posX - target.posX;
                    float targetY = e.posY - target.posY;
                    float distance = Mathf.Sqrt(targetX * targetX + targetY * targetY);
                    if (distance <= e.conf.collideR + target.conf.collideR + e.conf.spiltR * t) {
                        hurtByRole(e, target, n);
                        if (o != 0f && distance > 0.0001f) {
                            targetX /= distance;
                            targetY /= distance;
                            target.impulseX = -targetX * o;
                            target.impulseY = -targetY * o;
                        }
                    }
                }
            });
        }

        public void hitBackMe(PetView e, float t = 1f, int i = 10, float s = 0f) {
            (int xn, int yn) block = getXnYn(e.posX, e.posY);
            forfeachBlocksByAckView(e.camp, block.xn, block.yn, i, target => {
                if (target.curHp > 0) {
                    float o = e.posX - target.posX;
                    float l = e.posY - target.posY;
                    float n = Mathf.Sqrt(o * o + l * l);
                    if (n > 0.0001f && n <= e.conf.collideR + target.conf.collideR + e.conf.spiltR * t && s != 0f) {
                        o /= n;
                        l /= n;
                        target.impulseX = -o * s;
                        target.impulseY = -l * s;
                    }
                }
            });
        }

        /// 场上是否还有 camp 的敌方单位 (存活且未死亡)
        public bool hasEnemyPet(SheepCamp camp) {
            var enemyCamp = camp == SheepCamp.Red ? SheepCamp.Blue : SheepCamp.Red;

            foreach (var pet in pets) {
                if (pet.camp == enemyCamp && !pet.isDie && pet.curHp > 0) {
                    return true;
                }
            }

            return false;
        }

        /// e 附近 findR 格之内是否有可攻击的敌方单位
        public bool hasEnemyNear(PetView e, int findR) {
            if (findR <= 0) {
                return false;
            }

            (int xn, int yn) block = getXnYn(e.posX, e.posY);

            var found = false;

            forfeachBlocksByAckView(e.camp, block.xn, block.yn, findR, target => {
                if (found || target.isDie || target.curHp <= 0) {
                    return;
                }

                found = true;
            });

            return found;
        }

        /// 随机抽一个 camp 的敌方存活单位的位置 (蓄水池抽样, 不分配内存, 走 logicRandom 保证可复现).
        /// 返回 是否抽到.
        public bool tryPickRandomEnemyPos(SheepCamp camp, out float x, out float y) {
            var enemyCamp = camp == SheepCamp.Red ? SheepCamp.Blue : SheepCamp.Red;

            x = 0f;
            y = 0f;

            var seen = 0;

            foreach (var pet in pets) {
                if (pet.camp != enemyCamp || pet.isDie || pet.curHp <= 0) {
                    continue;
                }

                seen += 1;

                // 蓄水池抽样 (k = 1): 第 seen 个元素以 1/seen 的概率成为当前被抽中的
                if (RandomInt(0, seen) == 0) {
                    x = pet.posX;
                    y = pet.posY;
                }
            }

            return seen > 0;
        }

        /// 天降轰炸: 随机抽一个敌方单位作为落点中心, 在 scatterR 半径内投下 cnt 发炮弹.
        /// 炮弹从 startZ 高度垂直落下, 落地伤害与爆炸特效由 SheepBullet 配置驱动.
        /// 返回 实际投下的炮弹数 (场上没有敌人时为 0, 不浪费弹药)
        public int dropBombs(PetView caster, int bulletId, int cnt, int startZ, int scatterR) {
            if (cnt <= 0) {
                return 0;
            }

            if (!tryPickRandomEnemyPos(caster.camp, out var centerX, out var centerY)) {
                return 0;
            }

            for (var i = 0; i < cnt; i++) {
                // 在以 (centerX, centerY) 为圆心、scatterR 为半径的圆内随机取落点
                var angle = RandomFloat(0f, 2f * Mathf.PI);
                var r = scatterR * Mathf.Sqrt(Random01());
                var landX = centerX + r * Mathf.Cos(angle);
                var landY = centerY + r * Mathf.Sin(angle);

                createBullet(new BullteCreate() {
                    view_pet = caster,
                    bulletId = bulletId,
                    info = new BullteCreate.Info() {
                        startX = landX,
                        startY = landY,
                        startZ = startZ,
                        endX = landX,
                        endY = landY,
                        endZ = 0,
                        // 必须显式给方向, 否则 createBullet 会用 (end - start) 归一化,
                        // 而垂直落下时水平分量为 0 会出现除零.
                        dirX = 0,
                        dirY = 0,
                        dirZ = -1
                    }
                });
            }

            return cnt;
        }

        /// 全体解冻: 清除 camp 我方全部单位的冰冻/解冻状态, 立刻恢复战斗.
        /// 返回 实际解冻的单位数量
        public int unfreezeAlly(SheepCamp camp) {
            var count = 0;

            foreach (var pet in pets) {
                if (pet.camp != camp || pet.isDie) {
                    continue;
                }

                if (!pet.isFrozen && !pet.isThawing) {
                    continue;
                }

                pet.clearFreeze();
                count += 1;
            }

            return count;
        }

        /// 全体技能重置: 让 camp 我方单位重新回到出场 (Spurt) 状态, 从而再放一次自己的出场技能.
        /// caster 是施法者本人, 会被跳过.
        /// 返回 实际重置的单位数量
        public int resetAllySkill(SheepCamp camp, PetView caster) {
            var count = 0;

            foreach (var pet in pets) {
                if (pet.camp != camp) {
                    continue;
                }

                // 跳过施法者自己, 防止自我重置造成循环
                if (pet == caster) {
                    continue;
                }

                if (!canResetSkill(pet)) {
                    continue;
                }

                enterSpurtState(pet);
                count += 1;
            }

            return count;
        }

        /// 判断一个单位能否被"技能重置"影响.
        /// 这里刻意收得很窄, 只重置处于普通战斗状态的单位, 避免打断带有配对状态的技能
        /// (例如 Killer 处决会同时操作目标单位) 而产生跨技能的 bug.
        private static bool canResetSkill(PetView pet) {
            // 死亡 / 将死
            if (pet.isDie || pet.curHp <= 0) {
                return false;
            }

            // 没有出场技能, 没什么可重置
            if (pet.conf.skillSpurt == 0) {
                return false;
            }

            // 被冰冻/解冻中的单位无法行动, 重置只会浪费效果
            if (pet.isFrozen || pet.isThawing) {
                return false;
            }

            // 正在被其它技能锁定
            if (pet.isLock) {
                return false;
            }

            // 只重置普通战斗状态, 不打断正在进行的技能
            if (pet.state != SheepRoleState.Move &&
                pet.state != SheepRoleState.Attack &&
                pet.state != SheepRoleState.Spurt) {
                return false;
            }

            // 出场技能本身就是"全局支援"类的, 跳过, 否则两个支援单位会互相重置
            var skill = SheepSkill.getById(pet.conf.skillSpurt);
            if (skill == null || skill.skillType == SheepSkillType.Support) {
                return false;
            }

            return true;
        }

        /// 把单位放回"出场冲刺"状态. 与 PetLogicStart 切换到冲刺时的处理保持一致.
        public static void enterSpurtState(PetView pet) {
            var skill = SheepSkill.getById(pet.conf.skillSpurt);

            if (skill != null && skill.skillType == SheepSkillType.Charge) {
                pet.state = SheepRoleState.Charge;
                pet.subState = SheepRoleSubState.Spurt;
                pet.animType = SheepRoleAnimType.Spurt;
                return;
            }

            if (skill != null && skill.skillType == SheepSkillType.SpinSpurt) {
                pet.state = SheepRoleState.SpinSpurt;
                pet.animType = SheepRoleAnimType.Attack;
                return;
            }

            pet.state = SheepRoleState.Spurt;
            pet.subState = SheepRoleSubState.Spurt;
            pet.animType = pet.conf.isSpurtAnim ? SheepRoleAnimType.Spurt : SheepRoleAnimType.Idle;
        }

        /// 全局冰冻 (寒冰菇式大招): 冰冻 camp 的敌方 "全部" 单位, 不受距离限制.
        /// camp        施法方阵营 (冰冻的是它的敌方)
        /// freezeFrame 完全冰冻帧数 (期间完全不可动)
        /// thawFrame   解冻帧数 (期间行动能力从 0 线性恢复到 1)
        /// immuneFrame 完全恢复后的免疫帧数 (0 表示可以被下一次大招立刻再冻)
        /// caster      施法者, 为 null 时不结算伤害 (调试热键用)
        /// atkBet      附带伤害倍率 (0 表示纯控制)
        /// 返回 实际被冰冻的单位数量
        public int freezeEnemyAll(
            SheepCamp camp,
            int freezeFrame,
            int thawFrame,
            int immuneFrame,
            PetView caster = null,
            float atkBet = 0f
        ) {
            var enemyCamp = camp == SheepCamp.Red ? SheepCamp.Blue : SheepCamp.Red;

            // 伤害只在有施法者且倍率不为 0 时结算
            var hasDamage = caster != null && atkBet != 0f;
            float atk = 0f;
            if (hasDamage) {
                atk = atkBet * caster.conf.atk;
                if (caster.curAtkBuff != 0f) {
                    atk = Mathf.Floor(atk * (1f + caster.curAtkBuff / 100f));
                }
            }

            int count = 0;

            foreach (var pet in pets) {
                if (pet.camp != enemyCamp) {
                    continue;
                }

                if (!pet.applyFreeze(freezeFrame, thawFrame, immuneFrame)) {
                    continue;
                }

                count += 1;

                if (hasDamage) {
                    hurtByRole(caster, pet, atk);
                }
            }

            return count;
        }

        public static void hurtByRole(PetView e, PetView t, float i) {
            float s = SheepRoleRestraint.getById((int)t.conf.roleType).hitRate[(int)e.conf.roleType];
            int damage = Mathf.Max(1, Mathf.FloorToInt(i * s));
            float o = t.subCurHp(damage);
            if (o > 0 && o <= damage) {
            }
        }

        public static void hurtByBullet(BulletView e, PetView t, float i) {
            float s = SheepRoleRestraint.getById((int)t.conf.roleType).hitRate[(int)e.conf.roleType];
            int damage = Mathf.Max(1, Mathf.FloorToInt(i * s));
            float o = t.subCurHp(damage);
            if (o > 0 && o <= damage) {
            }
        }

        public static bool isCanAckByBullet(BulletView e, PetView petSkin, int i) {
            bool s = !petSkin.isDie;
            if (!s) {
                return s;
            }

            SheepRoleState o = petSkin.state;
            if (
                petSkin.roleId != 0 &&
                (
                    o == SheepRoleState.In ||
                    o == SheepRoleState.Dead ||
                    o == SheepRoleState.Merge ||
                    o == SheepRoleState.Res ||
                    o == SheepRoleState.Killer
                )
            ) {
                return false;
            }

            bool l = petSkin.camp != e.camp;
            if (!l) {
                return l;
            }

            if (e.conf.atkShapeType == SheepBulletAtkShapeType.Ring) {
                float bulletX = e.x;
                float bulletY = e.y;
                float targetX = petSkin.posX - bulletX;
                float targetY = petSkin.posY - bulletY;
                float distanceSqr = targetX * targetX + targetY * targetY;
                float distance = Mathf.Sqrt(distanceSqr);
                return distance < e.conf.maxRadiuses[i] && distance > e.conf.minRadiuses[i];
            }

            {
                float bulletX = e.x;
                float bulletY = e.y;
                float targetX = petSkin.posX - bulletX;
                float targetY = petSkin.posY - bulletY;
                float distanceSqr = targetX * targetX + targetY * targetY;
                return Mathf.Sqrt(distanceSqr) < e.conf.atkR;
            }
        }


        public FindTarResult findTar(PetView petSkin, int findR = 0) {
            float i = petSkin.posX;
            float o = petSkin.posY;
            (int xn, int yn) block = getXnYn(i, o);
            int xn = block.xn;
            int yn = block.yn;
            PetView r = null;
            PetView a = null;
            float c = 0f;

            if (findR == 0) {
                findR = petSkin.conf.findR;
            }

            forNearBlocksByAckView(petSkin, xn, yn, findR, targetPetView => {
                if (!targetPetView.isDie && targetPetView.camp != petSkin.camp) {
                    if (isCanAckByRole(petSkin, targetPetView)) {
                        r = targetPetView;
                        return true;
                    }

                    if (petSkin.conf.isFindMoveTar && a == null && isCanMove(petSkin, targetPetView)) {
                        float tx = targetPetView.posX - petSkin.posX;
                        float ty = targetPetView.posY - petSkin.posY;
                        c = tx * tx + ty * ty;
                        a = targetPetView;
                    }
                    else if (petSkin.conf.isFindMoveTar && a != null && isCanMove(petSkin, targetPetView)) {
                        float tx = targetPetView.posX - petSkin.posX;
                        float ty = targetPetView.posY - petSkin.posY;
                        float distance = tx * tx + ty * ty;
                        if (distance < c) {
                            c = distance;
                            a = targetPetView;
                        }
                    }

                    return false;
                }

                return false;
            });

            if (r != null) {
                petSkin.tarPosX = r.posX;
                petSkin.tarPosY = r.posY;
                return new FindTarResult() { atkTar = r };
            }

            PetView backBoss = getBackBoss(petSkin.camp);
            if (isCanAckByRole(petSkin, backBoss)) {
                petSkin.tarPosX = backBoss.posX;
                petSkin.tarPosY = backBoss.posY;
                return new FindTarResult() { atkTar = backBoss };
            }

            if (a != null) {
                return new FindTarResult() { moveTar = a };
            }

            if (petSkin.state == SheepRoleState.Spurt && petSkin.conf.skillSpurt == 0) {
                PetView t = null;
                findNearBlocksByCollisionView(petSkin, xn, yn, petSkin.conf.findR, target => {
                    if (target.state == SheepRoleState.Move) {
                        float s = target.posX - petSkin.posX;
                        float targetY = target.posY - petSkin.posY;
                        float distance = s * s + targetY * targetY;
                        float radius = target.conf.collideR + petSkin.conf.collideR;
                        if (distance < radius * radius * 0.25f) {
                            t = target;
                            return true;
                        }
                    }

                    return false;
                });

                if (t != null) {
                    return new FindTarResult() { moveTar = t };
                }
            }

            if (
                petSkin.state != SheepRoleState.Spurt ||
                petSkin.camp == SheepCamp.Red && petSkin.posX > petSkin.conf.runEndX ||
                petSkin.camp == SheepCamp.Blue && petSkin.posX < petSkin.conf.runEndX
            ) {
                return new FindTarResult() { moveBoss = backBoss };
            }

            return new FindTarResult();
        }

        public PetView findNearAck(PetView petSkin) {
            float t = petSkin.posX;
            float i = petSkin.posY;
            (int xn, int yn) block = getXnYn(t, i);
            PetView l = null;

            findNearBlocksByAckView(petSkin, block.xn, block.yn, petSkin.conf.findR, target => {
                if (!target.isDie && target.camp != petSkin.camp &&
                    isCanAckByRole(petSkin, target)) {
                    l = target;
                    return true;
                }

                return false;
            });

            if (l != null) {
                return l;
            }

            if (l == null) {
                PetView target = getBackBoss(petSkin.camp);
                if (isCanAckByRole(petSkin, target)) {
                    l = target;
                }
            }

            return l;
        }

        public PetView findFarAck(PetView e, int findR) {
            float posX = e.posX;
            float posY = e.posY;
            (int xn, int yn) block = getXnYn(posX, posY);
            PetView n = null;

            findFarBlocksByAckView(e, block.xn, block.yn, findR, target => {
                n = target;
                return true;
            });

            if (n == null) {
                PetView t = getBackBoss(e.camp);
                if (isCanAckByRole(e, t)) {
                    n = t;
                }
            }

            return n;
        }

        public PetView findRandomAck(PetView e, int findR) {
            float i = e.posX;
            float s = e.posY;
            (int xn, int yn) block = getXnYn(i, s);
            PetView n = null;

            findRandomBlocksByAckView(e, block.xn, block.yn, findR, target => {
                n = target;
                return true;
            });

            if (n == null) {
                PetView t = getBackBoss(e.camp);
                if (isCanAckByRole(e, t)) {
                    n = t;
                }
            }

            return n;
        }

        public static int getAtkRank(PetView petView, PetView targetPetView) {
            if (petView.conf.findAtkSort != null) {
                for (int i = 0; i < petView.conf.findAtkSort.Length; i++) {
                    if (petView.conf.findAtkSort[i] == (int)targetPetView.conf.roleType) {
                        return i;
                    }
                }
            }

            return 100;
        }

        public PetView findSortAck(PetView petView, int targetPetView) {
            float posX = petView.posX;
            float posY = petView.posY;
            (int xn, int yn) block = getXnYn(posX, posY);
            PetView n = null;
            int r = 100;
            int a = 0;

            if (petView.conf.findAtkSort != null) {
                a = petView.conf.findAtkSort[0];
            }

            findNearBlocksByAckView(petView, block.xn, block.yn, targetPetView, t => {
                if (!isCanAckByRole(petView, t)) {
                    return false;
                }

                if (n == null) {
                    n = t;
                    r = getAtkRank(petView, t);
                    return false;
                }

                if (t.roleId == (int)a) {
                    n = t;
                    return true;
                }

                PetView target = t;
                int s = getAtkRank(petView, t);
                if (s < r) {
                    n = target;
                    r = s;
                }

                return false;
            });

            if (n == null) {
                PetView t = getBackBoss(petView.camp);
                if (isCanAckByRole(petView, t)) {
                    n = t;
                }
            }

            return n;
        }

        public PetView findSortAck1(PetView petSkin, int findR) {
            float i = petSkin.posX;
            float s = petSkin.posY;
            (int xn, int yn) block = getXnYn(i, s);
            PetView n = null;
            int r = 100;
            int a = 0;

            if (petSkin.conf.findAtkSort != null) {
                a = petSkin.conf.findAtkSort[0];
            }

            findNearBlocksByAckView(petSkin, block.xn, block.yn, findR, t => {
                if (n == null) {
                    n = t;
                    r = getAtkRank(petSkin, t);
                    return false;
                }

                if (t.roleId == (int)a) {
                    n = t;
                    return true;
                }

                PetView target = t;
                int rank = getAtkRank(petSkin, t);
                if (rank < r) {
                    n = target;
                    r = rank;
                }

                return false;
            });

            if (n == null) {
                Boss backBoss = getBackBoss(petSkin.camp);
                if (isCanAckByRole(petSkin, backBoss)) {
                    n = backBoss;
                }
            }

            return n;
        }

        public void foreachFront(PetView e, Action<PetView> t, int i = 0, float o = 30f) {
            float l = e.posX;
            float n = e.posY;
            (int xn, int yn) block = getXnYn(l, n);
            float c = e.tarPosX - l;
            float f = e.tarPosY - n;
            float h = Mathf.Sqrt(c * c + f * f);

            if (h > 0f) {
                c /= h;
                f /= h;
                if (i == 0) {
                    i = e.conf.findR;
                }
            }
            else {
                c = e.camp == SheepCamp.Red ? 1f : -1f;
                f = 0f;
                if (i == 0) {
                    i = e.conf.findR;
                }
            }

            float p = Mathf.Cos(o * Mathf.PI / 180f);
            PetView u = null;
            float d = float.PositiveInfinity;

            forNearBlocksByAckView(e, block.xn, block.yn, i, target => {
                if (!target.isDie && target.camp != e.camp && isCanAckByRole(e, target)) {
                    float targetX = target.posX - l;
                    float targetY = target.posY - n;
                    float distanceSqr = targetX * targetX + targetY * targetY;
                    float distance = Mathf.Sqrt(targetX * targetX + targetY * targetY);
                    if (distance != 0f) {
                        if ((targetX * c + targetY * f) / distance > p && distanceSqr < d) {
                            d = distanceSqr;
                            u = target;
                            t(u);
                        }
                    }

                    return false;
                }

                return false;
            });
        }

        public void forfeachBlocksByAckView(SheepCamp camp, int xn, int yn, int splitN, Action<PetView> callback) {
            // 寻找敌方阵营
            var enemyCamp = camp == SheepCamp.Red ? SheepCamp.Blue : SheepCamp.Red;

            for (int n = -splitN; n <= splitN; n++) {
                for (int r = -splitN; r <= splitN; r++) {
                    var sheepCell = gridMap.getCell(xn + n, yn + r);
                    if (sheepCell == null) {
                        continue;
                    }

                    sheepCell.forEachPet(enemyCamp, (p) => {
                        callback(p);
                        return false;
                    });
                }
            }
        }

        public void forfeachBlocksByCollView(PetView petSkin, int xn, int yn, int splitN, Action<PetView> callback) {
            var camp = petSkin.camp;
            var collideId = petSkin.conf.collideGroup;

            for (int n = -splitN; n <= splitN; n++) {
                for (int r = -splitN; r <= splitN; r++) {
                    var sheepCell = gridMap.getCell(xn + n, yn + r);
                    if (sheepCell == null) {
                        continue;
                    }

                    sheepCell.forEachPet(camp, collideId, (p) => {
                        callback(p);
                        return false;
                    });
                }
            }
        }

        public bool forNearBlocksByAckView(PetView e, int t, int i, int o, Func<PetView, bool> callback) {
            // 寻找敌方阵营
            var enemyCamp = e.camp == SheepCamp.Red ? SheepCamp.Blue : SheepCamp.Red;
            int n = 0;

            Func<int, int, bool> r = (blockX, blockY) => {
                var sheepCell = gridMap.getCellSafe(blockX, blockY);
                sheepCell.forEachPet(enemyCamp, callback);
                return false;
            };

            for (int ring = 0; ring <= o; ring++) {
                if (ring != 0) {
                    Vector2Int topLeft = new Vector2Int(t - n, i + n);
                    Vector2Int topRight = new Vector2Int(t + n, i + n);
                    Vector2Int bottomRight = new Vector2Int(t + n, i - n);
                    Vector2Int bottomLeft = new Vector2Int(t - n, i - n);

                    if (Random01() < 0.5f) {
                        for (int x = topLeft.x; x < topRight.x; x++) {
                            if (r(x, topLeft.y)) return true;
                        }

                        for (int y = topRight.y; y > bottomRight.y; y--) {
                            if (r(topRight.x, y)) return true;
                        }

                        for (int x = bottomRight.x; x > bottomLeft.x; x--) {
                            if (r(x, bottomRight.y)) return true;
                        }

                        for (int y = bottomLeft.y; y < topLeft.y; y++) {
                            if (r(bottomLeft.x, y)) return true;
                        }
                    }
                    else {
                        for (int x = topRight.x; x > topLeft.x; x--) {
                            if (r(x, topLeft.y)) return true;
                        }

                        for (int y = topLeft.y; y > bottomLeft.y; y--) {
                            if (r(bottomLeft.x, y)) return true;
                        }

                        for (int x = bottomLeft.x; x < bottomRight.x; x++) {
                            if (r(x, bottomRight.y)) return true;
                        }

                        for (int y = bottomRight.y; y < topRight.y; y++) {
                            if (r(topRight.x, y)) return true;
                        }
                    }
                }
                else if (r(t, i)) {
                    return true;
                }

                n += 1;
            }

            return false;
        }

        public bool findFarBlocksByAckView(PetView petSkin, int xn, int yn, int findR, Func<PetView, bool> callback) {
            SheepCamp camp = petSkin.camp;
            camp = camp == SheepCamp.Red ? SheepCamp.Blue : SheepCamp.Red;

            Func<int, int, bool> n = (blockX, blockY) => {
                var sheepCell = gridMap.getCellSafe(blockX, blockY);

                return sheepCell.petCounts[(int)camp] != 0;
            };

            Func<int, int, bool> a = (blockX, blockY) => {
                var sheepCell = gridMap.getCellSafe(blockX, blockY);

                return sheepCell.forEachPet(camp, callback);
            };

            for (int ring = findR; ring > 0; ring--) {
                Vector2Int topLeft = new Vector2Int(xn - ring, yn + ring);
                Vector2Int topRight = new Vector2Int(xn + ring, yn + ring);
                Vector2Int bottomRight = new Vector2Int(xn + ring, yn - ring);
                Vector2Int bottomLeft = new Vector2Int(xn - ring, yn - ring);
                HashSet<Vector2Int> c = new HashSet<Vector2Int>();

                for (int x = topLeft.x; x < topRight.x; x++) {
                    if (n(x, topLeft.y)) c.Add(new Vector2Int(x, topLeft.y));
                }

                for (int y = topRight.y; y > bottomRight.y; y--) {
                    if (n(topRight.x, y)) c.Add(new Vector2Int(topRight.x, y));
                }

                for (int x = bottomRight.x; x > bottomLeft.x; x--) {
                    if (n(x, bottomRight.y)) c.Add(new Vector2Int(x, bottomRight.y));
                }

                for (int y = bottomLeft.y; y < topLeft.y; y++) {
                    if (n(bottomLeft.x, y)) c.Add(new Vector2Int(bottomLeft.x, y));
                }

                while (c.Count != 0) {
                    List<Vector2Int> points = new List<Vector2Int>();
                    foreach (Vector2Int point in c) {
                        points.Add(point);
                    }

                    int randomIndex = RandomInt(0, c.Count);
                    Vector2Int pointToCheck = points[randomIndex];
                    if (a(pointToCheck.x, pointToCheck.y)) {
                        return true;
                    }

                    c.Remove(pointToCheck);
                }
            }

            return n(xn, yn) && a(xn, yn);
        }

        public bool findRandomBlocksByAckView(PetView e, int t, int i, int findR, Func<PetView, bool> callback) {
            SheepCamp camp = e.camp;
            camp = camp == SheepCamp.Red ? SheepCamp.Blue : SheepCamp.Red;

            Func<int, int, bool> n = (blockX, blockY) => {
                var sheepCell = gridMap.getCellSafe(blockX, blockY);

                return sheepCell.petCounts[(int)camp] != 0;
            };

            Func<int, int, bool> a = (blockX, blockY) => {
                var sheepCell = gridMap.getCellSafe(blockX, blockY);

                return sheepCell.forEachPet(camp, callback);
            };

            List<int> c = new List<int>();
            for (int ring = 0; ring <= findR; ring++) {
                c.Add(ring);
            }

            c.Sort((left, right) => Random01() < 0.5f ? -1 : 1);

            for (int ringIndex = 0; ringIndex <= findR; ringIndex++) {
                int ring = c[ringIndex];
                Vector2Int topLeft = new Vector2Int(t - ring, i + ring);
                Vector2Int topRight = new Vector2Int(t + ring, i + ring);
                Vector2Int bottomRight = new Vector2Int(t + ring, i - ring);
                Vector2Int bottomLeft = new Vector2Int(t - ring, i - ring);
                List<Vector2Int> h = new List<Vector2Int>();

                for (int x = topLeft.x; x < topRight.x; x++) {
                    if (n(x, topLeft.y)) h.Add(new Vector2Int(x, topLeft.y));
                }

                for (int y = topRight.y; y > bottomRight.y; y--) {
                    if (n(topRight.x, y)) h.Add(new Vector2Int(topRight.x, y));
                }

                for (int x = bottomRight.x; x > bottomLeft.x; x--) {
                    if (n(x, bottomRight.y)) h.Add(new Vector2Int(x, bottomRight.y));
                }

                for (int y = bottomLeft.y; y < topLeft.y; y++) {
                    if (n(bottomLeft.x, y)) h.Add(new Vector2Int(bottomLeft.x, y));
                }

                h.Sort((left, right) => Random01() < 0.5f ? -1 : 1);
                while (h.Count != 0) {
                    int lastIndex = h.Count - 1;
                    Vector2Int point = h[lastIndex];
                    h.RemoveAt(lastIndex);
                    if (a(point.x, point.y)) {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool findNearBlocksByAckView(PetView e, int xn, int yn, int o, Func<PetView, bool> callback) {
            // 寻找敌方阵营
            var enemyCamp = e.camp == SheepCamp.Red ? SheepCamp.Blue : SheepCamp.Red;

            Func<int, int, bool> forEachPetByCell = (blockX, blockY) => {
                var sheepCell = gridMap.getCellSafe(blockX, blockY);
                return sheepCell.forEachPet(enemyCamp, callback);
            };

            int n = 0;

            for (int ring = 0; ring <= o; ring++) {
                if (ring != 0) {
                    Vector2Int topLeft = new Vector2Int(xn - n, yn + n);
                    Vector2Int topRight = new Vector2Int(xn + n, yn + n);
                    Vector2Int bottomRight = new Vector2Int(xn + n, yn - n);
                    Vector2Int bottomLeft = new Vector2Int(xn - n, yn - n);

                    if (Random01() < 0.5f) {
                        for (int x = topLeft.x; x < topRight.x; x++) {
                            if (forEachPetByCell(x, topLeft.y)) {
                                return true;
                            }
                        }

                        for (int y = topRight.y; y > bottomRight.y; y--) {
                            if (forEachPetByCell(topRight.x, y)) {
                                return true;
                            }
                        }

                        for (int x = bottomRight.x; x > bottomLeft.x; x--) {
                            if (forEachPetByCell(x, bottomRight.y)) {
                                return true;
                            }
                        }

                        for (int y = bottomLeft.y; y < topLeft.y; y++) {
                            if (forEachPetByCell(bottomLeft.x, y)) {
                                return true;
                            }
                        }
                    }
                    else {
                        for (int x = topRight.x; x > topLeft.x; x--) {
                            if (forEachPetByCell(x, topLeft.y)) {
                                return true;
                            }
                        }

                        for (int y = topLeft.y; y > bottomLeft.y; y--) {
                            if (forEachPetByCell(bottomLeft.x, y)) {
                                return true;
                            }
                        }

                        for (int x = bottomLeft.x; x < bottomRight.x; x++) {
                            if (forEachPetByCell(x, bottomRight.y)) {
                                return true;
                            }
                        }

                        for (int y = bottomRight.y; y < topRight.y; y++) {
                            if (forEachPetByCell(topRight.x, y)) {
                                return true;
                            }
                        }
                    }
                }
                else if (forEachPetByCell(xn, yn)) {
                    return true;
                }

                n += 1;
            }

            return false;
        }

        public bool findNearBlocksByCollisionView(PetView e, int xn, int yn, int o, Func<PetView, bool> callback) {
            // 寻找乙方阵营 和 碰撞 id 相同的 
            var camp = e.camp;
            var collideId = e.conf.collideGroup;

            Func<int, int, bool> forEachPetByCell = (blockX, blockY) => {
                var sheepCell = gridMap.getCellSafe(blockX, blockY);
                return sheepCell.forEachPet(camp, collideId, callback);
            };

            int n = 0;

            for (int ring = 0; ring <= o; ring++) {
                if (ring != 0) {
                    Vector2Int topLeft = new Vector2Int(xn - n, yn + n);
                    Vector2Int topRight = new Vector2Int(xn + n, yn + n);
                    Vector2Int bottomRight = new Vector2Int(xn + n, yn - n);
                    Vector2Int bottomLeft = new Vector2Int(xn - n, yn - n);

                    if (Random01() < 0.5f) {
                        for (int x = topLeft.x; x < topRight.x; x++) {
                            if (forEachPetByCell(x, topLeft.y)) {
                                return true;
                            }
                        }

                        for (int y = topRight.y; y > bottomRight.y; y--) {
                            if (forEachPetByCell(topRight.x, y)) {
                                return true;
                            }
                        }

                        for (int x = bottomRight.x; x > bottomLeft.x; x--) {
                            if (forEachPetByCell(x, bottomRight.y)) {
                                return true;
                            }
                        }

                        for (int y = bottomLeft.y; y < topLeft.y; y++) {
                            if (forEachPetByCell(bottomLeft.x, y)) {
                                return true;
                            }
                        }
                    }
                    else {
                        for (int x = topRight.x; x > topLeft.x; x--) {
                            if (forEachPetByCell(x, topLeft.y)) {
                                return true;
                            }
                        }

                        for (int y = topLeft.y; y > bottomLeft.y; y--) {
                            if (forEachPetByCell(bottomLeft.x, y)) {
                                return true;
                            }
                        }

                        for (int x = bottomLeft.x; x < bottomRight.x; x++) {
                            if (forEachPetByCell(x, bottomRight.y)) {
                                return true;
                            }
                        }

                        for (int y = bottomRight.y; y < topRight.y; y++) {
                            if (forEachPetByCell(topRight.x, y)) {
                                return true;
                            }
                        }
                    }
                }
                else if (forEachPetByCell(xn, yn)) {
                    return true;
                }

                n += 1;
            }

            return false;
        }
    }
}