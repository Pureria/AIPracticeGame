using System;
using System.Collections;
using System.Collections.Generic;
using CollectItem.Stage;
using UnityEngine;

namespace CollectItem.Collecter.NPC
{
    public class NPCCollecter : CollecterBase
    {
        private NPCStatesSO _npcStatesSO;
        private StageUpdaterSO _stageUpdaterSO;
        private bool _isInitialize = false;

        private Vector3 _nextPos;
        private Vector2Int _setPos;

        private float _lastMoveTime;
        
        public void SetData(NPCStatesSO npcStatesSo, StageUpdaterSO stageUpdaterSo, Vector3 spawnPos)
        {
            _npcStatesSO = npcStatesSo;
            _stageUpdaterSO = stageUpdaterSo;
            _isInitialize = true;
            _lastMoveTime = Time.time - _npcStatesSO.WaitTime;
            SetData(_npcStatesSO.DownTime, spawnPos);
        }

        private void OnEnable()
        {
            FinishThreatDownEvent += FinishThreatDown;
        }
        
        private void OnDisable()
        {
            FinishThreatDownEvent -= FinishThreatDown;
        }

        protected override void Update()
        {
            base.Update();
            
            if (!_isInitialize) return;
            if (!_canMove) return;
            
            if (Time.time - _lastMoveTime >= _npcStatesSO.WaitTime)
            {
                Move();
                _lastMoveTime = Time.time;
            }
            else
            {
                //現在の経過時間と移動時間の割合を求め、現在位置の補完
                float rate = (Time.time - _lastMoveTime) / _npcStatesSO.WaitTime;
                transform.position = Vector3.Lerp(transform.position, _nextPos, rate);
            }
        }
        
        private void FinishThreatDown()
        {
            _stageUpdaterSO.RemoveCollectorPos(_setPos);
            _setPos = new Vector2Int((int)_SpawnPos.x, (int)_SpawnPos.z);
            _stageUpdaterSO.SetCollectorPos(_setPos);
            transform.position = _SpawnPos;
        }
        
        private void Move()
        {
            _stageUpdaterSO.RemoveCollectorPos(_setPos);
            var pos = transform.position;
            var currentPos = new Vector2Int((int)pos.x, (int)pos.z);
            if(_stageUpdaterSO.GetSynthesisMap == null) return;
            float[,] dijkstraMap = _stageUpdaterSO.GetSynthesisMap(_npcStatesSO.ThreatWeight);
            _setPos = GetNextMoveNodes(dijkstraMap, currentPos);
            Vector2 nextPos = _setPos;
            if (_stageUpdaterSO.GetMapOffset != null)
            {
                nextPos += _stageUpdaterSO.GetMapOffset();
            }
            //transform.position = new Vector3(nextPos.x, pos.y, nextPos.y);
            _nextPos = new Vector3(nextPos.x, pos.y, nextPos.y);
            _stageUpdaterSO.SetCollectorPos(_setPos);
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
