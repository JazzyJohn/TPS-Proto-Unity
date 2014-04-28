using UnityEngine;
using System.Collections;

public class HitCounter : MonoBehaviour {
		private int count;
		private Pawn killer;
		private int startCount=0;
		private float lastTimeHit;
		public TextMesh textMesh;
		public void ShootCnt(GameObject newkiller){
			Pawn killerp =newkiller.GetComponent<Pawn>();
			if(killerp!=killer){
				
				killer=killerp;
			//-1 because  we must count first shoot before enter
				startCount=killer.statistic.shootCnt-1;
				
				count=0;
			}
			count++;
			lastTimeHit=  Time.time;
				
		}
		void Update(){
			if(lastTimeHit+5.0f< Time.time){
				killer= null;
				count=0;
			}
			float accuracy=0;
			
			if(killer!=null){
				float shootCnt =killer.statistic.shootCnt- startCount;
				if(shootCnt!=0){
					accuracy= count/shootCnt*100;
				}
			}
			
			textMesh.text = accuracy +"%";
			
		}
}