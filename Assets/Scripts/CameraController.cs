using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float zoomSensitivity;
    private Vector2 _f0Start;
    private Vector2 _f1Start;
    private float _swipeSensitivity;
    private Vector2 _swipeStart;


    private void Start()
    {
        _swipeSensitivity = 0.03f;
        zoomSensitivity = 5f;
    }

    private void Update()
    {
        if (Input.touchCount < 1) _swipeStart = Vector2.zero;
        if (Input.touchCount == 1) Swipe();
        if (Input.touchCount < 2)

        {
            _f0Start = Vector2.zero;
            _f1Start = Vector2.zero;
        }

        if (Input.touchCount == 2) ZoomAndroid();
    }

    private void ZoomAndroid()

    {
        if (_f0Start == Vector2.zero && _f1Start == Vector2.zero)

        {
            _f0Start = Input.GetTouch(0).position;
            _f1Start = Input.GetTouch(1).position;
        }

        Vector2 f0Position = Input.GetTouch(0).position;
        Vector2 f1Position = Input.GetTouch(1).position;
        float dir = Mathf.Sign(Vector2.Distance(_f1Start, _f0Start) - Vector2.Distance(f0Position, f1Position));
        GetComponent<Camera>().orthographicSize =
            CameraSize(GetComponent<Camera>().orthographicSize, dir);
    }

    private float CameraSize(float size, float delta)
    {
        return Mathf.Min(7, Mathf.Max(3, size + delta * Time.deltaTime * zoomSensitivity));
    }

    private void Swipe()
    {
        if (_swipeStart == Vector2.zero)
        {
            _swipeStart = Input.GetTouch(0).position;
            return;
        }

        Vector2 swipePosition = Input.GetTouch(0).position;
        transform.position = SwipePosition(swipePosition.x - _swipeStart.x, swipePosition.y - _swipeStart.y);
        _swipeStart = Vector2.zero;
    }

    private Vector3 SwipePosition(float posX, float posY)
    {
        var pos = new Vector3();
        Vector3 position = transform.position;
        pos.z = position.z;
        pos.x = Mathf.Min(14, Mathf.Max(5, position.x - posX * _swipeSensitivity));
        pos.y = Mathf.Min(7, Mathf.Max(3, position.y - posY * _swipeSensitivity));

        return pos;
    }
}