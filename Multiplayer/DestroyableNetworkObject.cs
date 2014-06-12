using UnityEngine;
using System.Collections;

public class DestroyableNetworkObject : MonoBehaviour {

	public PhotonView photonView;

	public IEnumerator CoroutineRequestKillMe(){
		yield return new WaitForSeconds(0.2f);
		photonView.RPC("KillMe",photonView.owner);

	}
	public void RequestKillMe(){

		photonView.RPC("KillMe",photonView.owner);
		
	}
	[RPC]
	public void KillMe(){
		//Debug.Log ("RPC KILL ME");
        ActualKillMe();
	
			
	}
	protected virtual void ActualKillMe(){
			PhotonNetwork.Destroy(gameObject);
	}

}
