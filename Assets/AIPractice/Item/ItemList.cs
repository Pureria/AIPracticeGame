using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CollectItem.Item
{
    public class ItemList : MonoBehaviour
    {
        [SerializeField] ItemListSO _itemListSO;

        private void Awake()
        {
            _itemListSO.ItemList.Clear();
        }

        private void FixedUpdate()
        {
            foreach (var item in _itemListSO.ItemList)
            {
                item.CheckSpawnItem();
            }
        }
    }
}
