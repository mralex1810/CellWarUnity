using Photon.Pun;
using UnityEngine;

public class Tentacle : MonoBehaviourPunCallbacks
{
    public GameObject startCell;
    public GameObject endCell;
    public Cell startCellController;
    public Cell endCellController;
    public int score;
    private int _damage;
    private LineRenderer _lineRenderer;
    public bool isStarted;
    private Vector3 _startPosition;
    private Vector3 _endPosition;
    private const int Speed = 100;
    [HideInInspector] public int counterEnd;
    [HideInInspector] public int counter;


    private void Start()
    {
        _damage = 0;
        _lineRenderer = GetComponent<LineRenderer>();
        startCellController = startCell.GetComponent<Cell>();
        endCellController = endCell.GetComponent<Cell>();
        _startPosition = _lineRenderer.GetPosition(0);
        _endPosition = _lineRenderer.GetPosition(1);
        counter = 0;
        counterEnd = (int)(Vector3.Distance(_startPosition, _endPosition) * 1000);

        CheckOwner();
    }

    private void Update()
    {
        if (!isStarted)
        {
            counter = Mathf.Min(counterEnd, counter + Speed);
            _lineRenderer.SetPosition(1, _startPosition + ((_endPosition - _startPosition) * counter / counterEnd));
            _lineRenderer.colorGradient.alphaKeys[1].alpha = 1f * counter / counterEnd;
            if (counter == counterEnd)
            {
                isStarted = true;
            }
            return;
        }
        if (_damage != 0)
            endCellController.Attack(startCellController.owner, _damage);
        _damage = 0;
    }

    private void OnMouseUpAsButton()
    {
        if (startCellController.owner == PhotonNetwork.LocalPlayer.ActorNumber) 
            startCellController.DestroyTentacleEvent(this);
    }
    /*
    private void OnMouseEnter()
    {
        if (startCellController.owner == PhotonNetwork.LocalPlayer.ActorNumber && Input.GetMouseButton(0))
            startCellController.DestroyTentacleEvent(this);
    }
    */

    public void Attack(int damage)
    {
        this._damage = damage;
    }

    private void OnDestroy()
    {
        if (!isStarted)
        {
            startCellController.Attack(startCellController.owner, score);
        }
        else
        {
            startCellController.Attack(startCellController.owner, score / 2);
            endCellController.Attack(startCellController.owner, score / 2);
        }
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
}
