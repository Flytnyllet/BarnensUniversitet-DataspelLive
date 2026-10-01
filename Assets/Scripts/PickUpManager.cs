using UnityEngine;

public class PickUpManager : MonoBehaviour
{
	[SerializeField] private Transform playerHand;
	[SerializeField] private Transform shovelHand;

	private GameObject heldItem;

	private void OnTriggerEnter(Collider other)
	{
		if (heldItem) return;
		if (!other.CompareTag(tag: "PickUp")) return;

		PickUp(other.gameObject);
	}

	private void PickUp(GameObject item)
	{
		heldItem = item;

		Collider col = heldItem.GetComponent<Collider>();
		col.enabled = false;

		if (item.CompareTag("Shovel"))
			item.transform.SetParent(shovelHand);
		else
			item.transform.SetParent(playerHand);

		item.transform.localPosition = Vector3.zero;
		item.transform.localRotation = Quaternion.identity;
	}
}