using UnityEngine;
using System.Collections;

public class StatisticMessage : MonoBehaviour {

	public UILabel Name;
	public UILabel Kill;
	public UILabel Dead;
	public UILabel HelpKill;
	public UILabel Lvl;
    public UILabel Rating;
	public string UID;
	public Player I_am;



    void Awake()
    {
	
	}

    public void Hide(bool Bool)
    {

        //Debug.Log ("Dead" + Deads);
        gameObject.SetActive(!Bool);
    }

	public void SetStartInfo( Player MyPlayer)
	{

		//Debug.Log ("Dead" + Deads);
        Name.text = MyPlayer.PlayerName;
        Kill.text = (MyPlayer.Score.Kill + MyPlayer.Score.AIKill + MyPlayer.Score.RobotKill).ToString();
        Dead.text = MyPlayer.Score.Death.ToString();
        HelpKill.text = MyPlayer.Score.Assist.ToString();
        Lvl.text = MyPlayer.Score.Lvl.ToString();
        if (Rating != null)
        {
            Rating.text = MyPlayer.Score.rating.ToString();
        }
		I_am = MyPlayer;
	}

	void Update()
	{
		//найти пинг
		//Ping.text = "0";
	}
}
