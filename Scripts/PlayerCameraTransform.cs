using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoysPlayer
{
    public class PlayerCameraTransform : MonoBehaviour
    {
        public Vector2 offset;
        [HideInInspector] public float x;
        [HideInInspector] public float z;
        [HideInInspector] public Transform pos;
        void Start()
        {
            pos = transform;
        }
        void Update()
        {
            pos.localRotation = Quaternion.Euler(x + offset.x, 0, z + offset.y);
        }
    }
}