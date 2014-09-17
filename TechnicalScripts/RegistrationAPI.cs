using UnityEngine;
using System.Collections;
using System.Xml;

public class RegistrationAPI : MonoBehaviour{
	private static RegistrationAPI s_Instance = null;
	
	public static RegistrationAPI instance {
		get {
			if (s_Instance == null) {
				//Debug.Log ("FIND");
				// This is where the magic happens.
				//  FindObjectOfType(...) returns the first AManager object in the scene.
				s_Instance =  FindObjectOfType(typeof (RegistrationAPI)) as RegistrationAPI;
			}

		
			// If it is still null, create a new instance
			if (s_Instance == null) {
			//	Debug.Log ("CREATE");
				GameObject obj = new GameObject("RegistrationAPI");
				s_Instance = obj.AddComponent(typeof (RegistrationAPI)) as RegistrationAPI;
				
			}
			
			return s_Instance;
		}
	}
	
	
	public void Registration(string email,string password,string nick){
			WWWForm form = new WWWForm ();
	
			form.AddField ("email", email);
			form.AddField ("password", password);
			form.AddField ("nick", nick);
			StartCoroutine(SendRegForm( form));

	}
	private IEnumerator SendRegForm(WWWForm form){
			WWW www = StatisticHandler.GetMeRightWWW(form,StatisticHandler.REGISTRATION);
			
			yield return www;
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(www.text);
			if(xmlDoc.SelectSingleNode("registration/status").InnerText=="true"){
                PlayerPrefs.SetString("login", xmlDoc.SelectSingleNode("registration/login").InnerText);
				GlobalPlayer.instance.FinishInnerLogin(xmlDoc.SelectSingleNode("registration/uid").InnerText,xmlDoc.SelectSingleNode("registration/nick").InnerText);
				MainMenuGUI menu = FindObjectOfType<MainMenuGUI>();
				if (menu != null)
				{
					menu.FinishLogin();
				}	
			}else{
				MainMenuGUI menu = FindObjectOfType<MainMenuGUI>();
				if (menu != null)
				{
					menu.SetRegistarationError(xmlDoc.SelectSingleNode("registration/error").InnerText);
				}	
			
		
			}
	}
	public void Login(string email,string password){
			WWWForm form = new WWWForm ();
	
			form.AddField ("email", email);
			form.AddField ("password", password);
			StartCoroutine(SendLoginForm( form));

	}
	private IEnumerator SendLoginForm(WWWForm form){
			WWW www = StatisticHandler.GetMeRightWWW(form,StatisticHandler.LOGIN);
			
			yield return www;
            Debug.Log(www.text);
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(www.text);
           
			if(xmlDoc.SelectSingleNode("login/status").InnerText=="true"){
                PlayerPrefs.SetString("login", xmlDoc.SelectSingleNode("login/login").InnerText);
				GlobalPlayer.instance.FinishInnerLogin(xmlDoc.SelectSingleNode("login/uid").InnerText,xmlDoc.SelectSingleNode("login/nick").InnerText);
				MainMenuGUI menu = FindObjectOfType<MainMenuGUI>();
				if (menu != null)
				{
					menu.FinishLogin();
				}	
			}else{
				MainMenuGUI menu = FindObjectOfType<MainMenuGUI>();
				if (menu != null)
				{
                    menu.SetLoginError(xmlDoc.SelectSingleNode("login/error").InnerText);
				}	
			
		
			}
	}
}