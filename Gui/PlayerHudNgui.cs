using UnityEngine;
using System.Collections;

public class PlayerHudNgui : MonoBehaviour {

    private Player LocalPlayer;
    PlayerMainGui.PlayerStats Stats= null;
    

    public UILabel healthLabel;
    public UILabel juggernautDropTime;
    public UILabel ammoInGun;
    public UILabel maxAmmoInGun;
    public UILabel ammoInBag;
    public UILabel reloadTime;
    
    public UILabel jetPackCharge;
    public UILabel gunName;
    //frags labels
    public UILabel Kills;
    public UILabel Death;
    public UILabel Assists;

    //Scene
    public UILabel BattleTime;

    //sprites
    public UISprite reloadingSprite;

    //Teams score labels
    public UILabel RedTeamScore;
    public UILabel BlueTeamScore;

	public UIPanel hudpanel;
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

    public enum HudState
    {
        Waiting = 0,
        Work = 1
    }

    void Update()
    {
        PlayerMainGui.GameStats gamestats = PVPGameRule.instance.GetStats();
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
            
            if (healthLabel) healthLabel.text = LocalPlayer.GetPlayerStats().health.ToString();

            if (juggernautDropTime) juggernautDropTime.text = Stats.robotTime.ToString("0.0") ;

            if (ammoInGun) ammoInGun.text = Stats.ammoInGun + "/" + Stats.ammoInGunMax + " (" + Stats.ammoInBag + ")";

            if (gunName) gunName.text = "Cant show rus";//Stats.gunName;

            if (reloadingSprite) reloadingSprite.fillAmount = 1 - Stats.reloadTime;

            if (jetPackCharge) jetPackCharge.text = Stats.jetPackCharge.ToString();


            if (Kills) Kills.text = LocalPlayer.Score.Kill.ToString();
            if (Death) Death.text = LocalPlayer.Score.Death.ToString();
            if (Assists) Assists.text = LocalPlayer.Score.Assist.ToString();


            if (RedTeamScore) RedTeamScore.text = gamestats.score[0].ToString();
            if (BlueTeamScore) BlueTeamScore.text = gamestats.score[1].ToString();
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
		hudpanel.enabled = false;
	}
}
