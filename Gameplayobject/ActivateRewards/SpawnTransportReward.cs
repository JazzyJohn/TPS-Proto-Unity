using UnityEngine;
using System.Collections;
using Sfs2X.Entities.Data;

public class SpawnTransportReward : ActivateReward {

   

    public string[] ghost;

    public LayerMask robotLayer;

    private Transform ghostObj;

    private GhostObject ghostClass;

    public float maxDistance;

    public bool canSpawn;

    public override void Select(Pawn pawn)
    {

        if (ghostObj != null)
        {
            return;
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

        base.Select(pawn);
    }

    public void Update()
    {
        if (ghostObj != null)
        {
            ghostObj.position = GetPosition();
            RaycastHit hitinfo;
            if (Physics.SphereCast(ghostObj.position + Vector3.up * ghostClass.size, ghostClass.size, Vector3.up, out hitinfo, 100.0f, ghostClass.blockLayer))
            {
                ghostClass.MakeNormal();
                canSpawn = false;
            }
            else
            {
                ghostClass.MakeBad();
                canSpawn = true;
            }
        }

    }
    public Vector3 GetPosition()
    {
        Ray centerofScreen = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1f));
      //  Debug.Log(Camera.main.transform.position);
        RaycastHit hitinfo;
        if (Physics.Raycast(centerofScreen, out hitinfo, maxDistance, robotLayer))
        {
           // Debug.Log(hitinfo.point);
            return hitinfo.point;
        }
        else
        {
            //Debug.Log(centerofScreen.GetPoint(maxDistance));
            return centerofScreen.GetPoint(maxDistance); //+ centerofScreen.direction * maxDistance;
        }
    }

    public virtual void Deselect(Pawn pawn)
    {
        Destroy(ghostObj.gameObject);
        base.Deselect(pawn);
    }

    public override void Activate(Pawn pawn)
    {
        if (!canSpawn)
        {
            return;
        }
        base.Activate(pawn);

        Player.localPlayer.SpawnBot(ghostObj.position+ Vector3.up*10.0f, ghostObj.rotation);
        Destroy(ghostObj.gameObject);

    }

}
