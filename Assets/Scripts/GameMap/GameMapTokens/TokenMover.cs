using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenMover
{
    public GameMapTile startTile = null;
    public GameMapTile endTile = null;

    public Vector3 startPosition = Vector3.zero;
    public Vector3 endPosition = Vector3.zero;

    public delegate Vector3 CurveEvaluationAction(Vector3 start, Vector3 end, float t);
    CurveEvaluationAction m_curveEvaluationDelegate = Vector3.Lerp;

    public void SetTiles(GameMapTile start, GameMapTile end)
    {
        startTile = start;
        endTile = end;

        SetPositions(start.position, end.position);
    }

    void SetPositions(Vector3 start, Vector3 end)
    {
        startPosition = start;
        endPosition = end;
    }

    public void SetEvaluator(CurveEvaluationAction curveEvaluationAction)
    {
        m_curveEvaluationDelegate = curveEvaluationAction;
    }

    // Will return a position based on the evaluation method between the start and end.
    // Defaults to a Lerp.
    public Vector3 Evaluate(float t)
    {
        return m_curveEvaluationDelegate.Invoke(startPosition, endPosition, t);
    }
}
