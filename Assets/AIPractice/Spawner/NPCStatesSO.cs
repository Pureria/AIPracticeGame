using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace CollectItem.Collecter.NPC
{
    [CreateAssetMenu(fileName = "NPCStatesSO", menuName = "CollectItem/NPC/NPCStatesSO")]
    public class NPCStatesSO : ScriptableObject
    {
        [Tooltip("次のノードに移動するまでの時間")]public float WaitTime;
        [Tooltip("脅威の重み"), Range(0,1)]public float ThreatWeight = 0.5f;
        [FormerlySerializedAs("DawnTime")] [Tooltip("脅威とぶつかった際のダウンタイム")]public float DownTime;
    }
}
