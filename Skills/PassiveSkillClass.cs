using UnityEngine;
using System;

using System.Collections;
using System.Collections.Generic;


public enum PASSIVESKILLCONDITION{NeedOpen,NeedSkillPoint}

public PassiveSkill{
	public PassiveSkill(int buff,int id,int lvl,bool open ){
			this.buff =buff;
			this.id = id;
			
	}
	
	public int buff;
	
	public int id;
	
	public int lvl;
	
	public bool open;
	
	public PASSIVESKILLCONDITION condition;
	
	public int openKey;
}

public class PassiveSkillClass {
	public Dicrionary<int,PassiveSkill> allSkill = new Dicrionary<int,PassiveSkill>() ;
	
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
			cached.add(skill.buff);			
		}
		
		return cached;
	}
	public bool CanOpen(int id){
		PassiveSkill skill = allSkill[id];
		switch(){
			case PASSIVESKILLCONDITION.NeedOpen:
				return openKey.Contains(skill.openKey);
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
		skillPointLeft= int.Parse (xmlDoc.SelectSingleNode ("leveling/skillpoint").InnerText);
		allSkill = new PassiveSkillClass[classAmount];
		int i =0;
		
		foreach (XmlNode classNode in xmlDoc.SelectNodes("leveling/passiveskill/class")) {
			PassiveSkillClass skillClass =new PassiveSkillClass(branchAmount,lvlAmount);
			allSkill[i++]=skillClass;
			int totalSkill=0;
			foreach (XmlNode skillNode in xmlDoc.SelectNodes("skill")) {
				PassiveSkill skill = new PassiveSkill();
				skill.id = int.Parse(skillnode.SelectSingleNode ("id").InnerText);
				skill.buff = int.Parse(skillnode.SelectSingleNode ("buff").InnerText);
				skill.lvl = int.Parse(skillnode.SelectSingleNode ("lvl").InnerText);
				skill.open = bool.Parse(skillnode.SelectSingleNode ("open").InnerText);
				if(skill.open){
					totalSkill+=	skill.lvl;
					skillClass.openSkills.Add(skill.id);
				}else{
					skill.condition = (PASSIVESKILLCONDITION)int.Parse(skillnode.SelectSingleNode ("condition").InnerText);
					skill.openKey = int.Parse(skillnode.SelectSingleNode ("openKey").InnerText);
				}
			
			}
			skillClass.totalPoint=totalPoint;
		}
		
		
	}
	
	public List<int>  GetSkills(int classID){
		return allSkill[classID].GetSkills();
	}
	
	public void SpendSkillpoint(int id){
		if(allSkill[classId].lvl<skillPointLeft){
			return;
		}
		if(allSkill[classId].CanOpen(branch,lvl){
			WWWForm form = new WWWForm ();
			form.AddField ("uid", GlobalPlayer.instance.UID);
			form.AddField ("id", id.ToString());
			StartCoroutine(SpendSkill(form)):
								
		}
		
		
	}
	private IEnumerator SpendSkill(WWWForm form){
			WWW w =StatisticHandler.GetMeRightWWW(form,StatisticHandler.SPEND_SKILL_POINT);
			
			yield return w;
			
			InitSkillTree(w);
			
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