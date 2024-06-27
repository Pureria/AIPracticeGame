using System;
using System.Collections;
using System.Collections.Generic;
using CollectItem.Stage;
using UnityEngine;

namespace CollectItem.Item
{
    public class DummyItem : MonoBehaviour
    {
        [SerializeField] private StageUpdaterSO _stageUpdaterSO;
        [SerializeField] private float _SpawnTime = 3.0f;
        private Vector2Int _currentPos;
        private bool _isHide;
        private float _hideTime;

        private void Start()
        {
            var pos = transform.position;
            _currentPos = new Vector2Int((int)pos.x, (int)pos.z);
            _isHide = false;
            _stageUpdaterSO.SetItemPos(_currentPos);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Collecter"))
            {
                _isHide = true;
                _hideTime = Time.time;
                _stageUpdaterSO.RemoveItemPos(_currentPos);
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
                    _stageUpdaterSO.SetItemPos(_currentPos);
                    gameObject.SetActive(true);
                }
            }
        }
    }
}
