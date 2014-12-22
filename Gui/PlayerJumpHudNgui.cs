﻿using UnityEngine;
using System.Collections;


public class PlayerJumpHudNgui : PlayerHudNgui
{
   
    protected void Update()
    {
        if (LocalPlayer) Stats = LocalPlayer.GetPlayerStats();
        Vector3 velocity = LocalPlayer.GetCurrentPawn().GetVelocity();
        healthLabel.text = velocity.magnitude.ToString("0.0"); ;

         Kills.text  =         JumpAssist.GetMeFlat(velocity).magnitude.ToString("0.0"); ;
         Assists.text = velocity.y.ToString("0.0");
    }
  
}
