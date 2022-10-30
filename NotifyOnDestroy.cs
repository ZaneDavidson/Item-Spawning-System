using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets;

public class NotifyOnDestroy : MonoBehaviour
{
    public event Action<AssetReference, NotifyOnDestroy> Destroyed;
    public AssetReference AssetReference {get; set; }

    public void OnDestroy()
    {
        Destroyed?.Invoke(AssetReference, this);
    }
}
