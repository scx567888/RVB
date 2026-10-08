using System;
using System.Collections.Generic;

namespace rvb.scripts {
    public sealed class SheepBullet {
        public int id;
        public string name = string.Empty;
        public int roleType;
        public int animId;
        public int moveType;
        public int startOffsetX;
        public int startOffsetY;
        public int startOffsetZ;
        public int endOffsetZ;
        public int curveHigh;
        public float atkBet;
        public float scale;
        public int endFrame;
        public int moveTimeFrame;
        public int[] atkFrames = Array.Empty<int>();
        public SheepBulletAtkShapeType atkShapeType;
        public int speed;
        public int findR;
        public int atkR;
        public int radius;
        public int[] maxRadiuses = Array.Empty<int>();
        public int[] minRadiuses = Array.Empty<int>();
        public int createBulletID;
        public int createBulletFrame;

        public static IReadOnlyList<SheepBullet> List => SheepBullets.All;

        public static SheepBullet getById(int id) {
            return SheepBullets.GetById(id);
        }

        public static bool TryGetById(int id, out SheepBullet bullet) {
            return SheepBullets.TryGetById(id, out bullet);
        }
    }

    public static class SheepBullets {
        // 弓箭
        public static readonly SheepBullet bullet_1 = new() {
            id = 1,
            name = "弓箭",
            roleType = 3,
            animId = 1,
            moveType = 5,
            startOffsetX = 0,
            startOffsetY = 0,
            startOffsetZ = 150,
            endOffsetZ = 0,
            curveHigh = 2000,
            atkBet = 1f,
            scale = 20f,
            endFrame = 20,
            moveTimeFrame = 18,
            atkShapeType = SheepBulletAtkShapeType.Round,
            speed = 0,
            findR = 4,
            atkR = 400,
            radius = 0,
            createBulletID = 13,
            createBulletFrame = 17,
            atkFrames = new[] { 18 },
            maxRadiuses = System.Array.Empty<int>(),
            minRadiuses = System.Array.Empty<int>()
        };

        // 技能弓箭
        public static readonly SheepBullet bullet_2 = new() {
            id = 2,
            name = "技能弓箭",
            roleType = 3,
            animId = 2,
            moveType = 2,
            startOffsetX = 0,
            startOffsetY = 0,
            startOffsetZ = 40,
            endOffsetZ = 0,
            curveHigh = 0,
            atkBet = 0.11f,
            scale = 6f,
            endFrame = 80,
            moveTimeFrame = 0,
            atkShapeType = SheepBulletAtkShapeType.Round,
            speed = 8000,
            findR = 2,
            atkR = 150,
            radius = 0,
            createBulletID = 0,
            createBulletFrame = 0,
            atkFrames = new[] { -1 },
            maxRadiuses = System.Array.Empty<int>(),
            minRadiuses = System.Array.Empty<int>()
        };

        // 抛
        public static readonly SheepBullet bullet_3 = new() {
            id = 3,
            name = "抛",
            roleType = 7,
            animId = 3,
            moveType = 5,
            startOffsetX = -300,
            startOffsetY = 0,
            startOffsetZ = 2200,
            endOffsetZ = 0,
            curveHigh = 7500,
            atkBet = 1f,
            scale = 10f,
            endFrame = 28,
            moveTimeFrame = 27,
            atkShapeType = SheepBulletAtkShapeType.Round,
            speed = 0,
            findR = 10,
            atkR = 1000,
            radius = 0,
            createBulletID = 12,
            createBulletFrame = 26,
            atkFrames = new[] { 27 },
            maxRadiuses = System.Array.Empty<int>(),
            minRadiuses = System.Array.Empty<int>()
        };

        // 抛
        public static readonly SheepBullet bullet_4 = new() {
            id = 4,
            name = "抛",
            roleType = 1,
            animId = 4,
            moveType = 5,
            startOffsetX = -100,
            startOffsetY = 0,
            startOffsetZ = 25,
            endOffsetZ = 0,
            curveHigh = 1800,
            atkBet = 1f,
            scale = 1.8f,
            endFrame = 36,
            moveTimeFrame = 33,
            atkShapeType = SheepBulletAtkShapeType.Round,
            speed = 0,
            findR = 3,
            atkR = 250,
            radius = 0,
            createBulletID = 0,
            createBulletFrame = 0,
            atkFrames = new[] { 33 },
            maxRadiuses = System.Array.Empty<int>(),
            minRadiuses = System.Array.Empty<int>()
        };

        // 抛
        public static readonly SheepBullet bullet_5 = new() {
            id = 5,
            name = "抛",
            roleType = 1,
            animId = 4,
            moveType = 5,
            startOffsetX = -125,
            startOffsetY = 0,
            startOffsetZ = 30,
            endOffsetZ = 0,
            curveHigh = 2100,
            atkBet = 1f,
            scale = 2.1f,
            endFrame = 42,
            moveTimeFrame = 39,
            atkShapeType = SheepBulletAtkShapeType.Round,
            speed = 0,
            findR = 3,
            atkR = 300,
            radius = 0,
            createBulletID = 0,
            createBulletFrame = 0,
            atkFrames = new[] { 39 },
            maxRadiuses = System.Array.Empty<int>(),
            minRadiuses = System.Array.Empty<int>()
        };

        // 抛
        public static readonly SheepBullet bullet_6 = new() {
            id = 6,
            name = "抛",
            roleType = 1,
            animId = 4,
            moveType = 5,
            startOffsetX = -150,
            startOffsetY = 0,
            startOffsetZ = 35,
            endOffsetZ = 0,
            curveHigh = 2400,
            atkBet = 1f,
            scale = 2.4f,
            endFrame = 48,
            moveTimeFrame = 45,
            atkShapeType = SheepBulletAtkShapeType.Round,
            speed = 0,
            findR = 4,
            atkR = 350,
            radius = 0,
            createBulletID = 0,
            createBulletFrame = 0,
            atkFrames = new[] { 45 },
            maxRadiuses = System.Array.Empty<int>(),
            minRadiuses = System.Array.Empty<int>()
        };

        // 抛
        public static readonly SheepBullet bullet_7 = new() {
            id = 7,
            name = "抛",
            roleType = 1,
            animId = 4,
            moveType = 5,
            startOffsetX = -175,
            startOffsetY = 0,
            startOffsetZ = 40,
            endOffsetZ = 0,
            curveHigh = 2700,
            atkBet = 1f,
            scale = 2.7f,
            endFrame = 54,
            moveTimeFrame = 51,
            atkShapeType = SheepBulletAtkShapeType.Round,
            speed = 0,
            findR = 4,
            atkR = 400,
            radius = 0,
            createBulletID = 0,
            createBulletFrame = 0,
            atkFrames = new[] { 51 },
            maxRadiuses = System.Array.Empty<int>(),
            minRadiuses = System.Array.Empty<int>()
        };

        // 抛
        public static readonly SheepBullet bullet_8 = new() {
            id = 8,
            name = "抛",
            roleType = 1,
            animId = 4,
            moveType = 5,
            startOffsetX = -200,
            startOffsetY = 0,
            startOffsetZ = 45,
            endOffsetZ = 0,
            curveHigh = 3000,
            atkBet = 1f,
            scale = 3f,
            endFrame = 60,
            moveTimeFrame = 57,
            atkShapeType = SheepBulletAtkShapeType.Round,
            speed = 0,
            findR = 5,
            atkR = 450,
            radius = 0,
            createBulletID = 0,
            createBulletFrame = 0,
            atkFrames = new[] { 57 },
            maxRadiuses = System.Array.Empty<int>(),
            minRadiuses = System.Array.Empty<int>()
        };

        // 小抛
        public static readonly SheepBullet bullet_9 = new() {
            id = 9,
            name = "小抛",
            roleType = 1,
            animId = 5,
            moveType = 5,
            startOffsetX = -50,
            startOffsetY = 0,
            startOffsetZ = 15,
            endOffsetZ = 0,
            curveHigh = 1000,
            atkBet = 1f,
            scale = 1.5f,
            endFrame = 30,
            moveTimeFrame = 27,
            atkShapeType = SheepBulletAtkShapeType.Round,
            speed = 0,
            findR = 2,
            atkR = 100,
            radius = 0,
            createBulletID = 0,
            createBulletFrame = 0,
            atkFrames = new[] { 27 },
            maxRadiuses = System.Array.Empty<int>(),
            minRadiuses = System.Array.Empty<int>()
        };

        // 音波
        public static readonly SheepBullet bullet_10 = new() {
            id = 10,
            name = "音波",
            roleType = 1,
            animId = 0,
            moveType = 1,
            startOffsetX = 0,
            startOffsetY = 0,
            startOffsetZ = 0,
            endOffsetZ = 0,
            curveHigh = 0,
            atkBet = 1f,
            scale = 0f,
            endFrame = 60,
            moveTimeFrame = 0,
            atkShapeType = SheepBulletAtkShapeType.Ring,
            speed = 0,
            findR = 20,
            atkR = 0,
            radius = 0,
            createBulletID = 0,
            createBulletFrame = 0,
            atkFrames = new[] { 5, 10, 16, 23, 31, 40, 50 },
            maxRadiuses = new[] { 100, 400, 700, 1000, 1300, 1600, 1900 },
            minRadiuses = new[] { 0, 310, 620, 930, 1240, 1550, 1840 }
        };

        // 飞棍
        public static readonly SheepBullet bullet_11 = new() {
            id = 11,
            name = "飞棍",
            roleType = 1,
            animId = 6,
            moveType = 5,
            startOffsetX = 0,
            startOffsetY = 0,
            startOffsetZ = 300,
            endOffsetZ = 0,
            curveHigh = 400,
            atkBet = 1f,
            scale = 2f,
            endFrame = 20,
            moveTimeFrame = 18,
            atkShapeType = SheepBulletAtkShapeType.Round,
            speed = 0,
            findR = 6,
            atkR = 600,
            radius = 0,
            createBulletID = 0,
            createBulletFrame = 0,
            atkFrames = new[] { 18 },
            maxRadiuses = System.Array.Empty<int>(),
            minRadiuses = System.Array.Empty<int>()
        };

        // 爆炸效果子弹（投石车右）
        public static readonly SheepBullet bullet_12 = new() {
            id = 12,
            name = "爆炸效果子弹（投石车右）",
            roleType = 7,
            animId = 8,
            moveType = 1,
            startOffsetX = 0,
            startOffsetY = 0,
            startOffsetZ = 0,
            endOffsetZ = 0,
            curveHigh = 0,
            atkBet = 0f,
            scale = 30f,
            endFrame = 23,
            moveTimeFrame = 23,
            atkShapeType = SheepBulletAtkShapeType.Round,
            speed = 0,
            findR = 0,
            atkR = 0,
            radius = 0,
            createBulletID = 0,
            createBulletFrame = 0,
            atkFrames = new[] { -100 },
            maxRadiuses = System.Array.Empty<int>(),
            minRadiuses = System.Array.Empty<int>()
        };

        // 爆炸效果子弹（弓箭）
        public static readonly SheepBullet bullet_13 = new() {
            id = 13,
            name = "爆炸效果子弹（弓箭）",
            roleType = 3,
            animId = 10,
            moveType = 1,
            startOffsetX = 0,
            startOffsetY = 0,
            startOffsetZ = 0,
            endOffsetZ = 0,
            curveHigh = 0,
            atkBet = 0f,
            scale = 12f,
            endFrame = 7,
            moveTimeFrame = 7,
            atkShapeType = SheepBulletAtkShapeType.Round,
            speed = 0,
            findR = 0,
            atkR = 0,
            radius = 0,
            createBulletID = 0,
            createBulletFrame = 0,
            atkFrames = new[] { -100 },
            maxRadiuses = System.Array.Empty<int>(),
            minRadiuses = System.Array.Empty<int>()
        };

        // 爆炸效果自爆（小兵）
        public static readonly SheepBullet bullet_14 = new() {
            id = 14,
            name = "爆炸效果自爆（小兵）",
            roleType = 1,
            animId = 11,
            moveType = 1,
            startOffsetX = 0,
            startOffsetY = 0,
            startOffsetZ = 0,
            endOffsetZ = 0,
            curveHigh = 0,
            atkBet = 10f,
            scale = 5f,
            endFrame = 7,
            moveTimeFrame = 7,
            atkShapeType = SheepBulletAtkShapeType.Round,
            speed = 0,
            findR = 15,
            atkR = 1200,
            radius = 0,
            createBulletID = 0,
            createBulletFrame = 0,
            atkFrames = new[] { 1 },
            maxRadiuses = System.Array.Empty<int>(),
            minRadiuses = System.Array.Empty<int>()
        };

        // 抛
        public static readonly SheepBullet bullet_15 = new() {
            id = 15,
            name = "抛",
            roleType = 7,
            animId = 18,
            moveType = 5,
            startOffsetX = -300,
            startOffsetY = 0,
            startOffsetZ = 2200,
            endOffsetZ = 0,
            curveHigh = 7500,
            atkBet = 1f,
            scale = 10f,
            endFrame = 28,
            moveTimeFrame = 27,
            atkShapeType = SheepBulletAtkShapeType.Round,
            speed = 0,
            findR = 10,
            atkR = 1000,
            radius = 0,
            createBulletID = 16,
            createBulletFrame = 26,
            atkFrames = new[] { 27 },
            maxRadiuses = System.Array.Empty<int>(),
            minRadiuses = System.Array.Empty<int>()
        };

        // 爆炸效果子弹（投石车左）
        public static readonly SheepBullet bullet_16 = new() {
            id = 16,
            name = "爆炸效果子弹（投石车左）",
            roleType = 7,
            animId = 19,
            moveType = 1,
            startOffsetX = 0,
            startOffsetY = 0,
            startOffsetZ = 0,
            endOffsetZ = 0,
            curveHigh = 0,
            atkBet = 0f,
            scale = 30f,
            endFrame = 23,
            moveTimeFrame = 23,
            atkShapeType = SheepBulletAtkShapeType.Round,
            speed = 0,
            findR = 0,
            atkR = 0,
            radius = 0,
            createBulletID = 0,
            createBulletFrame = 0,
            atkFrames = new[] { -100 },
            maxRadiuses = System.Array.Empty<int>(),
            minRadiuses = System.Array.Empty<int>()
        };

        // 测试弓箭
        public static readonly SheepBullet bullet_100 = new() {
            id = 100,
            name = "测试弓箭",
            roleType = 1,
            animId = 2,
            moveType = 2,
            startOffsetX = 50,
            startOffsetY = 0,
            startOffsetZ = 100,
            endOffsetZ = 0,
            curveHigh = 0,
            atkBet = 0.01f,
            scale = 1f,
            endFrame = 5,
            moveTimeFrame = 0,
            atkShapeType = SheepBulletAtkShapeType.Round,
            speed = 2000,
            findR = 1,
            atkR = 50,
            radius = 0,
            createBulletID = 0,
            createBulletFrame = 0,
            atkFrames = new[] { 4 },
            maxRadiuses = System.Array.Empty<int>(),
            minRadiuses = System.Array.Empty<int>()
        };

        // 测试冰爆术子弹
        public static readonly SheepBullet bullet_101 = new() {
            id = 101,
            name = "测试冰爆术子弹",
            roleType = 1,
            animId = 1,
            moveType = 10,
            startOffsetX = 0,
            startOffsetY = 0,
            startOffsetZ = 0,
            endOffsetZ = 0,
            curveHigh = 0,
            atkBet = 1f,
            scale = 2f,
            endFrame = 30,
            moveTimeFrame = 20,
            atkShapeType = SheepBulletAtkShapeType.Round,
            speed = 2000,
            findR = 1,
            atkR = 50,
            radius = 0,
            createBulletID = 0,
            createBulletFrame = 0,
            atkFrames = new[] { 4 },
            maxRadiuses = System.Array.Empty<int>(),
            minRadiuses = System.Array.Empty<int>()
        };

        // 测试剑雨子弹
        public static readonly SheepBullet bullet_102 = new() {
            id = 102,
            name = "测试剑雨子弹",
            roleType = 1,
            animId = 1,
            moveType = 10,
            startOffsetX = 0,
            startOffsetY = 0,
            startOffsetZ = 0,
            endOffsetZ = 0,
            curveHigh = 0,
            atkBet = 1f,
            scale = 2f,
            endFrame = 30,
            moveTimeFrame = 20,
            atkShapeType = SheepBulletAtkShapeType.Round,
            speed = 2000,
            findR = 1,
            atkR = 50,
            radius = 0,
            createBulletID = 0,
            createBulletFrame = 0,
            atkFrames = new[] { 4 },
            maxRadiuses = System.Array.Empty<int>(),
            minRadiuses = System.Array.Empty<int>()
        };

        // 测试鬼火子弹
        public static readonly SheepBullet bullet_103 = new() {
            id = 103,
            name = "测试鬼火子弹",
            roleType = 1,
            animId = 1,
            moveType = 8,
            startOffsetX = 0,
            startOffsetY = 0,
            startOffsetZ = 100,
            endOffsetZ = 0,
            curveHigh = 0,
            atkBet = 1f,
            scale = 2f,
            endFrame = 600,
            moveTimeFrame = 0,
            atkShapeType = SheepBulletAtkShapeType.Round,
            speed = 1,
            findR = 1,
            atkR = 50,
            radius = 200,
            createBulletID = 0,
            createBulletFrame = 0,
            atkFrames = new[] { -1 },
            maxRadiuses = System.Array.Empty<int>(),
            minRadiuses = System.Array.Empty<int>()
        };

        // 测试导弹子弹
        public static readonly SheepBullet bullet_104 = new() {
            id = 104,
            name = "测试导弹子弹",
            roleType = 1,
            animId = 1,
            moveType = 7,
            startOffsetX = 0,
            startOffsetY = 0,
            startOffsetZ = 0,
            endOffsetZ = 0,
            curveHigh = 0,
            atkBet = 1f,
            scale = 2f,
            endFrame = 30,
            moveTimeFrame = 20,
            atkShapeType = SheepBulletAtkShapeType.Round,
            speed = 2000,
            findR = 1,
            atkR = 50,
            radius = 0,
            createBulletID = 0,
            createBulletFrame = 0,
            atkFrames = new[] { 4 },
            maxRadiuses = System.Array.Empty<int>(),
            minRadiuses = System.Array.Empty<int>()
        };

        // 金箍棒
        public static readonly SheepBullet bullet_105 = new() {
            id = 105,
            name = "金箍棒",
            roleType = 10,
            animId = 7,
            moveType = 1,
            startOffsetX = 0,
            startOffsetY = -2382,
            startOffsetZ = -1325,
            endOffsetZ = 0,
            curveHigh = 0,
            atkBet = 1f,
            scale = 20f,
            endFrame = 60,
            moveTimeFrame = 60,
            atkShapeType = SheepBulletAtkShapeType.Round,
            speed = 0,
            findR = 25,
            atkR = 2500,
            radius = 0,
            createBulletID = 0,
            createBulletFrame = 0,
            atkFrames = new[] { 23 },
            maxRadiuses = System.Array.Empty<int>(),
            minRadiuses = System.Array.Empty<int>()
        };

        // 手掌
        public static readonly SheepBullet bullet_106 = new() {
            id = 106,
            name = "手掌",
            roleType = 10,
            animId = 9,
            moveType = 1,
            startOffsetX = 0,
            startOffsetY = -2382,
            startOffsetZ = -1325,
            endOffsetZ = 0,
            curveHigh = 0,
            atkBet = 1f,
            scale = 20f,
            endFrame = 60,
            moveTimeFrame = 60,
            atkShapeType = SheepBulletAtkShapeType.Round,
            speed = 0,
            findR = 25,
            atkR = 2500,
            radius = 0,
            createBulletID = 0,
            createBulletFrame = 0,
            atkFrames = new[] { 23 },
            maxRadiuses = System.Array.Empty<int>(),
            minRadiuses = System.Array.Empty<int>()
        };

        // 小金箍棒
        public static readonly SheepBullet bullet_107 = new() {
            id = 107,
            name = "小金箍棒",
            roleType = 1,
            animId = 12,
            moveType = 1,
            startOffsetX = 0,
            startOffsetY = -762,
            startOffsetZ = -440,
            endOffsetZ = 0,
            curveHigh = 0,
            atkBet = 1f,
            scale = 8f,
            endFrame = 60,
            moveTimeFrame = 60,
            atkShapeType = SheepBulletAtkShapeType.Round,
            speed = 0,
            findR = 7,
            atkR = 700,
            radius = 0,
            createBulletID = 0,
            createBulletFrame = 0,
            atkFrames = new[] { 7 },
            maxRadiuses = System.Array.Empty<int>(),
            minRadiuses = System.Array.Empty<int>()
        };

        // 小手掌
        public static readonly SheepBullet bullet_108 = new() {
            id = 108,
            name = "小手掌",
            roleType = 1,
            animId = 13,
            moveType = 1,
            startOffsetX = 0,
            startOffsetY = -1109,
            startOffsetZ = -640,
            endOffsetZ = 0,
            curveHigh = 0,
            atkBet = 1f,
            scale = 8f,
            endFrame = 60,
            moveTimeFrame = 60,
            atkShapeType = SheepBulletAtkShapeType.Round,
            speed = 0,
            findR = 7,
            atkR = 700,
            radius = 0,
            createBulletID = 0,
            createBulletFrame = 0,
            atkFrames = new[] { 7 },
            maxRadiuses = System.Array.Empty<int>(),
            minRadiuses = System.Array.Empty<int>()
        };

        // 火球
        public static readonly SheepBullet bullet_109 = new() {
            id = 109,
            name = "火球",
            roleType = 11,
            animId = 14,
            moveType = 3,
            startOffsetX = 0,
            startOffsetY = 0,
            startOffsetZ = 10000,
            endOffsetZ = 0,
            curveHigh = 10000,
            atkBet = 1f,
            scale = 20f,
            endFrame = 22,
            moveTimeFrame = 20,
            atkShapeType = SheepBulletAtkShapeType.Round,
            speed = 0,
            findR = 20,
            atkR = 2500,
            radius = 0,
            createBulletID = 110,
            createBulletFrame = 19,
            atkFrames = new[] { 20 },
            maxRadiuses = System.Array.Empty<int>(),
            minRadiuses = System.Array.Empty<int>()
        };

        // 火球爆炸
        public static readonly SheepBullet bullet_110 = new() {
            id = 110,
            name = "火球爆炸",
            roleType = 11,
            animId = 16,
            moveType = 1,
            startOffsetX = 0,
            startOffsetY = 0,
            startOffsetZ = -500,
            endOffsetZ = 0,
            curveHigh = 0,
            atkBet = 0f,
            scale = 35f,
            endFrame = 25,
            moveTimeFrame = 25,
            atkShapeType = SheepBulletAtkShapeType.Round,
            speed = 0,
            findR = 0,
            atkR = 0,
            radius = 0,
            createBulletID = 0,
            createBulletFrame = 0,
            atkFrames = new[] { -100 },
            maxRadiuses = System.Array.Empty<int>(),
            minRadiuses = System.Array.Empty<int>()
        };

        // 火球二
        public static readonly SheepBullet bullet_111 = new() {
            id = 111,
            name = "火球二",
            roleType = 11,
            animId = 15,
            moveType = 3,
            startOffsetX = 0,
            startOffsetY = 0,
            startOffsetZ = 10000,
            endOffsetZ = 0,
            curveHigh = 10000,
            atkBet = 1f,
            scale = 20f,
            endFrame = 22,
            moveTimeFrame = 20,
            atkShapeType = SheepBulletAtkShapeType.Round,
            speed = 0,
            findR = 20,
            atkR = 2500,
            radius = 0,
            createBulletID = 112,
            createBulletFrame = 19,
            atkFrames = new[] { 20 },
            maxRadiuses = System.Array.Empty<int>(),
            minRadiuses = System.Array.Empty<int>()
        };

        // 火球爆炸二
        public static readonly SheepBullet bullet_112 = new() {
            id = 112,
            name = "火球爆炸二",
            roleType = 11,
            animId = 17,
            moveType = 1,
            startOffsetX = 0,
            startOffsetY = 0,
            startOffsetZ = -500,
            endOffsetZ = 0,
            curveHigh = 0,
            atkBet = 0f,
            scale = 35f,
            endFrame = 25,
            moveTimeFrame = 25,
            atkShapeType = SheepBulletAtkShapeType.Round,
            speed = 0,
            findR = 0,
            atkR = 0,
            radius = 0,
            createBulletID = 0,
            createBulletFrame = 0,
            atkFrames = new[] { -100 },
            maxRadiuses = System.Array.Empty<int>(),
            minRadiuses = System.Array.Empty<int>()
        };

        // 炮弹 (天降轰炸本体): 从 startZ 垂直落下, 第 18 帧落地结算范围伤害并生成爆炸特效
        // 落地帧算法: startZ(1200) / (speed(2000) * 0.033) ≈ 18 帧
        // animId = 1 在 Sheep.unity 的 bulletRenderConfigs 里已配置
        // ⚠️ atkBet 在当前伤害链路里是死配置 (createBullet 只写 atkVue = 施法者 conf.atk,
        //    hurtByBullet 直接用 atkVue), 想调威力请改施法者兵种的 atk.
        public static readonly SheepBullet bullet_113 = new() {
            id = 113,
            name = "炮弹",
            roleType = 7,
            animId = 1,
            moveType = 10,
            startOffsetX = 0,
            startOffsetY = 0,
            startOffsetZ = 0,
            endOffsetZ = 0,
            curveHigh = 0,
            atkBet = 1f,
            scale = 8f,
            endFrame = 21,
            moveTimeFrame = 18,
            atkShapeType = SheepBulletAtkShapeType.Round,
            speed = 2000,
            findR = 2,
            atkR = 700,
            radius = 0,
            createBulletID = 16,
            createBulletFrame = 18,
            atkFrames = new[] { 18 },
            maxRadiuses = System.Array.Empty<int>(),
            minRadiuses = System.Array.Empty<int>()
        };

        // 自爆兵自爆 (animId = 19 在 Sheep.unity 里已配置, 与 bullet_16 同一套爆炸特效)
        public static readonly SheepBullet bullet_114 = new() {
            id = 114,
            name = "自爆兵自爆",
            roleType = 1,
            animId = 19,
            moveType = 1,
            startOffsetX = 0,
            startOffsetY = 0,
            startOffsetZ = 0,
            endOffsetZ = 0,
            curveHigh = 0,
            atkBet = 1f,
            scale = 25f,
            endFrame = 23,
            moveTimeFrame = 23,
            atkShapeType = SheepBulletAtkShapeType.Round,
            speed = 0,
            findR = 3,
            atkR = 1700,
            radius = 0,
            createBulletID = 0,
            createBulletFrame = 0,
            atkFrames = new[] { 1 },
            maxRadiuses = System.Array.Empty<int>(),
            minRadiuses = System.Array.Empty<int>()
        };

        public static readonly SheepBullet[] All = {
            bullet_1,
            bullet_2,
            bullet_3,
            bullet_4,
            bullet_5,
            bullet_6,
            bullet_7,
            bullet_8,
            bullet_9,
            bullet_10,
            bullet_11,
            bullet_12,
            bullet_13,
            bullet_14,
            bullet_15,
            bullet_16,
            bullet_100,
            bullet_101,
            bullet_102,
            bullet_103,
            bullet_104,
            bullet_105,
            bullet_106,
            bullet_107,
            bullet_108,
            bullet_109,
            bullet_110,
            bullet_111,
            bullet_112,
            bullet_113,
            bullet_114,
        };

        private static readonly Dictionary<int, SheepBullet> Map = BuildMap();

        public static SheepBullet GetById(int id) {
            if (!Map.TryGetValue(id, out SheepBullet bullet)) {
                throw new KeyNotFoundException($"不存在 SheepBullet 配置，ID: {id}");
            }

            return bullet;
        }

        public static bool TryGetById(int id, out SheepBullet bullet) {
            return Map.TryGetValue(id, out bullet);
        }

        private static Dictionary<int, SheepBullet> BuildMap() {
            var map = new Dictionary<int, SheepBullet>(All.Length);

            foreach (SheepBullet bullet in All) {
                if (!map.TryAdd(bullet.id, bullet)) {
                    throw new InvalidOperationException($"SheepBullet 存在重复 ID: {bullet.id}");
                }
            }

            return map;
        }
    }
}