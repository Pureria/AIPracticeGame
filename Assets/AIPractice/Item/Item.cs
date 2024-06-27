using System;
using System.Collections;
using System.Collections.Generic;
using CollectItem.Collecter;
using CollectItem.Stage;
using UnityEngine;

namespace CollectItem.Item
{
    public class Item : MonoBehaviour
    {
        [SerializeField] private StageUpdaterSO _stageUpdaterSo;
        [SerializeField] private ItemListSO _itemListSo;
        [SerializeField] private float _SpawnTime = 3.0f;
        
        private Vector2Int _currentPos;
        private bool _isHide;
        private float _hideTime;

        private void OnEnable()
        {
            _itemListSo.SetItem(this);
        }

        private void OnDestroy()
        {
            _itemListSo.RemoveItem(this);
        }

        private void Start()
        {
            var pos = transform.position;
            _currentPos = new Vector2Int((int)pos.x, (int)pos.z);
            _isHide = false;
            _stageUpdaterSo.SetItemPos(_currentPos);
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent<ICollecter>(out var collecter))
            {
                _isHide = true;
                _hideTime = Time.time;
                _stageUpdaterSo.RemoveItemPos(_currentPos);
                collecter.HitItem();
                gameObject.SetActive(false);
            }
        }

        public void CheckSpawnItem()
        {
            if (_isHide)
            {
                if (Time.time - _hideTime > _SpawnTime)
                {
                    _isHide = false;
                    _stageUpdaterSo.SetItemPos(_currentPos);
                    gameObject.SetActive(true);
                }
            }
        }
    }
}
