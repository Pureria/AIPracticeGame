using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Algorithm;

namespace CollectItem.Stage
{
    public class StageDebuger : MonoBehaviour
    {
        private enum DebugMode
        {
            MapLevel,
            ThreatLevel,
            ItemLevel,
            CollecterLevel,
        }
        
        [SerializeField] private DebugMode _debugMode;
        [SerializeField] private int _debugRadius = 10;
        [SerializeField] private StageUpdaterSO _stageUpdaterSO;
        [SerializeField, Range(0, 1)] private float _ThreatLevelWeight = 0.5f;
        private bool _isDebugMode;
        private int[,] _StageGrid;
        private Material[,] _DebugMaterials;
        private Material[,] _DefaultMaterials;
        private StageData _currentStageData;
        private GameObject[,] _StageObjects;
        
        public bool IsDebugMode => _isDebugMode;
        public int DebugRadius => _debugRadius;
        public float ThreatLevelWeight => _ThreatLevelWeight;

        private void Awake()
        {
            _isDebugMode = false;
        }

        private void Update()
        {
            if (_isDebugMode)
            {
                UpdateDebugMaterial();
            }
        }

        /// <summary>
        /// デバッグモード有効化
        /// </summary>
        public void EnableDebugMode()
        {
            if(_isDebugMode) return;
            int[,] dijkstraMap = DijkstraAlgorithm.GetDijkstraMap(_StageGrid, new Vector2Int(9, 0), out int maxCount);
            float[,] normalizeDijkstraMap = DijkstraAlgorithm.GetNormalizeDijkstraMap(dijkstraMap, maxCount);
            _DebugMaterials = new Material[_currentStageData.StageHeight, _currentStageData.StageWidth];
            _DefaultMaterials = new Material[_currentStageData.StageHeight, _currentStageData.StageWidth];
            
            //マップの色を変更
            for (int y = 0; y < _currentStageData.StageHeight; y++)
            {
                for (int x = 0; x < _currentStageData.StageWidth; x++)
                {
                    if(_StageGrid[y, x] == (int)StageObjectType.Wall) continue;
                    _DebugMaterials[y,x] = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                    _DefaultMaterials[y, x] = _StageObjects[y, x].GetComponent<Renderer>().material;
                    _StageObjects[y, x].GetComponent<Renderer>().material = _DebugMaterials[y, x];
                }
            }
            
            _isDebugMode = true;
        }
        
        /// <summary>
        /// デバッグモード無効化
        /// </summary>
        public void DisableDebugMode()
        {
            if(!_isDebugMode) return;
            _isDebugMode = false;
            for (int y = 0; y < _currentStageData.StageHeight; y++)
            {
                for (int x = 0; x < _currentStageData.StageWidth; x++)
                {
                    if(_StageGrid[y, x] == (int)StageObjectType.Wall) continue;
                    _StageObjects[y, x].GetComponent<Renderer>().material = _DefaultMaterials[y, x];
                    _DefaultMaterials[y, x] = null;
                    _DebugMaterials[y, x] = null;
                }
            }
        }
        
        /// <summary>
        /// デバッグ用マテリアルの更新
        /// </summary>
        private void UpdateDebugMaterial()
        {
            float[,] map = GetDebugNormalizeDijkstraMap();
            //マップの色を変更
            for (int y = 0; y < _currentStageData.StageHeight; y++)
            {
                for (int x = 0; x < _currentStageData.StageWidth; x++)
                {
                    if (_StageGrid[y, x] == (int)StageObjectType.Wall) continue;
                    float value = map[y, x];
                    float red = value;
                    float green = 1 - value;
                    float blue = 0;

                    if (value == 0 || value == 1) blue = 0;
                    else blue = 1 - Mathf.Abs(value - 0.5f) * 2;
                    
                    _DebugMaterials[y,x].color = new Color(red, green, blue, 1);
                    //遠い場所ほど緑に、近い場所ほど赤にする
                }
            }
        }

        public void SetData(StageData stageData, int[,] stageGrid, GameObject[,] stageObjects)
        {
            _currentStageData = stageData;
            _StageGrid = stageGrid;
            _StageObjects = stageObjects;
        }
        
        private float[,] GetDebugNormalizeDijkstraMap()
        {
            switch (_debugMode)
            {
                case DebugMode.MapLevel:
                    return _stageUpdaterSO.GetSynthesisMap(_ThreatLevelWeight);
                case DebugMode.ThreatLevel:
                    return _stageUpdaterSO.GetThreatMap();
                case DebugMode.ItemLevel:
                    return _stageUpdaterSO.GetItemMap();
                case DebugMode.CollecterLevel:
                    return _stageUpdaterSO.GetCollectorMap();
                default:
                    return _stageUpdaterSO.GetSynthesisMap(_ThreatLevelWeight);
            }
        }
    }
}
