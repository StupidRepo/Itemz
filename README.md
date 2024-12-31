# Itemz - Custom Item API (with Photon support ✨)
Itemz is a custom item API that allows you to register custom items within Content Warning.
It also provides you with a easy to use AssetBundleHandler to load your custom items from AssetBundles.

# API
## `Itemz.AssetBundleHandler`
The AssetBundleHandler is a class that allows you to load custom items from AssetBundles.
Example:
```csharp
using Itemz;

// load your bundle and create a new AssetBundleHandler instance with it
// in your plugin's load method.
// we will use Content Warning workshop mod specification
// but for BepInEx you can do Awake (or the MelonLoader equivalent)

[ContentWarningPlugin("...", "0.1", false)] // make sure you specify your mod as non-vanilla compatible
public class MyPlugin
{
	public static AssetBundleHandler LoadedBundle;

	static MyPlugin() {
		var bundle = AssetBundle.LoadFrom...; // load your asset bundle via AssetBundle.LoadFromFile, AssetBundle.LoadFromStream (for embedded asset bundles), etc.
		LoadedBundle = new AssetBundleHandler(bundle);
	}
}
```

Loading assets from the AssetBundleHandler is as simple as calling `LoadedBundle.GetAssetByName<T>(string name)`.

> [!NOTE]
> The `name` is the **path** of the asset in the AssetBundle, without the path or file extension.
> e.g.: If, in the Unity project you made your bundle from, the asset was in `Assets/MyPrefabs/MyCustomItem.prefab` then the name would be `MyCustomItem`.
> Another example: If the asset was in `Assets/MyPrefabs/MyFolder/AnotherFolder/Lol/HelloMoto.prefab` then the name would be `HelloMoto`.

Example:
```csharp
...
static MyPlugin() {
   ...
   LoadedBundle.GetAssetByName<Item>("MyCustomItem"); // Item is a ScriptableObject from the game code itself.
   LoadedBundle.GetAssetByName<AudioClip>("MyCustomAudioClip");
   LoadedBundle.GetAssetByName<GameObject>("MyCustomPrefab");
}
```

## `Itemz.Itemz`
The Itemz class is a static class for registering items with Photon and adding to game's Item DB.
Example:
```csharp
static MyPlugin() {
   ...
   // RegisterItem(Item item, string name, bool addDB)
   // item: the item to register
   // name: the name of the item's prefab for Photon, to allow you to PhotonNetwork.Instantiate the item's prefab
   // addDB: whether to add the item to the game's item database
	 // if you set this to true, you can allow the item to be purchased and seen in the shop
   	 // or make it spawnable in the game if set in the Item ScriptableObject itself by you
   Itemz.RegisterItem(LoadedBundle.GetAssetByName<Item>("MyCustomItem"), "PhotonNetworkName", false);
   
   // if you want to add a component to the item's GameObject at runtime, specify the component in the type parameter:
   // KEEP IN MIND THAT THIS WILL ADD THE COMPONENT TO THE ITEM'S GAMEOBJECT **REFERENCE**. That means if you reference the same GameObject in multiple items, the component will be added to all of them.
   // you should never use this unless Unity's editor is giving you trouble adding your component to the GameObject in the editor.
   //
   // if you *are* having trouble, compile your mod and add it to the Assets/Plugins/ folder in your Unity project directory.
   // make sure you have game DLLs in the Assets/Plugins/ folder too so Unity can recognize game references.
   Itemz.RegisterItem<CustomComponent>(LoadedBundle.GetAssetByName<Item>("MyCustomItem"), "PhotonNetworkName", false);
}
```

## Spawning in items across network
To spawn in items that **HAVE BEEN ADDED TO THE GAME'S ITEM DB**, use `PickupHandler.CreatePickup`.
```csharp
PickupHandler.CreatePickup(byte itemID, ItemInstanceData data, Vector3 position, Quaternion rotation)
```

To spawn in items that have **NOT** been added to the game's item DB, use `PhotonNetwork.Instantiate`.
```csharp
PhotonNetwork.Instantiate("PhotonNetworkName", position, rotation)
```