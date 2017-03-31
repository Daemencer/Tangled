using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Juice
{
	private bool shaking = false;
	private bool slowMotion = false;

	#region API Methods
	public void ScreenShake(GameObject camera, float duration, float speed, float force)
	{
		if (!shaking)
		{
			shaking = true;
			Utils.BeginCoroutine(camera, ScreenShakeCoroutine(Camera.main, duration, speed, force));
		}
	}

	
	public void SlowMotion(GameObject gameobject, float duration, float modifier)
	{
		if (!slowMotion)
		{
			slowMotion = true;
			Utils.BeginCoroutine(gameobject, SlowMotionCoroutine(duration, modifier));
		}
	}
	#endregion

	#region Private Coroutines
	private IEnumerator ScreenShakeCoroutine(Camera cam, float duration, float speed, float force)
	{
		float elapsed = 0.0f;

		Vector3 originalCamPos = cam.transform.parent.parent.position;
		float randomStart = Random.Range(-1.0f, 1.0f);

		while (elapsed < duration)
		{
			elapsed += Time.fixedDeltaTime;

			float percentComplete = elapsed / duration;

			float damper = 1.0f - Mathf.Clamp(2.0f * percentComplete - 1.0f, 0.0f, 1.0f);

			float alpha = randomStart + speed * percentComplete;

			float x = Mathf.PerlinNoise(alpha, 0.0f) * 2.0f - 1.0f;
			float y = Mathf.PerlinNoise(0.0f, alpha) * 2.0f - 1.0f;
			float z = Mathf.PerlinNoise(alpha, alpha) * 2.0f - 1.0f;

			x *= force * damper;
			y *= force * damper;
			z *= force * damper;

			Camera.main.transform.parent.parent.position = new Vector3(originalCamPos.x + x, originalCamPos.y + y, originalCamPos.z + z);

			yield return null;
		}

		Camera.main.transform.parent.parent.position = originalCamPos;
		shaking = false;
	}


	private IEnumerator SlowMotionCoroutine(float duration, float modifier)
	{
		float elapsed = 0.0f;

		while (elapsed < duration)
		{
			elapsed += Time.deltaTime;

			Time.timeScale = 1.0f / modifier;

			yield return null;
		}

		Time.timeScale = 1.0f;
		slowMotion = false;
	}
	#endregion
}