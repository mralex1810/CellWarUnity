using UnityEngine;

public class Bullet : MonoBehaviour
{
    private const int Speed = 100;
    private static uint _bullets;
    public Tentacle tentacle;
    public uint id;
    private int _counter;
    private Vector3 _endPosition;
    private Vector3 _startPosition;

    private void Start()
    {
        _counter = 0;
        id = _bullets++;
        _startPosition = tentacle.startCell.transform.position;
        _endPosition = tentacle.endCell.transform.position;
    }

    private void Update()
    {
        _counter += 100;
        if (_counter > tentacle.counter) Destroy(gameObject);
        SetPosition();
    }

    private void OnDestroy()
    {
        tentacle.AttackEnd();
    }

    private void SetPosition()
    {
        transform.position = _startPosition + (_endPosition - _startPosition) * _counter / tentacle.counterEnd;
    }
}