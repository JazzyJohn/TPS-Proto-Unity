using UnityEngine;
using System.Collections;

public class CrosshairBehaviour : MonoBehaviour {
	
	public UISprite skill;
	
	public UISprite skillReady;
	
	public Color skillNormalColor;
	
	public Color skillReadyColor;
	
	
	public string[] shootTypes;
	
	private int shootTypeNum=-1;
	
	public UISprite shootType;
	
	public UISprite  shootReady;
	
	public Color  shootNormalColor;
	
	public Color  shootReadyColor;
	
	
	private int afterPumpTypeNum=-1;
	
	public string[] afterPumpTypes;
	
	public UISprite afterPumpType;
	
	public UISprite  afterReady;
	
	public Color  afterNormalColor;
	
	public Color  afterReadyColor;
	
		
	public UILabel pumpAmount;
	
	public Transform center;
	
	public Transform[] corners
	
	public float maxCoef;
	
	public void UpdateCrosshair(PlayerMainGui.GameStats gamestats)[
		BaseWeapon weapon = gamestats.gun;
		if((int)weapon.prefiretype!=shootTypeNum){
			shootTypeNum =(int)weapon.prefiretype;
			shooType.spriteName = shootTypes[shootTypeNum];
		}
		
			
		if(weapon.PumpIsActing()){
			shootReady.color= shootReadyColor;
		}else{
			shootReady.color= shootNormalColor;
		}
		
		
		if((int)weapon.afterPumpAction!=afterPumpTypeNum){
			afterPumpTypeNum =(int)weapon.afterPumpAction;
			afterPumpType.spriteName = afterPumpTypes[afterPumpTypeNum];
		}
		pumpAmount.text = weapon.PumpCoef().String("0") +"/" +weapon.pumpAmount;
		
		if(weapon.AfterActing()){
			afterReady.color= afterReadyColor;
		}else{
			afterReady.color= afterNormalColor;
		}
		
		
		skill.spriteName = gamestats.skill;
		if(skill.skillready){
			skillReady.color= skillReadyColor;
		}else{
			skillReady.color= skillNormalColor;
		}
		
		MakeAimCoef(weapon.GetRandomeDirectionCoef());
		
	}

	private void MakeAimCoef(float coef){
		Vector3 position = center.position;
		coef=coef*maxCoef;
		for(int i=0;i <4; i++){
			switch(i){
				case 0
				corners[i].position = position+ ( new Vector(0,1,0))*coef;
				break;
				case 1
				corners[i].position = position+ ( new Vector(1,0,0))*coef;
				break;
				case 2
				corners[i].position = position+ ( new Vector(0,-1,0))*coef;
				break;
				case 3
				corners[i].position = position+ ( new Vector(-1,0,0))*coef;
				break;
			}
		}
		
	}
}