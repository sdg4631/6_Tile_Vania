﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour 
{
	[SerializeField] float runSpeed = 5f;
	Rigidbody2D myRigidBody;

	void Start() 
	{
		myRigidBody = GetComponent<Rigidbody2D>();
	}
	
	void Update() 
	{
		Run();
	}

	void Run()
	{
		float controlThrow = CrossPlatformInputManager.GetAxis("Horizontal"); // value is between -1 and +1
		Vector2 playerVelocity = new Vector2(controlThrow * runSpeed, myRigidBody.velocity.y);	
		myRigidBody.velocity = playerVelocity;	
	}
}
