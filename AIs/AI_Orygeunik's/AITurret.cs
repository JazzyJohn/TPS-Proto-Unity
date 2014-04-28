using UnityEngine;
using System.Collections;

public class AITurret : MonoBehaviour
{
    private GameObject[] _playerArray;

    private float
        _distanceToTarget,
        _distanceXRay;

    public GameObject[] PlayersList
    {
        set
        {
            _playerArray = value;
            Debug.Log(_playerArray.Length);
        }
    }

    public float DistanceXRay
    {
        set
        {
            if (value > 0)
                _distanceXRay = value;
        }
    }

    public void Tick()
    {
        if (DirectVisibility(out _distanceToTarget))
        {
            //code to animation attack
            Debug.Log("Shot");
        }
        else
        {
            if (_distanceToTarget <= _distanceXRay)
            {
                //Code to animation rotate
                Debug.Log("Rotate to target");
            }
            else
            {
                Debug.Log("i know that you here");
            }
        }
    }

    private GameObject SelectTarget()
    {
        //select target by distance
        //but i have mind select by argo
        float tempDis = 0f;
        float minDis = float.MaxValue;
        int ind = 0;
        if (_playerArray.Length > 0)
        {
            for (int i = 0; i < _playerArray.Length; i++)
            {
                tempDis = Vector3.Distance(transform.position, _playerArray[i].transform.position);
                if (tempDis < minDis)
                {
                    minDis = tempDis;
                    ind = i;
                }
            }
        }
        return _playerArray[ind];
    }
    private bool DirectVisibility(out float distance)
    {
        distance = 0f;
        RaycastHit hit = new RaycastHit();
        if (Physics.Linecast(transform.position, SelectTarget().transform.position, out hit))
            if (hit.collider.CompareTag("Player"))
                return true;
            else
            {
                distance = hit.distance;
                return false;
            }
        else
            return false;

    }
}
