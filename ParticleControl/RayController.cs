using UnityEngine;
using System.Collections;

public class RayController : RifleParticleController
{
    public LineRenderer rayRender;
	
	public ParticleSystem hitPoint;
	
	private Transform  hitTransform;
	
	protected void Start()
	{
		base.Start();
		hitTransform= hitPoint.transform;
	}

    public void SetRay(Vector3 start, Vector3 end,bool isHit)
    {
        rayRender.enabled = true;
        rayRender.SetPosition(0, start);
        rayRender.SetPosition(1, end);
        hitTransform.position = end - hitTransform.forward;
		if(!hitPoint.isPlaying&&isHit){
			hitPoint.Play();
		}
		if(hitPoint.isPlaying&&!isHit){
			hitPoint.Stop();
		}
		
    }

    public void StopRay()
    {
        rayRender.enabled = false;
        hitPoint.Stop();
    }


}
