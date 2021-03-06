﻿using UnityEngine;
using System.Collections;

public class ProgressBarsOnHUD : MonoBehaviour {

	PlayerMainGui.PlayerStats Stats = null;
	public PlayerHudNgui NguiHUD;

	public UIProgressBar JetPackEnergy;
	public UISprite JetPackEnergySprite;
	public float JetPackMax;
    public bool colorDepends = true;
    public bool showOnActive = false;
	float Energy;


	
	
	// Update is called once per frame
	void Update () 
	{
        if (JetPackEnergySprite == null)
        {
            return;
        }
		if (Stats != null)
		{
			Energy = Stats.jetPackCharge;

            if (colorDepends)
            {
                if (Energy > 0.6f)
                    JetPackEnergySprite.color = Color.green;
                else if (Energy <= 0.6f && Energy > 0.3f)
                    JetPackEnergySprite.color = Color.yellow;
                else if (Energy <= 0.3f)
                    JetPackEnergySprite.color = Color.red;

            }
            if (showOnActive)
            {
                if (Energy == 1.0f)
                {
                    JetPackEnergy.alpha = 0.0f;
                }
                else
                {
                    JetPackEnergy.alpha = 1.0f;
                }
            }
			JetPackEnergy.value = Energy;
		}
		else
			Stats = NguiHUD.Stats;
	}
}
