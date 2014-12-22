using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Variables;
using Sfs2X.Requests;
using Sfs2X.Logging;
using Sfs2X.Entities.Data;
using nstuff.juggerfall.extension.models;


public class JumpServerHolder : ServerHolder 
{

    public override void AfterPlayerCreate()
    {
       
        
        Player.localPlayer.selectedBot = 1;
        Player.localPlayer.selected = Choice._Player;
        Player.localPlayer.isStarted = true;
        Player.localPlayer.SetTeam(1);
    }
	








}
