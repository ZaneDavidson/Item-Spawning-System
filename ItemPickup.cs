using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] PlayerController pc;
    void OnTriggerEnter(Collider col)
    {
        if(pc.isHoldingWeapon == false)
        {
            ICollect item = col.GetComponent<ICollect>();
            if(item != null)
            {
                item.GetItem(col.transform.gameObject);
            }
        }
    }
}
