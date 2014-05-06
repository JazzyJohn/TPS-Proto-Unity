using UnityEngine;
using System.Collections;

public class ServerHolder : MonoBehaviour {


		// Use this for initialization
		void Start()
			{

				if (PhotonNetwork.inRoom) {
						Camera.main.GetComponent<PlayerMainGui> ().enabled = true;
			
						PhotonNetwork.Instantiate ("Player", Vector3.zero, Quaternion.identity, 0);
						if (PhotonNetwork.isMasterClient) {
							FindObjectOfType<PVPGameRule> ().StartGame ();
						}
				} else {
					PhotonNetwork.ConnectUsingSettings(PlayerManager.instance.version);
				}
		}
		
		
		void OnGUI()
		{
			GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
		}
		void OnJoinedLobby()
		{
			PhotonNetwork.JoinRandomRoom();
		}
		void OnPhotonRandomJoinFailed()
		{
			PhotonNetwork.CreateRoom(null);
		}
		void OnCreatedRoom()
		{
			FindObjectOfType<PVPGameRule> ().StartGame ();
		}
		void OnJoinedRoom()
		{
			Camera.main.GetComponent<PlayerMainGui> ().enabled = true;

			PhotonNetwork.Instantiate ("Player",Vector3.zero,Quaternion.identity,0);
		}
}
