using System.Collections.Generic;
using System.Threading.Tasks;
using scx.SpriteRenderer;
using sheep_game.utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace rvb
{
    public class RVBTest3 : MonoBehaviour
    {
        // 贴图
        public Texture2D texture;

        // json
        public TextAsset json;

        // 主材质
        public Material mainMaterial;

        // 高亮材质
        public Material highlightMaterial;

        // 渲染器
        private ScxSpriteRenderer scxSpriteRenderer;

        // Pet 列表
        private List<Pet> pets;

        private string[] spriteNames;

        // 动画播放帧率
        [SerializeField] private float animationFPS = 30f;

        // 动画计时器
        private float animationTimer = 0f;

        [SerializeField] private bool enableRotate = false;

        [SerializeField] private float rotateSpeed = 1f;

        [SerializeField] private int targetPetCount = 5000;

        [SerializeField] private int maxPetCount = 50000;

        private int lastTargetPetCount = -1;

        void Start()
        {
            var loadRoleResult = SheepSpriteAtlasLoader.loadBullet(texture, json.text);
            this.scxSpriteRenderer = new ScxSpriteRenderer(loadRoleResult, 100, mainMaterial, 5000);
            this.spriteNames = this.scxSpriteRenderer.getSpriteNames();

            this.scxSpriteRenderer.setParent(this.gameObject);

            this.pets = new List<Pet>();

            SetPetCount(targetPetCount);
        }

        // 计数器
        private int time = 0;

        void Update()
        {
            UpdatePetCount();

            UpdateRotate();

            // 测试更换材质
            if (time == 500)
            {
                this.scxSpriteRenderer.setMaterialTemplate(highlightMaterial);
            }

            if (time == 1000)
            {
                this.scxSpriteRenderer.setMaterialTemplate(mainMaterial);
            }

            time++;

            // 按指定 FPS 播放动画
            UpdateAnimationByFps(animationFPS);


            this.scxSpriteRenderer.update();
        }

        private void UpdateAnimationByFps(float fps)
        {
            if (fps <= 0f)
            {
                return;
            }

            animationTimer += Time.deltaTime;

            float frameInterval = 1f / fps;

            if (animationTimer < frameInterval)
            {
                return;
            }

            int step = Mathf.FloorToInt(animationTimer / frameInterval);
            animationTimer -= step * frameInterval;

            // 多核并行执行方式
            Parallel.For(0, pets.Count, i =>
            {
                var pet = pets[i];

                pet.frameIndex += step;

                int index = pet.frameIndex % spriteNames.Length;
                pet.renderUnit.setFrame(index);
            });

            // 传统方式
            // foreach (var pet in this.pets) {
            //
            //     pet.frameIndex += step;
            //
            //     int index = pet.frameIndex % spriteNames.Length;
            //     pet.renderUnit.setFrame(index);
            // }
        }

        private void UpdateRotate()
        {
            if (!enableRotate)
            {
                return;
            }

            var euler = transform.eulerAngles;
            euler.y += rotateSpeed * Time.deltaTime;
            transform.eulerAngles = euler;
        }

        public void SetPetCount(int count)
        {
            if (scxSpriteRenderer == null || spriteNames == null || spriteNames.Length == 0)
            {
                return;
            }

            count = Mathf.Clamp(count, 0, maxPetCount);

            // 数量增加：向 ScxSpriteRenderer 申请新的 unit
            while (pets.Count < count)
            {
                AddOnePet();
            }

            // 数量减少：把 unit 还给 ScxSpriteRenderer
            while (pets.Count > count)
            {
                RemoveLastPet();
            }

            targetPetCount = count;
            lastTargetPetCount = count;
        }

        private void AddOnePet()
        {
            var spriteRenderUnit = this.scxSpriteRenderer.createUnit();

            spriteRenderUnit.setVisible(true);
            // spriteRenderUnit.setPosition(
            //     Random.Range(-50f, 50f),
            //     Random.Range(-50f, 50f),
            //     Random.Range(-50f, 50f)
            // );

            spriteRenderUnit.setPosition(
                Random.Range(-50f, 50f),
                0,
                Random.Range(-50f, 50f)
            );

            spriteRenderUnit.setFrame(this.spriteNames[0]);

            var pet = new Pet(
                spriteRenderUnit,
                Random.Range(0, this.spriteNames.Length)
            );

            pets.Add(pet);
        }

        private void RemoveLastPet()
        {
            int lastIndex = pets.Count - 1;
            var pet = pets[lastIndex];

            // 先从自己的列表移除
            pets.RemoveAt(lastIndex);

            // 再还给 ScxSpriteRenderer
            pet.destroy();
        }

        private void UpdatePetCount()
        {
            if (targetPetCount != lastTargetPetCount)
            {
                SetPetCount(targetPetCount);
            }
        }
    }
}