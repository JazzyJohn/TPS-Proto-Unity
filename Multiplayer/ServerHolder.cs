using UnityEngine;
using System.Collections;

public class ServerHolder : MonoBehaviour {

	private const float FLOAT_COEF =100.0f;
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
		public static Vector3 ReadVectorFromShort(PhotonStream stream){
			Vector3 newPosition = Vector3.zero;
		//Debug.Log (stream.ReceiveNext ());
		newPosition.x = ((short)stream.ReceiveNext())/FLOAT_COEF;
		//Debug.Log (newPosition.x);
			newPosition.y = ((short)stream.ReceiveNext())/FLOAT_COEF;
			newPosition.z = ((short)stream.ReceiveNext())/FLOAT_COEF;
			return newPosition;
		}
		public static void WriteVectorToShort(PhotonStream stream,Vector3 vect){
			
			stream.SendNext((short)(vect.x*FLOAT_COEF));
			stream.SendNext((short)(vect.y*FLOAT_COEF));
			stream.SendNext((short)(vect.z*FLOAT_COEF));
			
		}
		void OnMasterClientSwitched( PhotonPlayer newMaster )
		{
			//TODO: director fix *** Complete ***
			if (PhotonNetwork.isMasterClient) {
				FindObjectOfType<PVPGameRule> ().StartGame ();	
			}
		}
}
