using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    public GameObject Map;
    public float Speed;
    public float SpeedMove;
    public bool Move = false;

    private Camera cam;
    private List<BoxCollider2D> colliders;
    private List<Vector2> path;
    private Vector2 previousPosition;
    private BoxCollider2D collider2D;
    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        colliders = Map.GetComponents<BoxCollider2D>().ToList();
        SpeedMove = 1 / SpeedMove;
        previousPosition = transform.position;
        collider2D = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            var t = Input.GetTouch(0);
            t.position = cam.ScreenToWorldPoint(t.position);
            path = GetPath(t);
        }
        if (path != null && path.Count != 0)
        {
            Move = true;
            var pos = path[0];
            var dX = transform.position.x - (previousPosition.x - pos.x) / SpeedMove;
            var dY = transform.position.y - (previousPosition.y - pos.y) / SpeedMove;
            transform.position = new Vector3(dX, dY, 0);
            if (isTartget(
                Vector2.Distance(previousPosition, pos) / (SpeedMove / 5),
                transform.position, pos))
            {
                previousPosition = transform.position;
                path.RemoveAt(0);
            }
        }
        else
        {
            Move = false;
        }
    }

    private List<Vector2> GetPath(Touch touch)
    {
        var dict = new Dictionary<Vector2, Vector2>();
        var oldPath = new HashSet<Vector2>();
        var nextPath = new Queue<Vector2>();
        if(!IsPosOnCollider(touch.position))
        {
            oldPath.Add(transform.position);
            nextPath.Enqueue(transform.position);
            dict[transform.position] = new Vector2(-1000, +1000);
        }

        while (nextPath.Count != 0)
        {
            var point = nextPath.Dequeue();
            var points = GetPoints(Speed, point)
                .Where(x => !IsPosOnCollider(x) && !oldPath.Contains(x))
                .ToList();
            foreach(var p in points)
            {
                nextPath.Enqueue(p);
                oldPath.Add(p);
                dict[p] = point;
                if (isTartget(Speed, p, touch.position))
                {
                    dict[touch.position] = p;
                    return getResult(dict, touch);
                }
            }
        }
        return null;
    }

    private bool isTartget(float distance, Vector2 pos, Vector2 target)
    {
        return Vector2.Distance(pos, target) <= distance;
    }

    private bool IsPosOnCollider(Vector2 pos)
    {
        var arr = new Vector2[] { pos, pos + (Vector2)collider2D.bounds.extents, pos - (Vector2)collider2D.bounds.extents };
        foreach(var col in colliders)
        {
            foreach (var a in arr)
            {
                if (col.bounds.Contains(a))
                {
                    return true;
                }
            }
            //if (col.bounds.Contains(pos))
            //{
            //    return true;
            //}
        }
        return false;
    }

    private IEnumerable<Vector2> GetPoints(float d, Vector2 pos)
    {
        yield return new Vector2(pos.x + d, pos.y);
        yield return new Vector2(pos.x, pos.y + d);
        yield return new Vector2(pos.x - d, pos.y);
        yield return new Vector2(pos.x, pos.y - d);
        yield return new Vector2(pos.x + GetDiagonalDistanse(d), pos.y + GetDiagonalDistanse(d));
        yield return new Vector2(pos.x - GetDiagonalDistanse(d), pos.y + GetDiagonalDistanse(d));
        yield return new Vector2(pos.x + GetDiagonalDistanse(d), pos.y - GetDiagonalDistanse(d));
        yield return new Vector2(pos.x - GetDiagonalDistanse(d), pos.y - GetDiagonalDistanse(d));
    }

    private float GetDiagonalDistanse(float d) => d / Mathf.Sqrt(2);

    private List<Vector2> getResult(Dictionary<Vector2, Vector2> dict, Touch touch)
    {
        var res = new List<Vector2>();
        res.Add(touch.position);
        var p = dict[touch.position];
        while(p.x != -1000 && p.y != +1000)
        {
            res.Add(p);
            p = dict[p];
        }
        res.Reverse();
        res.RemoveAt(0);
        return res;
    }
}
