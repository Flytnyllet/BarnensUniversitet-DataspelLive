using System;
using System.Collections.Generic;
using UnityEngine;

public class PickUpManager : MonoBehaviour
{
	[SerializeField] UIManager uiManager;
	PlayerTopDownController playerTopDownController;
	
	[SerializeField] private Transform playerHand;
	[SerializeField] private Transform shovelHand;

	[SerializeField] private float pickUpLockAfterSteal = 1.5f;
	[SerializeField] private List<GameObject> items;
	
	private float pickUpLockTimer;
	private GameObject heldItem;
	private bool insideDropZone;

	private readonly Dictionary<GameObject, (Vector3 position, Quaternion rotation)> itemOrigin = new();
	
	public int itemsCollected;
	public bool HasItem => heldItem != null;
	
	public event Action OnItemDelivered;
	
	private void Awake()
	{
		playerTopDownController = GetComponent<PlayerTopDownController>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("DropZone"))
		{
			insideDropZone = true;
			Drop();
			return;
		}

		if (insideDropZone) return;
		if (Time.time < pickUpLockAfterSteal) return;
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

		if(!itemOrigin.ContainsKey(item))
			itemOrigin[item] = (item.transform.position, item.transform.rotation);
		
		if (item.TryGetComponent(out Rigidbody rb))
			rb.isKinematic = true;

		Collider col = heldItem.GetComponent<Collider>();
		col.enabled = false;

		if (item.CompareTag("Shovel"))
		{
			item.transform.SetParent(shovelHand);
			playerTopDownController.holdingShovel = true;
			heldItem = null;
		}
		else if (item.CompareTag("PickUp"))
		{
			item.transform.SetParent(playerHand);
		}

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
		itemsCollected += 1;
		OnItemDelivered?.Invoke();
		if(itemsCollected == items.Count)
			uiManager.isGameWon = true;
	}

	public void StealItem()
	{
		if (!heldItem) return;

		GameObject item = heldItem;
		heldItem = null;
		pickUpLockTimer = Time.time + pickUpLockAfterSteal;
		
		item.transform.SetParent(null);
		
		if(itemOrigin.TryGetValue(item, out var origin))
			item.transform.SetPositionAndRotation(origin.position, origin.rotation);
		
		Collider col = item.GetComponent<Collider>();
		col.enabled = true;
	}
}