/// Credit BinaryX 
/// Sourced from - http://forum.unity3d.com/threads/scripts-useful-4-6-scripts-collection.264161/page-2#post-1945602
/// Updated by ddreaper - removed dependency on a custom ScrollRect script. Now implements drag interfaces and standard Scroll Rect.

using System;
using System.Collections.Generic;
using product_name;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(ScrollRect))]
    [AddComponentMenu("Layout/Extensions/Vertical Scroll Snap")]
    public class VerticalScrollSnap : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public List<PanelSwipeItem> _items = new List<PanelSwipeItem>();
        private Transform _screensContainer;

        private int _screens = 1;
        private int _startingScreen = 1;

        private bool _fastSwipeTimer = false;
        private int _fastSwipeCounter = 0;
        private int _fastSwipeTarget = 50;


        private List<Vector3> _positions;
        private ScrollRect _scroll_rect;
        private Vector3 _lerp_target;
        private bool _lerp;

        private int _containerSize;

        [Tooltip("The gameobject that contains toggles which suggest pagination. (optional)")]
        public GameObject Pagination;

        public Boolean UseFastSwipe = true;
        public int FastSwipeThreshold = 100;

        private bool _startDrag = true;
        private Vector3 _startPosition = new Vector3();
        private int _currentScreen;
        private bool fastSwipe = false; //to determine if a fast swipe was performed


        // Use this for initialization
        void Start()
        {
            _scroll_rect = gameObject.GetComponent<ScrollRect>();
            _screensContainer = _scroll_rect.content;
            DistributePages();

            _screens = _screensContainer.childCount;

            _lerp = false;

            _positions = new List<Vector3>();

            if (_screens > 0)
            {
                for (int i = 0; i < _screens; ++i)
                {
                    _scroll_rect.verticalNormalizedPosition = (float) i / (float) (_screens - 1);
                    _positions.Add(_screensContainer.localPosition);
                }
            }

            _currentScreen = _positions.Count - 1;
            _scroll_rect.verticalNormalizedPosition = _currentScreen / (_screens - 1);
            _items[_currentScreen].SelectItem();

            _containerSize = (int) _screensContainer.gameObject.GetComponent<RectTransform>().sizeDelta.y;
        }

        void Update()
        {
            if (_lerp)
            {
                _screensContainer.localPosition = Vector3.Lerp(_screensContainer.localPosition, _lerp_target, 7.5f * Time.deltaTime);
                if (Vector3.Distance(_screensContainer.localPosition, _lerp_target) < 0.005f)
                {
                    _lerp = false;
                }
            }

            if (_fastSwipeTimer)
            {
                _fastSwipeCounter++;
            }
        }


        //Because the CurrentScreen function is not so reliable, these are the functions used for swipes
        private void NextScreenCommand()
        {
            if (_currentScreen < _screens - 1)
            {
                _lerp = true;
                _lerp_target = _positions[_currentScreen + 1];
                _items[_currentScreen].DeSelectItem();
                _items[_currentScreen + 1].SelectItem();
            }
        }

        //Because the CurrentScreen function is not so reliable, these are the functions used for swipes
        private void PrevScreenCommand()
        {
            if (_currentScreen > 0)
            {
                _lerp = true;
                _lerp_target = _positions[_currentScreen - 1];
                _items[_currentScreen].DeSelectItem();
                _items[_currentScreen - 1].SelectItem();
            }
        }


        //find the closest registered point to the releasing point
        private int FindClosestFrom(Vector3 start, List<Vector3> positions)
        {
            float distance = Mathf.Infinity;
            int currentPositionIndex = 0;

            for (int i = 0; i < _positions.Count; i++)
            {
                if (Vector3.Distance(start, _positions[i]) < distance)
                {
                    distance = Vector3.Distance(start, _positions[i]);
                    currentPositionIndex = i;
                }
            }

            if (_currentScreen != currentPositionIndex)
            {
                _items[_currentScreen].DeSelectItem();
                _items[currentPositionIndex].SelectItem();
            }

            return currentPositionIndex;
        }


        //returns the current screen that the is seeing
        public int CurrentScreen()
        {
            float absPoz = Math.Abs(_screensContainer.gameObject.GetComponent<RectTransform>().offsetMin.y);
            absPoz = Mathf.Clamp(absPoz, 1, _containerSize - 1);
            float calc = (absPoz / _containerSize) * _screens;
            return (int) calc;
        }


        //used for changing between screen resolutions
        private void DistributePages()
        {
            float _offset = 0;
            float _step = Screen.height;
            float _dimension = 0;

            Vector2 panelDimensions = gameObject.GetComponent<RectTransform>().sizeDelta;


            float currentYPosition = 0;

            for (int i = 0; i < _screensContainer.transform.childCount; i++)
            {
                RectTransform child = _screensContainer.transform.GetChild(i).gameObject.GetComponent<RectTransform>();
                currentYPosition = _offset + i * _step;
                child.sizeDelta = new Vector2(panelDimensions.x, panelDimensions.y);
                child.anchoredPosition = new Vector2(0f - panelDimensions.x / 2, currentYPosition + panelDimensions.y / 2);
            }

            _dimension = currentYPosition + _offset * -1;

            _screensContainer.GetComponent<RectTransform>().offsetMax = new Vector2(0f, _dimension);
        }

        #region Interfaces

        public void OnBeginDrag(PointerEventData eventData)
        {
            _startPosition = _screensContainer.localPosition;
            _fastSwipeCounter = 0;
            _fastSwipeTimer = true;
            _currentScreen = CurrentScreen();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _startDrag = true;
            if (_scroll_rect.vertical)
            {
                if (UseFastSwipe)
                {
                    fastSwipe = false;
                    _fastSwipeTimer = false;
                    if (_fastSwipeCounter <= _fastSwipeTarget)
                    {
                        if (Math.Abs(_startPosition.y - _screensContainer.localPosition.y) > FastSwipeThreshold)
                        {
                            fastSwipe = true;
                        }
                    }

                    if (fastSwipe)
                    {
                        if (_startPosition.y - _screensContainer.localPosition.y > 0)
                        {
                            NextScreenCommand();
                        }
                        else
                        {
                            PrevScreenCommand();
                        }
                    }
                    else
                    {
                        _lerp = true;
                        _lerp_target = _positions[FindClosestFrom(_screensContainer.localPosition, _positions)];
                    }
                }
                else
                {
                    _lerp = true;
                    _lerp_target = _positions[FindClosestFrom(_screensContainer.localPosition, _positions)];
                }
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            _lerp = false;
            if (_startDrag)
            {
                OnBeginDrag(eventData);
                _startDrag = false;
            }
        }

        #endregion
    }
}