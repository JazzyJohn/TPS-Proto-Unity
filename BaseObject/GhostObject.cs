using UnityEngine;
using System.Collections;

public class GhostObject : MonoBehaviour {
	
	public Transfrom myTransform;
	
	public Color normalColor;
	 
	public Color badColor;
    void Awake()
    {
		myTransform = transform;
		
	}



}