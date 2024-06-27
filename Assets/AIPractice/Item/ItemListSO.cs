using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CollectItem.Item
{
    [CreateAssetMenu(fileName = "ItemListSO", menuName = "CollectItem/Item/ItemListSO")]
    public class ItemListSO : ScriptableObject
    {
        private List<Item> _itemList = new List<Item>();

        public List<Item> ItemList => _itemList;
        
        public void SetItem(Item item)
        {
            if (!_itemList.Contains(item))
            {
                _itemList.Add(item);
            }
        }
        
        public void RemoveItem(Item item)
        {
            if (_itemList.Contains(item))
            {
                _itemList.Remove(item);
            }
        }
    }
}
