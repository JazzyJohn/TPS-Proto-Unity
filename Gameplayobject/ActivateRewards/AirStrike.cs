using UnityEngine;
using System.Collections;
using Sfs2X.Entities.Data;

public class AirStrike : ActivateReward
{
    public SpawnRewardState state ;

    public string blockTag;

    public string teleportTag;

    public string[] prefabs;

    public string[] teleportPrefabs;

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
        base.Select(pawn);

    }

    public void Update (){
        if (ghostObj != null)
        {
            ghostObj.position = GetPosition();
            state = SpawnRewardState.AIR_DROP;
            
            Collider[] colliders = Physics.OverlapSphere(ghostObj.position+ Vector3.up * ghostClass.size, ghostClass.size);
            
            foreach (Collider zone in colliders)
            {
                if (zone.CompareTag(blockTag))
                {
                    state = SpawnRewardState.BLOCK;
                    break;
                }
                if (zone.CompareTag(teleportTag))
                {
                    state = SpawnRewardState.TELEPORT;
                    break;
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
            point = hitinfo.point;
        }
        else
        {
            //Debug.Log(centerofScreen.GetPoint(maxDistance));
            point = centerofScreen.GetPoint(maxDistance); //+ centerofScreen.direction * maxDistance;
        }
        if (Physics.Raycast(point, Vector3.down, out hitinfo, 10.0f, robotLayer))
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

        Building building;
        if(state ==SpawnRewardState.TELEPORT){
            building = NetworkController.Instance.SimplePrefabSpawn(teleportPrefabs[pawn.team - 1], GetPosition(), pawn.myTransform.rotation, new SFSObject(), false, NetworkController.PREFABTYPE.PLAYERBUILDING).GetComponent<Building>();
        }else{
            building = NetworkController.Instance.SimplePrefabSpawn(prefabs[pawn.team - 1], GetPosition(), pawn.myTransform.rotation, new SFSObject(), false, NetworkController.PREFABTYPE.PLAYERBUILDING).GetComponent<Building>();
    
        }
       building.SetOwner(pawn.player);
        Destroy(ghostObj.gameObject);
    }

}
