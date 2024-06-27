using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CollectItem.Collecter
{
    public abstract class CollecterBase : MonoBehaviour , ICollecter
    {
        protected int _collectCount;
        protected bool _canMove;
        protected float _hitThreatTime;
        protected float _hitThreatWaitTime;
        protected Vector3 _SpawnPos;

        protected Action FinishThreatDownEvent;

        protected virtual void Awake()
        {
            _collectCount = 0;
            _canMove = true;
            _hitThreatTime = 0;
        }

        protected virtual void Update()
        {
            if (!_canMove && Time.time - _hitThreatTime >= _hitThreatWaitTime)
            {
                FinishThreatDownEvent?.Invoke();
                _canMove = true;
            }
        }

        public virtual void HitItem()
        {
            _collectCount++;
        }

        public virtual void HitThreat()
        {
            _canMove = false;
            _hitThreatTime = Time.time;
        }

        protected void SetData(float hitThreatWaitTime, Vector3 initialPos)
        {
            _hitThreatWaitTime = hitThreatWaitTime;
            _SpawnPos = initialPos;
        }
    }
}
