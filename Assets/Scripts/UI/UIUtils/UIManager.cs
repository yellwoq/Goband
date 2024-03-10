using Microsoft.Cci;
using System;
using System.Collections.Generic;
using System.Reflection;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager: MonSingleton<UIManager>
{
    private Canvas m_canvas;
    private Dictionary<string, UIBase> ui_map_dic = new Dictionary<string, UIBase>();

    protected override void Awake()
    {
        base.Awake();
        m_canvas = GetComponentInParent<Canvas>();
        InitUI();
    }

    private void InitUI()
    {
        UIBase[] uibase = GetComponentsInChildren<UIBase>();
        for (int i = 0; i < uibase.Length; i++)
        {
            UIBase ui = uibase[i];
            if (!ui_map_dic.ContainsKey(ui.ui_name))
            {
                ui_map_dic.Add(ui.ui_name, ui);
            }
        }
    }

    public UIBase ShowPopup(string ui_name)
    {
        UIBase ui;
        if (! ui_map_dic.ContainsKey(ui_name))
        {
            if (! UIMapping.UI_CLS_TO_PATH.ContainsKey(ui_name))
            {
                Debug.LogError("UIÎ´½øÐÐ×¢²á£¡£¡£¡");
                return null;
            }
            Debug.Log(transform.Find(UIMapping.UI_CLS_TO_PATH[ui_name]));
            ui = transform.Find(UIMapping.UI_CLS_TO_PATH[ui_name]).GetComponent<UIBase>();
            if (ui == null)
            {
                Assembly assem = GetType().Assembly;
                Type ui_type = assem.GetType(ui_name);
                ui = transform.Find(
                    UIMapping.UI_CLS_TO_PATH[ui_name]).gameObject.AddComponent(ui_type) as UIBase;
            }
            ui.SetVisible(true);
            ui_map_dic.Add(ui_name, ui);
            return ui;
        }
        ui = ui_map_dic[ui_name];
        ui.SetVisible(true);
        return ui;
    }

    public UIBase GetUI(string ui_name)
    {
        if (!ui_map_dic.ContainsKey(ui_name))
        {
            return null;
        }
        return ui_map_dic[ui_name];
    }

    public bool HidePopup(string ui_name, bool is_destroy=false)
    {
        UIBase ui;
        if (!ui_map_dic.ContainsKey(ui_name))
        {
            return false;
        }
        ui = ui_map_dic[ui_name];
        if (!is_destroy)
        {
            ui.SetVisible(false);
        }
        else
        {
            Destroy(ui);
            ui_map_dic.Remove(ui_name);
        }
        return true;
    }
}