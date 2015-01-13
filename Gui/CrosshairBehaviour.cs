using UnityEngine;
using System.Collections;

public enum CrosshairColor{
	NEUTRAL,
	ENEMY,
	ALLY,

}

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

    public UIProgressBar pumpBar;

    public UISprite pumpBarSprite;



    public UISprite addonsSprite;

    public UISprite addonsReady;

    public UILabel addonsText;

    public UIWidget mainAddons;

   	
	public Transform center;
	
	public Transform[] corners;
	
	public UIWidget[] crosshairParts;
	
	public Color[] crosshairColor;
	
	public UIWidget crosshairMain;
	
	public UIWidget  aimCrosshair;

    public UIWidget allymark;

    public Transform damageRoot;

    public UITweener damageTweener;

    public UITweener damageFPSTweener;

    private Transform camTramsf;

	public float maxCoef;

    public bool isFps = false;
   
	public UITexture Blood1;
	public UITexture Blood2;
	public UITexture Crosshair;

	int w1;
	int w2;

	float WScreen;

	void ReSize()
	{
		WScreen = Screen.width;
		w1 = Blood1.width;
		w2 = Blood2.width;
		int H1 = Blood1.height;

		float newW = 1f*w1/H1;
		float newW2 = 1f*w2/Blood2.height;
		float newW3 = 1f*Crosshair.width/Crosshair.height;

		Blood1.uvRect = new Rect(-(newW-1)/2, 0f, newW, 1f);
		Blood2.uvRect = new Rect(-(newW2-1)/2, 0f, newW2, 1f);
		Crosshair.uvRect = new Rect(-(newW3-1)/2, 0f, newW3, 1f);
	}


   public void Update()
    {
        if (damageTweener != null && damageTweener.enabled)
        {
            RotateDamage();

        }

		if(WScreen != Screen.width)
			ReSize();
    }

   public void Start()
   {
       	camTramsf = Camera.main.transform;
		ReSize();
   }
	public void CrosshairType(CrosshairColor color){
        foreach (UIWidget widget in crosshairParts)
        {
            widget.color = crosshairColor[(int)color];
        }
        allymark.color = crosshairColor[(int)color]; 

      
	} 
	public void ToggleFpsAim(bool value)
    {
        isFps = value;
		if(value){
			crosshairMain.alpha= 0.0f;
			aimCrosshair.alpha = 1.0f;
		}else{
			crosshairMain.alpha = 1.0f;
			aimCrosshair.alpha = 0.0f;
		}
	}

    Vector3 hitPosition;
    public void ShowDamageIndicator(Vector3 direction)
    {
        if (!isFps)
        {
            hitPosition = direction;
            RotateDamage();

            damageTweener.tweenFactor = 0.0f;
            damageTweener.PlayForward();
        }
        else
        {
            damageFPSTweener.tweenFactor = 0.0f;
            damageFPSTweener.PlayForward();
        }

    }

    void RotateDamage()
    {
        float z_angle = Quaternion.FromToRotation(camTramsf.forward, -hitPosition).eulerAngles.y;


        damageRoot.localRotation = Quaternion.Euler(0, 0, 180 - z_angle);
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
                pumpBar.alpha = 1.0f;
                

            }
            else
            {
                pumpBar.alpha = 0.0f;
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
        float pumpcoef = weapon.PumpCoef() / +weapon.pumpAmount;
		pumpAmount.text = weapon.PumpCoef().ToString("0") +"/" +weapon.pumpAmount;
        if (pumpcoef > 0.6f)
            pumpBarSprite.color = Color.green;
        else if (pumpcoef <= 0.6f && pumpcoef > 0.3f)
            pumpBarSprite.color = Color.yellow;
        else if (pumpcoef <= 0.3f)
            pumpBarSprite.color = Color.red;

        pumpBar.value = pumpcoef;

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
                skillReady.fillAmount = 1 - (gamestats.skill.CoolDown() / gamestats.skill.coolDown);
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