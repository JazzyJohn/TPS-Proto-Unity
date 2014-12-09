using UnityEngine;
using System;

using System.Collections;
using System.Collections.Generic;


public class ScreenShootManager : MonoBehaviour{

	
	public byte[]  send;
    public void TakeScreenshotToWall(string message, bool anonce = false)
    {
        this.message = message;
        StartCoroutine(_TakeScreenshotToWall(anonce));
	
	}

    public string message;

    public IEnumerator _TakeScreenshotToWall(bool anonce)
	{
			yield return new WaitForEndOfFrame();
			Texture2D photo = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
			photo.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
			photo.Apply();
            send = photo.EncodeToPNG();
            Application.ExternalCall("VKGiveWallServer", message);
            yield return new WaitForEndOfFrame();
            if (anonce)
            {

                GUIHelper.SendMessage(TextGenerator.instance.GetSimpleText("ScreenShoot"));
            }
	
	
	}
	public void UploadURLToWall(string url){
		WWWForm form = new WWWForm();
		form.AddBinaryData("photo",send);
        StartCoroutine(_UploadScreenShootToWall(url, form));
	}
	
	public IEnumerator _UploadScreenShootToWall(string url,WWWForm form){
		WWW w = new WWW(url, form);
		yield return w;
		Application.ExternalCall ("VKWallPhotoPost",w.text);		
	}

    public void TakeScreenshot(bool anonce = false)
    {
        StartCoroutine(_TakeScreenshot(anonce));
	
	}

    public IEnumerator _TakeScreenshot(bool anonce )
	{
			yield return new WaitForEndOfFrame();
			Texture2D photo = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
			photo.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
			photo.Apply();
            send = photo.EncodeToPNG();
			Application.ExternalCall ("CreateAlbum");
            yield return new WaitForEndOfFrame();
            if (anonce)
            {
              
                GUIHelper.SendMessage(TextGenerator.instance.GetSimpleText("ScreenShoot"));
            }
	
	}
	public void UploadURL(string url){
		WWWForm form = new WWWForm();
		form.AddBinaryData("photo",send);
        StartCoroutine(_UploadScreenShoot(url, form));
	}
	
	public IEnumerator _UploadScreenShoot(string url,WWWForm form){
        Debug.Log("sending Scrennshot to" + url);
        WWW w = new  WWW(url, form);
		yield return w;
        Application.ExternalCall("VKSaveUpload", w.text);		
	}
	
	
	public void UploadComplite(){
	
		
		 
		  
	
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