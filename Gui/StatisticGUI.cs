using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class StatisticGUI : MonoBehaviour {
	public MainMenuStatistic statistic;

	public UIPanel statisticPanel;

    public MainMenuGUI MainMenu;

	void Start(){
	
		statistic.Main = this;	
		
		statistic.AllStat.AddRange(statistic.AllStatGrild.transform.GetComponentsInChildren<UILabel>());
        foreach (UILabel Label in statistic.AllStat)
		{
            statistic.DefValueStat.Add(Label.text);
		}
	
	}
	public void ShowProfile()
	{

        if (statisticPanel.alpha > 0f)
		{
			MainMenu.HideAllPanel();
			MainMenu._PanelsNgui.SliderPanel.alpha = 1f;
		}
		else
		{
			MainMenu.HideAllPanel();
			HideAllPanel();
			statisticPanel.alpha= 1.0f;
	
		}
      
	}
    public void HideAllPanel()
    {

    }
	public void EditCategoryStat(int num)
	{
        statistic.OpenTab = (MainMenuStatistic.Tab)num;

		switch(statistic.OpenTab)
		{
            case MainMenuStatistic.Tab.Achive:
                {
                    statistic.Clean(AchivGUI.type.Achiv);

                    List<Achievement> achivments = AchievementManager.instance.GetAchivment();
                    foreach (Achievement achiv in achivments)
                    {
                        statistic.AddObj(achiv, AchivGUI.type.Achiv);
                    }
                }
                break;
            case MainMenuStatistic.Tab.Missions:
                {

                    statistic.Clean(AchivGUI.type.Dailic);
                    List<Achievement> achivments = AchievementManager.instance.GetDaylics();
                    foreach (Achievement achiv in achivments)
                    {
                        statistic.AddObj(achiv, AchivGUI.type.Dailic);
                    }
                }
                
                break;
            case MainMenuStatistic.Tab.Stat:
            statistic.StatToDef();
            foreach (UILabel Label in statistic.AllStat)
			{
				//Label.text += полученное значение;
			}
			break;
		}
        StartCoroutine(statistic.ReSize());
	}
}

[Serializable]
public class MainMenuStatistic
{
    [HideInInspector]
    public StatisticGUI Main;
	public UISprite Background;
	public UITable[] Tables;
	public int CoutColl;

	public List<string> DefValueStat;
	public List<UILabel> AllStat;

	public UIGrid AllStatGrild;

	public enum Tab{Stat, Missions, Achive};
	public Tab OpenTab = Tab.Stat;

    public AchivGUI achivPerfab;

    public AchivGUI daylicPrefab;

	public UITable AchivTable;
	public UITable DailicTable;

	public void StatToDef()
	{
		for(int i = 0; i<AllStat.Count; i++)
		{
			AllStat[i].text = DefValueStat[i];
		}
	}
	
	int Width;

	public void AddObj(Achievement achiv, AchivGUI.type TypeObj)
	{
        Transform obj;
        if (TypeObj == AchivGUI.type.Achiv)
		     obj = Transform.Instantiate(achivPerfab.transform) as Transform;
        else
            obj = Transform.Instantiate(daylicPrefab.transform) as Transform;
		AchivGUI script = obj.GetComponent<AchivGUI>();
        script.Main = Main;
        script.Type = TypeObj;

        script.GetInfo(achiv);
	}

	public void GetParent(AchivGUI obj)
	{
		if(obj.Type == AchivGUI.type.Achiv)
			obj.transform.parent = AchivTable.transform;
		else
			obj.transform.parent = DailicTable.transform;
        obj.transform.localScale = new Vector3(1f, 1f, 1f);
        obj.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        obj.transform.localPosition = new Vector3(0f, 0f, 0f);
     
	

		obj.Widget.alpha = 1f;
	}

	public int EditCountColl
	{
		set
		{
			CoutColl = value;
			foreach(UITable Table in Tables)
			{
				Table.columns = value;
				Table.Reposition();
			}
		}
	}

	public void Clean(AchivGUI.type Type)
	{
		switch(Type)
		{
		case AchivGUI.type.Achiv:
             foreach (Transform Obj in AchivTable.transform)
			{
				GameObject.Destroy(Obj.gameObject);
			}
			
			break;
		case AchivGUI.type.Dailic:
            foreach (Transform Obj in DailicTable.transform)
            {
                GameObject.Destroy(Obj.gameObject);
            }
			break;
		}
	}

	public IEnumerator ReSize()
	{

        yield return new WaitForEndOfFrame();
       
			AchivTable.Reposition();
			DailicTable.Reposition();
		
	}
}
