using System;
using System.Collections;
using System.Collections.Generic;
using rvb.scripts;
using UnityEngine;
using UnityEngine.UIElements;

public class GameUI : MonoBehaviour
{
    private VisualElement redPanel;
    private VisualElement bluePanel;

    void OnEnable()
    {
        // 获取 UI 元素
        var root = GetComponent<UIDocument>().rootVisualElement;

        redPanel = root.Q<VisualElement>("red-panel");
        bluePanel = root.Q<VisualElement>("blue-panel");

        // 清空默认测试按钮
        redPanel.Clear();
        bluePanel.Clear();
    }

    private IEnumerator Start()
    {
        // 等待 SheepMgr 初始化
        while (SheepMgr.inc == null)
        {
            yield return null;
        }

        var sheepMgr = SheepMgr.inc;

        // 创建兵种生成按钮
        foreach (var roleIdValue in SheepRoleTypeInfos.All)
        {
            int roleId = roleIdValue.id;

            if (roleId == 0)
            {
                continue;
            }


            var roleInfo = SheepRoleTypeInfo.getById(roleId);

            var roleName = roleInfo == null ? roleId.ToString() : roleInfo.name[0].ToString();

            addRedButton(
                $"{roleName} ×10",
                () =>
                {
                    sheepMgr.produce_pets(
                        roleId,
                        10,
                        SheepCamp.Red
                    );
                }
            );

            addBlueButton(
                $"{roleName} ×10",
                () =>
                {
                    sheepMgr.produce_pets(
                        roleId,
                        10,
                        SheepCamp.Blue
                    );
                }
            );
        }
    }

    public Button addRedButton(string title, Action callback)
    {
        var button = new Button(callback)
        {
            text = title
        };

        button.AddToClassList("red-button");
        redPanel.Add(button);

        return button;
    }

    public Button addBlueButton(string title, Action callback)
    {
        var button = new Button(callback)
        {
            text = title
        };

        button.AddToClassList("blue-button");
        bluePanel.Add(button);

        return button;
    }
}