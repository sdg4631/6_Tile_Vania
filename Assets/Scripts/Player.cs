using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour 
{
	// Config
	[SerializeField] float runSpeed = 7f;
	[SerializeField] float sprintSpeed = 14f;
	[SerializeField] float jumpSpeed = 17f;
	[SerializeField] float bounceSpeed = 15f;
	[SerializeField] float climbSpeed = 7f;
	[SerializeField] int playerHitPoints = 3;
	[SerializeField] float invulnerabilityDuration = 2f;

	public bool knockFromRight;
	public float knockbackCount;
	public float knockbackLength;
	public float knockbackX;
	public float knockbackY;

	public float spriteBlinkingTimer = 0.0f;
	public float spriteBlinkingMiniDuration = 0.1f;
	public float spriteBlinkingTotalTimer = 0.0f;
	public bool startBlinking = false;

	public float bounceTimer = 1f;
	public float bounceTimeoutDuration = 0.5f;



	// State
	bool isAlive = true;
	bool isInvulnerable = false;
	
	// Cached Component References
	SpriteRenderer mySpriteRenderer;
	Rigidbody2D myRigidBody;
	Animator myAnimator;
	PolygonCollider2D myBodyCollider;
	BoxCollider2D myFeetCollider;
	float gravityScaleAtStart;
	Vector2 velocityAtStart;

	// Messages then methods
	void Start() 
	{
		mySpriteRenderer = GetComponent<SpriteRenderer>();
		myRigidBody = GetComponent<Rigidbody2D>();
		myAnimator = GetComponent<Animator>();
		myBodyCollider = GetComponent<PolygonCollider2D>();	
		myFeetCollider = GetComponent<BoxCollider2D>();
		gravityScaleAtStart = myRigidBody.gravityScale;
		velocityAtStart = myRigidBody.velocity;

		// makes sure enemy collision is enabled at start
		RestoreEnemyCollision();
	}
	
	void Update() 
	{
		if (!isAlive) { return; }

		Run();
		Jump();
		FlipSprite();
		ClimbLadder();
		BounceOffEnemy();
		TakeDamage();
		Knockback();
		Die();

		if (startBlinking == true)
		{
			StartBlinkingEffect();
		}
	}

    private void Run()
	{
		if (knockbackCount <= 0)
		{
			float horizontalThrow = CrossPlatformInputManager.GetAxis("Horizontal"); // value is between -1 and +1
			bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;

			if (CrossPlatformInputManager.GetButton("Sprint") && playerHasHorizontalSpeed)
			{
				Vector2 playerVelocity = new Vector2(horizontalThrow * sprintSpeed, myRigidBody.velocity.y);	
				myRigidBody.velocity = playerVelocity;	

				myAnimator.SetBool("Sprinting", true);
			}
			else 
			{
				Vector2 playerVelocity = new Vector2(horizontalThrow * runSpeed, myRigidBody.velocity.y);	
				myRigidBody.velocity = playerVelocity;	

				myAnimator.SetBool("Running", playerHasHorizontalSpeed);
				myAnimator.SetBool("Sprinting", false);
			}		
		}					
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
		if (knockbackCount <= 0)
		{
			bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
			if (playerHasHorizontalSpeed)
			{
				transform.localScale = new Vector2(Mathf.Sign(myRigidBody.velocity.x), 1f);
			}
		}
	}

	private void ClimbLadder()
	{
		if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ladders"))) 
		{ 
			myAnimator.SetBool("Climbing", false);
			myAnimator.SetBool("ClimbingIdle", false);
			myRigidBody.gravityScale = gravityScaleAtStart;
			return; 
		}

		float controlThrow = CrossPlatformInputManager.GetAxis("Vertical");
		Vector2 climbVelocity = new Vector2(myRigidBody.velocity.x, controlThrow * climbSpeed);
		myRigidBody.velocity = climbVelocity;
		myRigidBody.gravityScale = 0f;
		
		
		bool playerHasClimbingSpeed = Mathf.Abs(myRigidBody.velocity.y) > Mathf.Epsilon;
		myAnimator.SetBool("Climbing", playerHasClimbingSpeed);	

		if (!playerHasClimbingSpeed)
		{
			myAnimator.SetBool("ClimbingIdle", true);
		}
		else
		{
			myAnimator.SetBool("ClimbingIdle", false);
		}
	}

    private void BounceOffEnemy()
    {
		// Prevents bounce to be called multiple times on collision
		bounceTimer += Time.deltaTime;
		if (bounceTimer >= bounceTimeoutDuration)
		{
			if (myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Enemy")) && !myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
			{
				bounceTimer = 0.0f;

				Vector2 bounceVelocityToAdd = new Vector2(0f, bounceSpeed);
				myRigidBody.velocity += bounceVelocityToAdd;
			}
		}
    }

	private void TakeDamage()
	{
		if (!isInvulnerable)
		{
			if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemy", "Obstacles")))
            {
                StartInvulnerability();
				startBlinking = true;
                playerHitPoints--;
                FindObjectOfType<GameSession>().TakeHeart();
            }
        }
	}

    private void StartInvulnerability()
    {
        // temporary invulnerabilty after taking damage 
        if (playerHitPoints > 1)
        {
            isInvulnerable = true;
			IgnoreEnemyCollision();
            myAnimator.SetTrigger("Knockback");
            Invoke("StopInvulnerability", invulnerabilityDuration);
			Invoke("RestoreEnemyCollision", invulnerabilityDuration);
        }
    }

	private void StopInvulnerability()
	{
		isInvulnerable = false;	
	}

    private void StartBlinkingEffect()
    {
        spriteBlinkingTotalTimer += Time.deltaTime;
		if (spriteBlinkingTotalTimer >= invulnerabilityDuration)
		{
			startBlinking = false;
			spriteBlinkingTotalTimer = 0.0f;
			mySpriteRenderer.enabled = true;
			return;
		}

		spriteBlinkingTimer += Time.deltaTime;
		if (spriteBlinkingTimer >= spriteBlinkingMiniDuration)
		{
			spriteBlinkingTimer = 0.0f;
			if (mySpriteRenderer.enabled == true)
			{
				mySpriteRenderer.enabled = false;
			}
			else
			{
				mySpriteRenderer.enabled = true;
			}
		}
    }

    void Knockback()
	{
		if (knockbackCount > 0 && !myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Enemy")))
		{
			if (knockFromRight)
			{
				myRigidBody.velocity = new Vector2(-knockbackX, knockbackY);
			}
			if (!knockFromRight)
			{
				myRigidBody.velocity = new Vector2(knockbackX, knockbackY);
			}
			knockbackCount -= Time.deltaTime;	
		}			
	}

	public void Die()
	{
		if (playerHitPoints <= 0)
		{
			IgnoreEnemyCollision();		
			myAnimator.SetTrigger("Dying");
			myRigidBody.velocity = velocityAtStart;	
			isAlive = false;			
		}		
	}

	private void RestoreEnemyCollision()
	{
		Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Player"), false);
	}

	private void IgnoreEnemyCollision()
	{
		Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Player"), true);
	}
}
