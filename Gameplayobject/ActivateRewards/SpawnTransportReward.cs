﻿using UnityEngine;
using System.Collections;
using Sfs2X.Entities.Data;

public class SpawnTransportReward : ActivateReward {


    public SpawnRewardState state;


    public string blockTag;

    public string teleportTag;

    public string[] ghost;

    public LayerMask robotLayer;

    private Transform ghostObj;

    private GhostObject ghostClass;

    public float maxDistance;

    public bool canSpawn;

    public override bool Select(Pawn pawn)
    {

        if (ghostObj != null)
        {
            return false;
        }
        if (pawn.IsMount())
        {
            return false;
        }
        GameObject ghostGoBlue;
        GameObject resourceGameObject = null;
        if (PhotonResourceWrapper.allobject.ContainsKey(ghost[pawn.team - 1]))
        {
            ghostGoBlue = resourceGameObject = PhotonResourceWrapper.allobject[ghost[pawn.team - 1]];
        }
        else
        {
            ghostGoBlue = (GameObject)Resources.Load(ghost[pawn.team - 1], typeof(GameObject));

        }

        GameObject ghostGo = (GameObject)Instantiate(ghostGoBlue, GetPosition(), pawn.myTransform.rotation);
        ghostObj = ghostGo.transform;
        ghostClass = ghostGo.GetComponent<GhostObject>();

        return base.Select(pawn);
    }

    public void Update()
    {
        if (ghostObj != null)
        {
            ghostObj.position = GetPosition();
            Collider[] colliders = Physics.OverlapSphere(ghostObj.position + Vector3.up * ghostClass.size, ghostClass.size);
            state = SpawnRewardState.AIR_DROP;
            foreach (Collider zone in colliders)
            {
               // Debug.Log(zone.tag);
                if (zone.CompareTag(blockTag))
                {
                    state = SpawnRewardState.BLOCK;
                    break;
                }
                if (zone.CompareTag(teleportTag))
                {
                    state = SpawnRewardState.TELEPORT;
                  
                }
            }
            if (state == SpawnRewardState.BLOCK)
            {
                ghostClass.MakeBad();
                canSpawn = false;
            }
            else
            {
                ghostClass.MakeNormal();
                canSpawn = true;
            }
        }

    }
    public Vector3 GetPosition()
    {
        Ray centerofScreen = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1f));
      //  Debug.Log(Camera.main.transform.position);
        RaycastHit hitinfo;
        Vector3 point;
        if (Physics.Raycast(centerofScreen, out hitinfo, maxDistance, robotLayer))
        {
           // Debug.Log(hitinfo.point);
            point= hitinfo.point;
        }
        else
        {
            //Debug.Log(centerofScreen.GetPoint(maxDistance));
            point= centerofScreen.GetPoint(maxDistance); //+ centerofScreen.direction * maxDistance;
        }
        if (Physics.Raycast(point,Vector3.down, out hitinfo, 10.0f, robotLayer))
        {
            return hitinfo.point;
        }
        else
        {
            return point;
        }

    }

    public override void Deselect(Pawn pawn)
    {
        if (ghostObj != null)
        {
            Destroy(ghostObj.gameObject);
        }
        base.Deselect(pawn);
    }

    public override int Activate(Pawn pawn)
    {
        if (!canSpawn)
        {
            return 0;
        }
        GA.API.Design.NewEvent("Game:Reward:Activate:SpawmBear");
        if (state == SpawnRewardState.TELEPORT)
        {
            Player.localPlayer.SpawnBot(ghostObj.position + Vector3.up * 1.0f, ghostObj.rotation);
        }
        else
        {
            Player.localPlayer.SpawnBot(ghostObj.position + Vector3.up * 10.0f, ghostObj.rotation);
        }
        Destroy(ghostObj.gameObject);
        return base.Activate(pawn);

    }

}
