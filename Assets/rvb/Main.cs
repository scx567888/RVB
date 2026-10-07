using System;
using System.Collections.Generic;
using rvb.scripts;
using scx.SpriteRenderer;
using scx.TestSpriteRenderer;
using sheep_game.utils;
using UnityEngine;

namespace rvb {
    // 小兵渲染配置
    [Serializable]
    public class PetRenderConfig {
        public Texture2D texture;
        public TextAsset json;
        public SheepCamp camp;
        public int animId;
    }

    // 字段渲染配置
    [Serializable]
    public class BulletRenderConfig {
        public Texture2D texture;
        public TextAsset json;
        public SheepCamp camp;
        public int animId;
    }

    public class Main : MonoBehaviour {
        // 小兵渲染数据
        public List<PetRenderConfig> petRenderConfigs;

        // 子弹渲染数据
        public List<BulletRenderConfig> bulletRenderConfigs;

        // 主材质
        public Material mainMaterial;

        // 动画帧数解析器
        private SheepAnimFrameCountResolver animFrameCountResolver = new();
        
        private Dictionary<int,int> animFrameCountResolver1 = new();

        [Header("Logic")] 
        [SerializeField] private float logicFPS = 30f;
        [SerializeField] private int maxLogicStepsPerUnityFrame = 4;
        [SerializeField] private float logicToWorldScale = 0.01f;
        [SerializeField] private float logicHeightToWorldScale = 0.01f;
        [SerializeField] private int fallbackLogicalAnimationFrames = 30;

        [Header("Freeze")]
        // 完全冰冻时的颜色 (shader 会把顶点色乘到贴图上)
        [SerializeField] private Color freezeColor = new Color(0.36f, 0.67f, 1f, 1f);

        // 冰冻大招 (按 C / V 键直接触发) 的帧数. 30 逻辑帧 = 1 秒
        // 默认 60 + 60 = 全场冻 4 秒 (前 2 秒完全不可动, 后 2 秒逐渐解冻)
        [SerializeField] private int freezeUltFreezeFrame = 60;
        [SerializeField] private int freezeUltThawFrame = 60;

        // 完全恢复后的免疫帧数, 0 表示可以被下一次大招立刻再冻
        [SerializeField] private int freezeUltImmuneFrame = 0;

        // 冰法师 兵种 id (一次性全场冰冻单位)
        [SerializeField] private int iceMageRoleId = 31;

        // 技能兵种 id (与 SheepRoleTypeInfos 里的配置对应)
        private const int ROLE_PAO_JI_SHOU = 32;    // 炮击手: 天降轰炸
        private const int ROLE_ZI_BAO_BING = 33;    // 自爆兵: 无视碰撞冲到最前方自爆
        private const int ROLE_JIE_DONG = 34;       // 解冻祭司: 我方全体解冻
        private const int ROLE_ZHAN_GU_SHOU = 35;   // 战鼓手: 我方攻速翻倍
        private const int ROLE_HAO_LING_GUAN = 36;  // 号令官: 我方全体技能重置

        // 每次按键生成几个单位
        [SerializeField] private int skillUnitSpawnCount = 2;

        // 按照 [阵营][角色类型] 存储
        private Dictionary<int, ScxSpriteRenderer>[] petSpriteRenderers;

        // 按照 [子弹类型] 存储
        private Dictionary<int,ScxSpriteRenderer> bulletSpriteRenderers;

        private string[] spriteNames;
        private SheepMgr sheepMgr;
        private SheepCtl sheepCtl = new SheepCtl();

        private float logicAccumulator;
        private LoadRoleResult loadRoleResult;
        public BlockRender blockRender;

        private void Start() {
            sheepCtl.addFrameBlockCampCallback = (gridx, gridy, c) => blockRender.setColor(gridx,gridy, c);
            this.petSpriteRenderers = new[] {
                new Dictionary<int, ScxSpriteRenderer>(),
                new Dictionary<int, ScxSpriteRenderer>()
            };

            this.bulletSpriteRenderers = new Dictionary<int, ScxSpriteRenderer>();


            foreach (var petRenderConfig in petRenderConfigs) {
                var loadRoleResult =
                    SheepSpriteAtlasLoader.loadRole(petRenderConfig.texture, petRenderConfig.json.text);
                var scxSpriteRenderer = new ScxSpriteRenderer(
                    loadRoleResult.spriteAtlas,
                    100,
                    mainMaterial,
                    2000
                );

                foreach (var keyValuePair in loadRoleResult.animFrame) {
                    var k = keyValuePair.Key;
                    var v = keyValuePair.Value;
                    animFrameCountResolver.setAnimationFrameCount(petRenderConfig.camp, petRenderConfig.animId,
                        (SheepRoleAnimType)k, v);
                }

                scxSpriteRenderer.setParent(gameObject);
                this.petSpriteRenderers[(int)petRenderConfig.camp][(int)petRenderConfig.animId] = scxSpriteRenderer;
            }
            
            foreach (var bulletRenderConfig in bulletRenderConfigs) {
                var spriteAtlas =
                    SheepSpriteAtlasLoader.loadBullet(bulletRenderConfig.texture, bulletRenderConfig.json.text);
                
                var scxSpriteRenderer = new ScxSpriteRenderer(
                    spriteAtlas,
                    100,
                    mainMaterial,
                    2000
                );

                animFrameCountResolver1[bulletRenderConfig.animId] = scxSpriteRenderer.getSpriteNames().Length;
                
                scxSpriteRenderer.setParent(gameObject);

                this.bulletSpriteRenderers[bulletRenderConfig.animId] = scxSpriteRenderer;
            }


            sheepMgr = new SheepMgr(SheepConfigs.sheepConfig, animFrameCountResolver, sheepCtl);

            logicAccumulator = 0f;

            sheepMgr.gameIndex++;
            sheepMgr.setState(SheepRoomState.Start);
            sheepMgr.onGameStart();
            sheepMgr.game_clear();
        }


        public void StartBattle() {
            // 这里先跳过倒计时，直接进入战斗状态。
            sheepMgr.setState(SheepRoomState.Run);
            sheepMgr.onGameRun();
        }

        private void Update() {
            UpdateHotkeys();

            if (sheepMgr.state == SheepRoomState.Start || sheepMgr.state == SheepRoomState.Run) {
                RunLogicFrames();
            }

            // Unity 每个显示帧提交一次渲染。
            foreach (var p1 in petSpriteRenderers) {
                foreach (var scxSpriteRenderer in p1) {
                    scxSpriteRenderer.Value.update();
                }
            }
            
            foreach (var bulletSpriteRenderer in bulletSpriteRenderers) {
                bulletSpriteRenderer.Value.update();
            }
        }

        private void RunLogicFrames() {
            float safeFps = Mathf.Max(1f, logicFPS);
            float logicStepSeconds = 1f / safeFps;
            float logicStepMilliseconds = logicStepSeconds * 1000f;

            logicAccumulator += Time.deltaTime;

            var list = new List<(HashSet<PetView> del_pets, HashSet<BulletView> del_bullets)>();

            int stepCount = 0;
            while (logicAccumulator >= logicStepSeconds && stepCount < maxLogicStepsPerUnityFrame) {
                var gameUpdate = sheepMgr.game_update(sheepCtl, logicStepMilliseconds);
                list.Add(gameUpdate);
                SyncBossMarker(0);
                SyncBossMarker(1);

                logicAccumulator -= logicStepSeconds;
                stepCount++;
            }

            // 这一显示帧没有产生逻辑步时，不清理，否则画面会闪烁。
            if (stepCount == 0) {
                return;
            }

            foreach (var valueTuple in list) {
                foreach (var valueTupleDelPet in valueTuple.del_pets) {
                    valueTupleDelPet.renderUnit?.destroy();
                }

                foreach (var valueTupleDelBullet in valueTuple.del_bullets) {
                    valueTupleDelBullet.renderUnit?.destroy();
                }
            }

            foreach (var sheepMgrPet in sheepMgr.pets) {
                SyncRoleView(sheepMgrPet);
            }
            
            foreach (var sheepMgrPet in sheepMgr.bullets) {
                SyncBulletView(sheepMgrPet);
            }

            RecycleMissingRoleRenderers();
            RecycleMissingBulletRenderers();
        }

        private void SyncBossMarker(int poolIndex) {
            PetView bossView = sheepMgr.bosses[poolIndex];
            if (bossView == null || !bossView.isActive) {
                return;
            }

            // SyncRoleView(bossView);
        }

        private void SyncRoleView(PetView view) {
            var renderUnit = view.renderUnit;
            if (renderUnit == null) {
                renderUnit = petSpriteRenderers[(int)view.camp][(int)view.conf.animId].createUnit();
                view.renderUnit = renderUnit;
                renderUnit.setScale(view.conf.scale, view.conf.scale, 1f);
                renderUnit.setRotationFromEuler(45, 0, 0);

                var initialFrame = ResolveRoleSpriteFrame(view);
                renderUnit.setFrame(initialFrame);
                renderUnit.setVisible(true);
            }

            float worldX = view.animX * logicToWorldScale;
            float worldY = view.animZ * logicHeightToWorldScale;
            float worldZ = view.animY * logicToWorldScale;

            renderUnit.setVisible(true);
            renderUnit.setPosition(worldX, worldY, worldZ);

            var frameIndex = ResolveRoleSpriteFrame(view);
            renderUnit.setFrame(frameIndex);

            ApplyFreezeTint(view, renderUnit);
        }

        // 冰冻: 按冰冻进度上色. 完全冰冻时最蓝, 随着解冻逐渐恢复为白色.
        private void ApplyFreezeTint(PetView view, ScxSpriteRenderUnit renderUnit) {
            float ratio = view.freezeViewRatio;

            if (ratio <= 0f) {
                // 只在上一帧染过色时才需要还原, 避免每帧给所有单位写颜色
                if (view.freezeTinted) {
                    renderUnit.setColor((Color32)Color.white);
                    view.freezeTinted = false;
                }

                return;
            }

            renderUnit.setColor((Color32)Color.Lerp(Color.white, freezeColor, ratio));
            view.freezeTinted = true;
        }


        private void SyncBulletView(BulletView view) {
            var renderUnit = view.renderUnit;
            if (renderUnit == null) {
                renderUnit = bulletSpriteRenderers[(int)view.conf.animId].createUnit();
                view.renderUnit = renderUnit;
                renderUnit.setScale(view.conf.scale, view.conf.scale, 1f);
                renderUnit.setRotationFromEuler(45, 0, 0);

                var initialFrame = ResolveBulletSpriteFrame(view);
                renderUnit.setFrame(initialFrame);
                renderUnit.setVisible(true);
            }

            float worldX = view.x * logicToWorldScale;
            float worldY = view.z * logicHeightToWorldScale;
            float worldZ = view.y * logicToWorldScale;

            renderUnit.setVisible(true);
            renderUnit.setPosition(worldX, worldY, worldZ);

            var frameIndex = ResolveBulletSpriteFrame(view);
            renderUnit.setFrame(frameIndex);
            
            renderUnit.setRotation(CalcBulletRotation(view));
            
        }
        
        private Quaternion CalcBulletRotation(BulletView view)
        {
            return Quaternion.Euler(
                45f,
                0f,
                Mathf.Atan2(view.dirZ, view.dirX) * Mathf.Rad2Deg
            );
        }

        private string ResolveRoleSpriteFrame(PetView view) {
            var i = animFrameCountResolver.resolve(view.camp, view.conf.animId, view.animType);

            int localFrame = PositiveModulo(view.animFrame, i);
            return ((int)(view.animType)) + "-" + localFrame;
        }
        
        private string ResolveBulletSpriteFrame(BulletView view) {
            
            var i = animFrameCountResolver1[view.conf.animId];

            int localFrame = PositiveModulo(view.frame, i);
            return ""+localFrame;
        }

        private int ResolveLogicalAnimationFrameCount(PetView view) {
            // 这是逻辑状态完成所使用的动画长度，不是 Scx 图集总帧数。
            return Mathf.Max(1, fallbackLogicalAnimationFrames);
        }

        private int ClampSpriteIndex(int index) {
            if (spriteNames == null || spriteNames.Length == 0) {
                return 0;
            }

            return PositiveModulo(index, spriteNames.Length);
        }

        private static int PositiveModulo(int value, int modulo) {
            if (modulo <= 0) {
                return 0;
            }

            int result = value % modulo;
            return result < 0 ? result + modulo : result;
        }

        private void RecycleMissingRoleRenderers() {
            // staleRoleSlots.Clear();

            // foreach (KeyValuePair<string, ScxSpriteRenderUnit> pair in roleRenderers) {
            // if (!seenRoleSlots.Contains(pair.Key)) {
            // staleRoleSlots.Add(pair.Key);
            // }
            // }

            // foreach (var slot in staleRoleSlots) {
            // var renderPet = roleRenderers[slot];
            // roleRenderers.Remove(slot);
            // renderPet.destroy();
            // }
        }

        private void RecycleMissingBulletRenderers() {
            // staleBulletIds.Clear();
            //
            // foreach (KeyValuePair<int, ScxSpriteRenderUnit> pair in bulletRenderers) {
            //     if (!seenBulletIds.Contains(pair.Key)) {
            //         staleBulletIds.Add(pair.Key);
            //     }
            // }
            //
            // foreach (int id in staleBulletIds) {
            //     var renderPet = bulletRenderers[id];
            //     bulletRenderers.Remove(id);
            //     renderPet.destroy();
            // }
        }

        private void UpdateHotkeys() {
            if (Input.GetKeyDown(KeyCode.Space)) {
                StartBattle();
            }

            if (Input.GetKeyDown(KeyCode.R)) {
                SpawnArmy(SheepCamp.Red, 22, 10);
            }

            if (Input.GetKeyDown(KeyCode.B)) {
                SpawnArmy(SheepCamp.Blue, 22, 10);
            }

            // F / G: 生成冰法师 (出场即释放全场冰冻, 放完即消耗)
            if (Input.GetKeyDown(KeyCode.F)) {
                SpawnArmy(SheepCamp.Red, iceMageRoleId, 10);
            }

            if (Input.GetKeyDown(KeyCode.G)) {
                SpawnArmy(SheepCamp.Blue, iceMageRoleId, 10);
            }

            // C / V: 不出兵, 直接触发红 / 蓝方的全场冰冻大招 (效果演示)
            if (Input.GetKeyDown(KeyCode.C)) {
                FreezeEnemyAll(SheepCamp.Red);
            }

            if (Input.GetKeyDown(KeyCode.V)) {
                FreezeEnemyAll(SheepCamp.Blue);
            }

            // 数字键 1~5: 生成技能兵种. 不按 Shift = 红方, 按住 Shift = 蓝方
            var skillCamp = IsShiftDown() ? SheepCamp.Blue : SheepCamp.Red;

            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                SpawnArmy(skillCamp, ROLE_PAO_JI_SHOU, skillUnitSpawnCount);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2)) {
                SpawnArmy(skillCamp, ROLE_ZI_BAO_BING, skillUnitSpawnCount);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3)) {
                SpawnArmy(skillCamp, ROLE_JIE_DONG, skillUnitSpawnCount);
            }

            if (Input.GetKeyDown(KeyCode.Alpha4)) {
                SpawnArmy(skillCamp, ROLE_ZHAN_GU_SHOU, skillUnitSpawnCount);
            }

            if (Input.GetKeyDown(KeyCode.Alpha5)) {
                SpawnArmy(skillCamp, ROLE_HAO_LING_GUAN, skillUnitSpawnCount);
            }

            // if (Input.GetKeyDown(KeyCode.H) && highlightMaterial != null) {
            // scxSpriteRenderer.setMaterialTemplate(highlightMaterial);
            // }

            // if (Input.GetKeyDown(KeyCode.M) && mainMaterial != null) {
            // scxSpriteRenderer.setMaterialTemplate(mainMaterial);
            // }
        }

        private static bool IsShiftDown() {
            return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        }

        private void FreezeEnemyAll(SheepCamp camp) {
            if (sheepMgr == null) {
                return;
            }

            sheepMgr.freezeEnemyAll(
                camp,
                freezeUltFreezeFrame,
                freezeUltThawFrame,
                freezeUltImmuneFrame
            );
        }

        private void SpawnArmy(SheepCamp camp, int roleId, int count) {
            if (sheepMgr == null || count <= 0) {
                return;
            }

            sheepMgr.produce_pets(roleId, count, camp);
        }
    }
}