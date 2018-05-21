﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour 
{
	[SerializeField] float moveSpeed = 1f;
	
	Rigidbody2D myRigidBody;
	

	void Start()
	{
		myRigidBody = GetComponent<Rigidbody2D>();
	}
	
	
	void Update()
	{
		if (IsFacingRight())
		{
			myRigidBody.velocity = new Vector2(moveSpeed, 0);
		}
		else
		{
			myRigidBody.velocity = new Vector2(-moveSpeed, 0);			
		}
	}

	bool IsFacingRight()
	{
		return transform.localScale.x > 0;
	}

	// Flips sprite when it reaches a wall or ledge
	void OnTriggerExit2D(Collider2D collision)
	{
		transform.localScale = new Vector2(-(Mathf.Sign(myRigidBody.velocity.x)), 1f);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Player")
		{
			var player = other.gameObject.GetComponent<Player>();
			player.knockbackCount = player.knockbackLength;

			if (other.transform.position.x < transform.position.x)
			{
				player.knockFromRight = true;
			}
			else
			{
				player.knockFromRight = false;
			}
		}
	}
}
