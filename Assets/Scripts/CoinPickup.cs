using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour 
{
	[SerializeField] int coinValue = 100;
	[SerializeField] AudioClip coinPickupSFX;
	
	private void OnTriggerEnter2D(Collider2D collider)
	{
		AudioSource.PlayClipAtPoint(coinPickupSFX, Camera.main.transform.position);
		FindObjectOfType<GameSession>().AddToScore(coinValue);
		Destroy(gameObject);
	}

}
