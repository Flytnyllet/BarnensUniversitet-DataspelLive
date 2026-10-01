using System;
using UnityEngine;

public class PickUpManager : MonoBehaviour
{
	[SerializeField] private Transform playerHand;
	[SerializeField] private Transform shovelHand;

	private GameObject heldItem;
	private bool insideDropZone;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("DropZone"))
		{
			insideDropZone = true;
			Drop();
			return;
		}

		if (insideDropZone) return;
		if (heldItem) return;
		if (other.CompareTag(tag: "PickUp") || other.CompareTag(tag: "Shovel"))
			PickUp(other.gameObject);
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("DropZone"))
			insideDropZone = false;
	}

	private void PickUp(GameObject item)
	{
		heldItem = item;

		if (item.TryGetComponent(out Rigidbody rb))
			rb.isKinematic = true;

		Collider col = heldItem.GetComponent<Collider>();
		col.enabled = false;

		if (item.CompareTag("Shovel"))
			item.transform.SetParent(shovelHand);
		else
			item.transform.SetParent(playerHand);

		item.transform.localPosition = Vector3.zero;
		item.transform.localRotation = Quaternion.identity;
	}

	public void Drop()
	{
		if (!heldItem) return;

		heldItem.transform.SetParent(null);

		Collider col = heldItem.GetComponent<Collider>();
		col.enabled = true;
		col.isTrigger = false;

		if (heldItem.TryGetComponent(out Rigidbody rb))
			rb.isKinematic = false;

		heldItem = null;
	}
}