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

    private Player LocalPlayer;
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
	[System.Serializable]
	public class OneAnnonce{
		public  string sprite;
		
		public int priority;
		
	}
    public OneAnnonce[] annonceSprites;
	
	public int curAnnonce;
	
	public UISprite annonce;
    
	public UITweener annonceTweener;

    public UILabel annonceLabel;

    public string[] additionalSprites;

    public UISprite annonceAddSprite;
	
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
	public UIScrollView lvlView;
	
	public Transform  lvlTable;
	
	public Transform lvlPrefab;
	
	public UIScrollView moneyView;
	
	public Transform moneyTable;
	
	public Transform moneyPrefab;


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
    void Update()
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
     
            if (Kills) Kills.text = LocalPlayer.Score.Kill.ToString();
            if (Death) Death.text = LocalPlayer.Score.Death.ToString();
            if (Assists) Assists.text = LocalPlayer.Score.Assist.ToString();


            if (RedTeamScore) RedTeamScore.text = gamestats.score[0].ToString();
            if (BlueTeamScore) BlueTeamScore.text = gamestats.score[1].ToString();

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
		}
	}
	public void DeActivate(){
        if (hudpanel.enabled)
        {
            hudpanel.enabled = false;
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
            annonceLabel.text = text;
        }
        annonceTweener.tweenFactor = 0.0f;
        annonceTweener.PlayForward();

    }
	public void AddMoneyMessage(string text){
		Transform newTrans = Instantiate(moneyPrefab);
		newTrans.parent = moneyTable;
		newTrans.localScale= new Vector3(1f, 1f, 1f);
		newTrans.localEulerAngles  = new Vector3(0f, 0f, 0f);
		newTrans.localPosition= new Vector3(0f, 0f, 0f);
		newTrans.GetComponent<UILabel>().text = text;
		moneyView.ResetPostiton();
	}
	public void AddLvlMessage(string text){
		Transform newTrans = Instantiate(lvlPrefab);
		newTrans.parent = lvlTable;
		newTrans.localScale = new Vector3(1f, 1f, 1f);
		newTrans.localEulerAngles  = new Vector3(0f, 0f, 0f);
		newTrans.localPosition= new Vector3(0f, 0f, 0f);
		newTrans.GetComponent<UILabel>().text = text;
		lvlView.ResetPostiton();
	}
}
