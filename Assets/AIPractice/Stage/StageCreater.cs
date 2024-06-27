using System;
using System.Collections;
using System.Collections.Generic;
using CollectItem.Data;
using UnityEngine;
using UnityEngine.Serialization;


namespace CollectItem.Stage
{
    public class StageCreater : MonoBehaviour
    {
        [FormerlySerializedAs("_stage")]
        [Tooltip("Stage Info")] 
        [SerializeField] private StageUpdater stageUpdater;
        [SerializeField] private Transform _stageParent;
        
        [SerializeField] private GameObject _FlorePrefab;
        [SerializeField] private GameObject _WallPrefab;
        [SerializeField] private GameObject _ItemSpawnBlockPrefab;
        [SerializeField] private GameObject _ThreatSpawnBlockPrefab;
        [SerializeField] private GameObject _NpcSpawnBlockPrefab;
        [SerializeField] private GameObject _PlayerSpawnBlockPrefab;
        [SerializeField] private StageDataSO _debugStageDataSO;

        private void Awake()
        {
            StageData stageData = SaveData.LoadData<StageData>(_debugStageDataSO.stageDataJson);
            CreateStage(stageData, out int[,] stageGrid, out GameObject[,] stageObjects, out Vector2Int threatPosition);
            stageUpdater.SetStageData(stageData, stageGrid, stageObjects, threatPosition);
        }

        
        private void CreateStage(StageData data, out int[,] stageGrid, out GameObject[,] stageObjects, out Vector2Int threatPosition)
        {
            stageGrid = new int[data.StageHeight, data.StageWidth];
            stageObjects = new GameObject[data.StageHeight, data.StageWidth];
            threatPosition = new Vector2Int();
            for(int y = 0; y < data.StageHeight; y++)
            {
                for(int x = 0; x < data.StageWidth; x++)
                {
                    //ステージオブジェクト生成
                    switch (data.StageDataArray[y, x])
                    {
                        case (int)StageObjectType.Wall:
                            stageObjects[y, x] = Instantiate(_FlorePrefab, new Vector3(x + data.StageWidthOffset, 1, y + data.StageHeightOffset), Quaternion.identity);
                            stageGrid[y, x] = (int)StageObjectType.Wall;
                            break;
                        
                        case (int)StageObjectType.Floor:
                            stageObjects[y, x] = Instantiate(_FlorePrefab, new Vector3(x + data.StageWidthOffset, 0, y + data.StageHeightOffset), Quaternion.identity);
                            stageGrid[y, x] = (int)StageObjectType.Floor;
                            break;
                        
                        case (int)StageObjectType.ItemSpawnBlock:
                            stageObjects[y, x] = Instantiate(_ItemSpawnBlockPrefab, new Vector3(x + data.StageWidthOffset, 0, y + data.StageHeightOffset), Quaternion.identity);
                            stageGrid[y, x] = (int)StageObjectType.ItemSpawnBlock;
                            break;
                        
                        case (int)StageObjectType.ThreatSpawnBlock:
                            stageObjects[y, x] = Instantiate(_ThreatSpawnBlockPrefab, new Vector3(x + data.StageWidthOffset, 0, y + data.StageHeightOffset), Quaternion.identity);
                            stageGrid[y, x] = (int)StageObjectType.ThreatSpawnBlock;
                            threatPosition = new Vector2Int(x, y);
                            break;
                        
                        case (int)StageObjectType.NpcSpawnBlock:
                            stageObjects[y, x] = Instantiate(_NpcSpawnBlockPrefab, new Vector3(x + data.StageWidthOffset, 0, y + data.StageHeightOffset), Quaternion.identity);
                            stageGrid[y, x] = (int)StageObjectType.NpcSpawnBlock;
                            break;
                        
                        case (int)StageObjectType.PlayerSpawnBlock:
                            stageObjects[y, x] = Instantiate(_PlayerSpawnBlockPrefab, new Vector3(x + data.StageWidthOffset, 0, y + data.StageHeightOffset), Quaternion.identity);
                            stageGrid[y, x] = (int)StageObjectType.PlayerSpawnBlock;
                            break;
                            
                        default:
                            break;
                    }
                    
                    if(stageObjects[y,x] != null)
                        stageObjects[y,x].transform.parent = _stageParent;
                }
            }
        }
    }

    [System.Serializable]
    public class StageData
    {
        public int StageID;
        public int StageWidth;
        public int StageHeight;
        public float StageWidthOffset;
        public float StageHeightOffset;
        public int[,] StageDataArray;
    }
    
    /// <summary>
    /// ステージデータ用のEnum
    /// </summary>
    public enum StageObjectType
    {
        Wall,
        Floor,
        ItemSpawnBlock,
        ThreatSpawnBlock,
        NpcSpawnBlock,
        PlayerSpawnBlock,
    }
}
