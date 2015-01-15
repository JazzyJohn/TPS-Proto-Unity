using UnityEngine;
using System.Collections;

public class HitCounter : MonoBehaviour {
        private float count;
        private bool runTime=false;
        private float time;
		
		public TextMesh textMesh;
		public void ShootCnt(float dmg){
                if (!runTime)
                {
                    count = 0;
                   runTime = true;
               }
               count += dmg;
			
				
		}
		void Update(){
            if (runTime)
            {
                time += Time.deltaTime;
                if (time > 1.0f)
                {
                    time = 0;
                    runTime = false;
                }
			}

            textMesh.text = count + "DPS";
			
		}
}