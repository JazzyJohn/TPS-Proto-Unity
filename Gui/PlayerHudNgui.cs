using UnityEngine;
using System.Collections;


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
	RESTAKEJUGGER

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
    //public UILabel juggernautDropTime;
    public UILabel ammoInGun;
    public UILabel maxAmmoInGun;
    public UILabel ammoInBag;
    public UILabel reloadTime;
    
    public UILabel jetPackCharge;
    public UILabel pumpLabel;
    public UILabel gunName;
    //frags labels
    public UILabel Kills;
    public UILabel Death;
    public UILabel Assists;

    //Scene
    public UILabel BattleTime;

    //sprites
    public UISprite reloadingSprite;
	public UITexture weaponTexture;

    //bar
    public UIProgressBar reloadBar;

   

    //Teams score labels
    public UILabel RedTeamScore;
    public UILabel BlueTeamScore;

	public UIPanel hudpanel;

    public UIPanel practice;
	[System.Serializable]
	public class OneAnnonce{
		public  string sprite;
		
		public int priority;
		
		public AudioClip sound;
		
	}
    public OneAnnonce[] annonceSprites;
	
	public int curAnnonce;
	
	public UISprite annonce;
    
	public UITweener annonceTweener;
	
	public UILabel annonceName;

    public UILabel annonceLabel;

    public string[] additionalSprites;

    public AudioClip[] additionalSounds;

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
	
	public Transform goldPrefab;



   
	
	public UITweener hitTweener;

    public Transform stimPrefab;

    public Transform stimTableTransform;

    public UITable stimTable;

    public GAMEMODE mode;

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

        mapCamera = FindObjectOfType<NJGMapOnGUI>().GetComponentInChildren<Camera>();
    }
	public void Start(){
      
		annoncePlayer.loop = false;
	}
    protected void Update()
    {
        PlayerMainGui.GameStats gamestats = GameRule.instance.GetStats();
      
        if (LocalPlayer)Stats = LocalPlayer.GetPlayerStats();
        if (LocalPlayer != null)
        {
            if (BattleTime)
            {
                int tm = (int)Time.time;
                int Minutes = (int)(tm<60?0:tm / 60f);
                int Seconds = Mathf.Abs(tm - ((Minutes > 0 ? Minutes : 1)  * 60)); // абс для того, что бы секудны при первой минуте не отображались в минусе
                BattleTime.text = Minutes.ToString() + ":" + (tm < 60? 60 - Seconds : Seconds).ToString();
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
	            if (gunName) gunName.text = "Cant show rus";//Stats.gunName;
			}
            if (reloadingSprite) reloadingSprite.fillAmount = 1 - Stats.reloadTime;

            if (jetPackCharge) jetPackCharge.text = Stats.jetPackCharge.ToString("0.0");
            if (pumpLabel) pumpLabel.text = Stats.pumpCoef.ToString("0.0");

            if (Kills) Kills.text = (LocalPlayer.Score.Kill + LocalPlayer.Score.AIKill).ToString();
            if (Death) Death.text = LocalPlayer.Score.Death.ToString();
            if (Assists) Assists.text = LocalPlayer.Score.Assist.ToString();

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
           

            crosshair.UpdateCrosshair(Stats);
        }
       
    }
	
    public void SetLocalPlayer(Player player)
    {
        LocalPlayer = player;
        //TODO: Функция вызывается вообще при старте, скидывает непонятно чего, а когда игрок спавнится, ничего ен происходит
        Stats = player.GetPlayerStats();
    }
	public void Activate(){
      
		if (!hudpanel.enabled) {
			hudpanel.enabled = true;
            mapCamera.enabled = true;
		}
	}
	public void DeActivate(){
    
        if (hudpanel.enabled)
        {
            hudpanel.enabled = false;

          
            mapCamera.enabled = false;

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
		if(annonceSprites[curAnnonce].sound!=null){
			annoncePlayer.clip = annonceSprites[curAnnonce].sound;	
			annoncePlayer.Play();
        }
        else
        {
            if (additionalSounds[(int)newAddSprite] != null)
            {
                annoncePlayer.clip = additionalSounds[(int)newAddSprite];
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
}
