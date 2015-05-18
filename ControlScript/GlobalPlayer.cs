using UnityEngine;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

using Facebook.MiniJSON;
using Facebook;
using CodeStage.AntiCheat.Detectors;
using CodeStage.AntiCheat.ObscuredTypes;
public enum PLATFORMTYPE{
	VK,
	FACEBOOK
}
public enum AsyncNotify
{
    PREMIUM,
    RELOAD_ITEMS,
    HOLIDAY,
    DAYREWARD,
    ITEMDISCOUNT,
    ITEMTAKE
}
public class GlobalPlayer : MonoBehaviour {

   


	void Awake(){
        if (!isDebug)
        {

            UID = "";
        }
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
        cheatCheck = false;
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

    private Dictionary<string, int> statisticData = new Dictionary<string, int>();

	public string PlayerName="VK NAME";

	public PLATFORMTYPE platformType;

	public string UID;

    public bool cheatCheck = false;

	public int gold;
	
	public int cash;

    public int open_set;

	public int stamina;

    public bool isSingleAv;
	
    public Texture2D avatar;
    public bool isDebug=false;
	
	public int loadingStage = 0;

    public string STATISTIC_PHP = "";

    public  string STATISTIC_PHP_HTTPS = "";
	
	//this is for loading screen every long loading after loading increment  loadingStage when it's reached 
	//MAXLOADSTAGE load finish;
	private const int MAXLOADSTAGE =3;
	
	
	
	public bool isLoaded = false;
	
	public bool loaded = false;

    public DateTime timeShow = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

    public DateTime timeUpdate;

    public DateTime timeUpdateLast = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
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
        if (isSingleAv)
        {
            loadingStage++;
        }
        SpeedHackDetector.StartDetection(OnSpeedHackDetected, 1f, 5);
        ObscuredInt.onCheatingDetected = OnSpeedHackDetected;
    }


	void Update(){
		if(UID!=""&&!loaded&&(NewsManager.instance==null||(NewsManager.instance!=null&&NewsManager.instance.finished))){
			LoadAll();
		
		}
		if(InputManager.instance.GetButtonDown("FullScreen")){
			if(Screen.fullScreen){
				Screen.SetResolution(960, 600, false);
			}else{
				 FullScreen(true);
			}
            ResizeCall();
		}
        if (Input.GetKeyDown(KeyCode.F11))
        {
            ScreenShootManager.instance.TakeScreenshot();
        }
        if (Input.GetKeyDown(KeyCode.F10))
        {
            ScreenShootManager.instance.TakeScreenshotToWall(TextGenerator.instance.GetSimpleText("i'm in red rage"));
        }
        
		if(!isLoaded){
			if(loadingStage>=MAXLOADSTAGE){
				MainMenuGUI menu = FindObjectOfType<MainMenuGUI>();
				if (menu != null)
				{
					menu.LoadingFinish();
				}
				FinishedLoad();
                isLoaded = true;
			}
		
		}
       // Debug.Log(timeUpdate + " " + DateTime.Now);
        if (timeUpdateLast < timeUpdate && timeUpdate < DateTime.Now)
        {
            ReloadProfile(true);
            timeUpdateLast = timeUpdate;
        }
	}
	public void FinishedLoad(){
		LevelingManager.instance.SetNetworkLvl();
	
	}
	

	public static void FullScreen(bool FullScreen_Z= false){
        GA.API.Design.NewEvent("GUI:Settings:FullScreen"); 
		string[] x_y = new string[2];

		x_y[0] = Screen.resolutions[ Screen.resolutions.Length-1].width.ToString();
        x_y[1] = Screen.resolutions[Screen.resolutions.Length - 1].height.ToString();

		if (PlayerPrefs.GetString("ResolutionValue", "none") != "none")
			x_y = PlayerPrefs.GetString("ResolutionValue").Split('x');

        Debug.Log(x_y[0] +" "+ x_y[1]);
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
    public void parseProfileSImple(XmlDocument xmlDoc)
    {
        if (xmlDoc.SelectSingleNode("result/gold") != null)
        {
            gold = int.Parse(xmlDoc.SelectSingleNode("result/gold").InnerText);
            cash = int.Parse(xmlDoc.SelectSingleNode("result/cash").InnerText);
            PassiveSkillManager.instance.skillPointLeft = int.Parse(xmlDoc.SelectSingleNode("result/skillpoint").InnerText);
        }
    }
	
	public void parseProfile(string XML,bool parseFirsttTime = false){
        DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
//        Debug.Log(XML);
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(XML);
		gold = int.Parse (xmlDoc.SelectSingleNode ("player/gold").InnerText);
        if (xmlDoc.SelectSingleNode("player/new") != null && bool.Parse(xmlDoc.SelectSingleNode("player/gold").InnerText))
        {
            GA.API.Design.NewEvent("MetricFlags:NewUser");
        }
        else
        {
            GA.API.Design.NewEvent("MetricFlags:ReturningUser");
        }
		cash = int.Parse (xmlDoc.SelectSingleNode ("player/cash").InnerText);
        PassiveSkillManager.instance.skillPointLeft = int.Parse(xmlDoc.SelectSingleNode("player/skillpoint").InnerText);
		stamina = int.Parse (xmlDoc.SelectSingleNode ("player/stamina").InnerText);
		
		XmlNodeList list = xmlDoc.SelectNodes("player/notify");

        timeUpdate = dtDateTime.AddSeconds(int.Parse(xmlDoc.SelectSingleNode("player/update").InnerText)).ToLocalTime();
       
        open_set = int.Parse(xmlDoc.SelectSingleNode("player/open_set").InnerText);
        foreach (XmlNode node in list)
        {
			AsyncNotify type = (AsyncNotify)Enum.Parse(typeof(AsyncNotify), node.SelectSingleNode("type").InnerText);
			switch(type){
				case AsyncNotify.PREMIUM:
				    GUIHelper.SendMessage(TextGenerator.instance.GetSimpleText("PremiumStart"));
				    ItemManager.instance.ReloadItem();			
				break;
                case AsyncNotify.RELOAD_ITEMS:
                    ItemManager.instance.ReloadItem();	     
                break;
                case AsyncNotify.HOLIDAY:
                {
                    string text = node.SelectSingleNode("text").InnerText;
                    if (node.SelectSingleNode("item") == null)
                    {

                        GUIHelper.PushMessage(text);
                    }
                    else
                    {
                        ShopEvent evnt = new ShopEvent(ShopEventType.CAN_TAKE_HOLLIDAY, node.SelectSingleNode("item").InnerText, text);
                        GUIHelper.shopEvent = evnt;
                    }
                }
                break;
                case AsyncNotify.DAYREWARD:
                break;
                case AsyncNotify.ITEMDISCOUNT:
                {
                    DateTime lTimeShow = dtDateTime.AddSeconds(int.Parse(node.SelectSingleNode("start").InnerText));
                    if (lTimeShow <= timeShow)
                    {
                        continue;
                    }
                    timeShow = lTimeShow;
                    string text = node.SelectSingleNode("text").InnerText;
                    ShopEvent evnt = new ShopEvent(ShopEventType.DISCOUNT, node.SelectSingleNode("item").InnerText, text);
                    GUIHelper.shopEvent = evnt;
                }
                break;
                case AsyncNotify.ITEMTAKE:
                {

                    DateTime lTimeShow = dtDateTime.AddSeconds(int.Parse(node.SelectSingleNode("start").InnerText));
                    if (lTimeShow <= timeShow)
                    {
                        continue;
                    }
                    timeShow = lTimeShow;
                    string text = node.SelectSingleNode("text").InnerText;
                    ShopEvent evnt = new ShopEvent(ShopEventType.CAN_TAKE, node.SelectSingleNode("item").InnerText, text);
                    GUIHelper.shopEvent = evnt;
                }
                break;
			}
		}
        list = xmlDoc.SelectNodes("player/statistic/entry");
        statisticData.Clear();
        foreach (XmlNode node in list)
        {
            statisticData.Add(node.SelectSingleNode("key").InnerText, int.Parse(node.SelectSingleNode("value").InnerText));

        }
		TournamentManager.instance.ParseData(xmlDoc);
        PremiumManager.instance.ParseData(xmlDoc, "player");
        if (parseFirsttTime)
        {
            ItemManager.instance.Init(UID);
        }
    
	}


    public int GetStatisticData(string key)
    {

        if (statisticData.ContainsKey(key))
        {
            return statisticData[key];
        }
        return 0;
    }
	
	public void MathcEnd(){
        if (stamina > 0&& !GameRule.instance.IsPractice())
        {
            AfterGameBonuses.wasStamined = true;
            stamina--;
            StartCoroutine(LowerStamina());
        }
        else
        {
            AfterGameBonuses.wasStamined = false;
        }
	}
	protected IEnumerator LowerStamina(){
		WWWForm form = new WWWForm ();
		
		form.AddField ("uid", UID);
		WWW w = StatisticHandler.GetMeRightWWW(form,StatisticHandler.LOWER_STAMINA);
		yield return w;
	}
	
	protected IEnumerator  StartStats(string Uid,string Name,bool ParseFirstTime= false){
		WWWForm form = new WWWForm ();
		
		form.AddField ("uid", Uid);
		form.AddField ("name", Name);
		form.AddField ("tournament", 1);
        form.AddField("premium", 1);
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
        parseProfile(w.text, ParseFirstTime);
	}
    public IEnumerator ReloadStats(bool ParseFirstTime = false)
    {
		WWWForm form = new WWWForm ();

        form.AddField("uid", UID);
        form.AddField("premium", 1); 
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
        parseProfile(w.text, ParseFirstTime);
        MainMenuGUI menu =FindObjectOfType<MainMenuGUI>();
        if (menu != null)
        {
            menu.GetPlayerInfo();
            menu.SetPlayerInfoInGUI();
        
        
        }
        
        AddShops shops =FindObjectOfType<AddShops>();
        if (shops != null)
        {
            shops.ReloadFinished();
        
        
        }

     
	}	
	
	//EXTERNAL SECTION 
	
	public void AskJsForMagazine(string item){
		Application.ExternalCall ("ItemBuy",item);
	
	}
    public void ReloadProfile(bool ParseFirstTime = false)
    {
        StartCoroutine(ReloadStats(ParseFirstTime));
	}
	
	public void  SetName(string newname)
	{
		PlayerName = newname;
	
		Application.ExternalCall( "SayMyUid");
		
	}
	public void  SetUid(string uid)
	{

		UID = uid;
		//LoadAll();
	
	}
	
	public void LoadAll(){
		loaded= true;
		StartCoroutine(StartStats(UID,PlayerName,true));
		LevelingManager.instance.Init(UID);
		AchievementManager.instance.Init(UID);
		
        FindObjectOfType<RewardManager>().Init(UID);
        NetworkController.Instance.SetLogin(UID);
        StatisticManager.instance.Init(UID);
        GA.SettingsGA.SetCustomUserID(UID);
	}
	public void SetSid(string sid){
		StatisticHandler.SID = sid;
		Application.ExternalCall ("SayMyName");
	}
	public void FinishInnerLogin(string uid,string newname){
		PlayerName = newname;
        SetUid(uid);
	}
    public void AskAvatar(string text)
    {
        StartCoroutine(_LoadAvatar(text));

    }
	
	public void ReturnUsers(string text){
		StartCoroutine(	TournamentManager.instance.SetSocInfo(text));
	
	}
    public IEnumerator _LoadAvatar(string text)
    {
        WWW www = new WWW(text);

        Debug.Log("startLoad" + text);
       yield return www;
       avatar = new Texture2D(100, 100);
       www.LoadImageIntoTexture(avatar);
        MainMenuGUI gui = FindObjectOfType<MainMenuGUI>();
        if (gui != null)
        {
            gui.SetAvatar(avatar);
        }
    }

    private  void OnSpeedHackDetected(){
        if (cheatCheck)
        {
            return;
        }
        cheatCheck = true;
        FindObjectOfType<MainMenuGUI>().GoToMainMenu();

        GUIHelper.PushMessage(TextGenerator.instance.GetSimpleText("CHEATER"));
        GUIHelper.ShowAllMessage();
    }
	
}