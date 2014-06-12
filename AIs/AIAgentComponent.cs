using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public class AIAgentComponent : MonoBehaviour {
	
	private Agent agent = new Agent(5,0,true);
	private int stepsToSmooth = 4;
	private int stepsToCheck = 6;
	public bool smoothPath = true; 
	public float size;
	private LayerMask dynamicObstacleLayer;
	private LayerMask agentLayer;
	private float height = 1;
	private Node nodeWithAgent;
	private float radius = 0.5f;
	private bool agentAvoidance = true;
	private bool aceleration = false;
	public bool needJump = false;
	protected Vector3 target;

	protected Vector3 resultTranslate;
	protected Quaternion resultRotation;
	
	
	void Start () {
		
		agent.Launch (transform);
		dynamicObstacleLayer = PathfindingEngine.Instance.dynamicObstacleLayer;
		agentLayer = PathfindingEngine.Instance.agentLayer;
		height = PathfindingEngine.Instance.area.height;
		radius = PathfindingEngine.Instance.area.tileSize * 0.45f;
		agentAvoidance = PathfindingEngine.Instance.agentAvoidance;
	}
	
	public void SetSpeed(float speed){
		agent.speed = speed;
	}
	public Vector3 GetTranslate(){
		return resultTranslate;
	}
	public Quaternion GetRotation(){
		return resultRotation;
	}
	public Vector3 GetTarget(){
		if(agent.path.Count>0){
			//Debug.Log (agent.path[0]);
			return agent.path[0];
		}
		return Vector3.zero;
	}

	public void ParsePawn (Pawn controlledPawn)
	{
		size = controlledPawn.GetSize ()/2;
		agent.jumpHeight = controlledPawn.jumpHeight;
		agent.stepHeight = controlledPawn.stepHeight;
	}

	public void SetTarget(Vector3 newTarget,bool forced = false){

		if (forced) {
			agent.GoTo(newTarget);
			target= newTarget;
		} else {
			if((newTarget -target).sqrMagnitude>4.0f){
				agent.GoTo(newTarget);
				target= newTarget;
			}
		}
	}
	
	public void WalkUpdate () {
		resultTranslate  =Vector3.zero;
		needJump = false;
		if(agent.path.Count>0){
			bool walkable = true;
			//Check if exist some dynamic obstacle in our path.
			int stepsCount = 0;
			foreach (Vector3 step in agent.path) {
				if (stepsCount < stepsToCheck ){
					RaycastHit hit;
					if(Physics.Raycast (step+new Vector3(0,height/2,0), Vector3.down, out hit,height,dynamicObstacleLayer)) {
						walkable = false;
						break;
					}
				}else{ break; }
				stepsCount++;
			}

			if(agentAvoidance){
				stepsCount = 0;
				Vector3 stepBefore = agent.pivot.transform.position;
				foreach (Vector3 step in agent.path) {
					if (stepsCount < stepsToCheck/2){
						RaycastHit hit;
						Vector3 dir = step - stepBefore; 
						float dist = Vector3.Distance(step,stepBefore);
						if(Physics.SphereCast (step+new Vector3(0,0.1f,0),radius ,dir , out hit,dist,agentLayer)) {
							walkable = false;
							nodeWithAgent = PathfindingEngine.Instance.Vector3ToNode(step);
							nodeWithAgent.walkable = false;
							aceleration = true;
						}
						stepBefore = step;
					}else{ break; }
					stepsCount++;
				}
			}
		
			if(walkable){
				//Smooth path
				List<PathNode> Points = new List<PathNode>();
				stepsCount = 0;
				foreach (PathNode step in agent.path) {
					if (stepsCount < stepsToSmooth ){
						Points.Add(step);
					}else{ break; }
					stepsCount++;
				}
				if(smoothPath)  { Points = MakeSmooth(Points); }
				for(int i=stepsToSmooth; i<agent.path.Count; i++)  { Points.Add(agent.path[i]); }
				agent.path = Points;
				//Agent go to next step
				GotoNextStep();
			}else{
				//Re-Find the path.
				agent.GoTo(target);
			}
		}
	}

	
	
	public void GotoNextStep(){
		//if there's a path.
	
		if( agent.path.Count>0 ){
			bool nextStep = false;


			//Correction to solve the step back that appeared if we generate new target points in small time.
			if( agent.path.Count>1 ){
				Vector3 pa = agent.pivot.transform.position;
				Vector3 p0 = agent.path[0];
				Vector3 p1 = agent.path[1];
				float angleTo0 = HorizontalAngle(pa.x,pa.z,p0.x,p0.z);
				float angleTo1 = HorizontalAngle(pa.x,pa.z,p1.x,p1.z);
				if( Mathf.Abs(angleTo0 - angleTo1)>44 ){
					if(Vector3.Distance(pa,p1)<Vector3.Distance(p1,p0)){
						agent.path.RemoveAt(0);
						nextStep = true;
					}
				}
			}	
			

			if(!nextStep){
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
			}else{
				resultTranslate  =Vector3.zero;
			}
		}
	}
	public static bool IsRiched(Vector3 point,Vector3 target,float inputSize){
		//Debug.Log(Mathf.Abs (agent.pivot.transform.position.y - point.y) +"   " +"   "+size);

			if (PathfindingEngine.Instance.oneLevelHeight > Mathf.Abs (target.y - point.y)) {
						Vector3 flatPoint= point,flatPostion  =  target;
			flatPostion.y =0;
			flatPoint.y=0;
			//Debug.Log(Mathf.Abs (agent.pivot.transform.position.y - point.y) +"   "+ (flatPoint-flatPostion).sqrMagnitude +"   "+size);
			if((flatPoint-flatPostion).sqrMagnitude<inputSize*inputSize){
				return true;

			}
		}
		return false;
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
	void OnDrawGizmos(){
		if(showGizmo){
			if(showPath){
				if(agent.path!=null && agent.path.Count>0){
					Vector3 offset = new Vector3(0,0.1f,0);
					Gizmos.color = gizmoColorPath;
					Gizmos.DrawLine( transform.position + offset, agent.path[0] + offset );
					for(int i=1; i<agent.path.Count; i++){
						if (i>stepsToSmooth-2)
							Gizmos.color = new Color(1-gizmoColorPath.r,1-gizmoColorPath.g,1-gizmoColorPath.b, 0.1f);

						if(agent.path[i].needJump){
							Gizmos.DrawSphere(agent.path[i],1.0f);
						}
						Gizmos.DrawLine( agent.path[i-1] + offset , agent.path[i] + offset );
					}
				}
			}
		}
	}
}
