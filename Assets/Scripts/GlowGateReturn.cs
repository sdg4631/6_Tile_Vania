using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlowGateReturn : MonoBehaviour 
{

	[SerializeField] float levelLoadDelay = 1f;

	void OnTriggerEnter2D(Collider2D collider)
	{
		StartCoroutine(LoadPreviousLevel());
	}

	IEnumerator LoadPreviousLevel()
	{
		yield return new WaitForSecondsRealtime(levelLoadDelay);

		var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
		SceneManager.LoadScene(currentSceneIndex - 1);
	}
}
