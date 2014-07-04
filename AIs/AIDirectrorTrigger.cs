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
        AISwarmEnd
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
                    if (!other.GetComponent<Pawn>().isAi)
                    {
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
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
