using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeath : MonoBehaviour 
{
	[SerializeField] int enemyValue = 500;
	[SerializeField] float destroyDelay = .1f;

	Player player;

	void Start()
	{		
		player = FindObjectOfType<Player>();
	}
	
	void OnCollisionEnter2D(Collision2D other)
	{
		if (player.isInvulnerable == false) 
		{
			if (other.gameObject.tag == "Player")		
			{
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
