using UnityEngine;
using System;

using System.Collections;
using System.Collections.Generic;
using System.Xml;


public enum PASSIVESKILLCONDITION{None,NeedOpen,NeedSkillPoint};

public class BasePassiveSkill
{
    public string name;

    public string descr;

    public string iconGUI;

    public virtual bool Open()
    {
        return true; ;
    }
     public virtual bool CanBeOpen()
    {
        return true;
    }
}

public class PassiveSkill : BasePassiveSkill
{
	public PassiveSkill(){
			
			
	}

    public PassiveSkillClass passiveClass;

	public int buff;
	
	public int id;

    public int classId;
	
	public int lvl;
	
	public bool open;
	
	public PASSIVESKILLCONDITION condition;
	
	public int openKey;

    public int cash;

    public int gold;

    public int exp;
	
    public override bool CanBeOpen(){
        return passiveClass.CanOpen(id);
    }
    public override bool Open()
    {
        return open;
    }
}

public class RefundData
{
    public int cash=0;

    public int gold=0;

    public int exp=0;
	
}
public class PassiveSkillClass {
    public Dictionary<int, PassiveSkill> allSkill = new Dictionary<int, PassiveSkill>();
	
	public List<int> openSkills  = new List<int>();
	
	public List<int>  cached;
	
	public int totalPoint;
	
	public PassiveSkillClass(){
	
	}
	public List<int>  GetSkills(){
		if(cached!=null){
			return cached;
		}
		cached	 = new List<int>();
		foreach(int key  in openSkills)
		{
			PassiveSkill skill = allSkill[key];
			cached.Add(skill.buff);			
		}
		
		return cached;
	}
    public void Reset()
    {
        openSkills.Clear();
    }
	public void Open(int id){
		allSkill[id].open = true;
        openSkills.Add(id);
		cached=null;
	}
	public bool CanOpen(int id){
		PassiveSkill skill = allSkill[id];
		if(skill.open){
			return false;
		}
        switch (skill.condition)
        {
			case PASSIVESKILLCONDITION.NeedOpen:
                return openSkills.Contains(skill.openKey);
			break;
			case PASSIVESKILLCONDITION.NeedSkillPoint:
				return totalPoint>=skill.openKey;
			break;
		}
		return true;
	}

}

public class PassiveSkillManager : MonoBehaviour
{
	public PassiveSkillClass[] allSkill;
	
	public int skillPointLeft;

    Dictionary<int, PassiveSkill> allSkillDict = new Dictionary<int, PassiveSkill>();
	public void InitSkillTree(string XML){
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(XML);
       // Debug.Log(XML);
		int classAmount = int.Parse (xmlDoc.SelectSingleNode ("leveling/classes/classcount").InnerText);
		skillPointLeft= int.Parse (xmlDoc.SelectSingleNode ("leveling/player/skillpoint").InnerText);
		allSkill = new PassiveSkillClass[classAmount];
		int i =0;
		
		foreach (XmlNode classNode in xmlDoc.SelectNodes("leveling/passiveskill/class")) {
			PassiveSkillClass skillClass =new PassiveSkillClass();
			allSkill[i]=skillClass;
			int totalSkill=0;
            foreach (XmlNode skillNode in classNode.SelectNodes("skill"))
            {
				PassiveSkill skill = new PassiveSkill();
                skill.id = int.Parse(skillNode.SelectSingleNode("id").InnerText);
                skill.classId = i;
                skill.passiveClass = skillClass;
                skill.buff = int.Parse(skillNode.SelectSingleNode("buff").InnerText);
                skill.cash = int.Parse(skillNode.SelectSingleNode("cash").InnerText);
                skill.exp = int.Parse(skillNode.SelectSingleNode("exp").InnerText);
                skill.gold = int.Parse(skillNode.SelectSingleNode("gold").InnerText);
                skill.lvl = int.Parse(skillNode.SelectSingleNode("lvl").InnerText);
                skill.open = bool.Parse(skillNode.SelectSingleNode("open").InnerText);
				if(skill.open){
					totalSkill+=	skill.lvl;
					skillClass.openSkills.Add(skill.id);
				}else{
                    skill.condition = (PASSIVESKILLCONDITION)Enum.Parse(typeof(PASSIVESKILLCONDITION), skillNode.SelectSingleNode("condition").InnerText);
                    skill.openKey = int.Parse(skillNode.SelectSingleNode("openKey").InnerText);
				}
					
			
                skill.iconGUI = skillNode.SelectSingleNode("guiimage").InnerText;

                skill.name = skillNode.SelectSingleNode("name").InnerText;
                skill.descr = skillNode.SelectSingleNode("descr").InnerText;
                allSkillDict.Add(skill.id, skill);
                skillClass.allSkill[skill.id] = skill;
			}
            i++;
            skillClass.totalPoint = totalSkill;
		}
		
		
	}
	public void UpdateSkill(string XML,int classId,int skillId){
       // Debug.Log(XML);
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(XML);
        if (xmlDoc.SelectSingleNode("result/error").InnerText == "0")
        {
            skillPointLeft -= allSkillDict[skillId].exp;
            GlobalPlayer.instance.cash -= allSkillDict[skillId].cash;
            GlobalPlayer.instance.gold -= allSkillDict[skillId].gold;
			allSkill[classId].Open(skillId);
            FindObjectOfType<SkillSelectGUI>().Reset();
        }
        else
        {
            if (xmlDoc.SelectSingleNode("result/error").InnerText == "2")
            {
               FindObjectOfType<MainMenuGUI>().MoneyError();
            }
        }
        GUIHelper.ConnectionStop();
	}
	public List<int>  GetSkills(int classID){

        if (allSkill!=null&&allSkill.Length > classID&&allSkill[classID] != null)
        {
           
			return allSkill[classID].GetSkills();
		}else{
			return new List<int>();
		}
	}
    public Action callback;
	public void SpendSkillpoint(int classId,int id,Action callback){
		if(1>skillPointLeft){
			return;
		}
        GUIHelper.ShowConnectionStart();
		if(allSkill[classId].CanOpen(id)){
            this.callback = callback;
			WWWForm form = new WWWForm ();
			form.AddField ("uid", GlobalPlayer.instance.UID);
			form.AddField ("id", id.ToString());
			StartCoroutine(SpendSkill(form,classId,id));
								
		}
		
		
	}
	private IEnumerator SpendSkill(WWWForm form,int classId,int id){
			WWW w =StatisticHandler.GetMeRightWWW(form,StatisticHandler.SPEND_SKILL_POINT);
			
			yield return w;
			
			UpdateSkill(w.text,classId,id);
            callback();
			
	}
    public void ResetSkills()
    {
        foreach (PassiveSkill skill in allSkillDict.Values)
        {
            if (skill.open)
            {
                skill.open = false;
            }
        }
        foreach (PassiveSkillClass classPassive in allSkill)
        {
            if (classPassive != null)
            {
                classPassive.Reset();
            }
        }
    }
    public IEnumerator ResetSkillRequest()
    {
       
        
        WWWForm form = new WWWForm();

        form.AddField("uid", GlobalPlayer.instance.UID);
       

        GUIHelper.ShowConnectionStart();

        WWW w = StatisticHandler.GetMeRightWWW(form, StatisticHandler.RESET_SKILL);

        yield return w;

        Debug.Log(w.text);
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(w.text);

        if (xmlDoc.SelectSingleNode("result/error").InnerText == "0")
        {

            ///yield return new WaitForSeconds(10.0f);
            RefundData data  = GetRefundData();
            skillPointLeft +=data.exp;
            GlobalPlayer.instance.cash += data.cash;
            GlobalPlayer.instance.gold += data.gold -PremiumManager.instance.resetSkillPrice;
            ResetSkills();
            FindObjectOfType<SkillSelectGUI>().Reset();
            GUIHelper.ConnectionStop();
        }
        else
        {
            GUIHelper.ConnectionStop();
                if (xmlDoc.SelectSingleNode("result/error").InnerText == "2")
                {
                     FindObjectOfType<MainMenuGUI>().MoneyError();
                }
                //gui.SetError(xmlDoc.SelectSingleNode("result/errortext").InnerText);
            
        }

        //buyBlock =false;
    }
    public PassiveSkill GetSkill(int id)
    {
        if(allSkillDict.ContainsKey(id)){
            return allSkillDict[id];
        }
        return null;
    }

    public RefundData GetRefundData()
    {
            RefundData data = new RefundData();
           foreach(PassiveSkill skill in allSkillDict.Values) {
               if (skill.open)
               {
                //   Debug.Log(skill.exp);
                   data.cash += skill.cash;
                   data.gold += skill.gold;
                   data.exp += skill.exp;
               }
               
           }
           return data;
    }

	private static PassiveSkillManager s_Instance = null;
	
	public static PassiveSkillManager instance {
		get {
			if (s_Instance == null) {
				//Debug.Log ("FIND");
				// This is where the magic happens.
				//  FindObjectOfType(...) returns the first AManager object in the scene.
				s_Instance =  FindObjectOfType(typeof (PassiveSkillManager)) as PassiveSkillManager;
			}

		
			// If it is still null, create a new instance
			if (s_Instance == null) {
			//	Debug.Log ("CREATE");
				GameObject obj = new GameObject("LevelingManager");
				s_Instance = obj.AddComponent(typeof (PassiveSkillManager)) as PassiveSkillManager;
				
			}
			
			return s_Instance;
		}
	}

}