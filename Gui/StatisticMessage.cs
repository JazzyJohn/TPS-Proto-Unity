using UnityEngine;
using System.Collections;

public class StatisticMessage : MonoBehaviour {

	public UILabel Name;
	public UILabel Kill;
	public UILabel Dead;
	public UILabel HelpKill;
	public UILabel Ping;

	public string UID;
	public Player I_am;



	// Use this for initialization
	void Start () {
	
	}

	public void SetStartInfo(string NamePlayer, int Kills, int Deads, int HelpKills, Player MyPlayer)
	{
		Name.text = NamePlayer;
		Kill.text = Kills.ToString();
		Dead.text = Deads.ToString();
		HelpKill.text = HelpKills.ToString();
		I_am = MyPlayer;
	}

	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate()
	{
		//найти пинг
		Ping.text = "0";
	}
}
