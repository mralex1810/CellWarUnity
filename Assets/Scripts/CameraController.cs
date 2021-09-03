using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{
    public float zoomSensitivity;
    [SerializeField] private Game game;
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

        if (Input.GetKey("w")) transform.position = MovePosition(0f, 0.2f);
        if (Input.GetKey("a")) transform.position = MovePosition(-0.2f, 0f);
        if (Input.GetKey("s")) transform.position = MovePosition(0f, -0.2f);
        if (Input.GetKey("d")) transform.position = MovePosition(0.2f, 0f);
        if (Input.GetKey("z")) GetComponent<Camera>().orthographicSize = GetComponent<Camera>().orthographicSize + 0.5f;
        if (Input.GetKey("x")) GetComponent<Camera>().orthographicSize = GetComponent<Camera>().orthographicSize - 0.5f;
    }

    private void ZoomAndroid()

    {
        if (_f0Start == Vector2.zero && _f1Start == Vector2.zero)

        {
            _f0Start = Input.GetTouch(0).position;
            _f1Start = Input.GetTouch(1).position;
        }

        var f0Position = Input.GetTouch(0).position;
        var f1Position = Input.GetTouch(1).position;
        var dir = Mathf.Sign(Vector2.Distance(_f1Start, _f0Start) - Vector2.Distance(f0Position, f1Position));
        GetComponent<Camera>().orthographicSize =
            CameraSize(GetComponent<Camera>().orthographicSize, dir);
    }

    private float CameraSize(float size, float delta)
    {
        return Mathf.Min(10, Mathf.Max(4, size + delta * Time.deltaTime * zoomSensitivity));
    }

    private void Swipe()
    {
        if (_swipeStart == Vector2.zero)
        {
            _swipeStart = Input.GetTouch(0).position;
            return;
        }

        var swipePosition = Input.GetTouch(0).position;
        transform.position = SwipePosition(swipePosition.x - _swipeStart.x, swipePosition.y - _swipeStart.y);
        _swipeStart = Vector2.zero;
    }

    private Vector3 SwipePosition(float posX, float posY)
    {
        var pos = new Vector3();
        var position = transform.position;
        pos.z = position.z;

        var posXMin = 0;
        var posXMax = 0;
        var posYMin = 0;
        var posYMax = 0;
        switch (SceneManager.GetActiveScene().name)
        {
            case "GameWithBots1v1":
                posXMin = 4;
                posYMin = 2;
                posXMax = 15;
                posYMax = 8;
                break;
            case "GameWithPhoton1v1":
                posXMin = 4;
                posYMin = 2;
                posXMax = 15;
                posYMax = 8;
                break;
            case "GameWithPhoton4":
                posXMin = -17;
                posYMin = -7;
                posXMax = 17;
                posYMax = 7;
                break;
        }

        pos.x = Mathf.Min(posXMax, Mathf.Max(posXMin, position.x - posX * _swipeSensitivity));
        pos.y = Mathf.Min(posYMax, Mathf.Max(posYMin, position.y - posY * _swipeSensitivity));

        return pos;
    }

    private Vector3 MovePosition(float deltaPosX, float deltaPosY)
    {
        var pos = new Vector3();
        var position = transform.position;
        pos.z = position.z;
        pos.x = position.x + deltaPosX;
        pos.y = position.y + deltaPosY;

        return pos;
    }

    public void CentralizeCameraOnOwnerCell()
    {
        foreach (var cell in game.cellsController)
            if (cell.owner == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                var cellPos = cell.transform.position;
                transform.position = new Vector3(cellPos.x, cellPos.y, -10);
                break;
            }
    }
}