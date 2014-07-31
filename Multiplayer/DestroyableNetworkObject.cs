using UnityEngine;
using System.Collections;

public class DestroyableNetworkObject : MonoBehaviour {

	public FoxView foxView;

	public IEnumerator CoroutineRequestKillMe(){
 
		yield return new WaitForSeconds(0.2f);
       // Debug.Log("RPC KILL ME REQUEST" + this);
      
		foxView.Destroy();	
		KillMe();

	}
    public void StartCoroutineRequestKillMe()
    {
        StartCoroutine(CoroutineRequestKillMe());
    }
	public void RequestKillMe(){
       foxView.Destroy();	
		KillMe();
	}
	
	public void KillMe(){
		
        ActualKillMe();
	
			
	}
	protected virtual void ActualKillMe(){
			PhotonNetwork.Destroy(gameObject);
	}

}
