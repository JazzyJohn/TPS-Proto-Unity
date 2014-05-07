using UnityEngine;
using System.Collections;

public class ServerHolder : MonoBehaviour {

		private const int FLOAT_COEF =1000;
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
		private static Vector3 ReadVectorFromShort(PhotonStream stream){
			Vector3 newPosition = Vetcor3.Zero;
			newPosition.x = ((float)(short)stream.ReceiveNext())/FLOAT_COEF;
			newPosition.y = ((float)(short)stream.ReceiveNext())/FLOAT_COEF;
			newPosition.z = ((float)(short)stream.ReceiveNext())/FLOAT_COEF;
			return newPosition;
		}
		private static void WriteVectorToShort(PhotonStream stream,Vector3 vect){
			
			stream.SendNext((short)vect.x*FLOAT_COEF);
			stream.SendNext((short)vect.y*FLOAT_COEF);
			stream.SendNext((short)vect.z*FLOAT_COEF);
			
		}
}
