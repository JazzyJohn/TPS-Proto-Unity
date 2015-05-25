
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using Random = UnityEngine.Random;

public class LotteryManager : MonoSingleton<LotteryManager>
{
	private int m_freeReplays;
	private int m_boughtReplays;
	private int m_totalBoughtReplays;

    public int DebugRESULT;

    public static int LOTTERY_PRICE = 15;
	private int freeReplays
	{
		get { return m_freeReplays; }
		set {
			m_freeReplays = value;
			PlayerPrefs.SetInt("LotteryManager_freeReplays", m_freeReplays);
		}
	}
	
	private int boughtReplays
	{
		get { return m_boughtReplays; }
		set {
			m_boughtReplays = value;
			PlayerPrefs.SetInt("LotteryManager_boughtReplays", m_boughtReplays);
		}
	}

	private int totalBoughtReplays
	{
		get { return m_totalBoughtReplays; }
		set {
			m_totalBoughtReplays = value;
			PlayerPrefs.SetInt("LotteryManager_totalBoughtReplays", m_totalBoughtReplays);
		}
	}

	public int AvailableReplays
	{
		get { return freeReplays + boughtReplays; }
	}
	
	public bool CanPlay
	{
		get { return AvailableReplays > 0; }
	}

	public bool CanBuyReplay
	{
		
		get {
            if (GlobalPlayer.instance.gold > LOTTERY_PRICE)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
	}

	private const int nothingWinIndex = 2;
	private const int replayWinIndex = 7;

	private static int ln = 0;

	private List<Func<int, float>> chances = new List<Func<int, float>>()
	{
		n => 3f,   //0  30G
		n => 30f,  //1 1000K
		n => 0f,   //2 none
		n => 10f,  //3 skill
		n => 0.1f, //4 gold armor
		n => 10f,  //5 5000K
		n => (ln != n) && ((ln = n) % 20 == 0) ? 50 : 0.01f, //6 Gold Weapon
		n => 30f,  //7 Replay
		n => 0.1f + (n / 2) * 0.5f, //8 100G
		n => 5f,   //9 10000K
		n => 0.1f, //10 Kit
		n => 1f    //11 1E
	};

	public event EventHandler OnReplaysCountChanged = delegate { };

	private const int DAILY_BONUS = 2;

	protected override void Awake()
	{
		base.Awake();

		
	}

    public void Parse(XmlDocument xmlDoc,string rootElement)
    {
       
      
        m_boughtReplays = int.Parse(xmlDoc.SelectSingleNode(rootElement+"/lottery/boughtReplays").InnerText);
        m_totalBoughtReplays = int.Parse(xmlDoc.SelectSingleNode(rootElement + "/lottery/totalBoughtReplays").InnerText);
        m_freeReplays = int.Parse(xmlDoc.SelectSingleNode(rootElement + "/lottery/freeReplays").InnerText);

        IndicatorManager.instance.Set(IndicatorManager.LOTTERY, AvailableReplays);
    }


	public void BuyReplay (int amount)
	{
		/*if(!CanBuyReplay)
			return;*/

        StartCoroutine(_BuyReplay(amount));

	
	}
    public IEnumerator _BuyReplay(int amount)
    {
      
        WWWForm form = new WWWForm();

        form.AddField("amount", amount);
        form.AddField("uid",GlobalPlayer.instance.UID);

        GUIHelper.ShowConnectionStart();

        WWW w = StatisticHandler.GetMeRightWWW(form, StatisticHandler.BUY_LOTTERY_PLAY);

        yield return w;

        Debug.Log(w.text);
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(w.text);

        if (xmlDoc.SelectSingleNode("result/error").InnerText == "0")
        {
            GlobalPlayer.instance.gold -= LOTTERY_PRICE * amount;
            boughtReplays = boughtReplays + 1 * amount;
            totalBoughtReplays = totalBoughtReplays + 1 * amount;
            PublishReplaysChangedEvent();
        }
        else
        {
            FindObjectOfType<MainMenuGUI>().MoneyError();
        }
        GUIHelper.ConnectionStop();
    }
	private int result = -1;

	public int Play()
	{
		if(!CanPlay)
		{
			Debug.LogError("Check CanPlay before call Play()");
			return nothingWinIndex;
		}

		result = GetRandomResult();


        StartCoroutine(_SendResult(result));
		if(boughtReplays > 0)
		{
			boughtReplays = boughtReplays - 1;
		}
		else if(freeReplays > 0)
		{
			freeReplays = freeReplays - 1;
		}

		PublishReplaysChangedEvent();

		return result;
	}

	public void ApplyLastPlay()
	{
		if(result < 0)
			return;

		// тут надо отправить результа игры на сервер
		// 'this.result' это индекс выигранного элемента из списка 'chances'
		// что именно выиграно смотри комментарии около переменной 'chances'

		if(result == replayWinIndex)
		{
			freeReplays = freeReplays + 1;
			PublishReplaysChangedEvent();
		}

		result = -1;
	}
    public IEnumerator _SendResult(int _result)
    {

        WWWForm form = new WWWForm();

        form.AddField("result", _result);
        form.AddField("uid", GlobalPlayer.instance.UID);

      

        WWW w = StatisticHandler.GetMeRightWWW(form, StatisticHandler.SEND_LOTTERY_RESULT);

        yield return w;

        Debug.Log(w.text);
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(w.text);

        if (xmlDoc.SelectSingleNode("result/error").InnerText == "0")
        {
            Parse(xmlDoc, "result");
            string result = xmlDoc.SelectSingleNode("result/state").InnerText;
            IEnumerator numenator;
            switch(result){
               
                case "MONEY":
                    GlobalPlayer.instance.gold += int.Parse(xmlDoc.SelectSingleNode("result/gold").InnerText);
                    GlobalPlayer.instance.cash += int.Parse(xmlDoc.SelectSingleNode("result/cash").InnerText);
                    PassiveSkillManager.instance.skillPointLeft  += int.Parse(xmlDoc.SelectSingleNode("result/skillPointLeft").InnerText);
                    break;
                case "PREMIUM":
                    numenator = GlobalPlayer.instance.ReloadStats();
                    while (numenator.MoveNext())
                    {
                        yield return numenator.Current;
                    }
                    break;
                case "ITEM":
                    numenator = ItemManager.instance.ReLoadItemsSync();
                    while (numenator.MoveNext())
                    {
                        yield return numenator.Current;
                    }
                    break;
                 case "KIT":
                     numenator = GlobalPlayer.instance.ReloadStats();
                    while (numenator.MoveNext())
                    {
                        yield return numenator.Current;
                    }
                     numenator = ItemManager.instance.ReLoadItemsSync();
                    while (numenator.MoveNext())
                    {
                        yield return numenator.Current;
                    }
                    break;
            }
        }
    }
	private int GetRandomResult()
	{
        return DebugRESULT;
		float[] precalculated = new float[chances.Count];
		float sum = 0;

		for(int i = 0; i < chances.Count; i++)
		{
			float chance = chances[i](totalBoughtReplays);
			precalculated[i] = chance;
			sum += chance;
		}
		
		float randomWeight = Random.Range(0f, sum);
		float weight = 0;

		for(int i = 0; i < chances.Count; i++)
		{
			weight += precalculated[i];
			
			if(weight > randomWeight)
			{
				return i;
			}
		}

		Debug.LogWarning("Не удалось выбрать результат лотерейной игры");

		return nothingWinIndex;
	}

	private void PublishReplaysChangedEvent()
	{
        IndicatorManager.instance.Set(IndicatorManager.LOTTERY,AvailableReplays);
		this.OnReplaysCountChanged(this, EventArgs.Empty);
	}
}