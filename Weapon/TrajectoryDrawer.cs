using UnityEngine;
using System.Collections;

public class TrajectoryDrawer : MonoBehaviour {

	bool gravity;
	
	float startSpeed;
	
	float speedChange;
	
	public float PredictionTime;
	
	public bool rebound;
	
	public int reboundCnt;
	
	LineRenderer lineRenderer;
	
	public int amountOfSamples;
	
	public void Init(BaseProjectile projectileClass){
		gravity = projectileClass.rigidbody.useGravity;
		startSpeed = projectileClass.startImpulse;
		speedChange = projectileClass.GetSpeedChange();
		rebound = projectileClass.projHtEffect == BaseProjectile.HITEFFECT.Rebound;
		reboundCnt= projectileClass.projHitMax;
		lineRenderer = GetComponent<LineRenderer>();
	}
	public void Draw(Vector3 startPoint, Quaternion rotation){
		Vector3 G= Vector3.zero;
		if (gravity)
        {
             // a hacky way of making sure this gets initialized in editor too...
            // this assumes 60 samples / sec
            G = Physics.gravity;
        }
        Vector3 momentum = rotation*Vector3.forward*startSpeed ;
        Vector3 pos = startPoint;
        Vector3 last = startPoint;
		lineRenderer.SetPosition(0, pos);
		bool isHit= false;
		int _reboundCnt=0;
        lineRenderer.SetVertexCount((int)(PredictionTime * amountOfSamples) + 1);
        for (int i = 0; i < (int) (PredictionTime * 60); i++)
        {
            momentum += (G + momentum.normalized*speedChange)/amountOfSamples;
            pos += momentum / amountOfSamples;
			RaycastHit hit;
			if(Physics.Linecast(last,  pos, out hit)){
				pos = hit.point;
                if (rebound && _reboundCnt < reboundCnt)
                {
					_reboundCnt++;
					momentum =momentum - 2 * Vector3.Project(momentum, hit.normal);
				}else{
					lineRenderer.SetVertexCount(i+1);
					lineRenderer.SetPosition(i+1, pos);
					break;
				}
			}
			lineRenderer.SetPosition(i+1, pos);
			last = pos;
        }

	
	}
}