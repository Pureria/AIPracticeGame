using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "CollectItem/StageDataSO")]
public class StageDataSO : ScriptableObject
{
    public string StageName = "New Stage";
    public TextAsset stageDataJson;
}
