using UnityEngine;
using System.Collections;

public class SkillGui : MonoBehaviour {

	SkillSelectGUI skillSelectGUI;

    BasePassiveSkill skill;

	public int statusSkill
	{
		get
		{
			if(skill == null)
			{
				return 3;
			}

			if(skill.Open())
				return 0;
			else if(skill.CanBeOpen())
				return 1;
			else
				return 2;
		}
	}

    public int id;

    public UISprite sprite;

    public SkillSelectGUI.TypeSkill type;

	// Use this for initialization
	void Start () 
	{
		skillSelectGUI = Transform.FindObjectOfType<SkillSelectGUI>();
	}

	public void Show()
	{
        skillSelectGUI.ShowSkill(skill);
	}

	// Update is called once per frame
	void Update () 
	{
	
	}

    public void Resolve()
    {
        switch (type)
        {
            case SkillSelectGUI.TypeSkill.Normal:
                skill =PassiveSkillManager.instance.GetSkill(id);
                break;
            case SkillSelectGUI.TypeSkill.Prem:
                skill = PremiumManager.instance.GetSkill(id);
                break;
        }
        if (skill != null)
        {
            sprite.spriteName = skill.iconGUI;
        }


		StartCoroutine(setGUI());
        
    }

	public IEnumerator setGUI()
	{
		yield return new WaitForSeconds(0.1f);
		UIButton but = transform.GetComponent<UIButton>();
		switch(statusSkill)
		{
			//изучен
		case 0:
			but.normalSprite = "lot_silver";
			but.defaultColor = Color.green;
			break;
			//можно изучить
		case 1:
			but.normalSprite = "lot_silver";
			but.defaultColor = Color.white;
			break;
			//закрыт
		case 2:
			but.normalSprite = "lot_silver";
			but.defaultColor = Color.red;
			break;
			//Ошибка
		case 3:
			StartCoroutine(setGUI());
			break;
		}
		GetComponent<UISprite>().color = but.defaultColor;
	}
}
