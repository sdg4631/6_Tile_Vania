using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameSession : MonoBehaviour 
{
	[SerializeField] int playerHearts = 3;
	[SerializeField] int score = 0;

	[SerializeField] TextMeshProUGUI scoreText;

	[SerializeField] GameObject heart1, heart2, heart3;



	private void Awake()
	{
		int numGameSessions = FindObjectsOfType<GameSession>().Length;
		if (numGameSessions > 1)
		{
			Destroy(gameObject);
		}
		else
		{
			DontDestroyOnLoad(gameObject);
		}
	}

	void Start()
	{
		scoreText.text = score.ToString();
	}

	void Update()
	{
		DisplayPlayerHearts();
		ProcessPlayerDeath();
	}

    private void DisplayPlayerHearts()
    {
		if (playerHearts > 3) { playerHearts = 3; }

        switch (playerHearts)
		{
			case 3:
				heart3.gameObject.SetActive(true);
				heart2.gameObject.SetActive(true);
				heart1.gameObject.SetActive(true);
				break;
			case 2:
				heart3.gameObject.SetActive(false);
				heart2.gameObject.SetActive(true);
				heart1.gameObject.SetActive(true);
				break;	
			case 1:
				heart3.gameObject.SetActive(false);
				heart2.gameObject.SetActive(false);
				heart1.gameObject.SetActive(true);
				break;
			case 0:
				heart3.gameObject.SetActive(false);
				heart2.gameObject.SetActive(false);
				heart1.gameObject.SetActive(false);
				break;
		}
    }

    public void AddToScore(int pointsToAdd)
	{
		score += pointsToAdd;
		scoreText.text = score.ToString();
	}

	public void ProcessPlayerDeath()
	{
		 if (playerHearts < 1)
		{
			Invoke("ResetGameSession", 4f);
		}
	}

    public void TakeHeart()
    {
        playerHearts--;		
    }

    private void ResetGameSession()
    {
        SceneManager.LoadScene(0);
		Destroy(gameObject);
    }
}
