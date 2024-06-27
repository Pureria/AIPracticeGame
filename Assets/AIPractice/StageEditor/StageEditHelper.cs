using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CollectItem.Editor
{
    public class StageEditHelper : MonoBehaviour
    {
#if UNITY_EDITOR
        public Action<Vector3> GetClickPos;
        private Vector3 _mousePos;
        private Vector2 _offset;
        
        private void OnDrawGizmos()
        {
            UpdateGizmosPosition();
            CheckClick(_mousePos);
            Gizmos.DrawWireCube(_mousePos, Vector3.one);
        }

        private void CheckClick(Vector3 mousePos)
        {
            if (Event.current == null || Event.current.type != EventType.MouseUp) return;
            GetClickPos?.Invoke(_mousePos);
        }

        private void UpdateGizmosPosition()
        {
            Vector3 mousePos;
            if(Event.current == null) mousePos = Vector3.zero;
            else mousePos = Event.current.mousePosition;
            
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePos);
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            if (plane.Raycast(ray, out float distance))
            {
                Vector3 worldPos = ray.GetPoint(distance);
                worldPos = new Vector3Int((int)worldPos.x, 0, (int)worldPos.z);
                worldPos.x += _offset.x;
                worldPos.z += _offset.y;
                _mousePos = worldPos;
            }
        }
        
        public void SetOffset(Vector2 offset) => _offset = offset;
#endif
    }
}
