using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHandler : MonoBehaviour
{
    public TextAsset WeaponData;

    [System.Serializable]
    public class DataToLoad
    {
        public string itemName;
        public int[] objPosition;
        public int damage;
        public int attackSpeed;
        public int attackNum;
    }

    [System.Serializable]
    public class WeaponDataList
    {
        public DataToLoad[] Weapons;
    }

    WeaponDataList weaponDataList = new WeaponDataList();

    void OnEnable()
    {   
        weaponDataList = JsonUtility.FromJson<WeaponDataList>(WeaponData.text);
    }

    public DataToLoad getWeaponData(int index)
    {
       return weaponDataList.Weapons[index];
    }
}
