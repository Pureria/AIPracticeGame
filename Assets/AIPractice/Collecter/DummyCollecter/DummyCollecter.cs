using System.Collections;
using System.Collections.Generic;
using CollectItem.Stage;
using UnityEngine;

namespace CollectItem.Collecter.NPC
{
    public class DummyCollecter : MonoBehaviour, ICollecter
    {
        [SerializeField] private StageUpdaterSO _stageUpdaterSO;
        [SerializeField] private float _waitTime;
        [SerializeField, Range(0,1)] private float _threatWeight = 0.5f;
        private Vector3 _nextPos;
        private Vector2Int _setPos;

        private float _lastTime;
        private void Start()
        {
            var pos = transform.position;
            _lastTime = Time.time - _waitTime;
        }

        private void FixedUpdate()
        {
            // 一定時間ごとに移動
            if (Time.time - _lastTime >= _waitTime)
            {
                Move();
                _lastTime = Time.time;
            }
            else
            {
                //現在の経過時間と移動時間の割合を求め、現在位置の補完
                float rate = (Time.time - _lastTime) / _waitTime;
                transform.position = Vector3.Lerp(transform.position, _nextPos, rate);
            }
        }

        private void Move()
        {
            _stageUpdaterSO.RemoveCollectorPos(_setPos);
            var pos = transform.position;
            var currentPos = new Vector2Int((int)pos.x, (int)pos.z);
            if(_stageUpdaterSO.GetSynthesisMap == null) return;
            float[,] dijkstraMap = _stageUpdaterSO.GetSynthesisMap(_threatWeight);
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

        public void HitItem()
        {
        }

        public void HitThreat()
        {
        }
    }
}
