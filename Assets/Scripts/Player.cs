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
	[SerializeField] float climbSpeed = 7f;
	[SerializeField] int playerHitPoints = 3;

	public bool knockFromRight;
	public float knockbackCount;
	public float knockbackLength;
	public float knockbackX;
	public float knockbackY;



	// State
	bool isAlive = true;
	bool invulnerable = false;
	
	// Cached Component References
	Rigidbody2D myRigidBody;
	Animator myAnimator;
	CapsuleCollider2D myBodyCollider;
	BoxCollider2D myFeetCollider;
	float gravityScaleAtStart;
	Vector2 velocityAtStart;

	// Messages then methods
	void Start() 
	{
		myRigidBody = GetComponent<Rigidbody2D>();
		myAnimator = GetComponent<Animator>();
		myBodyCollider = GetComponent<CapsuleCollider2D>();	
		myFeetCollider = GetComponent<BoxCollider2D>();
		gravityScaleAtStart = myRigidBody.gravityScale;
		velocityAtStart = myRigidBody.velocity;

		// makes sure enemy collision is enabled at start
		Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Player"), false);
	}
	
	void Update() 
	{
		if (!isAlive) { return; }

		Run();
		Jump();
		FlipSprite();
		ClimbLadder();
		TakeDamage();
		Knockback();
		Die();
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

	private void TakeDamage()
	{
		if (!invulnerable)
		{
			if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemy", "Obstacles")))
            {
                StartInvulnerability();
				
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
            invulnerable = true;
            myAnimator.SetTrigger("Knockback");
            float invulnerabilityDuration = 1f;
            Invoke("StopInvulnerability", invulnerabilityDuration);
        }
    }

    void Knockback()
	{
		if (knockbackCount > 0)
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

	private void StopInvulnerability()
	{
		invulnerable = false;	
	}

	public void Die()
	{
		if (playerHitPoints <= 0)
		{
			Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Player"), true);			
			myAnimator.SetTrigger("Dying");
			myRigidBody.velocity = velocityAtStart;	
			isAlive = false;			
		}		
	}
}
