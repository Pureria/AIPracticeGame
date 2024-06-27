using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CollectItem.Stage
{
    [CreateAssetMenu(fileName = "StageUpdater", menuName = "CollectItem/StageUpdaterSO")]
    public class StageUpdaterSO : ScriptableObject
    {
        //public Func<Vector2Int, int[,]> GetDijkstraMap;
        public Func<float[,]> GetThreatMap;
        public Func<float[,]> GetItemMap;
        public Func<float[,]> GetCollectorMap;
        public Func<float[,]> GetDefaultSynthesisMap;
        public Func<float, float[,]> GetSynthesisMap;
        public Func<Vector2> GetMapOffset;
        public Action<Vector2Int> SetThreatPosition;

        public List<Vector2Int> ItemPosList { get; private set; } = new List<Vector2Int>();
        public List<Vector2Int> CollecterPosList { get; private set; } = new List<Vector2Int>();

        public void SetItemPos(Vector2Int pos)
        {
            if (!ItemPosList.Contains(pos))
            {
                ItemPosList.Add(pos);
            }
        }
        
        public void RemoveItemPos(Vector2Int pos)
        {
            if (ItemPosList.Contains(pos))
            {
                ItemPosList.Remove(pos);
            }
        }
        
        public void SetCollectorPos(Vector2Int pos)
        {
            if (!CollecterPosList.Contains(pos))
            {
                CollecterPosList.Add(pos);
            }
        }
        
        public void RemoveCollectorPos(Vector2Int pos)
        {
            if (CollecterPosList.Contains(pos))
            {
                CollecterPosList.Remove(pos);
            }
        }
        
        public void ResetItemPos() => ItemPosList.Clear();
        public void ResetCollectorPos() => CollecterPosList.Clear();
    }
}