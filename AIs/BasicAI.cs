using UnityEngine;
using System.Collections;

public class BasicAI : Pawn 
{
    public float angleRange = 160;   // угол обзора
    public float rangeDistance = 10; // дальность обзора
    public Pawn target;              // текущая цель

	void Start () {
	    
	}

	void Update () {
	
	}

    public void FindTarget()
    {
        target = null;

        foreach (Pawn pawn in PlayerManager.FindAllPawn())
        {
            if (Vector3.Distance(pawn.transform.position, transform.position) < rangeDistance)
            {
                Vector3 myPos = transform.position; // моя позиция
                Vector3 targetPos = pawn.transform.position; // позиция цели
                Vector3 myFacingNormal = transform.forward; //направление взгляда нашей турели
                Vector3 toTarget = (targetPos - myPos);
                toTarget.Normalize();

                float angle = Mathf.Acos(Vector3.Dot(myFacingNormal, toTarget)) * 180 / 3.141596f;

                if (angle <= angleRange / 2)
                {
                    Vector3 direction = (pawn.transform.position - transform.position);
                    direction.y += 0.3f;
                    direction.Normalize();
                    foreach (RaycastHit hit in Physics.RaycastAll(transform.position, direction, rangeDistance))
                        if (hit.transform == pawn.transform)
                        {
                            target = pawn;
                            break;
                        }
                }
            }
        }
    }


}
