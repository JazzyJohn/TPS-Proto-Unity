using UnityEngine;
using System.Collections;

public class HitCounter : MonoBehaviour {
        private float count;
        private float showCount;
        private bool runTime=false;
        private float time;

        public float timeToCount = 10.0f;
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
                if (time > timeToCount)
                {
                    time = 0;
                    showCount = count;
                    runTime = false;
                }
			}
            if (showCount == 0)
            {

                textMesh.text = count.ToString("0") + " DPS " + count.ToString("0") + " Last DPS Time:" + time.ToString("0");
            }
            else
            {

                textMesh.text = count.ToString("0") + " DPS " + showCount.ToString("0") + " Last DPS Time:" + time.ToString("0");
            }
			
		}
}