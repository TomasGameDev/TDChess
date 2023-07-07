using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoysPlayer
{
    public class PlayerCamera : MonoBehaviour, IPointerDownHandler, IDragHandler,IBeginDragHandler, IPointerUpHandler
    {
        [Header("Camera")]
        public Vector2 sens = Vector2.one * 0.2f;
        public float cameraRotHorizonLock = 90;
        public float cameraRot;
        public Transform cameraPos;
        public PlayerCameraTransform cameraBodyPos;
        public Transform cameraPivot;  
        public bool lockCamera;

        [Header("Focus")]
        bool isFocusing;
        public int onSelectTime = 30;
        [Tooltip("Size in pixels")] public float screenFocusDistance = 30;
        Vector2 focusPos;
        Vector2 lastFocusPos;
        int focusTime;

        [Header("Pinch")]
        public float panningSensitivity = 1;
        public float startTouchesDistance;
        public float startSize;
        public Vector2[] touches = new Vector2[2];

        [Header("Camera distance")]
        public float minSurfaceDistance = 0;
        public float maxSurfaceDistance = 10;
        public float _surfaceDistance;
        public float surfaceDistance
        {
            get { return _surfaceDistance; }
            set
            {
                _surfaceDistance = Mathf.Clamp(value, minSurfaceDistance, maxSurfaceDistance);
            }
        }
        public float surfaceNormalDistance = 0.01f;

        private void Start()
        {
            if (PlayerPrefs.HasKey("sensivity"))
                sens = Vector2.one * PlayerPrefs.GetFloat("sensivity");
            surfaceDistance = PlayerPrefs.HasKey("surfaceDistance") ? PlayerPrefs.GetFloat("surfaceDistance") : maxSurfaceDistance; 
        } 
        public LayerMask layerMask;

        public Transform[] touchesobject;
        public bool multiTouchEnabled;

        void Update()
        {
            if (isFocusing)
            {
                if (focusTime < onSelectTime)
                {
                    focusTime++;
                    if (Vector2.Distance(lastFocusPos, focusPos) > screenFocusDistance)
                        LockCameraView();
                }
            }
            RaycastHit hit;
            if (Physics.Raycast(cameraBodyPos.pos.position, cameraBodyPos.pos.TransformDirection(Vector3.back), out hit, surfaceDistance, ~layerMask))
            {
                cameraPos.position = hit.point + hit.normal * surfaceNormalDistance;
            }
            else
            {
                cameraPos.localPosition = Vector3.back * surfaceDistance;

#if UNITY_EDITOR

                if (Input.GetMouseButton(0))
                    touches[1] = Input.mousePosition;
                if (Input.GetMouseButton(1))
                    touches[0] = Input.mousePosition;
                touchesobject[0].gameObject.SetActive(multiTouchEnabled);
                touchesobject[1].gameObject.SetActive(multiTouchEnabled);
                touchesobject[0].position = touches[0];
                touchesobject[1].position = touches[1];

                //float scrollDelta = Input.mouseScrollDelta.y * 2f;
                //if (surfaceDistance + scrollDelta > minSurfaceDistance)
                //    scrollDelta = -.1f;
                //if (surfaceDistance + scrollDelta < maxSurfaceDistance)
                //    scrollDelta = .1f;
                //print(Input.mouseScrollDelta.y);
                surfaceDistance += Input.mouseScrollDelta.y * panningSensitivity;
#endif
            }
        }
        public void LockCameraView()
        {
            lastFocusPos = focusPos;
            isFocusing = false;
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            lastFocusPos = focusPos = eventData.position;
            focusTime = 0;
            isFocusing = true;
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            //Pinches 
            startSize = surfaceDistance;
#if !UNITY_EDITOR
            touches[0] = Input.touches[0].position;
            touches[1] = Input.touches[1].position;
#endif
            startTouchesDistance = Vector2.Distance(touches[0], touches[1]);
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (lockCamera)
                return;
            int touchesCount = Input.touchCount;
#if UNITY_EDITOR
            if (multiTouchEnabled)
                touchesCount = 2;
#endif
            if (touchesCount == 2)
            {
#if !UNITY_EDITOR
            touches[0] = Input.touches[0].position;
            touches[1] = Input.touches[1].position;
#endif
                float touchesDistance = startTouchesDistance - Vector2.Distance(touches[0], touches[1]); 
                surfaceDistance = startSize + touchesDistance * panningSensitivity * 0.01f;
            }
            else
            {
                cameraPivot.Rotate(new Vector3(0, eventData.delta.x * sens.y, 0));
                float r = Mathf.Abs(eventData.delta.y) * sens.x;
                if (eventData.delta.y < 0 && (cameraRotHorizonLock == 0 || cameraRot < cameraRotHorizonLock - r))
                {
                    cameraRot += r;
                }
                if (eventData.delta.y > 0 && (cameraRotHorizonLock == 0 || cameraRot > -cameraRotHorizonLock + r))
                {
                    cameraRot -= r;
                }
                cameraBodyPos.x = cameraRot;

            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (focusTime < onSelectTime && isFocusing)
            {
                TDChessController.instance.ThrowRay(eventData.position);
            }
            isFocusing = false;
            PlayerPrefs.SetFloat("surfaceDistance", _surfaceDistance);
        }
        public static bool IsPointInCollider(Vector3 _point, Collider _col)
        {
            return _col.ClosestPoint(_point) == _point;
        }

    }
}