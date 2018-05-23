using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeath : MonoBehaviour 
{
	[SerializeField] int enemyValue = 500;
	[SerializeField] float destroyDelay = 1f;
	[SerializeField] GameObject deathFX;

	Rigidbody2D myRigidBody;			
	Animator myAnimator;
	Player player;
	

	void Start()
	{		
		myRigidBody = GetComponent<Rigidbody2D>();
		myAnimator = GetComponent<Animator>();
		player = FindObjectOfType<Player>();
		
	}
	
	void OnCollisionEnter2D(Collision2D other)
	{
		if (player.isInvulnerable == false) 
		{
			if (other.gameObject.tag == "Player")		
			{
				gameObject.GetComponent<EnemyMovement>().enemyMoveSpeed = 0f;
				myRigidBody.velocity = new Vector2( 0f, -5f);
				myAnimator.SetTrigger("EnemyDeath");
				var enemyDeathFX = Instantiate(deathFX, transform.position, Quaternion.identity);
				enemyDeathFX.SetActive(true);
				Invoke("DestroyEnemy", destroyDelay);
			}
		}
	}

	public void DestroyEnemy()
	{
		FindObjectOfType<GameSession>().AddToScore(enemyValue);
		Destroy(gameObject);
	}



}
