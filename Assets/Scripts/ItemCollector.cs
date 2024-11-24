using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemCollector : MonoBehaviour
{
    private InventoryManager inventoryManager;
    private List<ItemData> nearbyItems = new List<ItemData>();
    private Carton currentCarton;
    private Shelf nearbyShelf;
    private float interactionRange = 3f;
    private List<GameObject> shelfItems = new List<GameObject>();
    private float placeCartonRange = 3f;

    private CrazyCart controls;

    public GameObject pickupParticlesPrefab;
    public AudioClip pickupSound;
    private AudioSource audioSource;

    void Awake()
    {
        controls = new CrazyCart();
        controls.Player.Interact.performed += ctx => OnInteract();
        controls.Player.Refill.performed += ctx => OnRefill();
        controls.Player.PlaceCarton.performed += ctx => OnPlaceCarton();
    }


    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    void Start()
    {
        inventoryManager = FindObjectOfType<InventoryManager>();
        audioSource = GetComponent<AudioSource>();
    }


    void Update()
    {
        FindNearbyShelf();
    }

    private void OnInteract()
    {
        if (currentCarton == null && nearbyItems.Count > 0)
        {
            PickupItem();
        }
        else if (currentCarton != null && nearbyShelf == null)
        {
            PickupCarton();
        }
    }

    private void OnRefill()
    {
        if (nearbyShelf != null)
        {
            RefillShelf(nearbyShelf);
        }
    }

    private void OnPlaceCarton()
    {
        PlaceCarton();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            ItemData itemData = other.GetComponent<ItemData>();
            if (itemData != null && !nearbyItems.Contains(itemData))
            {
                nearbyItems.Add(itemData);
            }
        }
        else if (other.CompareTag("Carton"))
        {
            Carton carton = other.GetComponent<Carton>();
            if (carton != null && currentCarton == null)
            {
                currentCarton = carton;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            ItemData itemData = other.GetComponent<ItemData>();
            if (itemData != null && nearbyItems.Contains(itemData))
            {
                nearbyItems.Remove(itemData);
            }
        }
        else if (other.CompareTag("Carton"))
        {
            Carton carton = other.GetComponent<Carton>();
            if (carton == currentCarton)
            {
                currentCarton = null;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Shelf"))
        {
            nearbyShelf = collision.collider.GetComponent<Shelf>();
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Shelf"))
        {
            nearbyShelf = null;
        }
    }

    private void PickupItem()
    {
        if (nearbyItems.Count > 0)
        {
            ItemData itemToPickup = nearbyItems[0];
            if (inventoryManager.IsInventoryFull())
            {
                inventoryManager.ShowInventoryFullMessage();
                return;
            }

            inventoryManager.AddItem(itemToPickup.itemSO);

            if (pickupSound != null && audioSource != null)
            {
                audioSource.volume = 1.5f; 
                audioSource.PlayOneShot(pickupSound); 
            }

            if (pickupParticlesPrefab != null)
            {
                Instantiate(pickupParticlesPrefab, itemToPickup.transform.position, Quaternion.identity);
            }

            itemToPickup.gameObject.SetActive(false);
            nearbyItems.RemoveAt(0);
        }
    }



    void PickupCarton()
    {
        if (currentCarton != null)
        {
            inventoryManager.PickupCarton(currentCarton);
            currentCarton.gameObject.SetActive(false);
        }
    }

    public void RefillShelf(Shelf shelf)
    {
        if (currentCarton == null || currentCarton.containedItem == null)
        {
            return;
        }

        if (!string.Equals(currentCarton.containedItem.itemSO.itemName, shelf.acceptedItemName, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (shelf.IsFull())
        {
            return;
        }

        shelf.RefillShelf(currentCarton.containedItem.itemSO, 1);
    }

    public void ActivateNextItem()
    {
        foreach (GameObject item in shelfItems)
        {
            if (!item.activeSelf)
            {
                item.SetActive(true);
                return;
            }
        }
    }

    void FindNearbyShelf()
    {
        Shelf[] allShelves = FindObjectsOfType<Shelf>();
        Shelf closestShelf = null;
        float closestDistance = interactionRange;

        foreach (Shelf shelf in allShelves)
        {
            float distance = Vector3.Distance(transform.position, shelf.transform.position);
            if (distance <= closestDistance)
            {
                closestShelf = shelf;
                closestDistance = distance;
            }
        }

        nearbyShelf = closestShelf;
    }

    void PlaceCarton()
    {
        if (currentCarton != null)
        {
            float distanceToOriginalPosition = Vector3.Distance(transform.position, currentCarton.OriginalPosition);
            if (distanceToOriginalPosition > placeCartonRange)
            {
                return;
            }

            currentCarton.ResetToOriginalPosition();

            if (inventoryManager != null)
            {
                inventoryManager.RemoveCarton(currentCarton);
            }

            currentCarton = null;
        }
    }
}
