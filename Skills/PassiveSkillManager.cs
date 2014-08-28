using UnityEngine;
using System;

using System.Collections;
using System.Collections.Generic;
using System.Xml;


public enum PASSIVESKILLCONDITION{NeedOpen,NeedSkillPoint};

public class  PassiveSkill{
	public PassiveSkill(){
			
			
	}
	
	public int buff;
	
	public int id;
	
	public int lvl;
	
	public bool open;
	
	public PASSIVESKILLCONDITION condition;
	
	public int openKey;
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
	public bool CanOpen(int id){
		PassiveSkill skill = allSkill[id];
        switch (skill.condition)
        {
			case PASSIVESKILLCONDITION.NeedOpen:
                return openSkills.Contains(skill.openKey);
			break;
			case PASSIVESKILLCONDITION.NeedSkillPoint:
				return totalPoint>=skill.openKey;
			break;
		}
		return false;
	}

}

public class PassiveSkillManager : MonoBehaviour
{
	public PassiveSkillClass[] allSkill;
	
	public int skillPointLeft;
	
	
	public void InitSkillTree(string XML){
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(XML);
		int classAmount = int.Parse (xmlDoc.SelectSingleNode ("leveling/classes/classcount").InnerText);
		skillPointLeft= int.Parse (xmlDoc.SelectSingleNode ("leveling/player/skillpoint").InnerText);
		allSkill = new PassiveSkillClass[classAmount];
		int i =0;
		
		foreach (XmlNode classNode in xmlDoc.SelectNodes("leveling/passiveskill/class")) {
			PassiveSkillClass skillClass =new PassiveSkillClass();
			allSkill[i++]=skillClass;
			int totalSkill=0;
            foreach (XmlNode skillNode in classNode.SelectNodes("skill"))
            {
				PassiveSkill skill = new PassiveSkill();
                skill.id = int.Parse(skillNode.SelectSingleNode("id").InnerText);
                skill.buff = int.Parse(skillNode.SelectSingleNode("buff").InnerText);
                skill.lvl = int.Parse(skillNode.SelectSingleNode("lvl").InnerText);
                skill.open = bool.Parse(skillNode.SelectSingleNode("open").InnerText);
				if(skill.open){
					totalSkill+=	skill.lvl;
					skillClass.openSkills.Add(skill.id);
				}else{
                    skill.condition = (PASSIVESKILLCONDITION)int.Parse(skillNode.SelectSingleNode("condition").InnerText);
                    skill.openKey = int.Parse(skillNode.SelectSingleNode("openKey").InnerText);
				}
                skillClass.allSkill[skill.id] = skill;
			}
            skillClass.totalPoint = totalSkill;
		}
		
		
	}
	
	public List<int>  GetSkills(int classID){
		return allSkill[classID].GetSkills();
	}
	
	public void SpendSkillpoint(int classId,int id){
		if(1>skillPointLeft){
			return;
		}
		if(allSkill[classId].CanOpen(id)){
			WWWForm form = new WWWForm ();
			form.AddField ("uid", GlobalPlayer.instance.UID);
			form.AddField ("id", id.ToString());
			StartCoroutine(SpendSkill(form));
								
		}
		
		
	}
	private IEnumerator SpendSkill(WWWForm form){
			WWW w =StatisticHandler.GetMeRightWWW(form,StatisticHandler.SPEND_SKILL_POINT);
			
			yield return w;
			
			InitSkillTree(w.text);
			
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