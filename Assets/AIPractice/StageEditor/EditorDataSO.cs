using UnityEngine;
using UnityEditor;

namespace CollectItem.Editor
{
    [CreateAssetMenu(fileName = "EditorData", menuName = "CollectItem/EditorDataSO")]
    public class EditorDataSO : ScriptableObject
    {
        public SceneAsset EditorSceneAsset;
        public DefaultAsset JsonStageDataPath;
        public DefaultAsset StageDataSOPath;
        
        [Header("Stage Prefabs")]
        public GameObject FloorPrefab;
        public GameObject ThreadSpawnPrefab;
        public GameObject ItemSpawnPrefab;
        public GameObject PlayerSpanwPrefab;
        public GameObject NPCSpawnPrefab;
    }
}
