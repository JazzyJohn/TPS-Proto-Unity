using UnityEngine;
using System.Collections;

public class GhostObject : MonoBehaviour {
	
	public Transform myTransform;

    public LayerMask blockLayer;
	
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
			lRendered.material.SetColor("_MainColor",	badColor);
		}

	}
	public void MakeNormal(){
		foreach (Renderer lRendered in myRenderers) {
            lRendered.material.SetColor("_MainColor", normalColor);
		}
		
	}

}