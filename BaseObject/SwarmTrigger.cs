using UnityEngine;
using System.Collections;

public class SwarmTrigger : MonoBehaviour {
    public SinglePlayerSwarm swarm;

    public void OnTriggerEnter(Collider other) 
    {
        //Debug.Log(other.transform.root);
        Pawn pawn = other.transform.root.GetComponent<Pawn>();
       // Debug.Log(pawn);
    //    Debug.Log(pawn.player);
        if (pawn != null&&pawn.player!=null) {
            swarm.Activate();
        }
    }
}
