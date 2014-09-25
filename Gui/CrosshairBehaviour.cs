using UnityEngine;
using System.Collections;

public class CrosshairBehaviour : MonoBehaviour {
 
	
	public UISprite skill;
	
	public UISprite skillReady;
	
	public Color skillNormalColor;
	
	public Color skillReadyColor;

    public UILabel skillReadyLablel;

    	
	public string[] shootTypes;
	
	private int shootTypeNum=-1;
	
	public UISprite shootType;
	
	public UISprite  shootReady;
	
	public Color  shootNormalColor;
	
	public Color  shootReadyColor;

    public UIWidget mainShoot;

	
	private int afterPumpTypeNum=-1;
	
	public string[] afterPumpTypes;
	
	public UISprite afterPumpType;
	
	public UISprite  afterReady;
	
	public Color  afterNormalColor;
	
	public Color  afterReadyColor;

    public UILabel pumpAmount;

    public UIWidget mainAfter;



    public UISprite addonsSprite;

    public UISprite addonsReady;

    public UILabel addonsText;

    public UIWidget mainAddons;


	
	public Transform center;
	
	public Transform[] corners;
	
	public float maxCoef;
    public void Awake()
    {
       
    }

	
	public void UpdateCrosshair(   PlayerMainGui.PlayerStats gamestats){
		BaseWeapon weapon = gamestats.gun;
        if (weapon == null)
        {
            return;
        }
		if((int)weapon.prefiretype!=shootTypeNum){
            shootTypeNum = (int)weapon.prefiretype;
            if (shootTypes[shootTypeNum] != "")
            {
                
                shootType.spriteName = shootTypes[shootTypeNum];
                mainShoot.alpha = 1.0f;
            }
            else
            {
                mainShoot.alpha = 0.0f;
            }
         
		
		}


        if (weapon.PumpIsActing())
        {
            shootReady.color = shootReadyColor;
        }
        else
        {
            shootReady.color = shootNormalColor;
        }
		
		
		if((int)weapon.afterPumpAction!=afterPumpTypeNum){
            afterPumpTypeNum = (int)weapon.afterPumpAction;
            if (afterPumpTypes[afterPumpTypeNum] != "")
            {
                
                afterPumpType.spriteName = afterPumpTypes[afterPumpTypeNum];
                mainAfter.alpha = 1.0f;
            }
            else
            {
                mainAfter.alpha = 0.0f;
            }
		}
		pumpAmount.text = weapon.PumpCoef().ToString("0") +"/" +weapon.pumpAmount;
		
		if(weapon.AfterActing()){
			afterReady.color= afterReadyColor;
		}else{
			afterReady.color= afterNormalColor;
		}

        if (skill != null)
        {
            //Debug.Log(gamestats.skill);
            //Debug.Log(gamestats.skillready);
            skill.spriteName = gamestats.skill.spriteName;
            if (gamestats.skill.Available())
            {
                skillReady.color = skillReadyColor;
                skillReadyLablel.text = "Press " + InputManager.instance.KeyName("Skill1");
            }
            else
            {
                skillReady.color = skillNormalColor;
                skillReadyLablel.text = gamestats.skill.CoolDown().ToString("0.0");
            }
        }

        if (weapon.projectileClass!=null&&weapon.projectileClass.detonator == BaseProjectile.DETONATOR.Manual)
        {
            addonsSprite.spriteName = "ostorojno";
            addonsText.text = InputManager.instance.KeyName("Detonate");
            mainAddons.alpha = 1.0f;
        }
        else
        {
            mainAddons.alpha = 0.0f;
        }
		MakeAimCoef(weapon.GetRandomeDirectionCoef());
		
	}

	private void MakeAimCoef(float coef){
        //Debug.Log(coef);
        Vector3 position = center.localPosition;
		coef=coef*maxCoef;
		for(int i=0;i <4; i++){
			switch(i){
				case 0:
				corners[i].localPosition = position+ ( new Vector3(0,1,0))*coef;
				break;
				case 1:
                corners[i].localPosition = position + (new Vector3(1, 0, 0)) * coef;
				break;
				case 2:
                corners[i].localPosition = position + (new Vector3(0, -1, 0)) * coef;
				break;
				case 3:
                corners[i].localPosition = position + (new Vector3(-1, 0, 0)) * coef;
				break;
			}
		}
		
	}
}