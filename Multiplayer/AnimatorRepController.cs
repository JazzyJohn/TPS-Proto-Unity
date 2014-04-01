using UnityEngine;
using System.Collections;

public class AnimatorRepController : MonoBehaviour {

	public Animator animator;

	private CharacterState _characterState;

	private Pawn owner;

	public PhotonView photonView;

	// Use this for initialization
	void Start () {
		owner = GetComponent<Pawn> ();
		photonView = GetComponent<PhotonView>();
	}
	
	// Update is called once per frame
	void Update () {
		if(photonView.isMine){
		if (animator != null) {
			if(owner.onMove) {
				animator.SetBool("Run",true);	
			
			}
			else 
			{
				animator.SetBool("Run",false);
					
			}
		}

	}

}
