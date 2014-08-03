using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using Sfs2X.Entities.Data;
using nstuff.juggerfall.extension.models;
 

public class PlayerView : MonoBehaviour {

    public static Dictionary<int,PlayerView> allPlayer = new Dictionary<int,PlayerView>();

    private int ownerId;

    public Player observed;
   
    public bool isMine;

    public void SetId(int userId)
    {
        ownerId = userId;
        isMine = NetworkController.smartFox.MySelf.Id == ownerId;
        observed = GetComponent<Player>();
        allPlayer.Add(ownerId, this);
    }

    public int GetId()
    {
        return ownerId;
    }
    public void SetNameUID(string UID, string PlayerName)
    {
        NetworkController.Instance.SetNameUIDRequest( UID,  PlayerName);
    }
    public void SetTeam(int team)
    {
        NetworkController.Instance.SetTeamRequest(team);
    }

    void Destroy()
    {
        allPlayer.Remove(ownerId);
    }



    public void NetUpdate(PlayerModel player)
    {
        Debug.Log("NET UPDATE " + player.uid + " name " + player.name + "team" + player.team);
        observed.UID = player.uid;
        observed.team =  player.team;
        observed.PlayerName = player.name;
        observed.Score.Kill = player.kill;
        observed.Score.Assist = player.assist;
        observed.Score.Death = player.death;
        observed.Score.RobotKill = player.robotKill;
    }
}
