using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using Sfs2X.Entities.Data;
using nstuff.juggerfall.extension.models;
using Sfs2X.Entities;
 

public class PlayerView : MonoBehaviour {

    public static Dictionary<int,PlayerView> allPlayer = new Dictionary<int,PlayerView>();

    private int ownerId;

    public Player observed;
   
    public bool isMine;

    public void SetId(int userId)
    {
     
        ownerId = userId;
        if (NetworkController.Instance.isSingle )
        {
            isMine = true;
        }
        else
        {
            if (NetworkController.smartFox == null||NetworkController.smartFox.MySelf == null)
            {
                NetworkController.Instance.BackToMenu();
            }
            isMine = NetworkController.smartFox.MySelf.Id == ownerId;

          
        }
        observed = GetComponent<Player>();
        allPlayer.Add(ownerId, this);
    }

    public int GetId()
    {
        return ownerId;
    }
    public void SetNameUID(string UID, string PlayerName)
    {
        if (NetworkController.Instance.isSingle)
        {
            return;
        }
        NetworkController.Instance.SetNameUIDRequest( UID,  PlayerName);
    }
    public void SetTeam(int team)
    {
        if (NetworkController.Instance.isSingle)
        {
            return;
        }
        NetworkController.Instance.SetTeamRequest(team);
    }

    void Destroy()
    {
        allPlayer.Remove(ownerId);
    }



    public void NetUpdate(PlayerModel player)
    {
//        Debug.Log("NET UPDATE " + player.uid + " name " + player.name + "team" + player.team + " player.kill" + player.kill);
        observed.UID = player.uid;
        observed.team =  player.team;
        observed.PlayerName = player.name;
        observed.Score.Kill = player.kill;
        observed.Score.AIKill = player.aikill;
        observed.Score.Assist = player.assist;
        observed.Score.Death = player.death;
        observed.Score.RobotKill = player.robotKill;
        User user = NetworkController.Instance.GetUserById(ownerId);
        if (user.GetVariable("lvl") != null)
        {
            observed.Score.Lvl = user.GetVariable("lvl").GetIntValue();
            observed.Score.rating = player.rating;
        }
    }

    public void SendStats()
    {
        if (NetworkController.Instance.isSingle)
        {
            return;
        }
        PlayerModel player = new PlayerModel();
      
        player.rating    = observed.Score.rating ;
        player.kill = observed.Score.Kill;
        player.aikill = observed.Score.AIKill;
        player.assist = observed.Score.Assist;
        player.death= observed.Score.Death;
        NetworkController.Instance.SetStats(player);
    }
}
