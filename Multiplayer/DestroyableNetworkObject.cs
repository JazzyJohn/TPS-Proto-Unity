using UnityEngine;
using System.Collections;

public class DestroyableNetworkObject : MonoBehaviour {

	public PhotonView photonView;

	public IEnumerator CoroutineRequestKillMe(){
 
		yield return new WaitForSeconds(0.2f);
       // Debug.Log("RPC KILL ME REQUEST" + this);
        if (photonView.isSceneView)
        {
            photonView.RPC("KillMe",PhotonTargets.MasterClient);
        }
        else
        {
            photonView.RPC("KillMe", photonView.owner);
        }

	}
    public void StartCoroutineRequestKillMe()
    {
        StartCoroutine(CoroutineRequestKillMe());
    }
	public void RequestKillMe(){
        if (photonView.isSceneView)
        {
            photonView.RPC("KillMe", PhotonTargets.MasterClient);
        }
        else
        {
            photonView.RPC("KillMe", photonView.owner);
        }
		
	}
	[RPC]
	public void KillMe(){
		
        ActualKillMe();
	
			
	}
	protected virtual void ActualKillMe(){
			PhotonNetwork.Destroy(gameObject);
	}

}
