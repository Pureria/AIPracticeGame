using System;
using System.Collections;
using System.Collections.Generic;
using Algorithm;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace CollectItem.Stage
{
    public class StageUpdater : MonoBehaviour
    {
        [SerializeField] private int _OffsetWidth, _OffsetHeight;
        [SerializeField] private StageUpdaterSO _stageUpdaterSO;
        [SerializeField, Range(0, 1)] private float _DefaultThreatLevelWeight = 0.5f;
        
        [Header("DebugMode")]
        [SerializeField] private StageDebuger _stageDebuger;

        private StageData _currentStageData;
        private int[,] _StageGrid;
        private Vector2Int _threatePosition;
        
        //脅威者を中心にした単純な脅威度マップ
        //1に近いほど脅威が高い
        private float[,] _ThreatLevelMap;
        
        //アイテムの位置を中心にしたマップ
        //0に近いほどアイテムが近い
        private float[,] _ItemLevelMap;
        
        //コレクターの位置を中心にしたマップ
        //0に近いほどコレクターが近い
        private float[,] _CollectorLevelMap;

        //複合マップ
        private float[,] _DefaultSynthesisMap;
        
        //Debug用
        [SerializeField]
        private Transform _dummyPlayer;

        private void OnEnable()
        {
            _stageUpdaterSO.GetThreatMap += GetThreatLevelMap;
            _stageUpdaterSO.GetItemMap += GetItemLevelMap;
            _stageUpdaterSO.GetCollectorMap += GetCollectorLevelMap;
            _stageUpdaterSO.GetDefaultSynthesisMap += GetSynthesisMap;
            _stageUpdaterSO.GetSynthesisMap += GetSynthesisMap;
            _stageUpdaterSO.GetMapOffset += GetMapOffset;
            _stageUpdaterSO.SetThreatPosition += SetThreatPosition;
        }

        private void OnDisable()
        {
            _stageUpdaterSO.GetThreatMap -= GetThreatLevelMap;
            _stageUpdaterSO.GetItemMap -= GetItemLevelMap;
            _stageUpdaterSO.GetCollectorMap -= GetCollectorLevelMap;
            _stageUpdaterSO.GetDefaultSynthesisMap -= GetSynthesisMap;
            _stageUpdaterSO.GetSynthesisMap -= GetSynthesisMap;
            _stageUpdaterSO.GetMapOffset -= GetMapOffset;
            _stageUpdaterSO.SetThreatPosition -= SetThreatPosition;
        }

        private void Awake()
        {
            _threatePosition = Vector2Int.zero;
            
            _stageUpdaterSO.ResetItemPos();
            _stageUpdaterSO.ResetCollectorPos();
        }
        
        private void Update()
        {
            if (_dummyPlayer != null)
            {
                float wo = _currentStageData.StageWidthOffset;
                float ho = _currentStageData.StageHeightOffset;
                _threatePosition = new Vector2Int((int)(_dummyPlayer.position.x), (int)(_dummyPlayer.position.z));
            }
        }

        private void FixedUpdate()
        {
            UpdateMap();
        }
        
        /// <summary>
        /// ステージデータをセット
        /// </summary>
        /// <param name="stageData">ステージデータ</param>
        /// <param name="stageGrid">ステージのグリッド</param>
        /// <param name="stageObjects">ステージ上のオブジェクト</param>

        public void SetStageData(StageData stageData, int[,] stageGrid, GameObject[,] stageObjects, Vector2Int threatePosition)
        {
            _currentStageData = stageData;
            _StageGrid = stageGrid;
            
            _ThreatLevelMap = new float[_currentStageData.StageHeight, _currentStageData.StageWidth];
            _ItemLevelMap = new float[_currentStageData.StageHeight, _currentStageData.StageWidth];
            _DefaultSynthesisMap = new float[_currentStageData.StageHeight, _currentStageData.StageWidth];

            if (_stageDebuger != null)
            {
                _stageDebuger.SetData(_currentStageData, _StageGrid, stageObjects);
            }
            
            _threatePosition = threatePosition;
            UpdateMap();
        }
        
        /// <summary>
        /// プレイヤーの位置をセット
        /// </summary>
        /// <param name="position">プレイヤーポジション</param>
        public void SetThreatPosition(Vector2Int position) => _threatePosition = position;
        
        public float[,] GetThreatLevelMap() { return _ThreatLevelMap; }
        public float[,] GetItemLevelMap() { return _ItemLevelMap; }
        public float[,] GetCollectorLevelMap() { return _CollectorLevelMap; }
        public float[,] GetSynthesisMap() { return _DefaultSynthesisMap; }
        
        public float[,] GetSynthesisMap(float threatLevelWeight)
        {
            float[,] ret = DijkstraAlgorithm.SynthesisDijkstraMap(_ThreatLevelMap, _ItemLevelMap, threatLevelWeight);
            return ret;
        }
        
        /// <summary>
        /// マップのオフセットを取得
        /// </summary>
        /// <returns>マップオフセット</returns>
        public Vector2 GetMapOffset() { return new Vector2(_currentStageData.StageWidthOffset, _currentStageData.StageHeightOffset); }

        private bool IsDebugMode()
        {
            if(_stageDebuger == null) return false;
            return _stageDebuger.IsDebugMode;
        }
        
        /// <summary>
        /// 合成マップの更新
        /// </summary>
        private void UpdateSynthesisMap()
        {
            if (_stageUpdaterSO.ItemPosList.Count == 0)
            {
                _DefaultSynthesisMap = _ThreatLevelMap;
                return;
            }
            
            UpdateItemLevelMap();
            
            //脅威度マップとアイテムマップを合成
            float threatLevelWeight = _DefaultThreatLevelWeight;
            if(IsDebugMode()) threatLevelWeight = _stageDebuger.ThreatLevelWeight;
            _DefaultSynthesisMap = DijkstraAlgorithm.SynthesisDijkstraMap(_ThreatLevelMap, _ItemLevelMap, threatLevelWeight);
        }
        
        /// <summary>
        /// アイテムマップの更新
        /// </summary>
        private void UpdateItemLevelMap()
        {
            //アイテムマップの更新
            List<Vector2Int> itemPosList = _stageUpdaterSO.ItemPosList;
            int[,] itemMap = DijkstraAlgorithm.GetDijkstraMap(_StageGrid, itemPosList[0], out int maxCount);
            int itemMaxCost = maxCount;
            //アイテムの位置を中心にしたマップ
            for (int i = 1; i < itemPosList.Count; i++)
            {
                int[,] itemMap2 = DijkstraAlgorithm.GetDijkstraMap(_StageGrid, itemPosList[i], out int maxCount2);
                itemMap = DijkstraAlgorithm.CompareMinDijkstraMap(itemMap, itemMap2, out int maxCost);
                
                if (itemMaxCost < maxCost) itemMaxCost = maxCost;
            }
            
            _ItemLevelMap = DijkstraAlgorithm.GetNormalizeDijkstraMap(itemMap, itemMaxCost);
        }

        private void UpdateCollecterMap()
        {
            List<Vector2Int> collecterPosList = _stageUpdaterSO.CollecterPosList;
            int[,] collecterMap = DijkstraAlgorithm.GetDijkstraMap(_StageGrid, collecterPosList[0], out int maxCount);
            int collectermaxcost = maxCount;
            //コレクターの位置を中心にしたマップ
            for (int i = 1; i < collecterPosList.Count; i++)
            {
                int[,] collecterMap2 = DijkstraAlgorithm.GetDijkstraMap(_StageGrid, collecterPosList[i], out int maxCount2);
                collecterMap = DijkstraAlgorithm.CompareMinDijkstraMap(collecterMap, collecterMap2, out int maxCost);
                
                if (collectermaxcost < maxCost) collectermaxcost = maxCost;
            }
            
            _CollectorLevelMap = DijkstraAlgorithm.GetNormalizeDijkstraMap(collecterMap, collectermaxcost);
        }

        private void UpdateMap()
        {
            //脅威度マップの更新
            _ThreatLevelMap = DijkstraAlgorithm.ReverseNormalizeDijkstraMap(_StageGrid, _threatePosition);
            int radius = 10;
            if(IsDebugMode()) radius = _stageDebuger.DebugRadius;
            int[,] visibilityMap = RadiationEvaluation.EvaluateAllDirections(_StageGrid, _threatePosition, radius);
            
            for(int y = 0; y < _currentStageData.StageHeight; y++)
            {
                for(int x = 0; x < _currentStageData.StageWidth; x++)
                {
                    if(_StageGrid[y, x] == 0) continue;
                    float mul = visibilityMap[y, x];
                    if (mul == 0) mul = 0.5f;
                    _ThreatLevelMap[y, x] = _ThreatLevelMap[y, x] * mul;
                }
            }

            UpdateSynthesisMap();
            
            if (_stageUpdaterSO.CollecterPosList.Count != 0)
            {
                UpdateCollecterMap();
            }
        }
    }
}
