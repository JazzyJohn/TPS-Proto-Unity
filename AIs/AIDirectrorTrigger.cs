using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AIDirectrorTrigger : MonoBehaviour
{
    public enum TRIGGERTYPE
    {
        PlayerEnter,
        Destroy,
        ObjectArrived,
        AISwarmEnd,
		ConquestEvents
    }

    public TRIGGERTYPE type;

    public List<EventDelegate> onEvent = new List<EventDelegate>();

    public GameObject waitingObject;

    public void OnDestroy()
    {
        if (type == TRIGGERTYPE.Destroy)
        {
            EventDelegate.Execute(onEvent);        
        }
    }
    void OnTriggerEnter(Collider other)
    {
       
      
            switch(type)
            {
                case  TRIGGERTYPE.PlayerEnter:
                    Pawn pawn = other.GetComponent<Pawn>();
                    if (pawn != null && !pawn.isAi)
                    {
                        Debug.Log("EVENT");
                        EventDelegate.Execute(onEvent);
                    }
                break;
                case  TRIGGERTYPE.ObjectArrived:
                    if (other.gameObject == waitingObject)
                    {
                        
                        EventDelegate.Execute(onEvent);
                    }
                break;
            
            }
       
    }
    public void SwarmEnd()
    {
        if (type == TRIGGERTYPE.AISwarmEnd) {
            EventDelegate.Execute(onEvent);
        }

    }
	 public void StartUse()
    {
        if (type == TRIGGERTYPE.ConquestEvents) {
            EventDelegate.Execute(onEvent);
        }

    }
	 public void StopUse()
    {
        if (type == TRIGGERTYPE.ConquestEvents) {
            EventDelegate.Execute(onEvent);
        }

    }
	
}
