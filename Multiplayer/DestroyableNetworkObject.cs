using UnityEngine;
using System.Collections;

public class DestroyableNetworkObject : MonoBehaviour {

	public PhotonView photonView;

	public IEnumerator CoroutineRequestKillMe(){
		yield return new WaitForSeconds(0.2f);
		photonView.RPC("KillMe",PhotonTargets.All);

	}
	public void RequestKillMe(){

		photonView.RPC("KillMe",PhotonTargets.All);
		
	}
	[RPC]
	public void KillMe(){
		//Debug.Log ("RPC KILL ME");
		if(photonView.isMine){
			PhotonNetwork.Destroy(gameObject);
		}		
	}


}
