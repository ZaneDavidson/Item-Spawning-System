using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class WeaponSpawn : MonoBehaviour
{
    public GameObject pointList;
    public Transform[] spawnPoints;
    public GameObject weapon;
    [SerializeField] private List<AssetReference> spawnReferences;
    AssetReference assetReference;
    int num;
    int rand;
    public int SpawnTimer = 1;
    public bool ToSpawn = true;

    private readonly Dictionary<AssetReference, AsyncOperationHandle<GameObject>> operationHandles = new Dictionary<AssetReference, AsyncOperationHandle<GameObject>>();
    private readonly Dictionary<AssetReference, List<GameObject>> loadedReferences = new Dictionary<AssetReference, List<GameObject>>();

    void Start()
    {
        Transform parent = this.transform;
        num = parent.childCount;
        spawnPoints = new Transform[num];
        StartCoroutine(SpawnOverTime(parent));    
    }

    void SpawnHandler(int index)
    {
        assetReference = spawnReferences[index];
        if(assetReference.RuntimeKeyIsValid() == false)
        {
            return;
        }

        if(operationHandles.ContainsKey(assetReference))
        {
            if(operationHandles[assetReference].IsDone)
            {
                SpawnFromLoadedReference(assetReference);
            }
            return;
        }
        LoadSpawn(assetReference);

    }

    void LoadSpawn(AssetReference assetReference)
    {
        var toLoad = Addressables.LoadAssetAsync<GameObject>(assetReference);
        operationHandles[assetReference] = toLoad;
        toLoad.Completed += (operation) =>
        {
            SpawnFromLoadedReference(assetReference);
        };
    }

    void SpawnFromLoadedReference(AssetReference assetReference)
    {
        assetReference.InstantiateAsync(spawnPoints[Random.Range(0, num)].position, Quaternion.identity).Completed += (asyncOperationHandle) =>
        {
            if(loadedReferences.ContainsKey(assetReference) == false)
            {
                loadedReferences[assetReference] = new List<GameObject>();
            }

            loadedReferences[assetReference].Add(asyncOperationHandle.Result);
            var notify = asyncOperationHandle.Result.AddComponent<NotifyOnDestroy>();
            notify.Destroyed += Remove;
            notify.AssetReference = assetReference;
        };
    }

    void Remove(AssetReference assetReference, NotifyOnDestroy notify)
    {
        Addressables.ReleaseInstance(notify.gameObject);
        loadedReferences[assetReference].Remove(notify.gameObject);
        if(loadedReferences[assetReference].Count == 0)
        {
            if(operationHandles[assetReference].IsValid())
            {
                Addressables.Release(operationHandles[assetReference]);
            }
            operationHandles.Remove(assetReference);
        }
    }

    IEnumerator SpawnOverTime(Transform parent)
    {
        while(ToSpawn)
        {
            yield return new WaitForSeconds(SpawnTimer);
            Transform current;
            for(int i = 0; i < num; i++) 
            {
                current = parent.GetChild(i);
                spawnPoints[i] = current.transform;
            }
            SpawnHandler(Rand());
        }
    }

    int Rand()
    {
        //reset rand
        rand = Random.Range(0, spawnReferences.Count);
        return rand;
    }   
}
