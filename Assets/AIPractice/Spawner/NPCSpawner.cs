using System;
using System.Collections;
using System.Collections.Generic;
using CollectItem.Collecter;
using CollectItem.Collecter.NPC;
using CollectItem.Stage;
using UnityEngine;
using UnityEngine.Serialization;

namespace CollectItem.Spawner
{
    public class NPCSpawner : MonoBehaviour
    {
        [SerializeField] private List<NPCStatesSO> _npcStatesSO = new List<NPCStatesSO>();
        [SerializeField] private GameObject _npcPrefab;
        [SerializeField] private StageUpdaterSO _stageUpdaterSO;
        [SerializeField] private float _spawnYPos = 1.5f;

        private void Start()
        {
            var npc = Instantiate(_npcPrefab, transform.position, Quaternion.identity);
            var pos = transform.position;
            Vector3 setPos = new Vector3Int((int)pos.x, 0, (int)pos.z);
            setPos.y = _spawnYPos;
            npc.transform.position = setPos;
            
            if(npc.TryGetComponent<NPCCollecter>(out var npcCollecter))
            {
                int index = UnityEngine.Random.Range(0, _npcStatesSO.Count);
                if(index >= _npcStatesSO.Count) index = _npcStatesSO.Count - 1;
                else if(index < 0) index = 0;
                
                npcCollecter.SetData(_npcStatesSO[index], _stageUpdaterSO, setPos);
            }
        }
    }
}
