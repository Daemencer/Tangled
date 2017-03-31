using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
	public static Coroutine BeginCoroutine(GameObject gameobject, IEnumerator routine)
	{
		var mono = gameobject.GetComponent<MonoBehaviour>();

		if (mono != null)
			return mono.StartCoroutine(routine);
		else
		{
			Debug.LogWarning("Trying to start a coroutine on a GameObject with no script attached.");
			return null;
		}
	}
}