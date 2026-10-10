using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomSound : MonoBehaviour
{
	[SerializeField] private AudioClip[] clips;
	[SerializeField] private float minDelay = 5f;
	[SerializeField] private float maxDelay = 15f;

	private AudioSource audioSource;

	private IEnumerator Start()
	{
		audioSource = GetComponent<AudioSource>();
		if (clips.Length == 0) yield break;

		while (true)
		{
			yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
			audioSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
		}
	}
}