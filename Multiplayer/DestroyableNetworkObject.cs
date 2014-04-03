using UnityEngine;
using System.Collections;

public class DestroyableNetworkObject : MonoBehaviour {

	public PhotonView photonView;

	public void RequestKillMe(){
		photonView.RPC("KillMe",PhotonTargets.All);
	}
	[RPC]
	public void KillMe(){
		if(photonView.isMine){
			PhotonNetwork.Destroy(photonView);
		}		
	}


}
