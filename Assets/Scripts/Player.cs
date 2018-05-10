using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour 
{
	// Config
	[SerializeField] float runSpeed = 7f;
	[SerializeField] float jumpSpeed = 17f;
	[SerializeField] float climbSpeed = 7f;


	// State
	bool isAlive = true;
	
	// Cached Component References
	Rigidbody2D myRigidBody;
	Animator myAnimator;
	CapsuleCollider2D myBodyCollider;
	BoxCollider2D myFeetCollider;
	float gravityScaleAtStart;


	// Messages then methods
	void Start() 
	{
		myRigidBody = GetComponent<Rigidbody2D>();
		myAnimator = GetComponent<Animator>();
		myBodyCollider = GetComponent<CapsuleCollider2D>();	
		myFeetCollider = GetComponent<BoxCollider2D>();
		gravityScaleAtStart = myRigidBody.gravityScale;
	}
	
	void Update() 
	{
		if (!isAlive) { return; }

		Run();
		Jump();
		FlipSprite();
		ClimbLadder();
		Die();
	}

	private void Run()
	{
		float horizontalThrow = CrossPlatformInputManager.GetAxis("Horizontal"); // value is between -1 and +1
		Vector2 playerVelocity = new Vector2(horizontalThrow * runSpeed, myRigidBody.velocity.y);	
		myRigidBody.velocity = playerVelocity;	

		bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
		myAnimator.SetBool("Running", playerHasHorizontalSpeed);	
	}

	private void Jump()
	{	
		if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) { return; }

		if (CrossPlatformInputManager.GetButtonDown("Jump"))
		{
			Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
			myRigidBody.velocity += jumpVelocityToAdd;
		}		
	}

	private void FlipSprite()
	{
		bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
		if (playerHasHorizontalSpeed)
		{
			transform.localScale = new Vector2(Mathf.Sign(myRigidBody.velocity.x), 1f);
		}
	}

	private void ClimbLadder()
	{
		if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ladders"))) 
		{ 
			myAnimator.SetBool("Climbing", false);
			myRigidBody.gravityScale = gravityScaleAtStart;
			return; 
		}

		float controlThrow = CrossPlatformInputManager.GetAxis("Vertical");
		Vector2 climbVelocity = new Vector2(myRigidBody.velocity.x, controlThrow * climbSpeed);
		myRigidBody.velocity = climbVelocity;
		myRigidBody.gravityScale = 0f;

		bool playerHasClimbingSpeed = Mathf.Abs(myRigidBody.velocity.y) > Mathf.Epsilon;
		myAnimator.SetBool("Climbing", playerHasClimbingSpeed);	
	}

	private void Die()
	{
		if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemy")) || myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Obstacles")) )
		{
			isAlive = false;
			myAnimator.SetTrigger("Dying");
		}
	}
}
