using System;
using System.Collections;
using System.Collections.Generic;
using CollectItem.Stage;
using UnityEngine;

namespace CollectItem.Item
{
    public class DummyItemList : MonoBehaviour
    {
        [SerializeField] private StageUpdaterSO _stageUpdaterSO;
        [SerializeField] private List<DummyItem> _dummyItemList = new List<DummyItem>();

        private void FixedUpdate()
        {
            foreach (var dummyItem in _dummyItemList)
            {
                dummyItem.CheckSpawnItem();
            }
        }
    }
}