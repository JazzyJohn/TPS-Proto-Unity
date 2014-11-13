using UnityEngine;
using System.Collections;
using System.Xml;

public class PremiumManager : MonoBehaviour {

    public static int DOUBLECOST = 1;
	
	public static int STAMINA_MULTIPLIER=2;
	
	public static int PREMIUM_MULTIPLIER= 2;
	
	bool isPremium = false;
	
	bool buyBlock = false;
	
	DateTime timeEnd;
	
	
	
	public void Start(){
		timeEnd = DateTime.Now;
	}
	
	
    public IEnumerator PayForDouble(AddShops shop)
    {
        if (buyBlock)
        {
            yield break;
        }
        buyBlock = true;
        WWWForm form = new WWWForm();

        form.AddField("uid", GlobalPlayer.instance.UID);
       

        WWW w = StatisticHandler.GetMeRightWWW(form, StatisticHandler.DOUBLE_REWARD);

        yield return w;
        Debug.Log(w.text);
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(w.text);

        if (xmlDoc.SelectSingleNode("result/error").InnerText == "0")
        {

            GlobalPlayer.instance.gold -= DOUBLECOST;
            shop.ShowDoubleReward();
            DoubleReward();
        }
        else
        {
            if (shop != null)
            {
                if (xmlDoc.SelectSingleNode("result/error").InnerText == "2")
                {
                    shop.AskMoneyShow(shop.BuyDouble);
                }
                shop.SetMessage(xmlDoc.SelectSingleNode("result/errortext").InnerText);
            }
        }
        buyBlock = false;
    }

	public string TimeLeft(){
		if(premium){
			timeEnd.Subtract(DateTime.Now).ToString("c");
		}
		return "";
	}
	
	public bool IsPremium(){
		return premium;
	}
	public void Update(){
		DateTime saveNow = DateTime.Now;
		if(saveNow>timeEnd&&premium){
			
			GUIHelper.SendMessage(TextGenerator.instance.GetSimpleText("PremiumEnd"));
			
			premium= false; 
		}	
		
	
	}
	
	public void SetPremium(bool isPremium,DateTime timeEnd){
		premium = isPremium;
		this.timeEnd = timeEnd;
		
	}
	
    private static PremiumManager s_Instance = null;

	public static PremiumManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                //Debug.Log ("FIND");
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first AManager object in the scene.
                s_Instance = FindObjectOfType(typeof(PremiumManager)) as PremiumManager;
            }


            // If it is still null, create a new instance
            if (s_Instance == null)
            {
                //	Debug.Log ("CREATE");
                GameObject obj = new GameObject("PremiumManager");
                s_Instance = obj.AddComponent(typeof(PremiumManager)) as PremiumManager;

            }

            return s_Instance;
        }
    }

    void DoubleReward()
    {
        RewardManager.instance.AddPremiumBoost(AfterGameBonuses.cashBoost);
        LevelingManager.instance.AddPremiumBoost(AfterGameBonuses.expBoost);
        AfterGameBonuses.done = true;
    }
	public static float GetMultiplier(){
		float multiplier = 1.0f;
		if(GlobalPlayer.instance.stamina>0){
			multiplier*= STAMINA_MULTIPLIER;
		}
		if(instance.premium){
			multiplier*= PREMIUM_MULTIPLIER;
		}
		return multiplier;
	}
}

public static class AfterGameBonuses
{

    public static int expBoost;
    public static int cashBoost;
    public static bool done;
	public static bool wasStamined;

    public static void Clear()
    {
        expBoost = 0;
        cashBoost = 0;
        done = false;
    }
}