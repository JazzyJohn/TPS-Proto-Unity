using UnityEngine;
using System.Collections;

public class ObjectSpawnPoint : MonoBehaviour {

	public float respawnTime= 10.0f;

	private float respawnTimer = 0.0f;

    public bool isAvalable = true, existObject = false;

    private PhotonView myView;

	protected GameObject spawnedObject;
	// Use this for initialization
    void Awake() { isAvalable = false; myView = GetComponent<PhotonView>(); }

    void OnMasterClientSwitched()
    {
        if (PhotonNetwork.isMasterClient) searchGameObject();
    }

    public void searchGameObject()
    {
        float radius = 5f;
        Collider[] cols = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider hit in cols)
        {
            if (hit && hit.tag == "Bonus" || hit.tag == "Player")
            {
                spawnedObject = hit.gameObject;
            }
        }
    }


	// Update is called once per frame
	void Update () {
            if (!isAvalable && spawnedObject == null && PhotonNetwork.isMasterClient)
            {
                respawnTimer += Time.deltaTime;
                if (respawnTimer > respawnTime)
                {
                    respawnTimer = 0.0f;
                    isAvalable = true;
                }
            }
		}	
}