using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cell : MonoBehaviour
{
    private const int CounterController = 40;

    private const double Radius = 0.35;
    private static readonly int[] ScoreToLvl = {0, 10, 25, 100, 250, 500, 990, 1000};
    private static readonly int[] ScoreToLvlGrayCells = {0, 5, 15, 50, 100, 200, 400, 410};
    public static int CellCounter;
    public int lvl;
    public int score;
    public int owner;
    public GameObject textScoreObject;
    public GameObject textTentaclesObject;
    public GameObject cellCircle;
    public GameObject textDistanceObject;
    public Sprite greenSprite;
    public Sprite redSprite;
    public Sprite graySprite;
    public int id;
    public int maxLvl;
    public int tentaclesMax;
    public GameObject tentaclePrefab;
    public int tentaclesCount;
    public Game game;
    public bool gameStarted;
    private int _counter;
    private SpriteRenderer _spriteRenderer;
    private List<Tentacle> _tentacles;
    private TextMesh _textDistance;
    private TextMesh _textScore;
    private TextMesh _textTentacles;

    private void Start()
    {
        id = CellCounter++;
        _tentacles = new List<Tentacle>(0);
        tentaclesCount = 0;
        _counter = 0;
        score = owner == 0 ? ScoreToLvlGrayCells[lvl] : ScoreToLvl[lvl];
        tentaclesMax = lvl / 3 + 1;
        _textScore = textScoreObject.GetComponent<TextMesh>();
        _textTentacles = textTentaclesObject.GetComponent<TextMesh>();
        _textDistance = textDistanceObject.GetComponent<TextMesh>();
        textDistanceObject.GetComponent<MeshRenderer>().enabled = false;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        CircleActive(false);
        CheckOwner();
        var scoreString = score.ToString();
        _textScore.text = scoreString;
        string tentaclesString = tentaclesCount.ToString() + '/' + tentaclesMax;
        _textTentacles.text = tentaclesString;
        maxLvl = Math.Min(7, lvl + 2);
    }

    public void Update()
    {
        if (!gameStarted)
        {
            gameStarted = game.gameStarted;
            return;
        }

        if (owner == 0)
        {
            var scoreString = score.ToString();
            _textScore.text = scoreString;
        }
        else
        {
            _counter += lvl + 1;
            if (_counter >= CounterController)
            {
                if (score >= ScoreToLvl[maxLvl])
                {
                    _counter += maxLvl * (CounterController / 10);
                    score = ScoreToLvl[maxLvl];
                }
                else
                {
                    score += 1;
                }

                lvl = NewLvl();
                tentaclesMax = (lvl + 2) / 3;
                if (tentaclesCount == 0) _counter += CounterController / 2;
                foreach (Tentacle tentacle in _tentacles) tentacle.Attack();
                _counter %= CounterController;
            }

            var scoreString = score.ToString();
            _textScore.text = scoreString;
            string tentaclesString = tentaclesCount.ToString() + '/' + tentaclesMax;
            _textTentacles.text = tentaclesString;
        }
    }

    private void OnMouseExit()
    {
        textDistanceObject.GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnMouseOver()
    {
        int scoreTentacle = game.CellOverEvent(transform.position);
        if (scoreTentacle == 0)
        {
            textDistanceObject.GetComponent<MeshRenderer>().enabled = false;
        }
        else if (scoreTentacle < 0)
        {
            textDistanceObject.GetComponent<MeshRenderer>().enabled = true;
            _textDistance.color = Color.red;
            _textDistance.text = (-scoreTentacle).ToString();
        }
        else
        {
            textDistanceObject.GetComponent<MeshRenderer>().enabled = true;
            _textDistance.color = Color.green;
            _textDistance.text = scoreTentacle.ToString();
        }
    }

    private void OnMouseUpAsButton()
    {
        game.CellPressEvent(gameObject);
    }

    public void CheckOwner()
    {
        _spriteRenderer.sprite = owner switch
        {
            0 => graySprite,
            1 => greenSprite,
            2 => redSprite,
            _ => _spriteRenderer.sprite
        };
        foreach (Tentacle tentacle in _tentacles) tentacle.CheckOwner();
    }

    public int NewLvl()
    {
        int index = Array.BinarySearch(ScoreToLvl, score);
        if (index < 0)
            return ~index;
        return index;
    }

    public void AddTentacle(GameObject secondCell, bool isBilateral)
    {
        GameObject tentacle = Instantiate(tentaclePrefab, new Vector3(), Quaternion.identity);
        var lineRenderer = tentacle.GetComponent<LineRenderer>();
        var edgeCollider2D = tentacle.GetComponent<EdgeCollider2D>();
        Vector3 positionOfFirstCell = transform.position;
        Vector3 positionOfSecondCell = secondCell.transform.position;
        lineRenderer.SetPositions(PositionsOfTentacle(positionOfFirstCell, positionOfSecondCell));
        edgeCollider2D.SetPoints(new List<Vector2>
        {
            new Vector2(lineRenderer.GetPosition(0).x, lineRenderer.GetPosition(0).y),
            new Vector2(lineRenderer.GetPosition(1).x, lineRenderer.GetPosition(1).y)
        });

        var tentacleController = tentacle.GetComponent<Tentacle>();
        tentacleController.startCell = gameObject;
        tentacleController.endCell = secondCell;
        tentacleController.score = (int) (Vector3.Distance(positionOfFirstCell, positionOfSecondCell) * 10);
        if (isBilateral)
        {
            score += tentacleController.score / 2;
            tentacleController.DoBilateral();
        }
        else
        {
            tentacleController.DoUniliteral();
        }

        score -= tentacleController.score;
        tentaclesCount++;
        _tentacles.Add(tentacleController);
    }

    public Tentacle FindTentacleByEndId(int endId)
    {
        foreach (Tentacle tentacle in _tentacles)
            if (tentacle.endCellController.id == endId)
                return tentacle;

        return null;
    }

    public void CircleActive(bool active)
    {
        cellCircle.GetComponent<SpriteRenderer>().enabled = active;
    }

    public void DestroyTentacle(int idEnd)
    {
        foreach (Tentacle tentacle in _tentacles.Where(tentacle => tentacle.endCellController.id == idEnd))
        {
            _tentacles.Remove(tentacle);
            Destroy(tentacle.gameObject);
            tentaclesCount--;
            break;
        }
    }

    public void DestroyTentacleEvent(Tentacle tentacle)
    {
        game.DestroyTentacle(tentacle.startCellController.id, tentacle.endCellController.id);
    }

    private static Vector3[] PositionsOfTentacle(Vector3 cellBegin, Vector3 cellEnd)
    {
        var answer = new Vector3[2];
        double alpha = Math.Atan(((double) cellEnd.y - cellBegin.y) / (cellEnd.x - cellBegin.x));
        if (cellBegin.x <= cellEnd.x)
        {
            answer[0].x = (float) (cellBegin.x + Radius * Math.Cos(alpha));
            answer[0].y = (float) (cellBegin.y + Radius * Math.Sin(alpha));
            answer[1].x = (float) (cellEnd.x - Radius * Math.Cos(alpha));
            answer[1].y = (float) (cellEnd.y - Radius * Math.Sin(alpha));
        }
        else
        {
            answer[0].x = (float) (cellBegin.x - Radius * Math.Cos(alpha));
            answer[0].y = (float) (cellBegin.y - Radius * Math.Sin(alpha));
            answer[1].x = (float) (cellEnd.x + Radius * Math.Cos(alpha));
            answer[1].y = (float) (cellEnd.y + Radius * Math.Sin(alpha));
        }


        return answer;
    }

    public void Attack(int attacker, int damage)
    {
        if (attacker == owner)
        {
            if (score >= ScoreToLvl[maxLvl]) _counter += damage * CounterController / Mathf.Max(1, tentaclesCount);
            else score = Math.Min(1000, score + damage);
        }
        else
        {
            if (damage <= score)
            {
                score -= damage;
                if (owner != 0) lvl = NewLvl();
            }
            else
            {
                if (owner == 0)
                    score = Math.Min(1000, ScoreToLvl[lvl] + damage - score);
                else
                    score = damage - score;
                owner = attacker;
                CheckOwner();
            }
        }
    }
}