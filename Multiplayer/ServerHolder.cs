using UnityEngine;
using System.Collections;

public class ServerHolder : MonoBehaviour {


		// Use this for initialization
		void Start()
		{
			PhotonNetwork.ConnectUsingSettings("0.0.7");
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
		void OnJoinedRoom()
		{
			Camera.main.GetComponent<PlayerMainGui> ().enabled = true;
			PhotonNetwork.Instantiate ("Player",Vector3.zero,Quaternion.identity,0);
		}
}
