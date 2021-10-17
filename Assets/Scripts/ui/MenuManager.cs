﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    /*public DialogPanel DialogPanel;

    public IslandDetailPanel IslandDetailPanel;

    public BoatSidePanel BoatSidePanel;
    public IslandSidePanel IslandSidePanel;

    public TopBarPanel TopBarPanel;

    public BoatRenderPanel BoatRightPanel;*/

    public ChaosEntityDetailPanel chaosEntityDetailPanel;
    // Start is called before the first frame update
    void Start()
    {
        HideOverlays();
    }

    public void HideOverlays()
    {
           chaosEntityDetailPanel.gameObject.SetActive(false);

    }

    public bool IsBigMenuOpen()
    {
        /*if (
            DialogPanel.gameObject.activeInHierarchy ||
            IslandDetailPanel.gameObject.activeInHierarchy
        )
        {
            return true;
        }*/

        return false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
