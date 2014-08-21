using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TargetType{SELF,PAWN,POINT,GROUPOFPAWN_BYSELF,GROUPOFPAWN_BYPAWN,GROUPOFPAWN_BYPOINT  }

public class SkillBehaviour : MonoBehaviour
{
	public string name;
	
	public float coolDown;
	
	private float _coolDown;
	
	public float activationTime;
	
	private float _activationTime;

    protected bool isUse = false;
	
	protected Pawn owner;
	
	public float radius;
	
	protected Pawn target;

    protected Vector3 targetPoint;
		
	public TargetType type;
	
	public GameObject targetEffect;
	
	public GameObject casterEffect;

    protected FoxView foxView;
	
	public void Init(Pawn owner){
		this.owner = owner;
        foxView = GetComponent<FoxView>();
	}
	
	public void Update(){
		Tick(Time.deltaTime);
	}
	protected  void Activate(){
        if (foxView.isMine)
        {
			AskActivate();
		}
		
		switch(type){
			case TargetType.SELF:
			case TargetType.PAWN:
				ActualUse(target.GetComponent<Pawn>());
                TargetVisualEffect(target.myTransform.position);
			break;
			case TargetType.POINT:
				ActualUse(targetPoint);
				TargetVisualEffect(targetPoint);
			break;
            case TargetType.GROUPOFPAWN_BYPAWN:
            case TargetType.GROUPOFPAWN_BYSELF:
            {
                TargetVisualEffect(target.myTransform.position);
                Collider[] hitColliders = Physics.OverlapSphere(target.myTransform.position, radius);
                List<Pawn> pawns = new List<Pawn>();
                for (int i = 0; i < hitColliders.Length; i++)
                {
                    Pawn _target = hitColliders[i].GetComponent<Pawn>();
                    pawns.Add(_target);
                }
                ActualUse(pawns);
            }
            break;
            case TargetType.GROUPOFPAWN_BYPOINT:
            {
                TargetVisualEffect(targetPoint);
                Collider[] hitColliders = Physics.OverlapSphere(targetPoint, radius);
                List<Pawn> pawns = new List<Pawn>();
                for (int i = 0; i < hitColliders.Length; i++)
                {
                    Pawn _target = hitColliders[i].GetComponent<Pawn>();
                    pawns.Add(_target);
                }
                ActualUse(pawns);
            }
            break;
		}
	}
	

	public virtual void TargetVisualEffect(Vector3 Position){
		if(targetEffect!=null){
            Instantiate(targetEffect, Position, Quaternion.identity);
		}
	}
	public virtual void CasterVisualEffect(){
		if(casterEffect!=null){
			GameObject effect  =    Instantiate(casterEffect, owner.myTransform.position,  owner.myTransform.rotation) as GameObject;
			effect.transform.parent = owner.myTransform;
		}
        if (foxView.isMine)
        {
			AskCast();
		}
		
	}
	
	
	
	
	
	public void Tick(float delta){
        if (isUse)
        {

            if (activationTime <= _activationTime)
            {
                isUse = false;
                _coolDown = 0;
                Activate();
                _activationTime = 0;
            }
            else
            {
                _activationTime += delta;
            }
        }
        else
        {
            if (_coolDown < coolDown)
            {
                _coolDown += delta;
            }
        }
	}
	public void Use(){
        Debug.Log("activate Skill");
		if(_coolDown>=coolDown){
			isUse = true;
			CasterVisualEffect();
			switch(type){
				case TargetType.SELF:
				case TargetType.GROUPOFPAWN_BYSELF:	
					target = owner;
                    break;
				case TargetType.PAWN:
				case TargetType.GROUPOFPAWN_BYPAWN:
					target =owner.curLookTarget.GetComponent<Pawn>();				
				break;
				case TargetType.POINT:
				case TargetType.GROUPOFPAWN_BYPOINT:
              
					targetPoint =owner.getCachedAimRotation();
                    Debug.Log("activate Skill" + targetPoint);
				break;				
			}
		}		
	}
	
	public void Use(Pawn pawn){
		if(_coolDown>=coolDown){
			isUse = true;
			CasterVisualEffect();
			target =pawn;
			
		}		
	}
	public void Use(Vector3 point){
		if(_coolDown>=coolDown){
			isUse = true;
			CasterVisualEffect();
			targetPoint =point;
			
		}		
	}
	public bool Available(){
		return !isUse&&_coolDown>=coolDown;
	}
	public bool Activate(){
		return inUse;
	}
    public void UnUse()
    {
        isUse = false;
        _activationTime = 0;
    }
	public float CoolDown(){
		return coolDown -_coolDown;
	}
	protected virtual void ActualUse(Pawn pawn){
	
	}
	
	protected virtual void ActualUse(List<Pawn> pawn){
	
	}
	
	protected virtual void ActualUse(Vector3 target){
	
	}
	
	protected void AskCast(){

            foxView.SkillCastEffect(name);

	
	}
	protected void 	AskActivate(){
		switch(type){
				case TargetType.SELF:
				case TargetType.GROUPOFPAWN_BYSELF:
                   foxView.SkillActivate(name);
                    break;
				case TargetType.PAWN:
				case TargetType.GROUPOFPAWN_BYPAWN:
					if(target!=null){

                         foxView.SkillActivate(name,target.foxView.viewID);
					}
				break;
				case TargetType.POINT:
				case TargetType.GROUPOFPAWN_BYPOINT:
                      foxView.SkillActivate(name, targetPoint);

				break;				
			}
	}
	

	public void RemoteActivateSkill(params object[] theObjects){
	
		switch(type){
				case TargetType.SELF:
				case TargetType.GROUPOFPAWN_BYSELF:	
					target = owner;
					Activate();
                    break;
				case TargetType.PAWN:
				case TargetType.GROUPOFPAWN_BYPAWN:
					int id = (int) theObjects[0];
					target=NetworkController.GetView(id).pawn;
				break;
				case TargetType.POINT:
				case TargetType.GROUPOFPAWN_BYPOINT:
	
					targetPoint = (Vector3) theObjects[0];			
									
				break;				
			}
	
	}
	
}