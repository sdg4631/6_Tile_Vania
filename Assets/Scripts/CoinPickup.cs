using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour 
{
	[SerializeField] AudioClip coinPickupSFX;
	
	private void OnTriggerEnter2D(Collider2D collider)
	{
		AudioSource.PlayClipAtPoint(coinPickupSFX, Camera.main.transform.position);

		// add coin to player score in GameSession

		Destroy(gameObject);
	}

}
