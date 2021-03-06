using Unity.Netcode;

using UnityEngine;

public class PrefabAssets : NetworkBehaviour{
    public static PrefabAssets Singleton { get; private set; }

    public GameObject prefabUI, world, playerNetPrefab, loadingScreenPrefab, consoleText, craftingUIListView;
    public GameObject mobEntity;


    public void Awake() => Singleton = this;

    public void Start() => gameObject.SetActive(false);
}
