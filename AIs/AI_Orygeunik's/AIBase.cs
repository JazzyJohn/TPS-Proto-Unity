using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SphereCollider))]
public class AIBase : MonoBehaviour
{
    public AIType TypeofBehavior;

    public float
        DetectionRadius,
        XRayDistance,
        TickPause;

    private SphereCollider _SC;

    private List<GameObject> _arrayPlayerInRadius = new List<GameObject>();

    private AITurret _turret;
    //private AIPatrol _patrol;
    //private AIHolder _holder;
    //private AIRusher _rusher;



    private void Awake()
    {
        _SC = GetComponent<SphereCollider>();
        _SC.isTrigger = true;
        _SC.radius = DetectionRadius;
        switch (TypeofBehavior)
        {
            case AIType.Turret:
                {
                    _turret = gameObject.AddComponent<AITurret>();
                    _turret.DistanceXRay = XRayDistance;
                }
                break;
            //case AIType.Patrol:
            //    {
            //        _patrol = gameObject.AddComponent<AIPatrol>();
            //        _patrol.DistanceXRay = XRayDistance;
            //    }
            //    break;
            //case AIType.Holder:
            //    {
            //        _holder = gameObject.AddComponent<AIHolder>();
            //        _holder.DistanceXRay = XRayDistance;
            //    }
            //    break;
            //case AIType.Rusher:
            //    {
            //        _rusher = gameObject.AddComponent<AIRusher>();
            //        _rusher.DistanceXRay = XRayDistance;
            //    }
            //    break;

        }
    }
    private IEnumerator Tick()
    {
        while (true)
        {
            switch (TypeofBehavior)
            {
                case AIType.Turret:
                    _turret.Tick();
                    break;

            }
            yield return new WaitForSeconds(TickPause);
            Debug.Log("Work");
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            _arrayPlayerInRadius.Add(col.gameObject);
            if (_arrayPlayerInRadius.Count > 0)
                switch (TypeofBehavior)
                {
                    case AIType.Turret:
                        _turret.PlayersList = _arrayPlayerInRadius.ToArray();
                        break;

                }
            if (_arrayPlayerInRadius.Count == 1)
                StartCoroutine("Tick");

        }
    }
    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            _arrayPlayerInRadius.Remove(col.gameObject);
            if (_arrayPlayerInRadius.Count == 0)
                StopCoroutine("Tick");
        }
    }
}

public enum AIType
{
    Turret,
    Patrol,
    Holder,
    Rusher
}