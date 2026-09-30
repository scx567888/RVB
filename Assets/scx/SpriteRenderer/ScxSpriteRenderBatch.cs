using System;
using System.Collections.Generic;
using UnityEngine;

namespace scx.SpriteRenderer {
    /// ScxSpriteRenderBatch.
    public sealed class ScxSpriteRenderBatch {
        // 四边形的基础网格信息
        private static readonly Vector3[] BASE_NORMALS = { new(0, 0, -1), new(0, 0, -1), new(0, 0, -1), new(0, 0, -1) };

        private static readonly Vector2[] BASE_UVS = {
            new(0, 0), // 0 左下
            new(1, 0), // 1 右下
            new(0, 1), // 2 左上
            new(1, 1) // 3 右上
        };

        private static readonly int[] BASE_INDICES = { 0, 3, 1, 3, 0, 2 };

        private readonly int capacity; // 容量

        private readonly Vector3[] positions; // 整个网格的 顶点 数据
        private readonly Vector3[] normals; // 整个网格的 法线 数据
        private readonly Vector2[] uvs; // 整个网格的 UV 数据
        private readonly int[] indices; // 整个网格的 索引 数据
        private readonly Color32[] colors; // 整个网格的 颜色 数据

        private readonly GameObject node; // 持有节点
        private readonly Mesh mesh; // 整个网格
        private readonly MeshRenderer meshRenderer; // 网格渲染器
        private readonly MeshFilter meshFilter; // 网格渲染器 (Filter)

        private readonly Stack<int> freeIndex; // 空闲的 索引
        
        private readonly float[] data; // 传递给 shader 的 data, 一个单元 长度为 8
        private readonly GraphicsBuffer dataBuffer; // 关联的 GPU buffer

        public ScxSpriteRenderBatch(int capacity, GameObject parentNode) {
            this.capacity = capacity;

            // 我们都是 四边形 小图 
            this.positions = new Vector3[capacity * 4];
            this.normals = new Vector3[capacity * 4];
            this.uvs = new Vector2[capacity * 4];
            this.indices = new int[capacity * 6];
            this.colors = new Color32[capacity * 4];

            // 初始化网格数据
            for (var i = 0; i < this.capacity; i++) {
                // 我们忽略填充 this.positions 以便 在视觉上默认隐藏所有单位

                var vertexOffset = i * 4;
                var indexOffset = i * 6;

                // 填充法线
                Array.Copy(BASE_NORMALS, 0, this.normals, vertexOffset, 4);
                // 填充 UV
                Array.Copy(BASE_UVS, 0, this.uvs, vertexOffset, 4);
                // 填充 索引 (索引需要计算偏移)
                this.indices[indexOffset + 0] = BASE_INDICES[0] + vertexOffset;
                this.indices[indexOffset + 1] = BASE_INDICES[1] + vertexOffset;
                this.indices[indexOffset + 2] = BASE_INDICES[2] + vertexOffset;
                this.indices[indexOffset + 3] = BASE_INDICES[3] + vertexOffset;
                this.indices[indexOffset + 4] = BASE_INDICES[4] + vertexOffset;
                this.indices[indexOffset + 5] = BASE_INDICES[5] + vertexOffset;

                // 填充颜色 (默认全用白色)
                this.colors[vertexOffset + 0] = Color.white;
                this.colors[vertexOffset + 1] = Color.white;
                this.colors[vertexOffset + 2] = Color.white;
                this.colors[vertexOffset + 3] = Color.white;
            }


            // 创建容器节点, 同时绑定到 父节点上.
            this.node = new GameObject("ScxSpriteRenderBatch");
            this.node.transform.SetParent(parentNode.transform, false);

            // 创建网格
            this.mesh = new Mesh {
                indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
            };

            this.mesh.MarkDynamic();
            this.mesh.vertices = positions;
            this.mesh.normals = normals;
            this.mesh.uv = uvs;
            this.mesh.triangles = indices;
            this.mesh.colors32 = colors;

            // 创建 MeshRenderer 和 MeshFilter
            this.meshRenderer = this.node.AddComponent<MeshRenderer>();
            this.meshFilter = this.node.AddComponent<MeshFilter>();
            this.meshFilter.sharedMesh = this.mesh;

            this.freeIndex = new Stack<int>(capacity);
            for (var i = 0; i < capacity; i++) {
                this.freeIndex.Push(i);
            }

            // 每个 Unit 固定 8 个 float
            this.data = new float[capacity * 8];

            // GPU Buffer
            this.dataBuffer = new GraphicsBuffer(
                GraphicsBuffer.Target.Structured,
                capacity * 8, // float 总数
                sizeof(float) // 每个元素 4 字节
            );

            // 上传初始数据
            this.dataBuffer.SetData(this.data);

            // 绑定到当前 Batch 的 Renderer
            var propertyBlock = new MaterialPropertyBlock();
            propertyBlock.SetBuffer("_UnitData", this.dataBuffer);
            this.meshRenderer.SetPropertyBlock(propertyBlock);
        }

        // ********************* GameObject 相关 ***********************

        public void setLayer(int layer) {
            this.node.layer = layer;
        }

        public void destroy() {
            // 销毁 unitDataBuffer
            this.dataBuffer.Dispose();
            // 销毁 GPU buffer (否则会导致内存泄露)
            UnityEngine.Object.Destroy(this.mesh);
            // 销毁 Node
            UnityEngine.Object.Destroy(this.node);
        }

        // ********************* free 相关 ***********************

        public int allocate() {
            return this.freeIndex.Pop();
        }

        public void release(int index) {
            // 重置这个 unit 的 data.
            Array.Clear(this.data, index * 8, 8);
            // 释放 index
            this.freeIndex.Push(index);
        }

        public bool hasFree() {
            return this.freeIndex.Count > 0;
        }

        public bool allFree() {
            return this.freeIndex.Count == this.capacity;
        }

        /// 更新 UVs
        public void setUVs(int index, Vector2 uv0, Vector2 uv1, Vector2 uv2, Vector2 uv3) {
            // 计算 Unit 在 uvs 数组中的起始位置
            var startIndex = index * 4;
            this.uvs[startIndex + 0] = uv0;
            this.uvs[startIndex + 1] = uv1;
            this.uvs[startIndex + 2] = uv2;
            this.uvs[startIndex + 3] = uv3;
        }

        /// 更新 Positions
        public void setPositions(int index, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
            // 计算 Unit 在 positions 数组中的起始位置
            var startIndex = index * 4;
            this.positions[startIndex + 0] = p0;
            this.positions[startIndex + 1] = p1;
            this.positions[startIndex + 2] = p2;
            this.positions[startIndex + 3] = p3;
        }

        /// 更新 颜色
        public void setColor(int index, Color32 color) {
            // 计算 Unit 在 colors 数组中的起始位置
            var startIndex = index * 4;
            this.colors[startIndex + 0] = color;
            this.colors[startIndex + 1] = color;
            this.colors[startIndex + 2] = color;
            this.colors[startIndex + 3] = color;
        }
        
        /// 更新 data (data 长度必须为 8)
        public void setData(int index, float[] data) {
            // 计算 Unit 在 unitData 数组中的起始位置
            var startIndex = index * 8;
            // 这里暂时不做检查 假设 data 是 8 长度数组.
            this.data[startIndex + 0] = data[0];
            this.data[startIndex + 1] = data[1];
            this.data[startIndex + 2] = data[2];
            this.data[startIndex + 3] = data[3];
            this.data[startIndex + 4] = data[4];
            this.data[startIndex + 5] = data[5];
            this.data[startIndex + 6] = data[6];
            this.data[startIndex + 7] = data[7];
        }

        /// 更新材质
        public void setMaterial(Material material) {
            this.meshRenderer.sharedMaterial = material;
        }

        /// 更新 网格
        public void update() {
            // 更新网格 (索引和法线无需更新)
            mesh.SetVertices(positions);
            mesh.SetUVs(0, uvs);
            mesh.SetColors(colors);

            // 更新包围盒
            mesh.RecalculateBounds();
            
            // 更新 unitDataBuffer
            dataBuffer.SetData(this.data);
        }
    }
}