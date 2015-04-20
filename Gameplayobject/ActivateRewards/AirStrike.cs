using UnityEngine;
using System.Collections;
using Sfs2X.Entities.Data;

public class AirStrike : ActivateReward
{

    public string[] prefabs;

    public string ghost;

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
        GameObject  ghostGo;
        GameObject resourceGameObject = null;
        if (PhotonResourceWrapper.allobject.ContainsKey(ghost))
        {
             ghostGo =resourceGameObject = PhotonResourceWrapper.allobject[ghost];
        }
        else
        {
             ghostGo = (GameObject)Resources.Load(ghost, typeof(GameObject));

        }

        ghostGo = (GameObject)Instantiate(ghostGo, GetPosition(), pawn.myTransform.rotation);
        ghostObj = ghostGo.transform;
        ghostClass = ghostGo.GetComponent<GhostObject>();


    }

    public void Update (){
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
    public  Vector3 GetPosition(){
        	Ray centerofScreen =Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
			RaycastHit hitinfo;
            if (Physics.Raycast(centerofScreen, out hitinfo, maxDistance, robotLayer))
            {
                return hitinfo.point;
            }
            else
            {
                return centerofScreen.origin + centerofScreen.direction * maxDistance;
            }
    }

    public virtual void Deselect(Pawn pawn)
    {
        Destroy(ghostObj.gameObject);
    }

    public override void Activate(Pawn pawn)
    {
        if (!canSpawn)
        {
            return;
        }
        base.Activate(pawn);
        Building building = NetworkController.Instance.SimplePrefabSpawn(prefabs[pawn.team - 1],  GetPosition(), pawn.myTransform.rotation, new SFSObject(), false, NetworkController.PREFABTYPE.PLAYERBUILDING).GetComponent<Building>();
        building.SetOwner(pawn.player);
       
    }

}
