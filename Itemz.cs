using Photon.Pun;
using Steamworks;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.CLI;

namespace Itemz;

[ContentWarningPlugin("stupidrepo.Itemz", "0.1", false)]
public static class Itemz
{
	public static Dictionary<string, Item> RegisteredItems = new();

	static Itemz()
	{
		Callback<LobbyEnter_t>.Create(OnLobbyEnter);
	}
	
	private static void OnLobbyEnter(LobbyEnter_t callback)
	{
		Debug.Log("Itemz is registering Photon items!");
		PUNPoolAddOurRegisteredItems();
	}

	public static void RegisterItem(Item? item, string photonName, bool addToDB = true)
	{
		if (item == null)
		{
			Debug.LogError("Item is null");
			return;
		}
		
		Debug.LogWarning("Adding item: " + item.displayName); 
		
		item.price = item.price > 0 ? item.price : 5;
		
		if(addToDB)
			SingletonAsset<ItemDatabase>.Instance.AddRuntimeEntry(item);
		
		RegisteredItems.Add(photonName, item);
	}

	public static void RegisterItem<T>(Item? item, string photonName, bool addToDB = false) where T : Component
	{
		if (item == null)
		{
			Debug.LogError("Item is null");
			return;
		}

		Debug.LogWarning("Adding item: " + item.displayName);

		item.itemObject.AddComponent<T>();
		item.price = item.price > 0 ? item.price : 5;

		if (addToDB)
		{
			AddToItemsDB(item);
		}

		AddToRegItems(item, photonName);
	}
	
	private static void AddToItemsDB(Item item)
	{
		if (SingletonAsset<ItemDatabase>.Instance.Objects.Contains(item))
		{
			Debug.LogWarning($"Item ${item.displayName} already exists in database, overwriting");
			SingletonAsset<ItemDatabase>.Instance.Objects.Remove(item);
		} else if(SingletonAsset<ItemDatabase>.Instance.Objects.Count > byte.MaxValue)
		{
			Debug.LogError("Item database is full, cannot add more items (max: " + byte.MaxValue + ")");
			return;
		}
		SingletonAsset<ItemDatabase>.Instance.AddRuntimeEntry(item);
	}
	
	private static void AddToRegItems(Item item, string photonName)
	{
		if (RegisteredItems.ContainsKey(photonName))
		{
			Debug.LogWarning($"Item with name {photonName} already exists in registered items, overwriting");
			RegisteredItems.Remove(photonName);
		}
		RegisteredItems.Add(photonName, item);
	}
	
	private static void PUNPoolAddOurRegisteredItems()
	{
		foreach (var item in RegisteredItems)
		{
			var defaultPool = ((DefaultPool)PhotonNetwork.prefabPool);

			Debug.Log("Registering item with PUN pool: " + item.Value.displayName);
			
			if(defaultPool.ResourceCache.ContainsKey(item.Key))
            {
                Debug.Log("Item already exists in PUN pool, skipping");
                continue;
            }
			
			defaultPool.ResourceCache.Add(item.Key, item.Value.itemObject);
			Debug.Log("Added!");
		}
	}
}

