using UnityEngine;
using System.Collections;

public class DestroyableNetworkObject : MonoBehaviour {

	public PhotonView photonView;

	public void RequestKillMe(){
		photonView.RPC("KillMe",PhotonTargets.All);
	}
	[RPC]
	public void KillMe(){
		Debug.Log ("RPC KILL ME");
		if(photonView.isMine){
			PhotonNetwork.Destroy(gameObject);
		}		
	}


}
