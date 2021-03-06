﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKnockback : MonoBehaviour 
{

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
