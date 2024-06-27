using System;
using CollectItem.Data;
using CollectItem.Stage;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace CollectItem.Editor
{
    public class GridPlacerWindow : EditorWindow
    {
        private static string EditorDataSOKey = "StageEditorDataSO";
        
        [ReadOnly] private TextAsset _SelectStageDataJson;
        private EditorDataSO _editorDataSo;
        
        private GameObject _RootObject;
        private Vector2Int _MapSize;
        private Vector2 _StageOffset;
        private StageData _SelectStageData;

        //エディタ上でのクリック待ちを行うかどうか
        private bool _isEditorClickWait;
        private StageObjectType _clickBlockType;
        private StageEditHelper _stageEditHelper;
        
        [MenuItem("Tools/Grid Placer")]
        public static void ShowWindow()
        {
            GetWindow<GridPlacerWindow>("Grid Placer");
        }

        private void OnEnable()
        {
            LoadEditorData();
            
            if (_stageEditHelper == null)
            {
                //シーンから"RootObject"という名前のオブジェクトを探す
                _RootObject = GameObject.Find("RootObject");
                if(_RootObject == null) _RootObject = new GameObject("RootObject");
            }
        }

        private void OnDisable()
        {                    
            SaveEditorData();
            if(_stageEditHelper != null)
                _stageEditHelper.GetClickPos -= SetBlock;
        }

        void OnGUI()
        {
            if (_editorDataSo == null)
            {
                _editorDataSo = (EditorDataSO)EditorGUILayout.ObjectField("Editor Data SO", _editorDataSo, typeof(EditorDataSO), false);
                return;
            }
            
            bool isOpenEditorScene = false;
            if (_editorDataSo.EditorSceneAsset != null)
            {
                if (SceneManager.GetActiveScene().name == _editorDataSo.EditorSceneAsset.name) isOpenEditorScene = true;
            }
            
            if(isOpenEditorScene) OpenEditorScene();
            else CloseEditorScene();
            
        }

        private void OpenEditorScene()
        {
            if (_RootObject == null)
            {
                //シーンから"RootObject"という名前のオブジェクトを探す
                _RootObject = GameObject.Find("RootObject");
                if(_RootObject == null) _RootObject = new GameObject("RootObject");
            }
            
            EditorGUILayout.BeginHorizontal();
            //Undoボタン
            if (GUILayout.Button("Undo"))
            {
                Undo.PerformUndo();
                GUIUtility.ExitGUI();
            }
            
            //Redoボタン
            if (GUILayout.Button("Redo"))
            {
                Undo.PerformRedo();
                GUIUtility.ExitGUI();
            }
            EditorGUILayout.EndHorizontal();
            
            GUILayout.Label("Stage Data Info");
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Select Stage Data Json", _SelectStageDataJson, typeof(TextAsset), false);
            GUI.enabled = true;
            GUILayout.Label("");
            
            GUILayout.Label("Grid Placer Settings", EditorStyles.boldLabel);
            //stageEditorSceneName = EditorGUILayout.TextField("Stage Editor Scene Name", stageEditorSceneName);
            _editorDataSo = (EditorDataSO)EditorGUILayout.ObjectField("Editor Data SO", _editorDataSo, typeof(EditorDataSO), false);
            _editorDataSo.EditorSceneAsset = (SceneAsset)EditorGUILayout.ObjectField("Editor Scene Asset", _editorDataSo.EditorSceneAsset, typeof(SceneAsset), false);
            _editorDataSo.JsonStageDataPath = (DefaultAsset)EditorGUILayout.ObjectField("Json Stage Data Path", _editorDataSo.JsonStageDataPath, typeof(DefaultAsset), false);
            _editorDataSo.StageDataSOPath = (DefaultAsset)EditorGUILayout.ObjectField("Stage Data SO Path", _editorDataSo.StageDataSOPath, typeof(DefaultAsset), false);
            
            GUILayout.Label("");
            GUILayout.Label("Stage Prefabs", EditorStyles.boldLabel);
            _editorDataSo.FloorPrefab = (GameObject)EditorGUILayout.ObjectField("Floor Prefab", _editorDataSo.FloorPrefab, typeof(GameObject), false);
            _editorDataSo.ThreadSpawnPrefab = (GameObject)EditorGUILayout.ObjectField("Thread Spawn Prefab", _editorDataSo.ThreadSpawnPrefab, typeof(GameObject), false);
            _editorDataSo.ItemSpawnPrefab = (GameObject)EditorGUILayout.ObjectField("Item Spawn Prefab", _editorDataSo.ItemSpawnPrefab, typeof(GameObject), false);
            _editorDataSo.PlayerSpanwPrefab = (GameObject)EditorGUILayout.ObjectField("Player Spawn Prefab", _editorDataSo.PlayerSpanwPrefab, typeof(GameObject), false);
            _editorDataSo.NPCSpawnPrefab = (GameObject)EditorGUILayout.ObjectField("NPC Spawn Prefab", _editorDataSo.NPCSpawnPrefab, typeof(GameObject), false);
            
            GUILayout.Label("");
            GUILayout.Label("Map Settings", EditorStyles.boldLabel);
            _MapSize = EditorGUILayout.Vector2IntField("Map Size", _MapSize);
            _StageOffset = EditorGUILayout.Vector2Field("Stage Offset", _StageOffset);
            
            if (GUILayout.Button("Fill Map Area"))
            {
                FillMapArea();
            }
            
            _clickBlockType = (StageObjectType)EditorGUILayout.EnumPopup("Click Block Type", _clickBlockType);
            if (_isEditorClickWait)
            {
                if (GUILayout.Button("Cancel Put Block")) _isEditorClickWait = false;
            }
            else
            {
                if(GUILayout.Button("Put Block")) _isEditorClickWait = true;
            }

            if (GUILayout.Button("All Clear Stage Object"))
            {
                ClearMapArea();
            }

            GUILayout.Space(100.0f);

            if (GUILayout.Button("Load Map Data"))
            {
                LoadAndGenerateEditor.Window(_RootObject.transform, SuccessGenerateStage);
                GUIUtility.ExitGUI();
            }

            if (GUILayout.Button("All Clear"))
            {
                _SelectStageDataJson = null;
                _RootObject.transform.position = Vector3.zero;
                ClearMapArea();
                _MapSize.x = 0;
                _MapSize.y = 0;
                _StageOffset = Vector2.zero;
            }

            if (GUILayout.Button("Overwrite and Save"))
            {
                if (_SelectStageDataJson == null)
                {
                    //上書きできるものがないと警告を出す
                    EditorGUILayout.HelpBox("Not Selected Stage Data", MessageType.Warning);
                    return;
                }

                SaveData.SaveStageData(_SelectStageDataJson, GetNewStageData());
            }

            if (GUILayout.Button("Create New Json Map Data"))
            {
                CreateJsonMapData();
            }

            if (GUILayout.Button("Create New JsonFile and New StageSO Data"))
            {
                CreateJsonMapData(out TextAsset jsonFile);
                SaveData.CreateNewStageDataSO(_editorDataSo.StageDataSOPath, "NewStageDataSO", jsonFile);
            }
            
            _RootObject.transform.position = new Vector3(_StageOffset.x, 0, _StageOffset.y);
            _stageEditHelper.SetOffset(_StageOffset);

            if (_stageEditHelper == null)
            {
                _stageEditHelper = FindObjectOfType<StageEditHelper>();
                if (_stageEditHelper == null)
                {
                    _stageEditHelper = new GameObject("StageEditHelper").AddComponent<StageEditHelper>();
                }
            }
            
            if(_stageEditHelper.GetClickPos == null)
                _stageEditHelper.GetClickPos += SetBlock;
        }

        private void SetBlock(Vector3 setPosition)
        {
            if (!_isEditorClickWait) return;
            if (SetStageBlock(setPosition, _clickBlockType, out GameObject blockObject))
            {
                Undo.RegisterCreatedObjectUndo(blockObject, "Place Prefab");
            }
        }

        private void CloseEditorScene()
        {
            //stageEditorSceneName = EditorGUILayout.TextField("Stage Editor Scene Name", stageEditorSceneName);
            
            _editorDataSo.EditorSceneAsset = (SceneAsset)EditorGUILayout.ObjectField("Editor Scene Asset", _editorDataSo.EditorSceneAsset, typeof(SceneAsset), false);
            if (_editorDataSo.EditorSceneAsset == null)
            {
                EditorGUILayout.HelpBox($"Please Set Editor Scene.", MessageType.Warning);
                return;
            }
            else
            {
                EditorGUILayout.HelpBox($"This tool can only be used in the '{_editorDataSo.EditorSceneAsset.name}' scene.", MessageType.Warning);
            }
            
            if(GUILayout.Button("Open Scene"))
            {
                OpenRequiredScene();
            }
        }

        void FillMapArea()
        {
            for(int z = 0;z < _MapSize.y; z++)
            {
                for(int x = 0; x < _MapSize.x; x++)
                {
                    Vector3 finalPosition = new Vector3(x, 0, z);
                    if(SetStageBlock(finalPosition, StageObjectType.Floor, out GameObject blockObject))
                    {
                        Undo.RegisterCreatedObjectUndo(blockObject, "Place Prefab");
                    }
                }
            }
        }

        private bool SetStageBlock(Vector3 pos, StageObjectType type, out GameObject blockObject)
        {
            blockObject = null;

            if (IsExistBlock(pos)) return false;
            
            GameObject prefab = null;
            switch (type)
            {
                case StageObjectType.Wall:
                    prefab = _editorDataSo.FloorPrefab;
                    break;
                case StageObjectType.Floor:
                    prefab = _editorDataSo.FloorPrefab;
                    break;
                case StageObjectType.ThreatSpawnBlock:
                    prefab = _editorDataSo.ThreadSpawnPrefab;
                    break;
                case StageObjectType.ItemSpawnBlock:
                    prefab = _editorDataSo.ItemSpawnPrefab;
                    break;
                case StageObjectType.PlayerSpawnBlock:
                    prefab = _editorDataSo.PlayerSpanwPrefab;
                    break;
                
                case StageObjectType.NpcSpawnBlock:
                    prefab = _editorDataSo.NPCSpawnPrefab;
                    break;
                
                default:
                    return false;
            }
            
            GameObject gameObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab, _RootObject.transform);
            gameObject.transform.localPosition = new Vector3Int((int)pos.x, (int)pos.y, (int)pos.z);
            //生成したオブジェクトにStageEditorBlockコンポーネントを追加
            StageEditorBlock stageEditorBlock = gameObject.AddComponent<StageEditorBlock>();
            stageEditorBlock.BlockType = type;
            blockObject = gameObject;
            return true;
        }
        
        private void ClearMapArea()
        {
            //RootObjectの子オブジェクトを全て削除
            int childCount = _RootObject.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Undo.DestroyObjectImmediate(_RootObject.transform.GetChild(0).gameObject);
            }
        }

        private void CreateJsonMapData()
        {
            CreateJsonMapData(out TextAsset jsonFile);
        }

        private StageData GetNewStageData()
        {
            StageData newData = new StageData();
            newData.StageWidth = _MapSize.x;
            newData.StageHeight = _MapSize.y;
            newData.StageDataArray = new int[_MapSize.y, _MapSize.x];
            newData.StageWidthOffset = _StageOffset.x;
            newData.StageHeightOffset = _StageOffset.y;
            for (int z = 0; z < _MapSize.y; z++)
            {
                for (int x = 0; x < _MapSize.x; x++)
                {
                    Vector3 finalPosition = new Vector3(x, 0, z);
                    newData.StageDataArray[z, x] = (int)GetPositionBlockType(finalPosition);
                }
            }
            return newData;
        }

        private void CreateJsonMapData(out TextAsset jsonFile)
        {
            StageData newData = GetNewStageData();
            SaveData.CreateNewStageJsonFile(_editorDataSo.JsonStageDataPath, newData, out jsonFile);
        }

        /// <summary>
        /// 指定した座標のオブジェクトがどのブロックタイプか調べる
        /// </summary>
        /// <param name="position">調べる座標</param>
        /// <returns></returns>
        private StageObjectType GetPositionBlockType(Vector3 position)
        {
            int childCount = _RootObject.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = _RootObject.transform.GetChild(i);
                if (child.localPosition == position)
                {
                    StageEditorBlock stageEditorBlock = child.GetComponent<StageEditorBlock>();
                    if (stageEditorBlock != null) return stageEditorBlock.BlockType;
                    else return StageObjectType.Wall;
                }
            }
            
            //ブロックが存在しないので壁用の値を返す
            return StageObjectType.Wall;
        }
        
        /// <summary>
        /// 引数で渡されたポジションにブロックが存在するかどうか調べる
        /// </summary>
        /// <param name="position">調べるポジション</param>
        /// <returns></returns>
        private bool IsExistBlock(Vector3 position)
        {
            int childCount = _RootObject.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = _RootObject.transform.GetChild(i);
                if (child.localPosition == position)
                {
                    return true;
                }
            }
            return false;
        }
        
        private void OpenRequiredScene()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                //EditorSceneManagerで_EditorSceneAssetを開く
                EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(_editorDataSo.EditorSceneAsset));
            }
        }

        private void SuccessGenerateStage(TextAsset stageDataJson, StageData stageData)
        {
            _SelectStageData = stageData;
            _SelectStageDataJson = stageDataJson;
            
            _StageOffset.x = stageData.StageWidthOffset;
            _StageOffset.y = stageData.StageHeightOffset;
            _MapSize.x = stageData.StageWidth;
            _MapSize.y = stageData.StageHeight;
            Undo.ClearAll();
        }

        private void LoadEditorData()
        {
            string path = EditorPrefs.GetString(EditorDataSOKey, "");
            if (!string.IsNullOrEmpty(path))
            {
                _editorDataSo = AssetDatabase.LoadAssetAtPath<EditorDataSO>(path);
            }
        }
        
        private void SaveEditorData()
        {
            if (_editorDataSo != null)
            {
                string path = AssetDatabase.GetAssetPath(_editorDataSo);
                EditorPrefs.SetString(EditorDataSOKey, path);
            }
        }
    }
}
