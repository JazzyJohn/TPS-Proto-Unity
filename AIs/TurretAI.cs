using UnityEngine;
using System.Collections;

public class TurretAI : BasicAI 
{
    public Transform weaponModel;

    void Start()
    {
        isAi = true;
    }

	void Update () 
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            IsInRange();
        }//*/

        
        if (isAi)
        {
            FindTarget();
            print(target);
            if (target != null)
            {
                print(target);
                weaponModel.LookAt(target.transform);
            }
        }
	}


    void AIControl()
    {

    }

    


    void OnDrawGizmosSelected()
    {
        Vector3 V1 = transform.position;
        Vector3 V2 = V1;
        Vector3 V3 = V1;
        Vector3 V4 = V1;
        Vector3 V5 = V1;
        float angle = transform.rotation.eulerAngles.y;
        V1.x += Mathf.Cos((-angle + 90 - angleRange / 2) * 3.14f / 180) * rangeDistance;
        V1.z += Mathf.Sin((-angle + 90 - angleRange / 2) * 3.14f / 180) * rangeDistance;
        V2.x += Mathf.Cos((-angle + 90 + angleRange / 2) * 3.14f / 180) * rangeDistance;
        V2.z += Mathf.Sin((-angle + 90 + angleRange / 2) * 3.14f / 180) * rangeDistance;
        V3.x += Mathf.Cos((-angle + 90) * 3.14f / 180) * rangeDistance;
        V3.z += Mathf.Sin((-angle + 90) * 3.14f / 180) * rangeDistance;

        V4.x += Mathf.Cos((-angle + 90 - angleRange / 4) * 3.14f / 180) * rangeDistance;
        V4.z += Mathf.Sin((-angle + 90 - angleRange / 4) * 3.14f / 180) * rangeDistance;
        V5.x += Mathf.Cos((-angle + 90 + angleRange / 4) * 3.14f / 180) * rangeDistance;
        V5.z += Mathf.Sin((-angle + 90 + angleRange / 4) * 3.14f / 180) * rangeDistance;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(V1, transform.position);
        Gizmos.DrawLine(V2, transform.position);
        Gizmos.DrawLine(V3, transform.position);

        Gizmos.DrawLine(V2, V5);
        Gizmos.DrawLine(V4, V1);
        Gizmos.DrawLine(V4, V3);
        Gizmos.DrawLine(V3, V5);
    }

}
