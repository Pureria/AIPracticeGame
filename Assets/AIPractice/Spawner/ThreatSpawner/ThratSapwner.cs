using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CollectItem.Spawner
{
    public class ThratSapwner : MonoBehaviour
    {
        [SerializeField] private GameObject _threatPrefab;
        [SerializeField] private float _spawnYOffset = 1.5f;

        private void Start()
        {
            var pos = transform.position;
            pos.y += _spawnYOffset;
            var threat = Instantiate(_threatPrefab, pos, Quaternion.identity);
        }
    }
}
