using UnityEngine;
using System.Collections;
using System;


public enum AnnonceType
{
    KILL,
    DOUBLEKILL,
    TRIPLIKILL,
    ULTRAKILL,
    MEGAKILL,
    RAMPAGE,
    RESLEAD,
    INTERGRALEAD,
    JUGGERREADY,
    AIKILL,
	WAVEFINISHONE,
	WAVEFINISHTWO,
	WAVEFINISHTHREE,
	SWARMCLEAR,
	JUGGERKILL,
	INTEGRATAKEJUGGER,
	RESTAKEJUGGER,
    WELEAD,
    WELOST

}

public enum AnnonceAddType
{
    NONE,
    HEADSHOT
}
public class PlayerHudNgui : MonoBehaviour {

    protected Player LocalPlayer;
    public PlayerMainGui.PlayerStats Stats= null;

    public DeadGUI _DeadGUI;

    public UILabel healthLabel;

    public UISprite armor;
    public UILabel armorLabel;
    //public UILabel juggernautDropTime;
    public UILabel ammoInGun;
   
    
    public UILabel jetPackCharge;
    public UILabel pumpLabel;
    public UILabel gunName;

    //Scene
    public UILabel BattleTime;

    //sprites
    public UISprite reloadingSprite;
	public UITexture weaponTexture;
    public UIRect grenade;
    public UITexture grenadeTexture;
    public UILabel grenadeAmount;
    public UISprite operationSprite;
    public UILabel operationLabel;

    public AchievHUD[] achievementGUI;
    //bar
    public UIProgressBar reloadBar;

   

    //Teams score labels
    public UILabel RedTeamScore;
    public UILabel BlueTeamScore;

    public UIWidget ScoreBar;

    public UIProgressBar RedTeamScoreBar;
    public UIProgressBar BlueTeamScoreBar;

	public UIPanel hudpanel;

    public UIPanel practice;
	[System.Serializable]
	public class OneAnnonce{
		public  string sprite;
		
		public int priority;

        public AudioClip soundTeamOne;

        public AudioClip soundTeamTwo;
		
	}
    public OneAnnonce[] annonceSprites;
	
	public int curAnnonce;
	
	public UISprite annonce;
    
	public UITweener annonceTweener;
	
	public UILabel annonceName;

    public UILabel annonceLabel;

    public string[] additionalSprites;

    public AudioClip[] additionalSoundsTeamOne;

    public AudioClip[] additionalSoundsTeamTwo;

    public UISprite annonceAddSprite;
			
	public AudioSource annoncePlayer;
	
	public UILabel tutorialText;
	
	public CrosshairBehaviour crosshair;

    /*
        public float robotTime=0;
		public float health=0;
		public float ammoInGun=0;
		public float ammoInGunMax=0;
		public float ammoInBag=0;
		public float reloadTime=0;
		public int jetPackCharge= 0;
		public string gunName="";
			
     */
	 
	 
	 //Message section
    public UITable lvlView;
	
	public Transform  lvlTable;
	
	public Transform lvlPrefab;
	
	public UITable moneyView;
	
	public Transform moneyTable;
	
	public Transform moneyPrefab;

    public Transform killerTable;

    public UITable killerView;

    public Transform killerPrefab;
	
	public Transform goldPrefab;

    public ActivateRewardIcon[] rewardsIcon;

    public UIProgressBar rating;

   
	
	public UITweener hitTweener;

    public Transform stimPrefab;

    public Transform stimTableTransform;

    public UITable stimTable;

    public GAMEMODE mode;

    public UILabel versionLabel;

    public Camera mapCamera;

    
    public enum HudState
    {
        Waiting = 0,
        Work = 1
    }
    public IEnumerator LateFrameResize()
    {
        yield return new WaitForSeconds(1.0f);
        hudpanel.Invalidate(true);
    }
    public void ReSize() //Правка позиции компонентов
    {


        StartCoroutine(LateFrameResize());
    }
    public void Awake()
    {
        NJGMapOnGUI map = FindObjectOfType<NJGMapOnGUI>();
        if (map != null) {
            mapCamera = map.GetComponentInChildren<Camera>();
        }
        versionLabel.text = "Version:" + PlayerManager.instance.version + " Date: " + System.DateTime.Now.ToShortDateString();
    }
	public void Start(){
      
		annoncePlayer.loop = false;
        Achievement[] achievement = AchievementManager.instance.GetTask();
        for (int i = 0; i < achievementGUI.Length; i++)
        {
            achievementGUI[i].Init(achievement[i]);
        }
	}
    protected void Update()
    {
        PlayerMainGui.GameStats gamestats = GameRule.instance.GetStats();
      
        if (LocalPlayer)Stats = LocalPlayer.GetPlayerStats();
        if (LocalPlayer != null)
        {
            if (BattleTime)
            {
                TimeSpan span = new TimeSpan((long)(GameRule.instance.GetTime() * TimeSpan.TicksPerSecond));
                BattleTime.text = string.Format("{0:D2} : {1:D2}",  span.Minutes, span.Seconds);
             
            }
            //float val = (LocalPlayer.GetPlayerStats().health / (LocalPlayer.GetPlayerStats().maxHealth / 100f)) / 100f; для прогресс бара кусок
            
            if (healthLabel) healthLabel.text = LocalPlayer.GetPlayerStats().health.ToString("0");

            //if (juggernautDropTime) juggernautDropTime.text = Stats.robotTime.ToString("0.0") ;

			if ( Stats.gun){

				if(Stats.gun.IsReloading()){
                    reloadBar.alpha = 1.0f;
                    reloadBar.value = Stats.gun.ReloadProgress();
					if (ammoInGun) ammoInGun.text = Stats.gun.ReloadTimer().ToString("0.0") + " (" + Stats.ammoInBag + ")";
				}else{
                    reloadBar.alpha = 0.0f;
					if (ammoInGun) ammoInGun.text = Stats.gun.curAmmo+ "/" + Stats.gun.clipSize + " (" + Stats.ammoInBag + ")";
				}
				weaponTexture.mainTexture = Stats.gun.HUDIcon;
	            gunName.text =  Stats.gun.weaponName;//Stats.gunName;
			}
            if (Stats.grenade!=null)
            {

               grenade.alpha =1.0f;
               grenadeTexture.mainTexture = Stats.grenade.HUDIcon;
               grenadeAmount.text = Stats.grenadeAmount.ToString();//Stats.gunName;
            }
            else
            {
                grenade.alpha = 0.0f;
            }
            if (Stats.armor)
            {
                if (Stats.armor.GetHP() > 0)
                {
                    armor.alpha = 1.0f;
                    armorLabel.text = Stats.armor.GetHP().ToString("0"); 
                }
                else
                {
                    armor.alpha = 0.0f;
                }
            }
            else
            {
                armor.alpha = 0.0f;
            }
            if (reloadingSprite) reloadingSprite.fillAmount = 1 - Stats.reloadTime;

            if (jetPackCharge) jetPackCharge.text = Stats.jetPackCharge.ToString("0.0");
            if (pumpLabel) pumpLabel.text = Stats.pumpCoef.ToString("0.0");

            operationLabel.text = TournamentManager.instance.currentOperation.myCounter.ToString();
          

            switch (mode)
            {
                case GAMEMODE.PVE_HOLD:
                    if (RedTeamScore) RedTeamScore.text = (gamestats.maxScore- gamestats.score[0]).ToString() ;
                     if (BlueTeamScore) BlueTeamScore.text = gamestats.score[1].ToString();
                    break;

                default:
                     if (RedTeamScore) RedTeamScore.text = gamestats.score[0].ToString();
                     if (BlueTeamScore) BlueTeamScore.text = gamestats.score[1].ToString();
                    break;
            }
            if (gamestats.showProgress)
            {
                ScoreBar.alpha = 1.0f;
                RedTeamScoreBar.value = (float)gamestats.score[0] / (float)gamestats.maxScore;
                BlueTeamScoreBar.value = (float)gamestats.score[1] / (float)gamestats.maxScore;
            }
            else
            {
                ScoreBar.alpha = 0.0f;
            }
            int i = 0;
            for (i = 0; i < Stats.rewards.Length;i++ )
            {
                rewardsIcon[i].Show(Stats.rewards[i]);
            }
            for (; i < rewardsIcon.Length; i++)
            {
                rewardsIcon[i].Show(RewardState.NO_ACTIVE);
            }
                rating.value = Stats.rating;
            crosshair.UpdateCrosshair(Stats);
        }
        if (AchievementManager.instance.ReloadHudTask())
        {
            Achievement[] achievement = AchievementManager.instance.GetTask();
            for (int i = 0; i < achievementGUI.Length; i++)
            {
                achievementGUI[i].Init(achievement[i]);
            }
        }
       
    }
	
    public void SetLocalPlayer(Player player)
    {
        LocalPlayer = player;
        //TODO: Функция вызывается вообще при старте, скидывает непонятно чего, а когда игрок спавнится, ничего ен происходит
        Stats = player.GetPlayerStats();
        operationSprite.spriteName = TournamentManager.instance.GetOperationSpriteName();
    }
	public void Activate(){
      
		if (!hudpanel.enabled) {
			hudpanel.enabled = true;
            if (mapCamera != null)
            {
                mapCamera.enabled = true;
            }
		}
        if (!this.enabled)
        {
            this.enabled = true;
        }
	}
	public void DeActivate(){

        if (hudpanel!=null&&hudpanel.enabled)
        {
            hudpanel.enabled = false;
            if (mapCamera != null)
            {

                mapCamera.enabled = false;
            }

        }
	}

    public void Annonce(AnnonceType type, AnnonceAddType newAddSprite, string text)
    {
        if (annonce.enabled == false) {
            annonce.enabled = true;
        }else{
            if (annonceSprites[curAnnonce].priority > annonceSprites[(int)type].priority && annonce.alpha>0)
            {
                //Debug.Log("return by priprity");
				return;
			}
		}
		curAnnonce =(int)type;
        annonce.spriteName = annonceSprites[curAnnonce].sprite;
        if (annonceAddSprite != null)
        {
            annonceAddSprite.spriteName = additionalSprites[(int)newAddSprite];
        }
        if (annonceLabel != null) {
            annonceLabel.text = TextGenerator.instance.GetAddAnnonceText(type, text);
        }
		if(annonceName!=null){
          //  Debug.Log(TextGenerator.instance.GetMainAnnonceText(type));
            annonceName.text = TextGenerator.instance.GetMainAnnonceText(type);
		}
		if( annonce.spriteName !=""){
			annonceTweener.tweenFactor = 0.0f;
			annonceTweener.PlayForward();
		}
        if (annonceSprites[curAnnonce].soundTeamOne != null)
        {
            if (type == AnnonceType.INTERGRALEAD) {
                if (Player.localPlayer.team == 1)
                {
                    annoncePlayer.clip = annonceSprites[(int)AnnonceType.WELEAD].soundTeamOne;
                    annoncePlayer.Play();
                }
                else
                {
                    annoncePlayer.clip = annonceSprites[(int)AnnonceType.WELOST].soundTeamOne;
                    annoncePlayer.Play();
                }
            
            
            }else if(type == AnnonceType.RESLEAD)
            {
                if (Player.localPlayer.team == 2)
                {
                    annoncePlayer.clip = annonceSprites[(int)AnnonceType.WELEAD].soundTeamTwo;
                    annoncePlayer.Play();
                }
                else
                {
                    annoncePlayer.clip = annonceSprites[(int)AnnonceType.WELOST].soundTeamTwo;
                    annoncePlayer.Play();
                }
            }
            else
            {
                if (Player.localPlayer.team == 2)
                {
                    annoncePlayer.clip = annonceSprites[curAnnonce].soundTeamTwo;
                    annoncePlayer.Play();
                }
                else
                {
                    annoncePlayer.clip = annonceSprites[curAnnonce].soundTeamOne;
                    annoncePlayer.Play();
                }
            }
        }
        else
        {

            if (additionalSoundsTeamOne[(int)newAddSprite] != null)
                {
                    if (Player.localPlayer.team == 1)
                    {
                        annoncePlayer.clip = additionalSoundsTeamOne[(int)newAddSprite];
                    }
                    else
                    {
                        annoncePlayer.clip = additionalSoundsTeamTwo[(int)newAddSprite];
                    }
                    annoncePlayer.Play();
                }
            
        }

    }
	public void AddMoneyMessage(string text,bool gold){
		Transform newTrans;
		if(gold){
            newTrans = Instantiate(goldPrefab) as Transform;
		}else{
            newTrans = Instantiate(moneyPrefab) as Transform;
		}
		newTrans.parent = moneyTable;
		newTrans.localScale= new Vector3(1f, 1f, 1f);
		newTrans.localEulerAngles  = new Vector3(0f, 0f, 0f);
		newTrans.localPosition= new Vector3(0f, 0f, 0f);
		newTrans.GetComponent<UILabel>().text = text;
        newTrans.GetComponent<SimpleDelayDestroy>().enabled = true;
        UITweener tweener = newTrans.GetComponent<UITweener>();
        tweener.tweenFactor = 0.0f;
        tweener.PlayForward();
		moneyView.Reposition();
	}
    public void KillHistory(string killer,string victim, int weapon)
    {
        Transform  newTrans = Instantiate(killerPrefab) as Transform;
      
        newTrans.parent = killerTable;
        newTrans.localScale = new Vector3(1f, 1f, 1f);
        newTrans.localEulerAngles = new Vector3(0f, 0f, 0f);
        newTrans.localPosition = new Vector3(0f, 0f, 0f);
        KillerGui killerGui = newTrans.GetComponent<KillerGui>();
        killerGui.killer.text = killer;
        killerGui.victim.text = victim;
        if (ItemManager.instance.GetWeaponSlotbByID(weapon) != null)
        {
            killerGui.icon.mainTexture = ItemManager.instance.GetWeaponSlotbByID(weapon).texture;
        }
        else
        {
            killerGui.icon.mainTexture = null;
        } 
       
        killerGui.destroyer.enabled = true;
        killerGui.tweener.tweenFactor = 0.0f;
        killerGui.tweener.PlayForward();
        killerView.Reposition();
    }
	public void AddLvlMessage(string text){
        Transform newTrans = Instantiate(lvlPrefab) as Transform;
		newTrans.parent = lvlTable;
		newTrans.localScale = new Vector3(1f, 1f, 1f);
		newTrans.localEulerAngles  = new Vector3(0f, 0f, 0f);
		newTrans.localPosition= new Vector3(0f, 0f, 0f);
		newTrans.GetComponent<UILabel>().text = text;
        newTrans.GetComponent<SimpleDelayDestroy>().enabled = true;
        UITweener tweener = newTrans.GetComponent<UITweener>();
        tweener.tweenFactor = 0.0f;
        tweener.PlayForward();
        lvlView.Reposition();
	}
 
    public void ShowDamageIndicator(Vector3 direction)
    {
        crosshair.ShowDamageIndicator(direction);
	}
   
	
	public void ShowHit(){
		hitTweener.tweenFactor = 0.0f;
        hitTweener.PlayForward();
	
	}
	public void CrosshairType(CrosshairColor color){
		crosshair.CrosshairType( color);
		
	
	}
    public void ToggleFpsAim(bool value)
    {
        crosshair.ToggleFpsAim(value);
    }

    public void ActivateStim(Texture2D stimTexture)
    {
        if (stimTableTransform == null)
        {
            return;

        }
        Transform newTrans = Instantiate(stimPrefab) as Transform;
        newTrans.parent = stimTableTransform;
        newTrans.localScale = new Vector3(1f, 1f, 1f);
        newTrans.localEulerAngles = new Vector3(0f, 0f, 0f);
        newTrans.localPosition = new Vector3(0f, 0f, 0f);
     
        newTrans.GetComponent<UITexture>().mainTexture = stimTexture;
     
        stimTable.Reposition();
    }

    public void ClearStim()
    {
        if (stimTableTransform == null)
        {
            return;

        }
        foreach (Transform child in stimTableTransform)
        {
            Destroy(child.gameObject);
        }
    }

    public  void PlayAnonce(AudioClip sound)
    {
        annoncePlayer.clip = sound;
        annoncePlayer.Play();
    }
}
