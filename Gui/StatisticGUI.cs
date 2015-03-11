using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class StatisticGUI : MonoBehaviour {
	

	public UIPanel statisticPanel;

    public MainMenuGUI MainMenu;

	
	public void ShowProfile()
	{
        StatisticGUIEntry[] etnrys = FindObjectsOfType<StatisticGUIEntry>();
        foreach (StatisticGUIEntry entry in etnrys)
        {
            entry.Init(GlobalPlayer.instance.GetStatisticData(entry.key));
        }
		
     
		statisticPanel.alpha= 1.0f;
	
      
	}
    public void HideAllPanel()
    {
        statisticPanel.alpha = 0.0f;
    }
	
}
