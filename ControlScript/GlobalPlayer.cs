using UnityEngine;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

using Facebook.MiniJSON;
using Facebook;
public enum PLATFORMTYPE{
	VK,
	FACEBOOK
}
public class GlobalPlayer : MonoBehaviour {

	void Awake(){
        
			if(FindObjectsOfType<GlobalPlayer>().Length>1){
				Destroy(gameObject);
			}else{
				
					switch(platformType){
						case PLATFORMTYPE.VK:
						
							Application.ExternalCall ("SayMyName");
						break;
						case PLATFORMTYPE.FACEBOOK:
							FB.Init(SetFaceBookInit, OnHideFaceBookUnity);
							Application.ExternalCall ("SayMyName");
						break;
						
					
					}
				
				
				DontDestroyOnLoad(transform.gameObject);
			}
		
	}
	void OnLevelWasLoaded(int level) {
		if(UID!=""&&level==0){
			MainMenuGUI menu = FindObjectOfType<MainMenuGUI>();
            if (menu != null)
            {
                menu.LoadingFinish();
            }
            StartCoroutine( ItemManager.instance.ReLoadItemsSync());
		}
	}
	public List<string> friendsInfo = new List<string>();

	public string PlayerName="VK NAME";

	public PLATFORMTYPE platformType;

	public string UID;
	
	public int gold;
	
	public int cash;

    public bool isDebug=false;
	
	public int loadingStage = 0;

    public string STATISTIC_PHP = "";

    public  string STATISTIC_PHP_HTTPS = "";
	
	//this is for loading screen every long loading after loading increment  loadingStage when it's reached 
	//MAXLOADSTAGE load finish;
	private const int MAXLOADSTAGE =2;
	
	
	
	public bool isLoaded = false;
	
	public void SetFaceBookInit() {
		if(FB.IsLoggedIn) {
			UID ="FB"+FB.UserId;
           Debug.Log("FB NAME" + FB.UserId);
			FB.API ("/me/", HttpMethod.GET, SetFacebookName, new Dictionary<string, string>());
			PlayerName="FACEBOOK_GUEST";
		} else {
           
            FB.Login("email,publish_actions", LoginCallback);    
		}
	}
    public void LoginCallback(FBResult result)
    {
        if (FB.IsLoggedIn)
        {
            UID = "FB" + FB.UserId;
           // Debug.Log("FB NAME"+ FB.UserId);
            FB.API("/me/", HttpMethod.GET, SetFacebookName, new Dictionary<string, string>());
            PlayerName = "FACEBOOK_GUEST";
        }

    }
	private void OnHideFaceBookUnity(bool isGameShown) {
       
	}
	
	public void SetFacebookName(FBResult result){
        Debug.Log("FB Result" + result.Text);
        Debug.Log("FB Result" + result.GetType());
		Dictionary<string,object> dict = Json.Deserialize(result.Text) as Dictionary<string,object>;
		PlayerName = (string)dict["name"];
		SetUid(UID);
	
	}

    // s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.
    private static GlobalPlayer s_Instance = null;

    // This defines a static instance property that attempts to find the manager object in the scene and
    // returns it to the caller.
    public static GlobalPlayer instance
    {
        get
        {
            if (s_Instance == null)
            {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first AManager object in the scene.
                s_Instance = FindObjectOfType(typeof(GlobalPlayer)) as GlobalPlayer;
            }

            return s_Instance;
        }
    }
    public void Start()
    {
		switch(Application.platform){
			
			case RuntimePlatform.WindowsWebPlayer:
			case RuntimePlatform.OSXWebPlayer:
			break;
			default:
              if (isDebug){
                  SetUid(UID);
              }else{
				    MainMenuGUI menu = FindObjectOfType<MainMenuGUI>();
				    if (menu != null)
				    {
					    menu.LoginPage();
				    }	
              }
			break;
		}
    }

	void Update(){
		if(InputManager.instance.GetButtonDown("FullScreen")){
			if(Screen.fullScreen){
				Screen.SetResolution(960, 600, false);
			}else{
				 FullScreen(true);
			}
            ResizeCall();
		}
		if(!isLoaded){
			if(loadingStage>=MAXLOADSTAGE){
				MainMenuGUI menu = FindObjectOfType<MainMenuGUI>();
				if (menu != null)
				{
					menu.LoadingFinish();
				}
                isLoaded = true;
			}
		
		}
	}
	public static void FullScreen(bool FullScreen_Z= false){
		string[] x_y = new string[2];

		x_y[0] = Screen.resolutions[ Screen.resolutions.Length-1].width.ToString();
        x_y[1] = Screen.resolutions[Screen.resolutions.Length - 1].height.ToString();

		if (PlayerPrefs.GetString("ResolutionValue", "none") != "none")
			x_y = PlayerPrefs.GetString("ResolutionValue").Split('x');
	    
        Screen.SetResolution(int.Parse(x_y[0]), int.Parse(x_y[1]), FullScreen_Z);
	}
   public  static void ResizeCall()
    {
        MainMenuGUI gui = FindObjectOfType<MainMenuGUI>();
        if(gui!=null){
            gui.ReSize();
        }
        PlayerMainGui hud = FindObjectOfType<PlayerMainGui>();
        if (hud != null)
        {
            hud.ReSize();
        }
    }
	public String GetPlayerName(){
		return PlayerName;
	}	
	public String GetUID(){
		return UID;
	}
	public void addFriendInfo(string vkId)
	{
		friendsInfo.Add (vkId);
	}
	
	public void addFriendInfoList(string vkIds)
	{
		//Debug.Log (vkIds);
		string[] ids = vkIds.Split (',');
		foreach (string id in ids)
			friendsInfo.Add (id);
	}
	
	
	public void parseProfile(string XML){
       
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(XML);
		gold = int.Parse (xmlDoc.SelectSingleNode ("player/gold").InnerText);
		cash = int.Parse (xmlDoc.SelectSingleNode ("player/cash").InnerText);
	}
	
	
	protected IEnumerator  StartStats(string Uid,string Name){
		WWWForm form = new WWWForm ();
		
		form.AddField ("uid", Uid);
		form.AddField ("name", Name);
		WWW w = null;
		if (String.Compare(Application.absoluteURL, 0, "https", 0,5) != 0) {
			
			Debug.Log ("STATS HTTP SEND" + StatisticHandler.GetSTATISTIC_PHP_HTTPS()+ StatisticHandler.ADD_USER);
			w = new WWW (StatisticHandler.GetSTATISTIC_PHP()+ StatisticHandler.ADD_USER, form);
		}
		else{
			Debug.Log ("STATS HTTPS SEND"+StatisticHandler.GetSTATISTIC_PHP_HTTPS()+ StatisticHandler.ADD_USER);
			w = new WWW (StatisticHandler.GetSTATISTIC_PHP_HTTPS()+ StatisticHandler.ADD_USER, form);
		}

		yield return w;
		//Debug.Log (w.text);
		parseProfile(w.text);
	}	
	public IEnumerator  ReloadStats(){
		WWWForm form = new WWWForm ();

        form.AddField("uid", UID);
		WWW w = null;
		if (String.Compare(Application.absoluteURL, 0, "https", 0,5) != 0) {
			
			Debug.Log ("STATS HTTP SEND" + StatisticHandler.GetSTATISTIC_PHP_HTTPS()+ StatisticHandler.RELOAD_STATS);
			w = new WWW (StatisticHandler.GetSTATISTIC_PHP()+ StatisticHandler.RELOAD_STATS, form);
		}
		else{
			Debug.Log ("STATS HTTPS SEND"+StatisticHandler.GetSTATISTIC_PHP_HTTPS()+ StatisticHandler.RELOAD_STATS);
			w = new WWW (StatisticHandler.GetSTATISTIC_PHP_HTTPS()+ StatisticHandler.RELOAD_STATS, form);
		}

		yield return w;
		parseProfile(w.text);
        MainMenuGUI menu =FindObjectOfType<MainMenuGUI>();
        if (menu != null)
        {
            menu.GetPlayerInfo();
            menu.SetPlayerInfoInGUI();
        }
	}	
	
	//EXTERNAL SECTION 
	
	public void AskJsForMagazine(string item){
		Application.ExternalCall ("ItemBuy",item);
	
	}
	public void ReloadProfile(){
		StartCoroutine(ReloadStats());
	}
	
	public void  SetName(string newname)
	{
		PlayerName = newname;
	
		Application.ExternalCall( "SayMyUid");
		
	}
	public void  SetUid(string uid)
	{

		UID = uid;
		
		StartCoroutine(StartStats(UID,PlayerName));
		LevelingManager.instance.Init(UID);
		AchievementManager.instance.Init(UID);
		ItemManager.instance.Init(UID);
        FindObjectOfType<RewardManager>().Init(UID);
        NetworkController.Instance.SetLogin(UID);
	}
	public void FinishInnerLogin(string uid,string newname){
		PlayerName = newname;
        SetUid(uid);
	}
	
}