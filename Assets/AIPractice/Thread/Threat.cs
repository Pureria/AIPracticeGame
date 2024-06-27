using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CollectItem.Collecter;
using CollectItem.Stage;
using UnityEngine;

namespace CollectItem.Thread
{
    public class Threat : MonoBehaviour
    {
        [SerializeField] private StageUpdaterSO _stageUpdaterSO;
        [SerializeField] private float _waitTime = 1.5f;

        private Vector2Int _setPos;
        private Vector3 _nextPos;
        private float _lastMoveTime;

        private void Start()
        {
            _lastMoveTime = Time.time - _waitTime;
            
        }

        private void Update()
        {
            if (Time.time - _lastMoveTime >= _waitTime)
            {
                Move();
                _lastMoveTime = Time.time;
            }
            else
            {
                //現在の経過時間と移動時間の割合を求め、現在位置の補完
                float rate = (Time.time - _lastMoveTime) / _waitTime;
                transform.position = Vector3.Lerp(transform.position, _nextPos, rate);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent<ICollecter>(out var collecter))
            {
                collecter.HitThreat();
            }
        }

        private void Move()
        {
            var pos = transform.position;
            var currentPos = new Vector2Int((int)pos.x, (int)pos.z);
            if(_stageUpdaterSO.GetCollectorMap == null) return;
            float[,] dijkstraMap = _stageUpdaterSO.GetCollectorMap();
            _setPos = GetNextMoveNodes(dijkstraMap, currentPos);
            Vector2 nextPos = _setPos;
            if (_stageUpdaterSO.GetMapOffset != null)
            {
                nextPos += _stageUpdaterSO.GetMapOffset();
            }
            //transform.position = new Vector3(nextPos.x, pos.y, nextPos.y);
            _nextPos = new Vector3(nextPos.x, pos.y, nextPos.y);
            _stageUpdaterSO?.SetThreatPosition(_setPos);
        }
        
        private Vector2Int GetNextMoveNodes(float[,] dijkstraMap, Vector2Int currentPos)
        {
            Vector2Int nextPos = Vector2Int.zero;
            float minCost = float.MaxValue;
            int[] dx = { 0, 1, 0, -1 };
            int[] dy = { 1, 0, -1, 0 };
            
            int height = dijkstraMap.GetLength(0);
            int width = dijkstraMap.GetLength(1);

            for (int i = 0; i < 4; i++)
            {
                int newX = currentPos.x + dx[i];
                int newY = currentPos.y + dy[i];

                if (newX >= 0 && newX < width && newY >= 0 && newY < height)
                {
                    if(dijkstraMap[newY,newX] < minCost)
                    {
                        minCost = dijkstraMap[newY, newX];
                        nextPos = new Vector2Int(newX, newY);
                    }
                }
            }

            return nextPos;
        }
    }
}
