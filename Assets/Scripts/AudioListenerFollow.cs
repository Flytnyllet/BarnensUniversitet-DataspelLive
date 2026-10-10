using UnityEngine;
public class AudioListenerFollow : MonoBehaviour
{
	[SerializeField] private Transform player;
	[SerializeField] private Transform cameraTransform;

	private void LateUpdate()
	{
		transform.SetPositionAndRotation(player.position, cameraTransform.rotation);
	}
}