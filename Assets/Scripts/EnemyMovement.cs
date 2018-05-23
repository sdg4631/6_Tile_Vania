using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour 
{
	[SerializeField] public float enemyMoveSpeed = 1f;
	
	Rigidbody2D myRigidBody;
	

	void Start()
	{
		myRigidBody = GetComponent<Rigidbody2D>();
	}
	
	
	void Update()
	{
		if (IsFacingRight())
		{
			myRigidBody.velocity = new Vector2(enemyMoveSpeed, 0);
		}
		else
		{
			myRigidBody.velocity = new Vector2(-enemyMoveSpeed, 0);			
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
}
