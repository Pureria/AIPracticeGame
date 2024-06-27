using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CollectItem.Item
{
    public class ItemSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _itemPrefab;
        
        private void Start()
        {
            var pos = transform.position;
            pos.y += 1;
            var item = Instantiate(_itemPrefab, pos, Quaternion.identity);
        }
    }
}
