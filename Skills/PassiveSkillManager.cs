using UnityEngine;
using System;

using System.Collections;
using System.Collections.Generic;
using System.Xml;


public enum PASSIVESKILLCONDITION{None,NeedOpen,NeedSkillPoint};

public class  PassiveSkill{
	public PassiveSkill(){
			
			
	}
	
	public int buff;
	
	public int id;
	
	public int lvl;
	
	public bool open;
	
	public PASSIVESKILLCONDITION condition;
	
	public int openKey;
	
	public Texture2D iconGUI;
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
	
	
	public IEnumerator InitSkillTree(string XML){
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(XML);
       // Debug.Log(XML);
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
                    skill.condition = (PASSIVESKILLCONDITION)Enum.Parse(typeof(PASSIVESKILLCONDITION), skillNode.SelectSingleNode("condition").InnerText);
                    skill.openKey = int.Parse(skillNode.SelectSingleNode("openKey").InnerText);
				}
					
				WWW www = StatisticHandler.GetMeRightWWW( skillNode.SelectSingleNode ("guiimage").InnerText);
			
				yield return www;
                skill.iconGUI = new Texture2D(www.texture.width, www.texture.height);
                www.LoadImageIntoTexture(skill.iconGUI);
                skillClass.allSkill[skill.id] = skill;
			}
            skillClass.totalPoint = totalSkill;
		}
		
		
	}
	public void UpdateSkill(string XML,int classId,int skillId){
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(XML);
		bool open = bool.Parse (xmlDoc.SelectSingleNode ("spendskill/open").InnerText);
		if(open){
			skillPointLeft= int.Parse (xmlDoc.SelectSingleNode ("spendskill/skillpoint").InnerText);
			allSkill[classId].Open(skillId);
		}
	}
	public List<int>  GetSkills(int classID){

        if (allSkill!=null&&allSkill.Length > classID&&allSkill[classID] != null)
        {
           
			return allSkill[classID].GetSkills();
		}else{
			return new List<int>();
		}
	}
	
	public void SpendSkillpoint(int classId,int id){
		if(1>skillPointLeft){
			return;
		}
		if(allSkill[classId].CanOpen(id)){
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