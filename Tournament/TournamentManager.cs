using UnityEngine;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

public class SocUser{
	public Texture2D avatar;
	
	public string name;
	
	public string uid;
    public SocUser(string uid)
    {
        this.uid = uid;
    }
}

public class Winner{
	public SocUser  user;
	
	public int score;
	
	public Winner(SocUser user,int score){
		this.user = user;
		this.score = score;
	}
}

public class BaseSocEvent{
	public bool isFinished;

	public bool isActive;

    public DateTime start;

    public DateTime end;
	
	public string name;

	public string desctiption;

    public int prizePlaces;
	
	public int[] goldReward;
	
	public int[] cashReward;

    public int[] expReward;

	public Winner[] winners;
}

public class Operation : BaseSocEvent
{

    public string id;

    public string counterEvent;

    public int toSendCounter;

    public int myCounter;

    public int myPlace;

    public void Increment(){
        myCounter++;
        toSendCounter++;
    }
    public void Increment(int i)
    {
        myCounter+=i;
        toSendCounter+=i;
    }
}

public enum TOPS
{
    KILLERS,
    AI_KILLERS,
    LVLS,
    CASH,
    DAYLIC
}


public class TournamentManager : MonoBehaviour, LocalPlayerListener, GameListener
{

    public static int KILLERS = 0;

    public static int AI_KILLERS = 1;

	public Dictionary<string,SocUser>  allUsers = new Dictionary<string,SocUser>();

	List<string> uids =  new List<string>();

    List<Winner[]> tops = new List<Winner[]>();


	

    public bool isLoaded = false;

    public bool dataLoaded = false;

    public Operation lastOperation;

    public Operation currentOperation;



    public Winner[] GetTop(TOPS top)
    {
        return tops[(int)top];
    }
    public List<Winner[]>  GetAllTops()
    {
        return tops;
    }

    public List<Winner[]> GetRandomTops(int amount, out List<int> indexs)
    {
        indexs = new List<int>();
        List<Winner[]> answer = new List<Winner[]>();
        if (amount >= tops.Count)
        {
            return tops;
        }
        List<int> index = new List<int>();
        for(int i =0;i <amount;i++){
            int r;
            do
            {
                r = UnityEngine.Random.Range(0, tops.Count);
            }
            while (index.Contains(r));
            index.Add(r);
            indexs.Add(r);
            answer.Add(tops[r]);
        }

        return answer;
    }

    public void ParseData(XmlDocument xmlDoc)
    {
        if (xmlDoc.SelectSingleNode("player/tournament") == null)
        {
			return;
		}
        XmlNodeList killersXml = xmlDoc.SelectNodes("player/globalkillers");

        Winner[] killers = new Winner[killersXml.Count];
		
		for(int j=0;j<killersXml.Count;j++){
			XmlNode node  =killersXml[j];
			killers[j] = new Winner(GetUser(node.SelectSingleNode("uid").InnerText),int.Parse(node.SelectSingleNode("score").InnerText));
			
		}
        tops.Add(killers);
        killersXml = xmlDoc.SelectNodes("player/globalaikillers");

        killers = new Winner[killersXml.Count];
		
		for(int j=0;j<killersXml.Count;j++){
			XmlNode node  =killersXml[j];
            killers[j] = new Winner(GetUser(node.SelectSingleNode("uid").InnerText), int.Parse(node.SelectSingleNode("score").InnerText));
			
		}
        tops.Add(killers);

        killersXml = xmlDoc.SelectNodes("player/toplvls");

        killers = new Winner[killersXml.Count];

        for (int j = 0; j < killersXml.Count; j++)
        {
            XmlNode node = killersXml[j];
            killers[j] = new Winner(GetUser(node.SelectSingleNode("uid").InnerText), int.Parse(node.SelectSingleNode("score").InnerText));

        }
        tops.Add(killers);

        killersXml = xmlDoc.SelectNodes("player/topcash");

        killers = new Winner[killersXml.Count];

        for (int j = 0; j < killersXml.Count; j++)
        {
            XmlNode node = killersXml[j];
            killers[j] = new Winner(GetUser(node.SelectSingleNode("uid").InnerText), int.Parse(node.SelectSingleNode("score").InnerText));

        }
        tops.Add(killers);

        killersXml = xmlDoc.SelectNodes("player/daylic");

        killers = new Winner[killersXml.Count];

        for (int j = 0; j < killersXml.Count; j++)
        {
            XmlNode node = killersXml[j];
            killers[j] = new Winner(GetUser(node.SelectSingleNode("uid").InnerText), int.Parse(node.SelectSingleNode("score").InnerText));

        }
        tops.Add(killers);
        XmlNode lastOperationNode  = xmlDoc.SelectSingleNode("player/lastoperation");
        if (lastOperationNode!=null)
        {
            lastOperation = new Operation();
            lastOperation.isFinished = false;
            lastOperation.isActive = false;
            ParseOperation(lastOperation, lastOperationNode);


        }
        XmlNode operationNode = xmlDoc.SelectSingleNode("player/currentoperation");
        if (operationNode != null)
        {
            currentOperation = new Operation();
            currentOperation.isFinished = false;
            currentOperation.isActive = true;
            ParseOperation(currentOperation, operationNode);


        }
        dataLoaded = true;
		Application.ExternalCall ("GetUsers",String.Join(", ", uids.ToArray()));
        EventHolder.instance.Bind(this);
	}

    public void ParseOperation(Operation oper, XmlNode node)
    {
        oper.id = node.SelectSingleNode("id").InnerText;
        oper.prizePlaces = int.Parse(node.SelectSingleNode("prizeplaces").InnerText);
        oper.cashReward = new int[oper.prizePlaces+1];
        XmlNodeList rewards = node.SelectNodes("cashReward");
        for (int i = 0; i <= oper.prizePlaces; i++)
        {
            oper.cashReward[i] = int.Parse(rewards[i].InnerText);
        }
        oper.goldReward = new int[oper.prizePlaces+1];
        rewards = node.SelectNodes("goldReward");
        for (int i = 0; i <= oper.prizePlaces; i++)
        {
            oper.goldReward[i] = int.Parse(rewards[i].InnerText);
        }

        oper.expReward = new int[oper.prizePlaces+1];
        XmlNodeList winners = node.SelectNodes("winners");
        oper.winners = new Winner[winners.Count];
        for (int j = 0; j < winners.Count; j++)
        {
            XmlNode winner = winners[j];
            oper.winners[j] = new Winner(GetUser(winner.SelectSingleNode("uid").InnerText), int.Parse(winner.SelectSingleNode("score").InnerText));

        }
        oper.start = DateTime.Parse(node.SelectSingleNode("start").InnerText);
        oper.end = DateTime.Parse(node.SelectSingleNode("end").InnerText);
        oper.name = node.SelectSingleNode("name").InnerText;
        oper.desctiption = node.SelectSingleNode("desctiption").InnerText;
        oper.counterEvent = node.SelectSingleNode("counterEvent").InnerText;
        oper.myCounter = int.Parse(node.SelectSingleNode("myCounter").InnerText);
        oper.myPlace = int.Parse(node.SelectSingleNode("myPlace").InnerText);
    }
	
	public SocUser GetUser(string uid){
		if(!allUsers.ContainsKey(uid)){
            int i=0;
            if (int.TryParse (uid,out i))
            {
                uids.Add(uid);
            }
			allUsers[uid] = new SocUser(uid);
		}
		
		return allUsers[uid];
	
	}
	
	public IEnumerator SetSocInfo(string data){
        
		uids.Clear();
		string[] dataSplit =  data.Split(',');
		foreach(string dateEntry in dataSplit){
			string[] oneEntry =  dateEntry.Split(';');
			allUsers[oneEntry[0]].name = oneEntry[1];
			
			WWW www = new WWW(oneEntry[2]);

			
		   yield return www;
		   allUsers[oneEntry[0]].avatar = new Texture2D(100, 100);
		   www.LoadImageIntoTexture( allUsers[oneEntry[0]].avatar);
		
		}
        isLoaded = true;
	}



    public IEnumerator SendData()
    {
        if (currentOperation.toSendCounter > 0)
        {
            WWWForm form = new WWWForm();

            form.AddField("uid", GlobalPlayer.instance.UID);
            form.AddField("oid", currentOperation.id);
            form.AddField("points", currentOperation.toSendCounter);
            currentOperation.toSendCounter = 0;
            WWW w = StatisticHandler.GetMeRightWWW(form, StatisticHandler.SAVE_OPERATION);
            yield return w;
            Debug.Log(w.text);
        }
    }



	private static TournamentManager s_Instance = null;
	
	public static TournamentManager instance {
		get {
			if (s_Instance == null) {
				//Debug.Log ("FIND");
				// This is where the magic happens.
				//  FindObjectOfType(...) returns the first AManager object in the scene.
				s_Instance =  FindObjectOfType(typeof (TournamentManager)) as TournamentManager;
			}

		
			// If it is still null, create a new instance
			if (s_Instance == null) {
			//	Debug.Log ("CREATE");
				GameObject obj = new GameObject("TextGenerator");
				s_Instance = obj.AddComponent(typeof (TournamentManager)) as TournamentManager;
				
			}
			
			return s_Instance;
		}
	}

    public static string EVENT_TEAM_WIN = "team_win";

    public static string EVENT_KILL = "kill";

    public static string EVENT_RUN = "run";

    public static string EVENT_HEAD_SHOT = "headshot";

    public Player myPlayer;

    public void EventStart()
    {
        
    }

    public void EventTeamWin(int teamNumber)
    {
        if (currentOperation != null && currentOperation.counterEvent == EVENT_TEAM_WIN)
        {
            if (myPlayer.team == teamNumber)
            {
                currentOperation.Increment();
            }


        }
        StartCoroutine(SendData());
    }

    public void EventRestart()
    {
       
    }

    public void EventRoomFinished()
    {
    
    }

    public void EventAppear(Player target)
    {
        if (target.isMine)
        {
            myPlayer = target;


        }
    }

    public void EventPawnDeadByPlayer(Player target, KillInfo killinfo)
    {
        if (myPlayer == target)
        {
            StartCoroutine(SendData());
        }
    }

    public void EventPawnDeadByAI(Player target)
    {
        if (myPlayer == target)
        {
            StartCoroutine(SendData());
        }
    }

    public void EventPawnKillPlayer(Player target, KillInfo killinfo)
    {
        if (currentOperation != null)
        {

            
                if (myPlayer == target)
                {
                    if (currentOperation.counterEvent == EVENT_KILL)
                    {
                         currentOperation.Increment();
                    }
                    if (currentOperation.counterEvent == EVENT_HEAD_SHOT && killinfo.isHeadShoot)
                    {
                        currentOperation.Increment();
                    }
                }

            
        }
    }

    public void EventPawnKillAI(Player target, KillInfo killinfo)
    {
        if (currentOperation != null)
        {


            if (myPlayer == target)
            {
                if (currentOperation.counterEvent == EVENT_KILL)
                {
                    currentOperation.Increment();
                }
                if (currentOperation.counterEvent == EVENT_HEAD_SHOT&&killinfo.isHeadShoot)
                {
                    currentOperation.Increment();
                }
            }


        }
    }

    public void EventJuggerKill(Player target, KillInfo killinfo)
    {
       
    }

    public void EventJuggerTake(Player target)
    {
     
    }

    public void EventPawnGround(Player target)
    {
       
    }

    public void EventPawnDoubleJump(Player target)
    {
     
    }

    Vector3 sprintStart;
    public void EventStartSprintRun(Player target, Vector3 Position)
    {
        if (currentOperation != null && currentOperation.counterEvent == EVENT_RUN)
        {
            if (myPlayer == target)
            {
                sprintStart = Position;
            }

        }
    }

    public void EventEndSprintRun(Player target, Vector3 Position)
    {
        if (currentOperation != null && currentOperation.counterEvent == EVENT_RUN)
        {
            if (myPlayer == target)
            {
                currentOperation.Increment(Mathf.RoundToInt((sprintStart-Position).magnitude*10.0f));
            }

        }
    }

    public void EventStartWallRun(Player target, Vector3 Position)
    {
     
    }

    public void EventEndWallRun(Player target, Vector3 Position)
    {
      
    }

    public void EventPawnReload(Player target)
    {
      
    }

    public void EventKilledByFriend(Player target, Player friend)
    {
       
    }

    public void EventKilledAFriend(Player target, Player friend, KillInfo killinfo)
    {
      
    }
    public void EventPawnKillAssistPlayer(Player target)
    {
       
    }
    public void EventPawnKillAssistAI(Player target)
    {
        
    }
}