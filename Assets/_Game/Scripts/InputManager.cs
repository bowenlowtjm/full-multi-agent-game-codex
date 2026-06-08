using System.Collections.Generic;
using UnityEngine;

namespace Pully.Game
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private float dragThreshold = 10f;
        [SerializeField] private float doubleTapInterval = 0.3f;

        private readonly Dictionary<int, TouchData> _activeTouches = new();
        private readonly Dictionary<int, float> _lastTapTimes = new();
        private readonly Dictionary<int, Vector2> _lastTapPositions = new();

        private Camera _camera;
        private SpawnerManager _spawner;
        private GameStateManager _state;

        public void Configure(SpawnerManager spawner, GameStateManager state)
        {
            _spawner = spawner;
            _state = state;
            _camera = Camera.main;
            if (_camera == null)
            {
                var go = new GameObject("Main Camera");
                _camera = go.AddComponent<Camera>();
                go.tag = "MainCamera";
                _camera.orthographic = true;
                _camera.transform.position = new Vector3(0f, 0f, -10f);
            }
        }

        private void Update()
        {
            if (_spawner == null || _camera == null) return;
            if (_state != null && _state.CurrentState != GameState.GAMEPLAY) return;

            if (Input.touchCount > 0)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    ProcessTouch(Input.GetTouch(i));
                }
            }
            else
            {
                ProcessMouse();
            }
        }

        private void ProcessTouch(Touch touch)
        {
            var pos = (Vector2)touch.position;
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    OnTouchBegan(touch.fingerId, pos);
                    break;
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    OnTouchMoved(touch.fingerId, pos);
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    OnTouchEnded(touch.fingerId, pos);
                    break;
            }
        }

        private void ProcessMouse()
        {
            Vector2 pos = Input.mousePosition;
            const int id = 0;
            if (Input.GetMouseButtonDown(0)) OnTouchBegan(id, pos);
            else if (Input.GetMouseButton(0)) OnTouchMoved(id, pos);
            else if (Input.GetMouseButtonUp(0)) OnTouchEnded(id, pos);
        }

        private void OnTouchBegan(int fingerId, Vector2 screenPos)
        {
            var target = RaycastTarget(screenPos);
            bool isDoubleTap = false;

            if (_lastTapTimes.TryGetValue(fingerId, out float lastTime) &&
                _lastTapPositions.TryGetValue(fingerId, out Vector2 lastPos))
            {
                if (Time.time - lastTime <= doubleTapInterval && Vector2.Distance(lastPos, screenPos) <= dragThreshold * 2f)
                {
                    isDoubleTap = true;
                }
            }

            _activeTouches[fingerId] = new TouchData
            {
                fingerId = fingerId,
                startPos = screenPos,
                currentPos = screenPos,
                startTime = Time.time,
                target = target,
                isDragging = false,
                isDoubleTap = isDoubleTap
            };

            if (!isDoubleTap)
            {
                _lastTapTimes[fingerId] = Time.time;
                _lastTapPositions[fingerId] = screenPos;
            }
            else
            {
                _lastTapTimes.Remove(fingerId);
                _lastTapPositions.Remove(fingerId);
                if (target != null)
                {
                    _spawner.TryResolve(target, GestureType.DoubleTap);
                }
            }
        }

        private void OnTouchMoved(int fingerId, Vector2 screenPos)
        {
            if (!_activeTouches.TryGetValue(fingerId, out var data)) return;
            data.currentPos = screenPos;

            if (!data.isDragging)
            {
                data.isDragging = Vector2.Distance(data.startPos, data.currentPos) > dragThreshold;
            }

            if (data.isDragging && data.target != null)
            {
                var world = _camera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Mathf.Abs(_camera.transform.position.z)));
                world.z = 0f;
                data.target.transform.position = world;
            }

            _activeTouches[fingerId] = data;
        }

        private void OnTouchEnded(int fingerId, Vector2 screenPos)
        {
            if (!_activeTouches.TryGetValue(fingerId, out var data)) return;

            if (data.target != null)
            {
                var duration = Time.time - data.startTime;
                var moved = Vector2.Distance(data.startPos, screenPos);

                if (!data.isDoubleTap)
                {
                    if (data.isDragging)
                    {
                        _spawner.TryResolve(data.target, GestureType.SwipeTap);
                    }
                    else if (duration >= _spawner.Ruleset.longPressDuration)
                    {
                        _spawner.TryResolve(data.target, GestureType.LongPress);
                    }
                    else if (duration <= _spawner.Ruleset.doubleTapWindow && moved <= dragThreshold)
                    {
                        _spawner.TryResolve(data.target, GestureType.SingleTap);
                    }
                }
            }

            _activeTouches.Remove(fingerId);
        }

        private TargetRuntime RaycastTarget(Vector2 screenPos)
        {
            Vector3 world = _camera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Mathf.Abs(_camera.transform.position.z)));
            var hit = Physics2D.OverlapPoint(new Vector2(world.x, world.y));
            return hit != null ? hit.GetComponent<TargetRuntime>() : null;
        }

        private struct TouchData
        {
            public int fingerId;
            public Vector2 startPos;
            public Vector2 currentPos;
            public float startTime;
            public TargetRuntime target;
            public bool isDragging;
            public bool isDoubleTap;
        }
    }
}
