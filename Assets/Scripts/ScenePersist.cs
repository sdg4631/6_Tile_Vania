﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePersist : MonoBehaviour 
{
	int startingSceneIndex;

	void Awake()
	{
		int numScenePersists = FindObjectsOfType<ScenePersist>().Length;
		if (numScenePersists > 1 )
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
		startingSceneIndex = SceneManager.GetActiveScene().buildIndex;
	}

	void Update()
	{
		int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
		if (currentSceneIndex != startingSceneIndex)
		{
			Destroy(gameObject);
		}
	}
}
