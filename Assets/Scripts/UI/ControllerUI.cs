using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ControllerUI : UIBase
{
    private TextMeshProUGUI show_txt;
    private Button click_btn;
    private Button easy_btn, middle_btn;

    private UnityAction outActionFunc;

    private void Awake()
    {
        show_txt = transform.FindChildComponentByName<TextMeshProUGUI>("tips_txt");
        Debug.Log(show_txt);
        click_btn = transform.FindChildComponentByName<Button>("Click_Btn");
        easy_btn = transform.FindChildComponentByName<Button>("Easy");
        middle_btn = transform.FindChildComponentByName<Button>("Middle");
    }


    void Start()
    {
        click_btn.onClick.AddListener(OnClick);
        easy_btn.onClick.AddListener(OnEasyClick);
        middle_btn.onClick.AddListener(OnMiddleClick);
    }

    private void OnMiddleClick()
    {
        ProcessController.Instance._m_ai.m_Type = AIType.MIDDLE;
        OnStart();
    }

    private void OnEasyClick()
    {
        ProcessController.Instance._m_ai.m_Type = AIType.EASY;
        OnStart();
    }

    public void SetShowTxt(string show_msg)
    {
        show_txt.text = show_msg;
    }

    public void ClickAddListener(UnityAction func)
    {
        outActionFunc = func;
        click_btn.onClick.AddListener(func);
    }

    public void ClickRemoveListener(UnityAction func)
    {
        outActionFunc = null;
        click_btn.onClick.RemoveListener(func);
    }

    void OnClick()
    {
        if (!middle_btn.IsActive())
        {
            Set_Btn_State(true);
            return;
        }
        ProcessController.Instance._m_ai.m_Type = AIType.DIFFICULT;
        OnStart();
    }

    void OnStart()
    {
        if (!ProcessController.Instance.IsGameStart)
        {
            EventManager.Instance.PublicEvent(EventType.GameStateChanged, true);
            if (outActionFunc != null) ClickRemoveListener(outActionFunc);
        }
        UIManager.Instance.HidePopup(ui_name);
        Set_Btn_State(false);
    }

    void Set_Btn_State(bool state)
    {
        middle_btn.gameObject.SetActive(state);
        easy_btn.gameObject.SetActive(state);
        click_btn.GetComponentInChildren<TextMeshProUGUI>().text = state ? "困难" : "开始游戏";
        show_txt.gameObject.SetActive(!state);
    }

    void Update()
    {
        
    }
}
