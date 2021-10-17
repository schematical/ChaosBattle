﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChaosEntityDetailPanel : MonoBehaviour
{


    private ChaosEntity _chaosEntity;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Show(ChaosEntity chaosEntity)
    {
        SetChaosEntity(chaosEntity);
        gameObject.SetActive(true);
    }

    private void SetChaosEntity(ChaosEntity chaosEntity)
    {
        _chaosEntity = chaosEntity;
    }
}
/*

public class DialogOptionButton:MonoBehaviour
{
    private DialogEventOption _dialogEventOption;
    public DialogOptionButtonSelectEvent onSelect = new DialogOptionButtonSelectEvent();

    void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }
    public void SetDialogEventOption(DialogEventOption dialogEventOption)
    {
        _dialogEventOption = dialogEventOption;
    }
    void OnClick()
    {
 
        onSelect.Invoke(_dialogEventOption);
    }
}
public class DialogOptionButtonSelectEvent : UnityEvent<DialogEventOption> {}
*/
