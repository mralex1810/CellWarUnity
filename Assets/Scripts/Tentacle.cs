using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Tentacle : MonoBehaviourPunCallbacks
{
    private const int Speed = 100;
    public GameObject startCell;
    public GameObject endCell;
    public Cell startCellController;
    public Cell endCellController;
    public int score;
    [HideInInspector] public int counterEnd;
    [HideInInspector] public int counterCenter;
    [HideInInspector] public int counter;
    public bool isBilateral;
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
        counter = 0;
        counterEnd = (int) (Vector3.Distance(_startPosition, _endPosition) * 1000);
        counterCenter = counterEnd / 2;
        CheckOwner();
    }

    private void Update()
    {
        if (_doingUnilateral)
        {
            counter = Mathf.Min(counterEnd, counter + Speed);
            SetPosition();
            if (counter == counterEnd) _doingUnilateral = false;
            return;
        }

        if (_doingBilateral)
        {
            if (counter > counterCenter)
                counter = Mathf.Max(counterCenter, counter - Speed);
            else if (counter < counterCenter)
                counter = Mathf.Min(counterCenter, counter + Speed);
            SetPosition();
            if (counter == counterCenter) _doingBilateral = false;
        }

        if (_damageFromStart != 0)
            endCellController.Attack(startCellController.owner, _damageFromStart);
        _damageFromStart = 0;
    }

    private void OnDestroy()
    {
        if (_doingUnilateral)
        {
            startCellController.Attack(startCellController.owner, score);
        }
        else if (isBilateral)
        {
            startCellController.Attack(startCellController.owner, score / 2);
        }
        else
        {
            startCellController.Attack(startCellController.owner, score / 2);
            endCellController.Attack(startCellController.owner, score / 2);
        }
    }

    private void OnMouseUpAsButton()
    {
        if (startCellController.owner == PhotonNetwork.LocalPlayer.ActorNumber)
            startCellController.DestroyTentacleEvent(this);
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

    public void Attack()
    {
        _damageFromStart = 1;
    }

    public void CheckOwner()
    {

        _lineRenderer.colorGradient = startCellController.owner switch
        {
            1 => Gradients.GreenBlue,
            2 => Gradients.RedBlue,
            _ => _lineRenderer.colorGradient
        };
    }

    public void DoBilateral()
    {
        isBilateral = true;
        _doingBilateral = true;
        _doingUnilateral = false;
    }

    public void DoUniliteral()
    {
        isBilateral = false;
        _doingUnilateral = true;
        _doingBilateral = false;
    }
}