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

    private UnityAction outActionFunc;

    private void Awake()
    {
        show_txt = transform.FindChildComponentByName<TextMeshProUGUI>("tips_txt");
        Debug.Log(show_txt);
        click_btn = transform.FindChildComponentByName<Button>("Click_Btn");
    }


    void Start()
    {
        click_btn.onClick.AddListener(OnClick);
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
        if (!ProcessController.Instance.IsGameStart)
        {
            EventManager.Instance.PublicEvent(EventType.GameStateChanged, true);
            if(outActionFunc!=null) ClickRemoveListener(outActionFunc);
        }
        UIManager.Instance.HidePopup(ui_name);
    }

    void Update()
    {
        
    }
}
