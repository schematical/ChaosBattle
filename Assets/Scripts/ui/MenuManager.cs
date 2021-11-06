﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public MainMenuPanel MainMenuPanel;
    public DebugPanel debugPanel;
    public ChaosEntityDetailPanel chaosEntityDetailPanel;
    // Start is called before the first frame update
    void Start()
    {
        HideOverlays();
        MainMenuPanel.gameObject.SetActive(true);
    }

    public void HideOverlays()
    {
           chaosEntityDetailPanel.gameObject.SetActive(false);
           debugPanel.gameObject.SetActive(false);
    }

    public bool IsBigMenuOpen()
    {
        if (
            MainMenuPanel.isActiveAndEnabled
        )
        {
            return true;
        }

        return false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HideAllMenues()
    {
     
    }
}
