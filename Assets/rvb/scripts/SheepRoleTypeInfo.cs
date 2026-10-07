using System;
using System.Collections.Generic;

namespace rvb.scripts {
    // 兵种静态配置
    public class SheepRoleTypeInfo {
        
        // 配置 id
        public int id;
        
        // 名称 [阵营]
        public string[] name = new []{"未命名","未命名"};
        
        // 角色类型
        public SheepRoleType roleType;
        
        // 碰撞 组
        public SheepCollideGroup collideGroup;
        
        // 动画 ID
        public int animId;
        
        // 缩放
        public float scale;
        
        
        public int detectCollideR;
        public float collideR;
        public SheepRoleState startState;
        
        // 阵营 ID
        public int formationId;
        
        
        public int runEndX;
        public int runSpeed;
        public int walkSpeed;
        public bool isSpurtAnim;
        public int skillSpurt;
        public int skillIn;

        // 是否无视碰撞 (出场时置 PetView.isNotConn, 自己不被挡, 也不挡别人)
        public bool isNotCollide;
        
        public int[] bullet = Array.Empty<int>();

        // 血量
        public float hp;

        // 攻击力
        public int atk;

        public int findR;
        public SheepRoleAtkType atkType;
        public int atkMoveType;
        public int atkMinMoveR;
        public int atkR;
        public float atkCd;
        public int[] readyAtks = Array.Empty<int>();
        public int finishAtk;

        // 溅射格子数
        public int splitN;

        // 溅射半径
        public int spiltR;

        public int fontSize;
        public bool isFindMoveTar;
        public float colliderElasticityScale;
        public int colliderNotMoveNum;
        public float colliderMoveScale;
        public int[] deadAnimType = Array.Empty<int>();
        public int isSteering;
        public int[] findAtkSort = Array.Empty<int>();
        public int hitBackDistance;
        // 是否到 boss 附近停止
        public int isLoongStopDistance;
        // 是否到 boss 附近停止的半径
        public int loongStopDistanceR;

        public static SheepRoleTypeInfo getById(int id) {
            return SheepRoleTypeInfos.GetById(id);
        }

        public static bool TryGetById(int id, out SheepRoleTypeInfo info) {
            return SheepRoleTypeInfos.TryGetById(id, out info);
        }
    }

    /// <summary>
    /// 全部兵种配置。字段名称与逆向得到的原始 JavaScript 配置保持一致。
    /// </summary>
    public static class SheepRoleTypeInfos {
        // 炮车 / 炮车
        public static readonly SheepRoleTypeInfo PAO_CHE = new() {
            id = 23,
            roleType = SheepRoleType.PAO_CHE,
            collideGroup = SheepCollideGroup.GROUP3,
            animId = 106,
            scale = 9f,
            detectCollideR = 4,
            collideR = 225f,
            startState = SheepRoleState.Spurt,
            formationId = 5,
            runEndX = 0,
            runSpeed = 3500,
            walkSpeed = 700,
            isSpurtAnim = false,
            skillSpurt = 0,
            skillIn = 0,
            hp = 28000f,
            atk = 6244,
            findR = 7,
            atkType = SheepRoleAtkType.Throw,
            atkMoveType = 1,
            atkMinMoveR = 2000,
            atkR = 5000,
            atkCd = 3.7f,
            finishAtk = 40,
            splitN = 4,
            spiltR = 540,
            fontSize = 72,
            isFindMoveTar = true,
            colliderElasticityScale = 1.5f,
            colliderNotMoveNum = 3,
            colliderMoveScale = 0.5f,
            isSteering = 1,
            hitBackDistance = 0,
            isLoongStopDistance = 1,
            loongStopDistanceR = 5000,
            name = new[] {
                "炮车",
                "炮车"
            },
            bullet = new[] {
                15,
                3
            },
            readyAtks = new[] {
                14
            },
            deadAnimType = new int[] { },
            findAtkSort = new[] {
                7,
                5,
                6,
                8,
                3,
                4,
                2,
                1
            },
        };

        // 黄金炮车 / 黄金炮车
        public static readonly SheepRoleTypeInfo HUANG_JIN_PAO_CHE = new() {
            id = 29,
            roleType = SheepRoleType.PAO_CHE,
            collideGroup = SheepCollideGroup.GROUP3,
            animId = 116,
            scale = 9f,
            detectCollideR = 4,
            collideR = 225f,
            startState = SheepRoleState.Spurt,
            formationId = 5,
            runEndX = 0,
            runSpeed = 3500,
            walkSpeed = 700,
            isSpurtAnim = false,
            skillSpurt = 0,
            skillIn = 0,
            hp = 32666.6666666667f,
            atk = 6244,
            findR = 7,
            atkType = SheepRoleAtkType.Throw,
            atkMoveType = 1,
            atkMinMoveR = 2000,
            atkR = 5000,
            atkCd = 3.7f,
            finishAtk = 40,
            splitN = 4,
            spiltR = 540,
            fontSize = 72,
            isFindMoveTar = true,
            colliderElasticityScale = 1.5f,
            colliderNotMoveNum = 3,
            colliderMoveScale = 0.5f,
            isSteering = 1,
            hitBackDistance = 0,
            isLoongStopDistance = 1,
            loongStopDistanceR = 5000,
            name = new[] {
                "黄金炮车",
                "黄金炮车"
            },
            bullet = new[] {
                15,
                3
            },
            readyAtks = new[] {
                14
            },
            deadAnimType = new int[] { },
            findAtkSort = new[] {
                7,
                5,
                6,
                8,
                3,
                4,
                2,
                1
            },
        };

        // 旋风斩 / 旋风斩
        public static readonly SheepRoleTypeInfo XUAN_FENG_ZHAN = new() {
            id = 19,
            roleType = SheepRoleType.XUAN_FENG_ZHAN,
            collideGroup = SheepCollideGroup.GROUP5,
            animId = 105,
            scale = 7f,
            detectCollideR = 2,
            collideR = 112.5f,
            startState = SheepRoleState.SpinSpurt,
            formationId = 8,
            runEndX = 100,
            runSpeed = 3500,
            walkSpeed = 700,
            isSpurtAnim = true,
            skillSpurt = 140001,
            skillIn = 0,
            hp = 24000f,
            atk = 400,
            findR = 24,
            atkType = SheepRoleAtkType.Nearest,
            atkMoveType = 0,
            atkMinMoveR = 25,
            atkR = 75,
            atkCd = 0f,
            finishAtk = 16,
            splitN = 3,
            spiltR = 200,
            fontSize = 88,
            isFindMoveTar = true,
            colliderElasticityScale = 1.1f,
            colliderNotMoveNum = 3,
            colliderMoveScale = 0.5f,
            isSteering = 1,
            hitBackDistance = 0,
            isLoongStopDistance = 0,
            loongStopDistanceR = 0,
            name = new[] {
                "旋风斩",
                "旋风斩"
            },
            bullet = new int[] { },
            readyAtks = new[] {
                8
            },
            deadAnimType = new int[] { },
            findAtkSort = new[] {
                4,
                3,
                7,
                5,
                2,
                6,
                8,
                1
            },
        };

        // 大旋风斩 / 大旋风斩
        public static readonly SheepRoleTypeInfo DA_XUAN_FENG_ZHAN = new() {
            id = 26,
            roleType = SheepRoleType.XUAN_FENG_ZHAN,
            collideGroup = SheepCollideGroup.GROUP5,
            animId = 115,
            scale = 7f,
            detectCollideR = 2,
            collideR = 112.5f,
            startState = SheepRoleState.SpinSpurt,
            formationId = 8,
            runEndX = 100,
            runSpeed = 3500,
            walkSpeed = 700,
            isSpurtAnim = true,
            skillSpurt = 140001,
            skillIn = 0,
            hp = 28000f,
            atk = 400,
            findR = 24,
            atkType = SheepRoleAtkType.Nearest,
            atkMoveType = 0,
            atkMinMoveR = 25,
            atkR = 75,
            atkCd = 0f,
            finishAtk = 16,
            splitN = 3,
            spiltR = 200,
            fontSize = 88,
            isFindMoveTar = true,
            colliderElasticityScale = 1.1f,
            colliderNotMoveNum = 3,
            colliderMoveScale = 0.5f,
            isSteering = 1,
            hitBackDistance = 0,
            isLoongStopDistance = 0,
            loongStopDistanceR = 0,
            name = new[] {
                "大旋风斩",
                "大旋风斩"
            },
            bullet = new int[] { },
            readyAtks = new[] {
                8
            },
            deadAnimType = new int[] { },
            findAtkSort = new[] {
                4,
                3,
                7,
                5,
                2,
                6,
                8,
                1
            },
        };

        // 先锋兵 / 先锋兵
        public static readonly SheepRoleTypeInfo XIAN_FENG_BING = new() {
            id = 7,
            roleType = SheepRoleType.XUAN_FENG_ZHAN,
            collideGroup = SheepCollideGroup.GROUP7,
            animId = 8,
            scale = 7.5f,
            detectCollideR = 3,
            collideR = 180f,
            startState = 0,
            formationId = 7,
            runEndX = 0,
            runSpeed = 3000,
            walkSpeed = 600,
            isSpurtAnim = false,
            skillSpurt = 0,
            skillIn = 0,
            hp = 10000f,
            atk = 150,
            findR = 1,
            atkType = SheepRoleAtkType.Nearest,
            atkMoveType = 1,
            atkMinMoveR = 100,
            atkR = 150,
            atkCd = 0.2f,
            finishAtk = 36,
            splitN = 3,
            spiltR = 300,
            fontSize = 96,
            isFindMoveTar = true,
            colliderElasticityScale = 1.1f,
            colliderNotMoveNum = 1,
            colliderMoveScale = 0.1f,
            isSteering = 1,
            hitBackDistance = 0,
            isLoongStopDistance = 0,
            loongStopDistanceR = 0,
            name = new[] {
                "先锋兵",
                "先锋兵"
            },
            bullet = new int[] { },
            readyAtks = new[] {
                18
            },
            deadAnimType = new int[] { },
            findAtkSort = new int[] { },
        };

        // 小兵 / 小兵
        public static readonly SheepRoleTypeInfo XIAO_BING = new() {
            id = 22,
            roleType = SheepRoleType.XIAO_BING,
            collideGroup = SheepCollideGroup.GROUP0,
            animId = 100,
            scale = 3f,
            detectCollideR = 2,
            collideR = 110f,
            startState = SheepRoleState.Spurt,
            formationId = 1,
            runEndX = 100,
            runSpeed = 3500,
            walkSpeed = 500,
            isSpurtAnim = false,
            skillSpurt = 0,
            skillIn = 0,
            hp = 100f,
            atk = 10,
            findR = 1,
            atkType = SheepRoleAtkType.Nearest,
            atkMoveType = 0,
            atkMinMoveR = 0,
            atkR = 55,
            atkCd = 0.45f,
            finishAtk = 16,
            splitN = 1,
            spiltR = 55,
            fontSize = 48,
            isFindMoveTar = true,
            colliderElasticityScale = 1.3f,
            colliderNotMoveNum = 5,
            colliderMoveScale = 0.8f,
            isSteering = 1,
            hitBackDistance = 0,
            isLoongStopDistance = 0,
            loongStopDistanceR = 0,
            name = new[] {
                "小兵",
                "小兵"
            },
            bullet = new int[] { },
            readyAtks = new[] {
                8
            },
            deadAnimType = new[] {
                4,
                15,
                16
            },
            findAtkSort = new int[] { },
        };

        // 刺客 / 刺客
        public static readonly SheepRoleTypeInfo CI_KE = new() {
            id = 16,
            roleType = SheepRoleType.CI_KE,
            collideGroup = SheepCollideGroup.GROUP1,
            animId = 101,
            scale = 6f,
            detectCollideR = 2,
            collideR = 112.5f,
            startState = SheepRoleState.Spurt,
            formationId = 2,
            runEndX = 100,
            runSpeed = 3500,
            walkSpeed = 700,
            isSpurtAnim = false,
            skillSpurt = 20006,
            skillIn = 0,
            hp = 1000f,
            atk = 240,
            findR = 1,
            atkType = SheepRoleAtkType.Nearest,
            atkMoveType = 1,
            atkMinMoveR = 25,
            atkR = 275,
            atkCd = 0f,
            finishAtk = 16,
            splitN = 1,
            spiltR = 90,
            fontSize = 88,
            isFindMoveTar = true,
            colliderElasticityScale = 1.1f,
            colliderNotMoveNum = 3,
            colliderMoveScale = 0.5f,
            isSteering = 1,
            hitBackDistance = 0,
            isLoongStopDistance = 0,
            loongStopDistanceR = 0,
            name = new[] {
                "刺客",
                "刺客"
            },
            bullet = new int[] { },
            readyAtks = new[] {
                8
            },
            deadAnimType = new int[] { },
            findAtkSort = new int[] { },
        };

        // 大刺客 / 大刺客
        public static readonly SheepRoleTypeInfo DA_CI_KE = new() {
            id = 17,
            roleType = SheepRoleType.CI_KE,
            collideGroup = SheepCollideGroup.GROUP1,
            animId = 111,
            scale = 5f,
            detectCollideR = 2,
            collideR = 112.5f,
            startState = SheepRoleState.Spurt,
            formationId = 2,
            runEndX = 100,
            runSpeed = 3500,
            walkSpeed = 700,
            isSpurtAnim = false,
            skillSpurt = 20006,
            skillIn = 0,
            hp = 1600f,
            atk = 384,
            findR = 1,
            atkType = SheepRoleAtkType.Nearest,
            atkMoveType = 1,
            atkMinMoveR = 25,
            atkR = 75,
            atkCd = 0f,
            finishAtk = 16,
            splitN = 2,
            spiltR = 130,
            fontSize = 88,
            isFindMoveTar = true,
            colliderElasticityScale = 1.1f,
            colliderNotMoveNum = 3,
            colliderMoveScale = 0.5f,
            isSteering = 1,
            hitBackDistance = 0,
            isLoongStopDistance = 0,
            loongStopDistanceR = 0,
            name = new[] {
                "大刺客",
                "大刺客"
            },
            bullet = new int[] { },
            readyAtks = new[] {
                8
            },
            deadAnimType = new int[] { },
            findAtkSort = new int[] { },
        };

        // 刺客 / 刺客
        public static readonly SheepRoleTypeInfo CI_KE_1 = new() {
            id = 102,
            roleType = SheepRoleType.CI_KE,
            collideGroup = SheepCollideGroup.GROUP1,
            animId = 101,
            scale = 6f,
            detectCollideR = 2,
            collideR = 112.5f,
            startState = SheepRoleState.Spurt,
            formationId = 10,
            runEndX = 100,
            runSpeed = 3500,
            walkSpeed = 700,
            isSpurtAnim = false,
            skillSpurt = 20006,
            skillIn = 0,
            hp = 1000f,
            atk = 240,
            findR = 1,
            atkType = SheepRoleAtkType.Nearest,
            atkMoveType = 1,
            atkMinMoveR = 25,
            atkR = 275,
            atkCd = 0f,
            finishAtk = 16,
            splitN = 1,
            spiltR = 90,
            fontSize = 88,
            isFindMoveTar = true,
            colliderElasticityScale = 1.1f,
            colliderNotMoveNum = 3,
            colliderMoveScale = 0.5f,
            isSteering = 1,
            hitBackDistance = 0,
            isLoongStopDistance = 0,
            loongStopDistanceR = 0,
            name = new[] {
                "刺客",
                "刺客"
            },
            bullet = new int[] { },
            readyAtks = new[] {
                8
            },
            deadAnimType = new int[] { },
            findAtkSort = new int[] { },
        };

        // 盾兵 / 盾兵
        public static readonly SheepRoleTypeInfo DUN_BING = new() {
            id = 18,
            roleType = SheepRoleType.DUN_BING,
            collideGroup = SheepCollideGroup.GROUP5,
            animId = 103,
            scale = 5f,
            detectCollideR = 2,
            collideR = 180f,
            startState = SheepRoleState.Spurt,
            formationId = 4,
            runEndX = 100,
            runSpeed = 3500,
            walkSpeed = 700,
            isSpurtAnim = false,
            skillSpurt = 0,
            skillIn = 0,
            hp = 24000f,
            atk = 650,
            findR = 1,
            atkType = SheepRoleAtkType.Nearest,
            atkMoveType = 1,
            atkMinMoveR = 25,
            atkR = 75,
            atkCd = 0f,
            finishAtk = 65,
            splitN = 3,
            spiltR = 200,
            fontSize = 88,
            isFindMoveTar = true,
            colliderElasticityScale = 1.1f,
            colliderNotMoveNum = 3,
            colliderMoveScale = 0.5f,
            isSteering = 0,
            hitBackDistance = 0,
            isLoongStopDistance = 0,
            loongStopDistanceR = 0,
            name = new[] {
                "盾兵",
                "盾兵"
            },
            bullet = new int[] { },
            readyAtks = new[] {
                30
            },
            deadAnimType = new int[] { },
            findAtkSort = new[] {
                6,
                5,
                4,
                2,
                1,
                3,
                7
            },
        };

        // 重甲盾兵 / 重甲盾兵
        public static readonly SheepRoleTypeInfo ZHONG_JIA_DUN_BING = new() {
            id = 25,
            roleType = SheepRoleType.DUN_BING,
            collideGroup = SheepCollideGroup.GROUP5,
            animId = 113,
            scale = 5f,
            detectCollideR = 2,
            collideR = 180f,
            startState = SheepRoleState.Spurt,
            formationId = 4,
            runEndX = 100,
            runSpeed = 3500,
            walkSpeed = 700,
            isSpurtAnim = false,
            skillSpurt = 0,
            skillIn = 0,
            hp = 28000f,
            atk = 650,
            findR = 1,
            atkType = SheepRoleAtkType.Nearest,
            atkMoveType = 1,
            atkMinMoveR = 25,
            atkR = 75,
            atkCd = 0f,
            finishAtk = 65,
            splitN = 3,
            spiltR = 200,
            fontSize = 88,
            isFindMoveTar = true,
            colliderElasticityScale = 1.1f,
            colliderNotMoveNum = 3,
            colliderMoveScale = 0.5f,
            isSteering = 0,
            hitBackDistance = 0,
            isLoongStopDistance = 0,
            loongStopDistanceR = 0,
            name = new[] {
                "重甲盾兵",
                "重甲盾兵"
            },
            bullet = new int[] { },
            readyAtks = new[] {
                30
            },
            deadAnimType = new int[] { },
            findAtkSort = new[] {
                6,
                5,
                4,
                2,
                1,
                3,
                7
            },
        };

        // 重甲盾兵 / 重甲盾兵
        public static readonly SheepRoleTypeInfo ZHONG_JIA_DUN_BING_1 = new() {
            id = 104,
            roleType = SheepRoleType.DUN_BING,
            collideGroup = SheepCollideGroup.GROUP5,
            animId = 113,
            scale = 5f,
            detectCollideR = 2,
            collideR = 180f,
            startState = SheepRoleState.Spurt,
            formationId = 12,
            runEndX = 100,
            runSpeed = 3500,
            walkSpeed = 700,
            isSpurtAnim = false,
            skillSpurt = 0,
            skillIn = 0,
            hp = 31544.4444444444f,
            atk = 650,
            findR = 1,
            atkType = SheepRoleAtkType.Nearest,
            atkMoveType = 1,
            atkMinMoveR = 25,
            atkR = 75,
            atkCd = 0f,
            finishAtk = 65,
            splitN = 3,
            spiltR = 200,
            fontSize = 88,
            isFindMoveTar = true,
            colliderElasticityScale = 1.1f,
            colliderNotMoveNum = 3,
            colliderMoveScale = 0.5f,
            isSteering = 0,
            hitBackDistance = 0,
            isLoongStopDistance = 0,
            loongStopDistanceR = 0,
            name = new[] {
                "重甲盾兵",
                "重甲盾兵"
            },
            bullet = new int[] { },
            readyAtks = new[] {
                30
            },
            deadAnimType = new int[] { },
            findAtkSort = new[] {
                6,
                5,
                4,
                2,
                1,
                3,
                7
            },
        };

        // 弓箭手 / 弓箭手
        public static readonly SheepRoleTypeInfo GONG_JIAN_SHOU = new() {
            id = 21,
            roleType = SheepRoleType.GONG_JIAN_SHOU,
            collideGroup = SheepCollideGroup.GROUP2,
            animId = 102,
            scale = 5f,
            detectCollideR = 3,
            collideR = 180f,
            startState = SheepRoleState.Spurt,
            formationId = 3,
            runEndX = 0,
            runSpeed = 3500,
            walkSpeed = 700,
            isSpurtAnim = false,
            skillSpurt = 30001,
            skillIn = 0,
            hp = 3569f,
            atk = 170,
            findR = 7,
            atkType = SheepRoleAtkType.Throw,
            atkMoveType = 1,
            atkMinMoveR = 1500,
            atkR = 3000,
            atkCd = 0.27f,
            finishAtk = 15,
            splitN = 2,
            spiltR = 100,
            fontSize = 56,
            isFindMoveTar = true,
            colliderElasticityScale = 1.5f,
            colliderNotMoveNum = 5,
            colliderMoveScale = 0.2f,
            isSteering = 1,
            hitBackDistance = 0,
            isLoongStopDistance = 1,
            loongStopDistanceR = 3000,
            name = new[] {
                "弓箭手",
                "弓箭手"
            },
            bullet = new[] {
                1,
                1
            },
            readyAtks = new[] {
                6
            },
            deadAnimType = new int[] { },
            findAtkSort = new[] {
                4,
                5,
                3,
                7,
                6,
                2,
                1
            },
        };

        // 游侠 / 游侠
        public static readonly SheepRoleTypeInfo YOU_XIA = new() {
            id = 28,
            roleType = SheepRoleType.GONG_JIAN_SHOU,
            collideGroup = SheepCollideGroup.GROUP2,
            animId = 112,
            scale = 5f,
            detectCollideR = 3,
            collideR = 180f,
            startState = SheepRoleState.Spurt,
            formationId = 3,
            runEndX = 0,
            runSpeed = 3500,
            walkSpeed = 700,
            isSpurtAnim = false,
            skillSpurt = 30001,
            skillIn = 0,
            hp = 4163.83333333333f,
            atk = 170,
            findR = 7,
            atkType = SheepRoleAtkType.Throw,
            atkMoveType = 1,
            atkMinMoveR = 1500,
            atkR = 3000,
            atkCd = 0.27f,
            finishAtk = 15,
            splitN = 2,
            spiltR = 100,
            fontSize = 56,
            isFindMoveTar = true,
            colliderElasticityScale = 1.5f,
            colliderNotMoveNum = 5,
            colliderMoveScale = 0.2f,
            isSteering = 1,
            hitBackDistance = 0,
            isLoongStopDistance = 1,
            loongStopDistanceR = 3000,
            name = new[] {
                "游侠",
                "游侠"
            },
            bullet = new[] {
                1,
                1
            },
            readyAtks = new[] {
                6
            },
            deadAnimType = new int[] { },
            findAtkSort = new[] {
                4,
                5,
                3,
                7,
                6,
                2,
                1
            },
        };

        // 游侠 / 游侠
        public static readonly SheepRoleTypeInfo YOU_XIA_1 = new() {
            id = 103,
            roleType = SheepRoleType.GONG_JIAN_SHOU,
            collideGroup = SheepCollideGroup.GROUP2,
            animId = 112,
            scale = 5f,
            detectCollideR = 3,
            collideR = 180f,
            startState = SheepRoleState.Spurt,
            formationId = 11,
            runEndX = 0,
            runSpeed = 3500,
            walkSpeed = 700,
            isSpurtAnim = false,
            skillSpurt = 30001,
            skillIn = 0,
            hp = 8235.66666666667f,
            atk = 170,
            findR = 7,
            atkType = SheepRoleAtkType.Throw,
            atkMoveType = 1,
            atkMinMoveR = 1500,
            atkR = 3000,
            atkCd = 0.27f,
            finishAtk = 15,
            splitN = 2,
            spiltR = 100,
            fontSize = 56,
            isFindMoveTar = true,
            colliderElasticityScale = 1.5f,
            colliderNotMoveNum = 5,
            colliderMoveScale = 0.2f,
            isSteering = 1,
            hitBackDistance = 0,
            isLoongStopDistance = 1,
            loongStopDistanceR = 3000,
            name = new[] {
                "游侠",
                "游侠"
            },
            bullet = new[] {
                1,
                1
            },
            readyAtks = new[] {
                6
            },
            deadAnimType = new int[] { },
            findAtkSort = new[] {
                4,
                5,
                3,
                7,
                6,
                2,
                1
            },
        };

        // 冲锋兵 / 冲锋兵
        public static readonly SheepRoleTypeInfo CHONG_FENG_BING = new() {
            id = 20,
            roleType = SheepRoleType.CHONG_FENG_BING,
            collideGroup = SheepCollideGroup.GROUP4,
            animId = 104,
            scale = 7f,
            detectCollideR = 5,
            collideR = 240f,
            startState = SheepRoleState.Charge,
            formationId = 4,
            runEndX = 200,
            runSpeed = 4500,
            walkSpeed = 700,
            isSpurtAnim = true,
            skillSpurt = 110001,
            skillIn = 0,
            hp = 38800f,
            atk = 800,
            findR = 1,
            atkType = SheepRoleAtkType.Nearest,
            atkMoveType = 1,
            atkMinMoveR = 150,
            atkR = 200,
            atkCd = 0f,
            finishAtk = 70,
            splitN = 3,
            spiltR = 200,
            fontSize = 80,
            isFindMoveTar = true,
            colliderElasticityScale = 1.5f,
            colliderNotMoveNum = 5,
            colliderMoveScale = 0.2f,
            isSteering = 1,
            hitBackDistance = 0,
            isLoongStopDistance = 0,
            loongStopDistanceR = 0,
            name = new[] {
                "冲锋兵",
                "冲锋兵"
            },
            bullet = new int[] { },
            readyAtks = new[] {
                20
            },
            deadAnimType = new int[] { },
            findAtkSort = new int[] { },
        };

        // 大冲锋兵 / 大冲锋兵
        public static readonly SheepRoleTypeInfo DA_CHONG_FENG_BING = new() {
            id = 27,
            roleType = SheepRoleType.CHONG_FENG_BING,
            collideGroup = SheepCollideGroup.GROUP4,
            animId = 114,
            scale = 7f,
            detectCollideR = 5,
            collideR = 240f,
            startState = SheepRoleState.Charge,
            formationId = 4,
            runEndX = 200,
            runSpeed = 4500,
            walkSpeed = 700,
            isSpurtAnim = true,
            skillSpurt = 110001,
            skillIn = 0,
            hp = 45266.6666666667f,
            atk = 800,
            findR = 1,
            atkType = SheepRoleAtkType.Nearest,
            atkMoveType = 1,
            atkMinMoveR = 150,
            atkR = 200,
            atkCd = 0f,
            finishAtk = 70,
            splitN = 3,
            spiltR = 200,
            fontSize = 80,
            isFindMoveTar = true,
            colliderElasticityScale = 1.5f,
            colliderNotMoveNum = 5,
            colliderMoveScale = 0.2f,
            isSteering = 1,
            hitBackDistance = 0,
            isLoongStopDistance = 0,
            loongStopDistanceR = 0,
            name = new[] {
                "大冲锋兵",
                "大冲锋兵"
            },
            bullet = new int[] { },
            readyAtks = new[] {
                20
            },
            deadAnimType = new int[] { },
            findAtkSort = new int[] { },
        };

        // 大冲锋兵 / 大冲锋兵
        public static readonly SheepRoleTypeInfo DA_CHONG_FENG_BING_1 = new() {
            id = 106,
            roleType = SheepRoleType.CHONG_FENG_BING,
            collideGroup = SheepCollideGroup.GROUP4,
            animId = 114,
            scale = 7f,
            detectCollideR = 5,
            collideR = 240f,
            startState = SheepRoleState.Charge,
            formationId = 12,
            runEndX = 200,
            runSpeed = 4500,
            walkSpeed = 700,
            isSpurtAnim = true,
            skillSpurt = 110001,
            skillIn = 0,
            hp = 44244.4444444444f,
            atk = 800,
            findR = 1,
            atkType = SheepRoleAtkType.Nearest,
            atkMoveType = 1,
            atkMinMoveR = 150,
            atkR = 200,
            atkCd = 0f,
            finishAtk = 70,
            splitN = 3,
            spiltR = 200,
            fontSize = 80,
            isFindMoveTar = true,
            colliderElasticityScale = 1.5f,
            colliderNotMoveNum = 5,
            colliderMoveScale = 0.2f,
            isSteering = 1,
            hitBackDistance = 0,
            isLoongStopDistance = 0,
            loongStopDistanceR = 0,
            name = new[] {
                "大冲锋兵",
                "大冲锋兵"
            },
            bullet = new int[] { },
            readyAtks = new[] {
                20
            },
            deadAnimType = new int[] { },
            findAtkSort = new int[] { },
        };

        // 羊神 / 狼神
        public static readonly SheepRoleTypeInfo YANG_SHEN = new() {
            id = 24,
            roleType = SheepRoleType.YANG_SHEN,
            collideGroup = SheepCollideGroup.GROUP8,
            animId = 107,
            scale = 20f,
            detectCollideR = 6,
            collideR = 300f,
            startState = 0,
            formationId = 7,
            runEndX = 0,
            runSpeed = 3000,
            walkSpeed = 600,
            isSpurtAnim = false,
            skillSpurt = 0,
            skillIn = 10004,
            hp = 125000f,
            atk = 3600,
            findR = 2,
            atkType = SheepRoleAtkType.Nearest,
            atkMoveType = 0,
            atkMinMoveR = 500,
            atkR = 300,
            atkCd = 0.2f,
            finishAtk = 60,
            splitN = 8,
            spiltR = 800,
            fontSize = 96,
            isFindMoveTar = true,
            colliderElasticityScale = 1.1f,
            colliderNotMoveNum = 1,
            colliderMoveScale = 0.1f,
            isSteering = 0,
            hitBackDistance = 0,
            isLoongStopDistance = 0,
            loongStopDistanceR = 0,
            name = new[] {
                "羊神",
                "狼神"
            },
            bullet = new int[] { },
            readyAtks = new[] {
                21
            },
            deadAnimType = new int[] { },
            findAtkSort = new[] {
                8,
                7,
                6,
                5,
                4,
                3,
                2,
                1
            },
        };

        // 麒麟 / 麒麟
        public static readonly SheepRoleTypeInfo QI_LIN = new() {
            id = 30,
            roleType = SheepRoleType.QI_LIN,
            collideGroup = SheepCollideGroup.GROUP6,
            animId = 108,
            scale = 12f,
            detectCollideR = 10,
            collideR = 480f,
            startState = SheepRoleState.ChargePlus,
            formationId = 9,
            runEndX = 3200,
            runSpeed = 4500,
            walkSpeed = 700,
            isSpurtAnim = true,
            skillSpurt = 160001,
            skillIn = 0,
            hp = 3884666.66666667f,
            atk = 3000,
            findR = 1,
            atkType = SheepRoleAtkType.Nearest,
            atkMoveType = 1,
            atkMinMoveR = 150,
            atkR = 1200,
            atkCd = 0f,
            finishAtk = 70,
            splitN = 3,
            spiltR = 400,
            fontSize = 80,
            isFindMoveTar = true,
            colliderElasticityScale = 1.5f,
            colliderNotMoveNum = 5,
            colliderMoveScale = 0.2f,
            isSteering = 1,
            hitBackDistance = 1000,
            isLoongStopDistance = 0,
            loongStopDistanceR = 0,
            name = new[] {
                "麒麟",
                "麒麟"
            },
            bullet = new int[] { },
            readyAtks = new[] {
                20
            },
            deadAnimType = new int[] { },
            findAtkSort = new int[] { },
        };

        // Boss
        public static readonly SheepRoleTypeInfo BOSS = new() {
            id = 0,
            roleType = SheepRoleType.BOSS,
            collideGroup = SheepCollideGroup.GROUP0,
            animId = 0,
            scale = 1f,
            detectCollideR = 5,
            collideR = 600f,
            startState = SheepRoleState.Spurt,
            formationId = 0,
            runEndX = 0,
            runSpeed = 400,
            walkSpeed = 70,
            isSpurtAnim = false,
            skillSpurt = 0,
            skillIn = 0,
            hp = 1000f,
            atk = 400,
            findR = 3,
            atkType = SheepRoleAtkType.Nearest,
            atkMoveType = 0,
            atkMinMoveR = 0,
            atkR = 600,
            atkCd = 4f,
            finishAtk = 12,
            splitN = 5,
            spiltR = 500,
            fontSize = 0,
            isFindMoveTar = false,
            colliderElasticityScale = 1.5f,
            colliderNotMoveNum = 5,
            colliderMoveScale = 0.2f,
            isSteering = 1,
            hitBackDistance = 0,
            isLoongStopDistance = 0,
            loongStopDistanceR = 0,
            name = new string[] { },
            bullet = new int[] { },
            readyAtks = new[] {
                18
            },
            deadAnimType = new int[] { },
            findAtkSort = new int[] { },
        };

        // 冰法师 / 冰法师 (一次性全场冰冻大招单位, 出场即释放、放完即消耗)
        // 复用弓箭手的图集(animId=102)与克制关系(roleType=GONG_JIAN_SHOU)
        public static readonly SheepRoleTypeInfo BING_FA_SHI = new() {
            id = 31,
            roleType = SheepRoleType.GONG_JIAN_SHOU,
            collideGroup = SheepCollideGroup.GROUP2,
            animId = 102,
            scale = 5f,
            detectCollideR = 3,
            collideR = 180f,
            startState = SheepRoleState.Spurt,
            formationId = 3,
            runEndX = 0,
            runSpeed = 3500,
            walkSpeed = 700,
            isSpurtAnim = false,
            skillSpurt = 170001,
            skillIn = 0,
            hp = 2600f,
            atk = 140,
            findR = 7,
            atkType = SheepRoleAtkType.Throw,
            atkMoveType = 1,
            atkMinMoveR = 1500,
            atkR = 2600,
            atkCd = 0.35f,
            finishAtk = 15,
            splitN = 2,
            spiltR = 100,
            fontSize = 56,
            isFindMoveTar = true,
            colliderElasticityScale = 1.5f,
            colliderNotMoveNum = 5,
            colliderMoveScale = 0.2f,
            isSteering = 1,
            hitBackDistance = 0,
            isLoongStopDistance = 1,
            loongStopDistanceR = 3000,
            name = new[] {
                "冰法师",
                "冰法师"
            },
            // 它出场即施法、施法完即死亡, 永远不会进入 Attack 状态, 所以攻击相关配置留空
            bullet = new int[] { },
            readyAtks = new int[] { },
            deadAnimType = new int[] { },
            findAtkSort = new[] {
                4,
                5,
                3,
                7,
                6,
                2,
                1
            },
        };

        // 炮击手 / 炮击手 (一次性天降轰炸单位, 出场即释放、放完即消耗)
        // 复用炮车的图集(animId=106)与克制关系(roleType=PAO_CHE)
        public static readonly SheepRoleTypeInfo PAO_JI_SHOU = new() {
            id = 32,
            roleType = SheepRoleType.PAO_CHE,
            collideGroup = SheepCollideGroup.GROUP3,
            animId = 106,
            scale = 9f,
            detectCollideR = 4,
            collideR = 225f,
            startState = SheepRoleState.Spurt,
            formationId = 5,
            runEndX = 0,
            runSpeed = 3500,
            walkSpeed = 700,
            isSpurtAnim = false,
            skillSpurt = 180001,
            skillIn = 0,
            isNotCollide = false,
            hp = 6000f,
            atk = 900,
            findR = 7,
            atkType = SheepRoleAtkType.Throw,
            atkMoveType = 0,
            atkMinMoveR = 2000,
            atkR = 5000,
            atkCd = 3.7f,
            finishAtk = 40,
            splitN = 4,
            spiltR = 540,
            fontSize = 72,
            isFindMoveTar = true,
            colliderElasticityScale = 1.5f,
            colliderNotMoveNum = 5,
            colliderMoveScale = 0.2f,
            isSteering = 1,
            hitBackDistance = 0,
            isLoongStopDistance = 0,
            loongStopDistanceR = 0,
            // 注意: 炮弹伤害 = 本兵种的 atk × 克制系数 (子弹的 atkBet 不参与计算),
            //       想调轰炸威力改这里的 atk
            name = new[] {
                "炮击手",
                "炮击手"
            },
            // 出场即施法、施法完即死亡, 永远不会进入 Attack 状态, 所以攻击相关配置留空
            bullet = new int[] { },
            readyAtks = new int[] { },
            deadAnimType = new int[] { },
            findAtkSort = new int[] { },
        };

        // 自爆兵 / 自爆兵 (无视碰撞, 一路冲到最前方, 碰到敌人就自爆)
        // 复用冲锋兵的图集(animId=104, 有 Spurt / Boom 动画)与克制关系
        public static readonly SheepRoleTypeInfo ZI_BAO_BING = new() {
            id = 33,
            roleType = SheepRoleType.CHONG_FENG_BING,
            collideGroup = SheepCollideGroup.GROUP4,
            animId = 104,
            scale = 5f,
            detectCollideR = 2,
            collideR = 150f,
            startState = SheepRoleState.Spurt,
            formationId = 4,
            runEndX = 0,
            runSpeed = 4200,
            walkSpeed = 1200,
            isSpurtAnim = true,
            skillSpurt = 120002,
            skillIn = 0,
            // 关键: 无视碰撞, 才能径直穿过人群冲到最前方
            isNotCollide = true,
            hp = 1500f,
            atk = 600,
            findR = 2,
            atkType = SheepRoleAtkType.Nearest,
            atkMoveType = 0,
            atkMinMoveR = 0,
            atkR = 200,
            atkCd = 1f,
            finishAtk = 10,
            splitN = 2,
            spiltR = 200,
            fontSize = 56,
            isFindMoveTar = false,
            colliderElasticityScale = 1f,
            colliderNotMoveNum = 99,
            colliderMoveScale = 1f,
            isSteering = 1,
            hitBackDistance = 0,
            isLoongStopDistance = 0,
            loongStopDistanceR = 0,
            // 注意: 自爆伤害 = 本兵种的 atk × 克制系数 (子弹的 atkBet 不参与计算),
            //       想调自爆威力改这里的 atk
            name = new[] {
                "自爆兵",
                "自爆兵"
            },
            bullet = new int[] { },
            readyAtks = new int[] { },
            deadAnimType = new int[] { },
            findAtkSort = new int[] { },
        };

        // 解冻祭司 / 解冻祭司 (一次性全体解冻单位)
        // 复用盾兵的图集(animId=103)与克制关系(roleType=DUN_BING)
        public static readonly SheepRoleTypeInfo JIE_DONG_JI_SI = new() {
            id = 34,
            roleType = SheepRoleType.DUN_BING,
            collideGroup = SheepCollideGroup.GROUP1,
            animId = 103,
            scale = 5f,
            detectCollideR = 2,
            collideR = 160f,
            startState = SheepRoleState.Spurt,
            formationId = 2,
            runEndX = 0,
            runSpeed = 3500,
            walkSpeed = 700,
            isSpurtAnim = false,
            skillSpurt = 190001,
            skillIn = 0,
            isNotCollide = false,
            hp = 3000f,
            atk = 100,
            findR = 3,
            atkType = SheepRoleAtkType.Nearest,
            atkMoveType = 0,
            atkMinMoveR = 0,
            atkR = 200,
            atkCd = 1f,
            finishAtk = 10,
            splitN = 1,
            spiltR = 100,
            fontSize = 56,
            isFindMoveTar = true,
            colliderElasticityScale = 1.3f,
            colliderNotMoveNum = 5,
            colliderMoveScale = 0.5f,
            isSteering = 1,
            hitBackDistance = 0,
            isLoongStopDistance = 0,
            loongStopDistanceR = 0,
            name = new[] {
                "解冻祭司",
                "解冻祭司"
            },
            bullet = new int[] { },
            readyAtks = new int[] { },
            deadAnimType = new int[] { },
            findAtkSort = new int[] { },
        };

        // 战鼓手 / 战鼓手 (出场进入 Buff 状态, 持续期间我方攻速翻倍, 结束后继续战斗)
        // 复用小兵的图集(animId=100)与克制关系(roleType=XIAO_BING)
        public static readonly SheepRoleTypeInfo ZHAN_GU_SHOU = new() {
            id = 35,
            roleType = SheepRoleType.XIAO_BING,
            collideGroup = SheepCollideGroup.GROUP0,
            animId = 100,
            scale = 3.6f,
            detectCollideR = 2,
            collideR = 120f,
            startState = SheepRoleState.Spurt,
            formationId = 1,
            runEndX = 100,
            runSpeed = 3500,
            walkSpeed = 500,
            isSpurtAnim = false,
            skillSpurt = 90001,
            skillIn = 0,
            isNotCollide = false,
            hp = 2000f,
            atk = 30,
            findR = 1,
            atkType = SheepRoleAtkType.Nearest,
            atkMoveType = 0,
            atkMinMoveR = 0,
            atkR = 55,
            atkCd = 0.45f,
            finishAtk = 16,
            splitN = 1,
            spiltR = 55,
            fontSize = 48,
            isFindMoveTar = true,
            colliderElasticityScale = 1.3f,
            colliderNotMoveNum = 5,
            colliderMoveScale = 0.8f,
            isSteering = 1,
            hitBackDistance = 0,
            isLoongStopDistance = 0,
            loongStopDistanceR = 0,
            name = new[] {
                "战鼓手",
                "战鼓手"
            },
            bullet = new int[] { },
            readyAtks = new[] {
                8
            },
            deadAnimType = new int[] { },
            findAtkSort = new int[] { },
        };

        // 号令官 / 号令官 (一次性全军技能重置单位)
        // 复用羊神的图集(animId=107, 有 Palm 动画)
        // 注意: 这里刻意不用 roleType = YANG_SHEN —— SheepMgr.onGameRun() 会把所有
        //       YANG_SHEN 单位强制改成 Palm 状态 + readySkillId = 70002, 会覆盖掉本单位的支援技能.
        //       改用 XUAN_FENG_ZHAN 的克制关系, 规避这个跨技能冲突.
        public static readonly SheepRoleTypeInfo HAO_LING_GUAN = new() {
            id = 36,
            roleType = SheepRoleType.XUAN_FENG_ZHAN,
            collideGroup = SheepCollideGroup.GROUP5,
            animId = 107,
            scale = 10f,
            detectCollideR = 3,
            collideR = 300f,
            startState = SheepRoleState.Spurt,
            formationId = 6,
            runEndX = 0,
            runSpeed = 3000,
            walkSpeed = 600,
            isSpurtAnim = false,
            skillSpurt = 190002,
            skillIn = 0,
            isNotCollide = false,
            hp = 8000f,
            atk = 500,
            findR = 3,
            atkType = SheepRoleAtkType.Nearest,
            atkMoveType = 0,
            atkMinMoveR = 0,
            atkR = 400,
            atkCd = 2f,
            finishAtk = 20,
            splitN = 2,
            spiltR = 300,
            fontSize = 72,
            isFindMoveTar = true,
            colliderElasticityScale = 1.3f,
            colliderNotMoveNum = 5,
            colliderMoveScale = 0.5f,
            isSteering = 1,
            hitBackDistance = 0,
            isLoongStopDistance = 0,
            loongStopDistanceR = 0,
            name = new[] {
                "号令官",
                "号令官"
            },
            bullet = new int[] { },
            readyAtks = new int[] { },
            deadAnimType = new int[] { },
            findAtkSort = new int[] { },
        };

        public static readonly SheepRoleTypeInfo[] _data = {
            BOSS,
            XIAN_FENG_BING,
            CI_KE,
            DA_CI_KE,
            DUN_BING,
            XUAN_FENG_ZHAN,
            CHONG_FENG_BING,
            GONG_JIAN_SHOU,
            XIAO_BING,
            PAO_CHE,
            YANG_SHEN,
            ZHONG_JIA_DUN_BING,
            DA_XUAN_FENG_ZHAN,
            DA_CHONG_FENG_BING,
            YOU_XIA,
            HUANG_JIN_PAO_CHE,
            QI_LIN,
            CI_KE_1,
            YOU_XIA_1,
            ZHONG_JIA_DUN_BING_1,
            DA_CHONG_FENG_BING_1,
            BING_FA_SHI,
            PAO_JI_SHOU,
            ZI_BAO_BING,
            JIE_DONG_JI_SI,
            ZHAN_GU_SHOU,
            HAO_LING_GUAN,
        };

        private static readonly Dictionary<int, SheepRoleTypeInfo> _map = CreateMap();

        public static IReadOnlyList<SheepRoleTypeInfo> All => _data;

        public static SheepRoleTypeInfo GetById(int id) {
            if (_map.TryGetValue(id, out SheepRoleTypeInfo info)) {
                return info;
            }

            throw new KeyNotFoundException($"没有找到 SheepRoleTypeInfo，id = {id}");
        }

        public static bool TryGetById(int id, out SheepRoleTypeInfo info) {
            return _map.TryGetValue(id, out info);
        }

        private static Dictionary<int, SheepRoleTypeInfo> CreateMap() {
            var map = new Dictionary<int, SheepRoleTypeInfo>(_data.Length);

            foreach (SheepRoleTypeInfo info in _data) {
                if (map.ContainsKey(info.id)) {
                    throw new InvalidOperationException($"重复的兵种配置 ID：{info.id}");
                }

                map.Add(info.id, info);
            }

            return map;
        }
    }
}