using UnityEngine;
using System.Collections;

public class RayController : RifleParticleController
{
    public LineRenderer rayRender;

    public void SetRay(Vector3 start, Vector3 end)
    {
        rayRender.enabled = true;
        rayRender.SetPosition(0, start);
        rayRender.SetPosition(1, end);
    }

    public void StopRay()
    {
        rayRender.enabled = false;
    }


}
