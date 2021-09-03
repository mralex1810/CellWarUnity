using System.Collections.Generic;
using UnityEngine;

public class Tentacle : MonoBehaviour
{
    private const int Speed = 100;
    public GameObject startCell;
    public GameObject endCell;
    public Cell startCellController;
    public Cell endCellController;
    public int score;
    public int counterEnd;
    public int counterCenter;
    public int counter;
    public bool isBilateral;
    public Tentacle oppositeTentacle;

    public GameObject bulletPrefab;
    [SerializeField] private bool _quickBreaking;
    private bool _breaking;

    private int _damageFromStart;
    private bool _doingBilateral;
    private bool _doingUnilateral;
    private EdgeCollider2D _edgeCollider2D;
    private Vector3 _endPosition;
    private LineRenderer _lineRenderer;
    private Vector3 _startPosition;


    private void Start()
    {
        _damageFromStart = 0;
        _lineRenderer = GetComponent<LineRenderer>();
        _edgeCollider2D = GetComponent<EdgeCollider2D>();
        startCellController = startCell.GetComponent<Cell>();
        endCellController = endCell.GetComponent<Cell>();
        _startPosition = _lineRenderer.GetPosition(0);
        _endPosition = _lineRenderer.GetPosition(1);
        counterEnd = score * Speed;
        counterCenter = counterEnd / 2;
        counter = _quickBreaking ? counterCenter : 0;
        CheckOwner();
    }

    private void Update()
    {
        var counterStart = counter;
        if (_breaking)
        {
            counter = Mathf.Max(0, counter - Speed);
            SetPosition();
        }

        if (_doingUnilateral)
        {
            counter = Mathf.Min(counterEnd, counter + Speed);
            SetPosition();
            if (counter == counterEnd) _doingUnilateral = false;
        }

        if (_doingBilateral)
        {
            if (counter >= counterCenter && counter + oppositeTentacle.counter < counterEnd)
                counter = Mathf.Max(counterCenter, Mathf.Min(counter + Speed, counterEnd - oppositeTentacle.counter));
            else if (counter > counterCenter) counter = Mathf.Max(counterCenter, counter - Speed);
            if (counter < counterCenter)
                counter = Mathf.Min(counterCenter, counter + Speed, counterEnd - oppositeTentacle.counter);
            SetPosition();
            if (counter == counterCenter && counter == oppositeTentacle.counter) _doingBilateral = false;
        }

        var damage = (counterStart - counter) / Speed;
        if (damage != 0)
            startCellController.Attack(_quickBreaking ? endCellController.owner : startCellController.owner, damage);
        if (_doingBilateral || _doingUnilateral || _breaking) SetPosition();
        if (_doingUnilateral) return;
        if (_damageFromStart != 0)
            endCellController.Attack(startCellController.owner, _damageFromStart);
        _damageFromStart = 0;
        if (_breaking && counter == 0)
        {
            startCellController.SendDeleteTentacle(endCellController.id);
            Destroy(gameObject);
        }
    }

    private void OnMouseUpAsButton()
    {
        startCellController.TentaclePressEvent(this);
    }

    public void DestroyIt()
    {
        if (counter == counterEnd)
        {
            counter = counterCenter;
            endCellController.AddTentacle(startCell, quickDestroying: true);
        }

        _doingBilateral = false;
        _doingUnilateral = false;
        _breaking = true;
    }

    private void SetPosition()
    {
        _lineRenderer.SetPosition(1, _startPosition + (_endPosition - _startPosition) * counter / counterEnd);
        _edgeCollider2D.SetPoints(new List<Vector2>
        {
            new Vector2(_lineRenderer.GetPosition(0).x, _lineRenderer.GetPosition(0).y),
            new Vector2(_lineRenderer.GetPosition(1).x, _lineRenderer.GetPosition(1).y)
        });
    }
    /*
    private void OnMouseEnter()
    {
        if (startCellController.owner == PhotonNetwork.LocalPlayer.ActorNumber && Input.GetMouseButton(0))
            startCellController.DestroyTentacleEvent(this);
    }
    */

    public void AttackStart()
    {
        var endCellPos = endCell.transform.position;
        var startCellPos = startCell.transform.position;
        var bullet = Instantiate(bulletPrefab, _lineRenderer.GetPosition(0), Quaternion.identity);
        var bulletController = bullet.GetComponent<Bullet>();
        bullet.transform.rotation = Quaternion.Euler(0, 0,
            180f * Mathf.Atan((endCellPos.y - startCellPos.y) / (endCellPos.x - startCellPos.x)
            ) / Mathf.PI);
        bulletController.tentacle = this;
        if (endCellPos.x - startCellPos.x < 0) bullet.gameObject.GetComponent<SpriteRenderer>().flipX = true;
    }

    public void AttackEnd()
    {
        if (isBilateral && counter >= counterCenter || !isBilateral && counter == counterEnd)
            endCellController.Attack(startCellController.owner, 1);
        else
            startCellController.Attack(startCellController.owner, 1);
    }

    public void CheckOwner()
    {
        _lineRenderer.colorGradient = _quickBreaking
            ? Gradients.GenGradientTwoColors(Color.blue, Game.ColorOfOwner(endCellController.owner))
            : Gradients.GenGradientTwoColors(Game.ColorOfOwner(startCellController.owner), Color.blue);
    }

    public void DoBilateral()
    {
        isBilateral = true;
        _doingBilateral = true;
        _doingUnilateral = false;
    }

    public void DoUnilaterally()
    {
        isBilateral = false;
        _doingUnilateral = true;
        _doingBilateral = false;
    }

    public bool IsIncrease()
    {
        if (_doingUnilateral) return true;
        return _doingBilateral && counter < counterCenter;
    }

    public void QuickDestroy()
    {
        _quickBreaking = true;
        _breaking = true;
    }
}