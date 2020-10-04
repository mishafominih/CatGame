using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public float speed = 1;

    private MovePlayer mP;
    private Vector2 PlayerPos;
    private Timer timer;
    void Start()
    {
        mP = GameObject.FindGameObjectWithTag("Player").GetComponent<MovePlayer>();
        timer = new Timer(speed);
    }

    void Update()
    {
        timer.Check();
        if (!mP.Move)
        {
            PlayerPos = mP.gameObject.transform.position;
        }
        else timer.Null();
        var dX = transform.position.x + (PlayerPos.x - transform.position.x) * (timer.GetTime() / speed);
        var dY = transform.position.y + (PlayerPos.y - transform.position.y) * (timer.GetTime() / speed);
        transform.position = new Vector3(dX, dY, -10);
    }
}
