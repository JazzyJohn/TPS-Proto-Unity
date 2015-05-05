using UnityEngine;
using System.Collections;
using System.Xml;
using System;
using System.Collections.Generic;

public enum PremiumSkillType
{
    EXP,
    MONEY,
    SKILL,
    HP_BOOST,

}

public class PremiumSkill : BasePassiveSkill
{
   
    
    public PremiumSkillType type;

    public int amount;

    public bool maxAmount;

    public List<string> eventTriggers = new List<string>();

    public int[] price = new int[PremiumManager.PRICE_COUNT];

    public int id;

    public DateTime timeEnd;
    public override bool Open()
    {
        return timeEnd > DateTime.Now;
    }

    public int team;
}

public class PremiumManager : MonoBehaviour {

    public const int PRICE_COUNT = 3;

    public int resetSkillPrice = 70;

    public static float REGEN_TIME = 10.0f;

    public static float REGEN_MIN = 20.0f;

    public PremiumSkill regen;


    Dictionary<int, PremiumSkill> skills = new Dictionary<int, PremiumSkill>();

    List<PremiumSkill> open = new List<PremiumSkill>();
	
	int setSize =   2;
	
	bool isPremium = false;

    bool buyBlock = false;



    public void ParseData(XmlDocument xmlDoc,string root)
    {
		
		
		XmlNodeList list = xmlDoc.SelectNodes(root +"/premiumskill");

        DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        foreach (XmlNode node in list)
        {
            int id = int.Parse(node.SelectSingleNode("id").InnerText);
            PremiumSkill skill;
            if (skills.ContainsKey(id))
            {
                skill = skills[id];
            }else{
                skill = new PremiumSkill();
                skills[id] = skill;
                skill.id = id;
                skill.name = node.SelectSingleNode("name").InnerText;
                skill.iconGUI = node.SelectSingleNode("icon").InnerText;
                skill.descr = node.SelectSingleNode("descr").InnerText;
                skill.type = (PremiumSkillType)System.Enum.Parse(typeof(PremiumSkillType), node.SelectSingleNode("type").InnerText);
                if (skill.type == PremiumSkillType.HP_BOOST)
                {
                    regen = skill;
                }
                skill.amount = int.Parse(node.SelectSingleNode("gameData").InnerText);
                skill.maxAmount = bool.Parse(node.SelectSingleNode("maxAmount").InnerText);
                foreach (XmlNode trigger in node.SelectNodes("eventTriggers"))
                {
                    skill.eventTriggers.Add(trigger.InnerText);  
                }
                XmlNodeList  prices = node.SelectNodes("price");
                for (int i = 0; i < PRICE_COUNT;i++ )
                {
                    XmlNode price = prices[i];
                    skill.price[i] = int.Parse(price.InnerText);
                }
                skill.team = int.Parse(node.SelectSingleNode("team").InnerText);
            }

            if (node.SelectSingleNode("timeEnd").InnerText != "")
            {
                try
                {


                    skill.timeEnd = dtDateTime.AddSeconds(int.Parse(node.SelectSingleNode("timeEnd").InnerText)).ToLocalTime();

                }
                catch (Exception)
                {

                    Debug.LogError("date format  exeption");
                }

            }
            if (skill.Open() && !open.Contains(skill))
            {
                open.Add(skill);
            }

        }
	}

    public IEnumerator BuyItem(int itemId,int price)
    {
        if (buyBlock)
        {
            yield break;
        }
        buyBlock = true;
        WWWForm form = new WWWForm();

        form.AddField("uid", GlobalPlayer.instance.UID);
        form.AddField("itemid", itemId);
        form.AddField("price", price);
        GUIHelper.ShowConnectionStart();

        WWW w = StatisticHandler.GetMeRightWWW(form, StatisticHandler.BUY_SKILL_ITEM);

        yield return w;

        Debug.Log(w.text);
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(w.text);

        if (xmlDoc.SelectSingleNode("result/error").InnerText == "0")
        {
            GlobalPlayer.instance.gold -=skills[itemId].price[price];
            ParseData(xmlDoc, "result");
            FindObjectOfType<SkillSelectGUI>().Reset();
        }
        else
        {
         /*   if (gui != null)
            {
                if (xmlDoc.SelectSingleNode("result/error").InnerText == "2")
                {
                    gui.Shop.MainMenu.MoneyError();
                }
                gui.SetError(xmlDoc.SelectSingleNode("result/errortext").InnerText);
            }*/
        }
        GUIHelper.ConnectionStop();
        buyBlock = false;
    }
				
	public bool IsPremium(){
        return isPremium;
	}
	public void Update(){
		
	
	}
	

	public void SetSetSize(int setSize){
		this.setSize =setSize;
	}
	
	public int GetSetSize(){
		return setSize;
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

    public static float GetMultiplierExp(string cause, int team)
    {
        return 1 + (instance.GetMod(PremiumSkillType.EXP, cause, team) / 100.0f);
	}
    public static float GetMultiplierMoney(string cause, int team)
    {

        return 1 + (instance.GetMod(PremiumSkillType.MONEY, cause, team) / 100.0f); 
    }

    public float GetMod(PremiumSkillType type,string cause, int team)
    {
        int maxAmount=0;
        int totalSum=0;
        foreach (PremiumSkill skill in open)
        {
            if (skill.Open()&&skill.type == type 
                && (skill.eventTriggers.Count == 0 || skill.eventTriggers.Contains(cause))
                && (skill.team==0||skill.team==team))
            {
                if (skill.maxAmount && maxAmount<skill.amount)
                {
                    maxAmount = skill.amount;
                }
                else
                {
                    totalSum += skill.amount;
                }
            }
        }
        totalSum += maxAmount;
        return totalSum;
    }
    public List<int> GetSkills()
    {
        List<int> lists = new List<int>();
        foreach (PremiumSkill skill in open)
        {
            if (skill.Open()&&skill.type == PremiumSkillType.SKILL)
            {
                lists.Add(skill.amount);
            }
        }
        return lists;
    }

    public void Regen(Pawn target, float time, float max)
    {
        if (target.health < max * REGEN_MIN / 100.0f && time + REGEN_TIME < Time.time)
        {
            if (regen!=null&&regen.Open())
            {
                target.health = max;
            }
        }
    }
    public PremiumSkill GetSkill(int id)
    {
        if (skills.ContainsKey(id))
        {
            return skills[id];
        }
        return null;
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