using UnityEngine;
using System;

using System.Collections;
using System.Collections.Generic;


public class ScreenShootManager : MonoBehaviour{

	
	public byte[]  send;
	public void TakeScreenshotToWall(){
		StartCoroutine(_TakeScreenshotToWall());
	
	}
	
	public  IEnumerator _TakeScreenshotToWall()
	{
			yield return new WaitForEndOfFrame();
			Texture2D photo = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
			photo.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
			photo.Apply();
			send = tex.EncodeToPNG();
			Application.ExternalCall ("VKGiveWallServer");		
	
	
	}
	public void UploadURLToWall(string url){
		WWWForm form = new WWWForm();
		form.AddBinaryData("photo",send);
		StartCourutine(_UploadScreenShootToWall(url,form));
	}
	
	public IEnumerator _UploadScreenShootToWall(string url,WWWForm form){
		WWW w = WWW(url, form);
		yield return w;
		Application.ExternalCall ("VKWallPhotoPost",w.text);		
	}
	
	public void TakeScreenshot(){
		StartCoroutine(TakeScreenshot());
	
	}
	
	public  IEnumeratorTakeScreenshot()
	{
			yield return new WaitForEndOfFrame();
			Texture2D photo = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
			photo.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
			photo.Apply();
			send = tex.EncodeToPNG();
			Application.ExternalCall ("CreateAlbum");		
	
	
	}
	public void UploadURLToWall(string url){
		WWWForm form = new WWWForm();
		form.AddBinaryData("photo",send);
		StartCourutine(_UploadScreenShootToWall(url,form));
	}
	
	public IEnumerator _UploadScreenShootToWall(string url,WWWForm form){
		WWW w = WWW(url, form);
		yield return w;
		Application.ExternalCall ("VKWallPhotoPost",w.text);		
	}
	
	
	public void UploadComplite(){
	
		if(!PlayerPrefs.HasKey("UploadComplite"){
			PlayerPrefs.SetInt("UploadComplite", 1);	
			GUIHelper.SendMessage(TextGenerator.instance.GetSimpleText("ScreenShoot"));
		}
		
		 
		  
	
	}
	private static ScreenShootManager s_Instance = null;
	
	public static ScreenShootManager instance {
		get {
			if (s_Instance == null) {
				//Debug.Log ("FIND");
				// This is where the magic happens.
				//  FindObjectOfType(...) returns the first AManager object in the scene.
				s_Instance =  FindObjectOfType(typeof (ScreenShootManager)) as ScreenShootManager;
			}

		
			// If it is still null, create a new instance
			if (s_Instance == null) {
			//	Debug.Log ("CREATE");
				GameObject obj = new GameObject("ScreenShootManager");
				s_Instance = obj.AddComponent(typeof (ScreenShootManager)) as ScreenShootManager;
				
			}
			
			return s_Instance;
		}
	}
}