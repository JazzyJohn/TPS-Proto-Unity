using UnityEngine;
using System.Collections;

public class TeamDeathmatch_PVPGameRule : PVPGameRule {

		public int killMax;
		
		public override bool IsGameEnded(){
			for(int i=0;i<teamKill.Length;i++){
				if(killMax<=teamKill[i]){
					return true;
				}
			}
			return base.IsGameEnded();
		}
		void Awake(){
			
			base.Awake();
			maxScore=killMax;
		}
		public override void ActuakKillCount(int team){
		Debug.Log ("KILL COUNT" + team);
			teamKill[team-1]++;
			teamScore[team-1]++;
		}
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
		if (stream.isWriting) {
			for(int i=0;i<teamKill.Length;i++){
				stream.SendNext(teamKill[i]);
			}
		} else {
			for(int i=0;i<teamKill.Length;i++){
				teamKill[i] =(int) stream.ReceiveNext();
			}

			
		}
		base.OnPhotonSerializeView( stream,  info);
		
	}

}