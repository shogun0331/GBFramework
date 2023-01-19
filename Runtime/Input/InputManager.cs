using UnityEngine;
using UnityEngine.EventSystems;
using GB.Global;
using System.Collections.Generic;
namespace GB
{
    /// <remarks>
    /// <copyright file="InputManager.cs" company="GB">
    /// The MIT License (MIT)
    /// 
    /// Copyright (c) 2021 GB
    /// 
    /// Permission is hereby granted, free of charge, to any person obtaining a copy
    /// of this software and associated documentation files (the "Software"), to deal
    /// in the Software without restriction, including without limitation the rights
    /// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    /// copies of the Software, and to permit persons to whom the Software is
    /// furnished to do so, subject to the following conditions:
    /// 
    /// The above copyright notice and this permission notice shall be included in
    /// all copies or substantial portions of the Software.
    /// 
    /// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    /// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    /// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    /// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    /// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    /// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    /// THE SOFTWARE.
    /// 
    /// 

    public class InputManager : MonoBehaviour
    {

        public delegate void touchDelegate(TouchPhase phase, int id,
                        float x, float y, float dx, float dy);

        public event touchDelegate touchEvent;

        public delegate void keyDelegate(KeyCode keyCode);
        public event keyDelegate keyEvent;

        private Vector3 _preMousePosition = Vector3.zero;
        //private Vector2 _preTouchPosition = Vector2.zero;
        private Dictionary<int, Vector2> _preTouchPositionDic;
        private Dictionary<int, Vector2> preTouchPositionDic
        {
            get
            {
                if (_preTouchPositionDic == null)
                {
                    _preTouchPositionDic = new Dictionary<int, Vector2>();
                }

                return _preTouchPositionDic;
            }
        }

        private bool _isClick = false;

        private float screenWidth = 720.0f;
        private float screenHeight = 1280.0f;


        #region singleton

        const HideFlags Flags = HideFlags.HideInHierarchy | HideFlags.DontSave;
        public static InputManager Instance => Application.isPlaying ? ComponentSingleton<InputManager>.Get(true, Flags) : null;

        #endregion


        // Use this for initialization
        void Start()
        {


        }

        public void OnDestroy()
        {
            touchEvent = null;

        }

        // Update is called once per frame
        void Update()
        {
            screenWidth = Screen.width;
            screenHeight = Screen.height;

            processTouch();
            processKey();

        }

        private void processTouch()
        {
            // if has not delegate, do nothing
            if (touchEvent == null)
                return;

            // if (EventSystem.current.currentSelectedGameObject != null)
            // {
            //     //Debug.Log("EventSystem.current is Null!");
            //     return;
            // }


#if UNITY_EDITOR
            if (EventSystem.current.IsPointerOverGameObject() &&
        (Input.GetMouseButton(0)))
            {
                return;
            }
#endif



            int touchCount = Input.touchCount;

            if (touchCount != 0)
            {
                for (int i = 0; i < touchCount; ++i)
                {
                    // touch position coordinate
                    Vector2 touchPosition = Vector2.zero;

                    // touch position trans to viewport coordinate
                    Vector2 tViewPos = Vector2.zero;

                    // touch delta position coordibnate
                    Vector2 touchDeltaPos = Vector2.zero;

                    // touch delta position coordinate to viewport delta coordinate
                    Vector2 tDeltaViewPos = Vector2.zero;

                    Touch touch = Input.GetTouch(i);

                    // if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    // {
                    //     _preTouchPosition = touch.position;
                    //     return;
                    // }

                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            // get touch began position
                            touchPosition = touch.position;
                            preTouchPositionDic[touch.fingerId] = touchPosition;

                            // change touch coord to viewport coord
                            tViewPos = new Vector2(touchPosition.x / screenWidth,
                                            touchPosition.y / screenHeight);

                            // action touch event
                            touchEvent(TouchPhase.Began, touch.fingerId, tViewPos.x,
                                            tViewPos.y, 0, 0);

                            break;
                        case TouchPhase.Moved:
                            // get touch moved position
                            touchPosition = touch.position;
                            // get touch moved delta position
                            touchDeltaPos = touchPosition - preTouchPositionDic[touch.fingerId];

                            // change touch coord to viewport coord
                            tViewPos = new Vector2(touchPosition.x / screenWidth,
                                            touchPosition.y / screenHeight);
                            // change touch delta coord to viewport delta coord
                            tDeltaViewPos = new Vector2(touchDeltaPos.x / screenWidth,
                                            touchDeltaPos.y / screenHeight);

                            // action touch event
                            touchEvent(TouchPhase.Moved, touch.fingerId, tViewPos.x,
                                            tViewPos.y, tDeltaViewPos.x,
                                            tDeltaViewPos.y);

                            preTouchPositionDic[touch.fingerId] = touchPosition;
                            break;
                        case TouchPhase.Ended:
                            // get touch moved position
                            touchPosition = touch.position;
                            // get touch moved delta position
                            touchDeltaPos = touchPosition - preTouchPositionDic[touch.fingerId];

                            // change touch coord to viewport coord
                            tViewPos = new Vector2(touchPosition.x / screenWidth,
                                            touchPosition.y / screenHeight);
                            // change touch delta coord to viewport delta coord
                            tDeltaViewPos = new Vector2(touchDeltaPos.x / screenWidth,
                                            touchDeltaPos.y / screenHeight);


                            // action touch event
                            touchEvent(TouchPhase.Ended, touch.fingerId, tViewPos.x, tViewPos.y,
                                            tDeltaViewPos.x, tDeltaViewPos.y);

                            _isClick = false;
                            break;
                    }
                }
            }
            else       // has not touch, get mouse Control
            {
                /* mouse control */
                for (int i = 0; i < 2; ++i)
                {
                    if (Input.GetMouseButtonDown(i))
                    {   // mouse down
                        // get mouse position
                        Vector3 position = Input.mousePosition;
                        _preMousePosition = position;

                        // convert screen coor to viewport coor
                        Vector2 viewPos = new Vector2(position.x / screenWidth,
                                        position.y / screenHeight);

                        // action touch Event
                        touchEvent(TouchPhase.Began, -1, viewPos.x, viewPos.y, 0, 0);

                        _isClick = true;
                    }
                    else if (Input.GetMouseButtonUp(i))
                    {   // mouse up
                        // get mouse position
                        Vector3 position = Input.mousePosition;
                        // get mouse delta position
                        Vector3 deltaPos = position - _preMousePosition;

                        // convert screen coord to viewport coord
                        Vector2 viewPos = new Vector2(position.x / screenWidth,
                                        position.y / screenHeight);
                        Vector2 viewDeltaPos = new Vector2(deltaPos.x / screenWidth,
                                        deltaPos.y / screenHeight);

                        // action touch event
                        touchEvent(TouchPhase.Ended, -1, viewPos.x, viewPos.y,
                                        viewDeltaPos.x, viewDeltaPos.y);

                        _isClick = false;
                    }
                    else if (Input.GetMouseButton(i))
                    {   // mouse move
                        // action only mouse clicked
                        if (_isClick)
                        {
                            // get mouse position
                            Vector3 position = Input.mousePosition;

                            // get mouse delta position
                            Vector3 deltaPos = position - _preMousePosition;

                            // convert screen coord to viewport coord
                            Vector2 viewPos = new Vector2(position.x / screenWidth,
                                            position.y / screenHeight);

                            Vector2 viewDeltaPos = new Vector2(deltaPos.x / screenWidth,
                                            deltaPos.y / screenHeight);

                            // action touch event
                            touchEvent(TouchPhase.Moved, -1, viewPos.x, viewPos.y,
                                            viewDeltaPos.x, viewDeltaPos.y);

                            _preMousePosition = Input.mousePosition;
                        }
                    }
                }
            }
        }


        public bool keytouched = false;

        private void processKey()
        {
            keytouched = false;

#if UNITY_EDITOR

            // else if (Input.GetKeyDown(KeyCode.Alpha5))
            // {
            // }
            // else if (Input.GetKeyDown(KeyCode.Alpha6))
            // {
            // }
            // else if (Input.GetKeyDown(KeyCode.Alpha7))
            // {
            // }
            // else if (Input.GetKeyDown(KeyCode.Alpha8))
            // {
            // }
            // else if (Input.GetKeyDown(KeyCode.Space))
            // {

            // }
            // else if (Input.GetKeyDown(KeyCode.LeftShift))
            // {


            // }

            // else if (Input.GetKeyDown(KeyCode.Q))
            // {


            // }
            // else if (Input.GetKeyDown(KeyCode.W))
            // {


            // }
            // else if (Input.GetKeyDown(KeyCode.E))
            // {


            // }

            if (_isClick) return;

#endif
        }


    }
}