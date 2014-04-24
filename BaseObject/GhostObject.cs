using UnityEngine;
using System.Collections;

public class GhostObject : MonoBehaviour {
	
	public Transform myTransform;
	
	public Color normalColor;
	 
	public Color badColor;

	public Renderer[] myRenderers;

	public float size;
    void Awake()
    {
		myTransform = transform;
		
	}
	public void MakeBad(){
		foreach (Renderer lRendered in myRenderers) {
			lRendered.material.SetColor("_TintColor",	badColor);
		}

	}
	public void MakeNormal(){
		foreach (Renderer lRendered in myRenderers) {
			lRendered.material.SetColor("_TintColor",	normalColor);
		}
		
	}

}