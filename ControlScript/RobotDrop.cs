using UnityEngine;
using System.Collections;

public class RobotDrop : MonoBehaviour {
	public float gravity = 20.0F;
	private CharacterController controller;
	private Vector3 moveDirection = Vector3.zero;
	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController>();
		moveDirection = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {

		moveDirection.y -= gravity * Time.deltaTime;
		controller.Move(moveDirection * Time.deltaTime);
	}
}
