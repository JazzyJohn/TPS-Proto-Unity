using UnityEngine;
using System.Collections;

public class ObjectSpawnPoint : MonoBehaviour {

	public float respawnTime= 10.0f;

	private float respawnTimer = 0.0f;

    private bool isAvalable = false;
	
	private bool sendAv= false;

    protected foxView myView;

	protected GameObject spawnedObject;
	// Use this for initialization
	void Awake() { 
		myView = GetComponent<foxView>(); 
		respawnTimer = respawnTime;
	}

	public bool IsAvalable(){
		return isAvalable;
	
	}
	public void SetIsAvalable(bool isAvalable){
		this.isAvalable = isAvalable;
		

	}



	// Update is called once per frame
	void Update () {
            if (!isAvalable && spawnedObject == null && foxView.isMine)
            {
				
				
                respawnTimer += Time.deltaTime;
                if (respawnTimer > respawnTime)
                {
                    respawnTimer = 0.0f;
                   		SetIsAvalable(true);
					
				
                }
            }
	}	
	

	
	
}