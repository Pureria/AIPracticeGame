using System;
using System.Collections;
using System.Collections.Generic;
using CollectItem.Data;
using CollectItem.Stage;
using UnityEditor;
using UnityEngine;

namespace CollectItem.Editor
{
    public sealed class LoadAndGenerateEditor : EditorWindow
    {
        private static Transform _parent;
        private static Action<TextAsset, StageData> _successCallback;
        
        private TextAsset _stageDataJson;
        private StageDataSO _stageDataSO;
        private EditorDataSO _editorDataSo;
        
        public static void Window(Transform parent, Action<TextAsset, StageData> successCallback)
        {
            _parent = parent;
            _successCallback += successCallback;
            var window = CreateInstance<LoadAndGenerateEditor>();
            window.ShowModal();
        }

        private void OnGUI()
        {
            if (_stageDataJson == null)
                _stageDataSO = (StageDataSO)EditorGUILayout.ObjectField("Stage Data SO", _stageDataSO, typeof(StageDataSO), false);
            
            if(_stageDataSO == null)
                _stageDataJson = (TextAsset)EditorGUILayout.ObjectField("Stage Data Json", _stageDataJson, typeof(TextAsset), false);
            
            _editorDataSo = (EditorDataSO)EditorGUILayout.ObjectField("Floor Prefab", _editorDataSo, typeof(EditorDataSO), false);
            
            GUILayout.Space(100.0f);

            if ((_stageDataJson != null || _stageDataSO != null) && _editorDataSo != null)
            {
                if (GUILayout.Button("Generate"))
                {
                    if (GenerateStage(out TextAsset loadFile, out StageData loadStageData))
                    {
                        _successCallback?.Invoke(loadFile, loadStageData);
                    }
                    Close();
                    GUIUtility.ExitGUI();
                }
            }
            else
            {
                GUILayout.Label("Please select the Stage Data Json and Floor Prefab.");
            }
            
            if (GUILayout.Button("Cancel"))
            {
                Close();
                _successCallback = null;
                GUIUtility.ExitGUI();
            }
        }

        private bool GenerateStage(out TextAsset loadFile, out StageData loadStageData)
        {
            loadFile = null;
            loadStageData = null;
            if (_parent == null || _editorDataSo == null)
            {
                Debug.LogError("Failed Generate Stage.");
                return false;
            }

            TextAsset jsonFile = null;
            if (_stageDataSO != null) jsonFile = _stageDataSO.stageDataJson;
            else jsonFile = _stageDataJson;
            loadFile = jsonFile;
            
            StageData stageData = SaveData.LoadData<StageData>(jsonFile);
            if (stageData == null)
            {
                Debug.LogError("Failed Load Stage Data.");
                return false;
            }
            
            loadStageData = stageData;
            int childCount = _parent.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                DestroyImmediate(_parent.transform.GetChild(0).gameObject);
            }

            Undo.ClearAll();

            _parent.transform.position = new Vector3(stageData.StageWidthOffset, 0, stageData.StageHeightOffset);
            for (int y = 0; y < stageData.StageHeight; y++)
            {
                for (int x = 0; x < stageData.StageWidth; x++)
                {
                    Vector3 finalPosition = new Vector3(x, 0, y);
                    
                    /*
                    if (stageData.StageDataArray[y, x] != 1) continue;
                    Instantiate(_floorPrefab, _parent).transform.localPosition = finalPosition;
                    */
                    GameObject obj = null;
                    StageObjectType type = StageObjectType.Wall;
                    switch (stageData.StageDataArray[y, x])
                    {
                        case (int)StageObjectType.Wall:
                            obj = Instantiate(_editorDataSo.FloorPrefab, _parent);
                            finalPosition.y += 1;
                            obj.transform.localPosition = finalPosition;
                            type = StageObjectType.Wall;
                            break;
                        
                        case (int)StageObjectType.Floor:
                            obj = Instantiate(_editorDataSo.FloorPrefab, _parent);
                            obj.transform.localPosition = finalPosition;
                            type = StageObjectType.Floor;
                            break;
                        
                        case (int)StageObjectType.ItemSpawnBlock:
                            obj = Instantiate(_editorDataSo.ItemSpawnPrefab, _parent);
                            obj.transform.localPosition = finalPosition;
                            type = StageObjectType.ItemSpawnBlock;
                            break;
                        
                        case (int)StageObjectType.ThreatSpawnBlock:
                            obj = Instantiate(_editorDataSo.ThreadSpawnPrefab, _parent);
                            obj.transform.localPosition = finalPosition;
                            type = StageObjectType.ThreatSpawnBlock;
                            break;
                        
                        case (int)StageObjectType.NpcSpawnBlock:
                            obj = Instantiate(_editorDataSo.NPCSpawnPrefab, _parent);
                            obj.transform.localPosition = finalPosition;
                            type = StageObjectType.NpcSpawnBlock;
                            break;
                        
                        case (int)StageObjectType.PlayerSpawnBlock:
                            obj = Instantiate(_editorDataSo.PlayerSpanwPrefab, _parent);
                            obj.transform.localPosition = finalPosition;
                            type = StageObjectType.PlayerSpawnBlock;
                            break;
                        
                        default:
                            break;
                    }
                    
                    if (obj != null)
                    {
                        StageEditorBlock block = obj.AddComponent<StageEditorBlock>();
                        block.BlockType = type;
                    }
                }
            }

            return true;
        }
    }
}
