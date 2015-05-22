using UnityEngine;
using System.Collections;
using System;

public class SkillSelectGUI : MonoBehaviour {


	public enum TypeSkill{Normal, Prem};

	public TweenAlpha tween;
	public UIWidget widget;

    public BasePassiveSkill _skill;

    public InventoryGUI shop;

    public UIWidget resetWindow;
	
	public ElementNGUI elementNGUI;

	bool finish = true;

    bool inited = false;

    string baseFormat;

    void Awake()
    {
        baseFormat = elementNGUI.premActiveLabel.text;
    } 

	public void Finish()
	{
		finish = true;
	}

	IEnumerator PinPong() //Эфект появления скилла
	{
		if(widget.alpha == 1f)
		{
			finish = false;
			tween.PlayReverse();

			while(!finish)
			{
				yield return new WaitForEndOfFrame();
			}
		}

        SetSkillValue();

		tween.PlayForward();
	}


	//Записывает значения в интерфейс NGUI
	void SetSkillValue() 
	{


        elementNGUI.name.text = _skill.name;
        elementNGUI.description.text = _skill.descr;

        PassiveSkill skill = _skill as PassiveSkill;
        if (skill != null)
        {
            if (skill.open)
            {
                elementNGUI.Block.alpha = 0f;
                elementNGUI.Normal.alpha = 0f;
                elementNGUI.open.alpha = 1f;
            }
            else
            {
                if (skill.CanBeOpen())
                {
                    elementNGUI.Block.alpha = 0f;
                    elementNGUI.Normal.alpha = 1f;
                }
                else
                {
                    elementNGUI.Block.alpha = 1f;
                    elementNGUI.Normal.alpha = 0f;
                }
               
                elementNGUI.open.alpha = 0f;
            }
           
            elementNGUI.Prem.alpha = 0f;
            elementNGUI.premActive.alpha = 0f;
            elementNGUI.costN.credit.text = skill.cash.ToString();
            elementNGUI.costN.gold.text = skill.gold.ToString();
            elementNGUI.costN.exp.text = skill.exp.ToString();
        }
        PremiumSkill pSkill = _skill as PremiumSkill;
        if (pSkill != null)
        {
            elementNGUI.Normal.alpha = 0f;
            elementNGUI.open.alpha = 0f;
            elementNGUI.Block.alpha = 0f;
            if (pSkill.Open())
            { 
                elementNGUI.Prem.alpha = 0f;
                elementNGUI.premActive.alpha = 1f;
               
            }
            else
            {
                elementNGUI.Prem.alpha = 1f;
                elementNGUI.premActive.alpha = 0f;
            }
          
            elementNGUI.costP.day1.text = pSkill.price[0].ToString();
            elementNGUI.costP.day5.text = pSkill.price[1].ToString();
            elementNGUI.costP.day30.text = pSkill.price[2].ToString();
        }
	
	}
    public void Update()
    {


        PremiumSkill pSkill = _skill as PremiumSkill;
        if (pSkill != null && pSkill.Open())
        {


            elementNGUI.premActiveLabel.text = IndicatorManager.GetLeftTime(pSkill.timeEnd);
        }
    }
    public void ShowSkill(BasePassiveSkill skill)
	{
        if (skill == null)
        {
            return;
        }
		if(!finish) //Для исключения спама(нажатий быстрее чем раз в 0,4 сек)
			return;
        _skill = skill;
       
		StartCoroutine(PinPong());
	}

	public void ShowResetSkillWin()
	{
		RefundData data = PassiveSkillManager.instance.GetRefundData();

        elementNGUI.costResetSlill.text = PremiumManager.instance.resetSkillPrice.ToString();
		elementNGUI.resetCredit.text = data.cash.ToString();
		elementNGUI.resetGold.text =data.gold.ToString();
        elementNGUI.resetExp.text = data.exp.ToString();
	}

	public void ChangleClass(GameObject obj, int selectClass)
	{
		UIToggle tougle = obj.GetComponent<UIToggle>();
		if(!tougle.value)
			return;

		/*
		foreach(SkillGui skill in tougle.activeSprite.transform.GetComponentsInChildren<SkillGui>())
		{
			UIButton but = skill.transform.GetComponent<UIButton>();
			switch(skill.statusSkill)
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
			}
			skill.GetComponent<UISprite>().color = but.defaultColor;
		}
		*/

		finish = false;
		tween.PlayReverse();
	}

    public void InitSkills()
    {
        elementNGUI.cash.text = GlobalPlayer.instance.cash.ToString();
        elementNGUI.gold.text = GlobalPlayer.instance.gold.ToString();
        elementNGUI.exp.text = PassiveSkillManager.instance.skillPointLeft.ToString();
        if (inited)
        {
            return;
        }
        inited = true;
        SkillGui[] guis = FindObjectsOfType<SkillGui>();
        foreach (SkillGui gui in guis)
        {
            gui.Resolve();

        }
    }
    public void Open()
    {
         PassiveSkill skill = _skill as PassiveSkill;
        if (skill != null)
        {
            PassiveSkillManager.instance.SpendSkillpoint(skill.classId, skill.id, Reset);
        }
    }
    int priceKey;
    public void BuyPremskill(int price)
    {
          PremiumSkill skill = _skill as PremiumSkill;
          if (skill != null)
          {
              priceKey = price;
              shop.askWindow.action = FinishBuy;
              string text;

              text = TextGenerator.instance.GetMoneyText("buyGoldPrice", skill.price[price]);


              shop.askWindow.Show(text);
          }
    }
    public void FinishBuy()
    {
    
        PremiumSkill skill = _skill as PremiumSkill;
        if (skill != null)
        {
            StartCoroutine(PremiumManager.instance.BuyItem(skill.id, priceKey));
        }
    }
    public void Reset()
    {
        inited = false;
        InitSkills();
        SetSkillValue();
    }
    public void ResetSkills()
    {
        resetWindow.alpha = 0.0f;
        StartCoroutine(PassiveSkillManager.instance.ResetSkillRequest());
    }
	[System.Serializable]
	public class ElementNGUI
	{
		public UILabel name;
		public UILabel description;

		public CostNormal costN;
		public CostPrem costP;

		public UIWidget Normal;
		public UIWidget Prem;
        public UIWidget Block;

        public UIWidget open;
        public UIWidget premActive;



        public UILabel premActiveLabel;

		[System.Serializable]
		public struct CostNormal
		{
			public UILabel credit;
			public UILabel gold;
			public UILabel exp;
		}
		
		[System.Serializable]
		public struct CostPrem
		{
			public UILabel day1;
			public UILabel day5;
			public UILabel day30;
		}

		public UILabel costResetSlill;

		public UILabel resetCredit;
		public UILabel resetGold;
		public UILabel resetExp;

        public UILabel exp;
        public UILabel cash;
        public UILabel gold;
	}
}
