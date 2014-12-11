using UnityEngine;
using System.Collections;
using System.Collections.Generic;


enum PathType{NATIVE, PATHFINDINGENGINE}

public class AIAgentComponent : MonoBehaviour {

     
	private PathType type;
	//PathFinding
	private Agent agent = new Agent(5,0,true);
	private int stepsToSmooth = 4;
	private int stepsToCheck = 6;
	public bool smoothPath = true; 
	
	
	private LayerMask agentLayer;

	private Node nodeWithAgent;

	private bool aceleration = false;
	public bool needJump = false;
	//NATIVE
	public LayerMask obstacleMask;
    public LayerMask dynamicObstacleLayer;
	protected float pawnSpeed;
    protected NavMeshAgent nativeAgent;
	protected int curCorner;
	protected Transform myTransform;
	protected bool validPath;
	
	protected float stepHeight;
	protected float jumpHeight;
	
	private float radius = 0.5f;
	private float height = 1;
	public float size;

	protected Vector3 target;
	
	private bool agentAvoidance = true;

	protected Vector3 resultTranslate;
	protected Quaternion resultRotation;
	
	protected Vector3 lastOnMeshPoint;
	
	public bool recalc= false;
	void Awake(){
		if(PathfindingEngine.Instance==null){
            type = PathType.NATIVE;
                	myTransform =transform;
                    nativeAgent = GetComponent<NavMeshAgent>();
                    nativeAgent.updatePosition = false;
                    nativeAgent.updateRotation = false;
		}else{
			type = PathType.PATHFINDINGENGINE;

		}
	}
	
	void Start () {
		switch(type){
			case PathType.PATHFINDINGENGINE:
				agent.Launch (transform);
				dynamicObstacleLayer = PathfindingEngine.Instance.dynamicObstacleLayer;
				agentLayer = PathfindingEngine.Instance.agentLayer;
				height = PathfindingEngine.Instance.area.Height;
				radius = PathfindingEngine.Instance.area.tileSize * 0.45f;
				agentAvoidance = PathfindingEngine.Instance.agentAvoidance;
				break;
			case PathType.NATIVE:
			
				break;
			}
	}
	
	public void SetSpeed(float speed){
		switch(type){
			case PathType.PATHFINDINGENGINE:
				
				agent.speed = speed;
				target = Vector3.zero;
			break;
			case PathType.NATIVE:

                nativeAgent.speed = speed;
				break;
			}
	}
	public Vector3 GetTranslate(){
		return resultTranslate.normalized;
	}
	public Quaternion GetRotation(){
		return resultRotation;
	}
	public Vector3 GetFinishPoint(){
		return target;
	}
	public Vector3 GetTarget(){
		switch(type){
			case PathType.PATHFINDINGENGINE:
				
			if(agent.path.Count>0){
				//Debug.Log (agent.path[0]);
				return agent.path[0];
			}
			break;
			case PathType.NATIVE:
				if(nativeAgent.pathStatus!=NavMeshPathStatus.PathInvalid){
					return nativeAgent.desiredVelocity;
				}
				
				break;
			}
		return Vector3.zero;
	}

	public void ParsePawn (Pawn controlledPawn)
	{
		size = controlledPawn.GetSize ()/2;
			switch(type){
			case PathType.PATHFINDINGENGINE:
				agent.jumpHeight = controlledPawn.jumpHeight;
				agent.stepHeight = controlledPawn.stepHeight;
				break;
			case PathType.NATIVE:
				jumpHeight = controlledPawn.jumpHeight;
				stepHeight = controlledPawn.stepHeight;
                nativeAgent.ResetPath();
			
			break;
			}
	
	}

	public void SetTarget(Vector3 newTarget,bool forced = false){

		if (forced) {
			if((newTarget -target).sqrMagnitude>4.0f||agent.path.Count>5||recalc){
					ForcedSetTarget(newTarget);
			}
				
		} else {
			if((newTarget -target).sqrMagnitude>4.0f){
				ForcedSetTarget(newTarget);
			}
		}
	}
	public void ForcedSetTarget(Vector3 newTarget){
        Debug.Log("I recalculate path");
		recalc= false;
		target= newTarget;
		switch(type){
			case PathType.PATHFINDINGENGINE:

				agent.GoTo(newTarget);
				break;
			case PathType.NATIVE:
               

                nativeAgent.SetDestination(newTarget);			
				break;
		
	    }
    }
	public void WalkUpdate () {
		resultTranslate  =Vector3.zero;
       
		switch(type){
			case PathType.PATHFINDINGENGINE:
				needJump = false;
				if(agent.path.Count>0){
					bool walkable = true;
					//Check if exist some dynamic obstacle in our path.
				
				
					if(walkable){
						//Smooth path
						//Agent go to next step
						GotoNextStepEngine();
					}else{
						//Re-Find the path.
						agent.GoTo(target);
					}
				}
			break;
			case PathType.NATIVE:
          
				if(!nativeAgent.isPathStale){
					lastOnMeshPoint = myTransform.position;
				}
				GotoNextStepNative();
				
			break;
		}
		
	}

    public bool IsPathBad()
    {	switch(type){
			case PathType.PATHFINDINGENGINE:
			   return 	agent.pathrejected || (!agent.search && agent.path.Count == 0);
			break;
			case PathType.NATIVE:
            return   nativeAgent.pathStatus==NavMeshPathStatus.PathPartial;
			break;
			
		}
		return false;
    }
	
	public Vector3 GetUnStuckDirection(){
		switch(type){
			case PathType.PATHFINDINGENGINE:
			   return  Vector3.Forward;
			break;
			case PathType.NATIVE:
			    NavMeshHit hit;
				if (!agent.Raycast(myTransform.position+ myTransform.forward*pawn.getSize()*2.0f, out hit)) {
					return myTransform.forward;
				}else if(!agent.Raycast(myTransform.position- myTransform.forward*pawn.getSize()*2.0f, out hit)) {
					return myTransform.back;
				}else if(!agent.Raycast(myTransform.position+ myTransform.right*pawn.getSize()*2.0f, out hit)) {
					return myTransform.right;
				}else {
					return -myTransform.right;
				}
				
			break;
			
		}
		
	}	
	
	public void GotoNextStepEngine(){
		//if there's a path.
	
		if( agent.path.Count>0 ){
			



			while(agent.path.Count>1 &&IsRiched(agent.path[0],agent.pivot.transform.position,size)){
				agent.path.RemoveAt(0);
			}

           // Debug.Log(nextStep + "  " + agent.path[0] + "  " + agent.path.Count);
		
			//Get the next waypoint...
			PathNode point=agent.path[0];
			//...and rotate pivot towards it.
			Vector3 dir=point-agent.pivot.transform.position ;
			if(dir!=Vector3.zero){
				agent.pivot.transform.rotation=Quaternion.Slerp(agent.pivot.transform.rotation, Quaternion.LookRotation(dir),Time.deltaTime*15);
			}
			//Calculate the distance between current pivot position and next waypoint.
			float dist=Vector3.Distance(agent.pivot.transform.position, point);
			//Move towards the waypoint.
			Vector3 direction=(point-agent.pivot.transform.position).normalized;
			float speed = agent.speed;
		
	
			resultTranslate  =direction * Mathf.Min(dist, speed * Time.deltaTime)/Time.deltaTime;
	
			//Assign transform position with height and pivot position.
			//transform.parent = agent.pivot.transform;
			//transform.position = agent.pivot.transform.position + new Vector3(0,agent.yOffset,0);
			needJump = point.needJump;
			
			if(dir!=Vector3.zero){
				resultRotation= Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir),Time.deltaTime*15);
			}
			//resultRotation = transform.rotation;
			//If the agent arrive to waypoint position, delete waypoint from the path.

			if(IsRiched(point,agent.pivot.transform.position,size)){
				agent.path.RemoveAt(0);
			}
			
		}
	}
	public void GotoNextStepNative(){
		//if there's a path.
        Debug.Log(nativeAgent.pathStatus +"  "+nativeAgent.isPathStale);
		switch( nativeAgent.pathStatus){
			
		case NavMeshPathStatus.PathInvalid:
			Vector3 direction;
			if(lastOnMeshPoint.sqrMagnitude>0){
				direction  = lastOnMeshPoint -myTransform.position;	
			}else{
				NavMeshHit hit;
				if (nativeAgent.FindClosestEdge(out hit))
				direction = hit.position -myTransform.position;
                else
                    direction = lastOnMeshPoint - myTransform.position;	
			
			}
            resultTranslate = direction;




            if (resultTranslate != Vector3.zero)
            {
                resultRotation = Quaternion.Slerp(myTransform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 15);
			}
		
		break;
		
		case NavMeshPathStatus.PathPartial:
			resultTranslate= Vector3.zero;
		
		break;
		case NavMeshPathStatus.PathComplete:
		
            resultTranslate =nativeAgent.desiredVelocity;

            if (resultTranslate != Vector3.zero)
            {
                resultRotation = Quaternion.Slerp(myTransform.rotation, Quaternion.LookRotation(resultTranslate), Time.deltaTime * 15);
			}
		break;
		}
	
        
       
	}

	
	
	public bool IsRiched(Vector3 point,Vector3 target,float inputSize){
		//Debug.Log(Mathf.Abs (agent.pivot.transform.position.y - point.y) +"   " +"   "+size);
            if ((point.y - target.y) < stepHeight)
            {
				if((point-target).sqrMagnitude<inputSize*inputSize){
					return true;
				}
			}else{
				Vector3 flatPoint= point,flatPostion  =  target;
				flatPostion.y =0;
				flatPoint.y=0;
			
				if((flatPoint-flatPostion).sqrMagnitude<inputSize*inputSize){
					return true;

				}
			}
		
		return false;
	}
    public static Vector3 FlatDifference(Vector3 target, Vector3 position)
    {
        Vector3 flatPoint = position, flatPosition = target;
            flatPosition.y = 0;
			flatPoint.y=0;
            return (flatPosition - flatPoint);
    }
	
	public float HorizontalAngle(float X1, float Y1, float X2, float Y2) {
		if (Y2 == Y1) { return (X1 > X2) ? 180 : 0; }
		if (X2 == X1) { return (Y2 > Y1) ? 90 : 270; }
		float tangent = (X2 - X1) / (Y2 - Y1);
		double ang = (float) Mathf.Atan(tangent) * 57.2958;
		if (Y2-Y1 < 0) ang -= 180;
		return (float) ang;
	}
	
	
	
	
	public List<PathNode> MakeSmooth(List<PathNode> _points){
		int curvedLength = ((_points.Count)*Mathf.RoundToInt(1.4f))-1;
		List<PathNode> curvedPoints = new List<PathNode>(curvedLength);
		
		for(int pointInTimeOnCurve = 0;pointInTimeOnCurve < curvedLength+1;pointInTimeOnCurve++){
			float t = Mathf.InverseLerp(0,curvedLength,pointInTimeOnCurve);
			
			for(int j = curvedLength; j > 0; j--){
				for (int i = 0; i < j; i++){
					_points[i].node = (1-t)*_points[i].node + t*_points[i+1].node;
				}
			}
			curvedPoints.Add(_points[0]);
		}
        for (int i = 0; i < curvedPoints.Count-1;i++ )
        {
            curvedPoints[i].needJump = agent.IsJump(curvedPoints[i], curvedPoints[i+1]);
        }
		return(curvedPoints);
	}
	
	
	
	//draw  current path.
	public bool showGizmo;
	public bool showPath;
	public Color gizmoColorPath = Color.blue;
    void OnDrawGizmos()
    {
        if (showGizmo)
        {
            if (showPath)
            {
                switch (type)
                {
                    case PathType.PATHFINDINGENGINE:
                        if (agent.path != null && agent.path.Count > 0)
                        {
                            Vector3 offset = new Vector3(0, 0.1f, 0);
                            Gizmos.color = gizmoColorPath;
                            Gizmos.DrawLine(transform.position + offset, agent.path[0] + offset);
                            for (int i = 1; i < agent.path.Count; i++)
                            {
                                if (i > stepsToSmooth - 2)
                                    Gizmos.color = new Color(1 - gizmoColorPath.r, 1 - gizmoColorPath.g, 1 - gizmoColorPath.b, 0.1f);

                                if (agent.path[i].needJump)
                                {
                                    Gizmos.DrawSphere(agent.path[i], 1.0f);
                                }
                                Gizmos.DrawLine(agent.path[i - 1] + offset, agent.path[i] + offset);
                            }
                        }
                        break;
                  case PathType.NATIVE:

                        Gizmos.DrawSphere(nativeAgent.nextPosition, 1.0f);
                        
                        break;
                }
            }
        }
    }
}
