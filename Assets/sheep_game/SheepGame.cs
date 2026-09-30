using System.Collections.Generic;
using scx.SpriteRenderer;
using sheep_game.utils;
using sheep;
using UnityEngine;

namespace sheep_game {
    public class SheepGame : MonoBehaviour {
        // ********************* 逻辑帧相关 *********************

        // 逻辑帧率
        [Range(1, 240)] public int tickRate = 30;

        // 逻辑帧率累加器
        private double tickAccumulator = 0;

        // ********************* 资源相关 *********************

        // 渲染器资源
        public Texture2D texture;
        public TextAsset json;
        public Material mainMaterial;

        // ********************* 渲染器相关 *********************

        private ScxSpriteRenderer scxSpriteRenderer;

        // ********************* 渲染插值相关 *********************

        public bool useLerp = true;

        // ********************* 主逻辑相关 *********************

        private SheepWorld sheepWorld;

        // *******************  测试  ****************
        public GameObject boss;
        public GameObject boss1;

        void Start() {
            // 创建渲染器
            var loadRoleResult = SheepSpriteAtlasLoader.loadRole(texture, json.text);
            this.scxSpriteRenderer = new ScxSpriteRenderer(
                loadRoleResult.spriteAtlas,
                100,
                mainMaterial,
                1000
            );
            this.scxSpriteRenderer.setParent(this.gameObject);

            // 初始化主逻辑
            sheepWorld = new SheepWorld();
        }

        void Update() {
            UpdateTest();
            tick();
        }

        // 执行逻辑帧
        public void tick() {
            var results = new List<HashSet<Pet>>();

            // 计算 tick 帧 时长
            double tickInterval = 1.0 / tickRate;

            // 累加 tick 帧 计数
            tickAccumulator += Time.deltaTime;

            // 调度逻辑帧
            while (tickAccumulator >= tickInterval) {
                // 记录状态 用于插值
                foreach (var pet in sheepWorld.pets) {
                    pet.renderHandle.lastX = pet.x;
                    pet.renderHandle.lastY = pet.y;
                }

                // 执行 sheepWorld.tick()
                var result = sheepWorld.tick();
                results.Add(result);
                tickAccumulator -= tickInterval;
            }

            // 处理渲染
            foreach (var result in results) {
                foreach (var pet in result) {
                    pet.renderHandle?.scxSpriteRenderUnit.destroy();
                }
            }

            // 计算插值
            float alpha = (float)(tickAccumulator / tickInterval);

            // 渲染
            render(alpha);
        }

        // 执行渲染
        void render(float alpha) {
            var pets = sheepWorld.pets;

            foreach (var pet in pets) {
                if (pet.renderHandle == null) {
                    pet.renderHandle = new PetRenderHandle() {
                        scxSpriteRenderUnit = scxSpriteRenderer.createUnit(),
                    };
                    pet.renderHandle.scxSpriteRenderUnit.setRotationFromEuler(45, 0, 0);
                    pet.renderHandle.scxSpriteRenderUnit.setVisible(true);
                    pet.renderHandle.lastX = pet.x; // 防止插值瞬移
                    pet.renderHandle.lastY = pet.y; // 防止插值瞬移
                }

                renderPet(pet, alpha);
            }

            // 更新渲染器
            scxSpriteRenderer.update();
        }

        public void renderPet(Pet pet, float alpha) {
            Vector3 renderPosition;
            // 判断是否启用线性插值
            if (useLerp&&pet.moveIntent.moveMode!=PetMoveMode.TELEPORT) {
                renderPosition = Vector3.Lerp(
                    new Vector3(pet.renderHandle.lastX, 0, pet.renderHandle.lastY),
                    new Vector3(pet.x, 0, pet.y),
                    alpha
                );
            }
            else {
                renderPosition = new Vector3(pet.x, 0, pet.y);
            }

            pet.renderHandle.scxSpriteRenderUnit.setPosition(renderPosition.x, renderPosition.y, renderPosition.z);
            pet.renderHandle.scxSpriteRenderUnit.setFrame(pet.frame % 10);
            if (pet.flash) {
                // 在 shader 中约定 第一位 不为 0 闪烁.
                pet.renderHandle.scxSpriteRenderUnit.setData(new float[] { 1, 0, 0, 0, 0, 0, 0, 0 });
            }
        }

        // *************************** 测试用 *********************************


        private void UpdateTest() {
            // 通过空格创建的不闪烁
            if (Input.GetKeyDown(KeyCode.Space)) {
                for (int i = 0; i < 500; i++) {
                    sheepWorld.addPrePet(new Pet() {
                        id = sheepWorld.getNextPetId(),
                        moveIntent = new PetMoveIntent() {
                            moveSpeed = sheepWorld.randomFloat(0.25f, 0.5f)
                        },
                        collideIntent = new PetCollideIntent() {
                            // collideRadius = 0.5f,
                            // collideMoveScale = 1,
                            // collideElasticityScale = 1.3f / 4,
                            // collideNotMoveNum = 500
                        },
                        x = sheepWorld.randomFloat(-50f, 50f),
                        y = sheepWorld.randomFloat(-50f, 50f),
                        flash = false
                    });
                }
            }
            
            // 通过 F 创建的让他 闪烁
            if (Input.GetKeyDown(KeyCode.F)) {
                for (int i = 0; i < 500; i++) {
                    sheepWorld.addPrePet(new Pet() {
                        id = sheepWorld.getNextPetId(),
                        moveIntent = new PetMoveIntent() {
                            moveSpeed = sheepWorld.randomFloat(0.5f, 1f)
                        },
                        collideIntent = new PetCollideIntent() {
                            // collideRadius = 0.5f,
                            // collideMoveScale = 1,
                            // collideElasticityScale = 1.3f / 4,
                            // collideNotMoveNum = 500
                        },
                        x = sheepWorld.randomFloat(-50f, 50f),
                        y = sheepWorld.randomFloat(-50f, 50f),
                        flash = true
                    });
                }
            }

            if (Input.GetKeyDown(KeyCode.R)) {
                int i = 0;
                foreach (var pet in sheepWorld.pets) {
                    if (i >= 500) { // 一次最多删除 500 个单位
                        break;
                    }

                    sheepWorld.addDelPet(pet);
                    i++;
                }
            }


            sheepWorld.bossX = boss.transform.position.x;
            sheepWorld.bossY = boss.transform.position.z;

            sheepWorld.boss1X = boss1.transform.position.x;
            sheepWorld.boss1Y = boss1.transform.position.z;
        }

        void OnGUI() {
            // 计算区域
            var w = Screen.width;
            var h = Screen.height;

            // 设置显示样式
            var position = new Rect(100, 100, w, h * 2f / 100);

            // 设置样式
            var style = new GUIStyle {
                alignment = TextAnchor.UpperLeft,
                fontSize = h * 2 / 50,
                normal = {
                    textColor = Color.green
                }
            };

            // 绘制在屏幕左上角
            GUI.Label(position, "pet 数量 : " + sheepWorld.pets.Count, style);
        }
    }
}