using UnityEngine;

namespace scx.SpriteRenderer {
    /// ScxSpriteRenderUnit
    public sealed class ScxSpriteRenderUnit {
        private ScxSpriteRenderer spriteRenderer;
        private ScxSpriteRenderBatch renderBatch;
        public readonly int batchID;
        public readonly int index;

        private ScxSpriteRenderData sprite;
        private Vector3 position;
        private Quaternion rotation;
        private Vector3 scale;
        private Color32 color;
        private bool visible;

        private Vector3 p0;
        private Vector3 p1;
        private Vector3 p2;
        private Vector3 p3;

        public ScxSpriteRenderUnit(
            ScxSpriteRenderer spriteRenderer,
            ScxSpriteRenderBatch renderBatch,
            int batchID,
            int index
        ) {
            this.spriteRenderer = spriteRenderer;
            this.renderBatch = renderBatch;
            this.batchID = batchID;
            this.index = index;

            // 默认初始化第一个 
            this.sprite = spriteRenderer.getSpriteByIndex(0);
            this.position = new Vector3(0, 0, 0);
            this.rotation = new Quaternion(0, 0, 0, 1);
            this.scale = new Vector3(1, 1, 1);
            this.color = Color.white;
            this.visible = false;
            // 因为 ScxSpriteRenderUnit 会复用 index, 这里 重置 uvs
            this.renderBatch.setUVs(this.index, this.sprite.uv0, this.sprite.uv1, this.sprite.uv2, this.sprite.uv3);
            // 同上 这里重置 color
            this.renderBatch.setColor(this.index, this.color);
        }

        // UV
        public void setFrame(string name) {
            this.sprite = this.spriteRenderer.getSpriteByName(name);
            this.renderBatch.setUVs(this.index, sprite.uv0, sprite.uv1, sprite.uv2, sprite.uv3);
            this.updateUnitVertices();
        }

        // UV
        public void setFrame(int index) {
            this.sprite = this.spriteRenderer.getSpriteByIndex(index);
            this.renderBatch.setUVs(this.index, sprite.uv0, sprite.uv1, sprite.uv2, sprite.uv3);
            this.updateUnitVertices();
        }

        // 位置
        public void setPosition(float x, float y, float z) {
            this.position.Set(x, y, z);
            this.updateUnitVertices();
        }

        public Vector3 getPosition() {
            return this.position;
        }

        public void translate(float dx, float dy, float dz) {
            this.position.x += dx;
            this.position.y += dy;
            this.position.z += dz;
            this.updateUnitVertices();
        }

        // 旋转
        public void setRotation(float x, float y, float z, float w) {
            this.rotation.Set(x, y, z, w);
            this.updateUnitVertices();
        }
        
        public void setRotation(Quaternion rotation)
        {
            this.rotation = rotation;
            this.updateUnitVertices();
        }

        public void setRotationFromEuler(float x, float y, float z) {
            this.rotation = Quaternion.Euler(x, y, z);
            this.updateUnitVertices();
        }

        public Quaternion getRotation() {
            return rotation;
        }

        public void rotate(float dx, float dy, float dz, float dw) {
            this.rotation *= new Quaternion(dx, dy, dz, dw);
            this.updateUnitVertices();
        }

        public void rotateFromEuler(float dx, float dy, float dz) {
            this.rotation *= Quaternion.Euler(dx, dy, dz);
            this.updateUnitVertices();
        }

        // 缩放
        public void setScale(float x, float y, float z) {
            this.scale.Set(x, y, z);
            this.updateUnitVertices();
        }

        public Vector3 getScale() {
            return this.scale;
        }

        // 颜色
        public void setColor(Color32 color) {
            this.color = color;
            this.renderBatch.setColor(this.index, color);
        }

        // 颜色
        public Color32 getColor() {
            return this.color;
        }
        
        // Data (data 长度必须为 8)
        public void setData(float[] data) {
            this.renderBatch.setData(this.index, data);
        }

        // 可见性
        public void setVisible(bool visible) {
            // 如果和之前一样 跳过
            if (this.visible == visible) {
                return;
            }

            this.visible = visible;

            if (this.visible) {
                this.updateUnitVertices();
            }
            else {
                // 通过将单元的所有顶点塌缩到 0 点(0, 0, 0), 使其在视觉上隐藏/移除
                this.renderBatch.setPositions(index, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero);
            }
        }

        public bool getVisible() {
            return visible;
        }

        // 销毁
        public void destroy() {
            this.spriteRenderer.destroyUnit(this);
            // 置空 防止后续外部调用
            this.spriteRenderer = null;
            this.renderBatch = null;
        }

        private void updateUnitVertices() {
            if (!this.visible) {
                return;
            }

            var qx = rotation.x;
            var qy = rotation.y;
            var qz = rotation.z;
            var qw = rotation.w;

            var x2 = qx + qx;
            var y2 = qy + qy;
            var z2 = qz + qz;

            var xx = qx * x2;
            var xy = qx * y2;
            var xz = qx * z2;
            var yy = qy * y2;
            var yz = qy * z2;
            var zz = qz * z2;
            var wx = qw * x2;
            var wy = qw * y2;
            var wz = qw * z2;

            var sx = scale.x;
            var sy = scale.y;
            var sz = scale.z;


            var m00 = (1 - (yy + zz)) * sx;
            var m01 = (xy + wz) * sx;
            var m02 = (xz - wy) * sx;

            var m04 = (xy - wz) * sy;
            var m05 = (1 - (xx + zz)) * sy;
            var m06 = (yz + wx) * sy;

            var m08 = (xz + wy) * sz;
            var m09 = (yz - wx) * sz;
            var m10 = (1 - (xx + yy)) * sz;

            var m12 = position.x;
            var m13 = position.y;
            var m14 = position.z;

            var sprite = this.sprite;

            // 更新 positions
            p0.x = m00 * sprite.p0x + m04 * sprite.p0y + m08 * sprite.p0z + m12;
            p0.y = m01 * sprite.p0x + m05 * sprite.p0y + m09 * sprite.p0z + m13;
            p0.z = m02 * sprite.p0x + m06 * sprite.p0y + m10 * sprite.p0z + m14;

            p1.x = m00 * sprite.p1x + m04 * sprite.p1y + m08 * sprite.p1z + m12;
            p1.y = m01 * sprite.p1x + m05 * sprite.p1y + m09 * sprite.p1z + m13;
            p1.z = m02 * sprite.p1x + m06 * sprite.p1y + m10 * sprite.p1z + m14;

            p2.x = m00 * sprite.p2x + m04 * sprite.p2y + m08 * sprite.p2z + m12;
            p2.y = m01 * sprite.p2x + m05 * sprite.p2y + m09 * sprite.p2z + m13;
            p2.z = m02 * sprite.p2x + m06 * sprite.p2y + m10 * sprite.p2z + m14;

            p3.x = m00 * sprite.p3x + m04 * sprite.p3y + m08 * sprite.p3z + m12;
            p3.y = m01 * sprite.p3x + m05 * sprite.p3y + m09 * sprite.p3z + m13;
            p3.z = m02 * sprite.p3x + m06 * sprite.p3y + m10 * sprite.p3z + m14;

            this.renderBatch.setPositions(index, p0, p1, p2, p3);
        }
    }
}