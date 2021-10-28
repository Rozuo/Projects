using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The rook class is an extension of the piece. 
/// This chess piece will only move forward, right, backward and left until the end of 
/// the board or the space before an ally piece.
/// </summary>
/// 
/// Author: Rozario (Ross) Beaudin
/// 
public class Rook : Pieces
{

    /// <summary>
    /// Initialize the targets and obstacles while seting up the vertHoriRays()
    /// </summary>
    /// 
    /// 2020-10-30 RB Documentation
    /// 
    void Start()
    {
        targets = new GameObject[4];
        obstacles = new Transform[4];
        SetUpVertHoriRays();
    }

    /// <summary>
    /// Updates every fixed frame the ver hori 
    /// rays and the obstacles/targets that are present.
    /// </summary>
    /// 
    /// 2020-10-30 RB  Documentation
    /// 
    void FixedUpdate()
    {
        UpdateVertHoriRays();
        ObstacleDetection();
        CaptureDectection();
    }

    /// <summary>
    /// The method move is responsible for moving the rook in a straight line. 
    /// If the move has no obstacles in the way.
    /// </summary>
    /// <param name="tilePosition">The tile we want to move to.</param>
    /// <returns>True if successful move, false otherwise.</returns>
    /// 
    /// 2020-10-30 RB Documentation
    /// 
    public override bool Move(Vector3 tilePosition)
    {
        // right obstacle
        if (obstacles[0] == null || tilePosition.x == obstacles[0].position.x && tilePosition.z < obstacles[0].position.z)
        {
            Debug.Log("got in 0 diagonal");
            Debug.Log(obstacles[0]);
            if (tilePosition.x - transform.position.x == 0 && tilePosition.z - transform.position.z > 0)
            {
                transform.Translate(Vector3.Scale((Vector3.forward + Vector3.right), tilePosition - transform.position));
                return true;
            }
        }

        // right obstacle
        if (obstacles[1] == null || tilePosition.x < obstacles[1].position.x && tilePosition.z == obstacles[1].position.z)
        {
            Debug.Log("got in 1 diagonal");
            if (tilePosition.x - transform.position.x > 0 && tilePosition.z - transform.position.z == 0)
            {
                transform.Translate(Vector3.Scale((Vector3.forward + Vector3.right), tilePosition - transform.position));
                return true;
            }
        }

        // bottom obstacle
        if (obstacles[2] == null || tilePosition.x == obstacles[2].position.x && tilePosition.z > obstacles[2].position.z)
        {
            Debug.Log("got in 2 diagonal");
            if (tilePosition.x - transform.position.x == 0 && tilePosition.z - transform.position.z < 0)
            {
                transform.Translate(Vector3.Scale((Vector3.forward + Vector3.right), tilePosition - transform.position));
                return true;
            }
        }

        // left obstacle
        if (obstacles[3] == null || tilePosition.x > obstacles[3].position.x && tilePosition.z == obstacles[3].position.z)
        {
            Debug.Log("got in 3 diagonal");
            if (tilePosition.x - transform.position.x < 0 && tilePosition.z - transform.position.z == 0)
            {
                transform.Translate(Vector3.Scale((Vector3.forward + Vector3.right), tilePosition - transform.position));
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Detects all pieces in that are horizontally or vertically placed from the rook.
    /// that are not the same color and designates them as targets.
    /// </summary>
    /// <returns>True if we have a target. False otherwise.</returns>
    /// 
    /// 2020-10-30 RB Documentation
    /// 
    public override bool CaptureDectection()
    {
        RaycastHit hit = new RaycastHit();
        GameObject gO;
        targets = new GameObject[4];

        if (Physics.Raycast(vertHoriRays[0], out hit, 15f))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Piece") && hit.collider.gameObject.GetComponent<Pieces>().color != color)
            {
                gO = hit.collider.gameObject;
                StoreTarget(gO);
                gO.GetComponent<Pieces>().SetEnemyCanCapture(true);
                //Debug.Log("we in here gO = " + gO);
            }
        }

        hit = new RaycastHit();
        if (Physics.Raycast(vertHoriRays[3], out hit, 15f))
        {

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Piece") && hit.collider.gameObject.GetComponent<Pieces>().color != color)
            {
                //Debug.DrawLine(transform.position, hit.point, Color.red);
                gO = hit.collider.gameObject;
                StoreTarget(gO);
                gO.GetComponent<Pieces>().SetEnemyCanCapture(true);
                //Debug.Log("we in here gO = " + gO);
            }
        }

        hit = new RaycastHit();
        if (Physics.Raycast(vertHoriRays[1], out hit, 15f))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Piece") && hit.collider.gameObject.GetComponent<Pieces>().color != color)
            {
                //Debug.Log("In attack range = " + hit.collider.gameObject.name);
                gO = hit.collider.gameObject;
                StoreTarget(gO);
                gO.GetComponent<Pieces>().SetEnemyCanCapture(true);
                //Debug.Log("we in here gO = " + gO);
            }
        }

        hit = new RaycastHit();
        if (Physics.Raycast(vertHoriRays[2], out hit, 15f))
        {
            
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Piece") && hit.collider.gameObject.GetComponent<Pieces>().color != color)
            {
                //Debug.DrawLine(transform.position, hit.point, Color.red);
                gO = hit.collider.gameObject;
                StoreTarget(gO);
                gO.GetComponent<Pieces>().SetEnemyCanCapture(true);
                //Debug.Log("we in here gO = " + gO);
            }
        }

        foreach (GameObject gameObj in targets)
        {
            if (gameObj != null)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// We detect all chess pieces in 4 straight lines from the rooks current transform.
    /// Position and stores them in the obstacles array.
    /// </summary>
    /// 
    /// 2020-10-30 RB Documentation
    /// 
    public override void ObstacleDetection()
    {
        RaycastHit hit = new RaycastHit();

        //Debug.DrawRay(transform.position, vertHoriRays[0].direction * 15f, Color.red);
        //Debug.DrawRay(transform.position, vertHoriRays[3].direction * 15f, Color.red);
        //Debug.DrawRay(transform.position, vertHoriRays[1].direction * 15f, Color.red);
        //Debug.DrawRay(transform.position, vertHoriRays[2].direction * 15f, Color.red);

        // top
        if (Physics.Raycast(vertHoriRays[0], out hit, 15f) && hit.collider.gameObject.layer == LayerMask.NameToLayer("Piece"))
        {
            obstacles[0] = hit.collider.gameObject.transform;
        }
        else
        {
            obstacles[0] = null;
        }

        // left
        hit = new RaycastHit();
        if (Physics.Raycast(vertHoriRays[3], out hit, 15f) && hit.collider.gameObject.layer == LayerMask.NameToLayer("Piece"))
        {
            obstacles[3] = hit.collider.gameObject.transform;
        }
        else
        {
            obstacles[3] = null;
        }

        // right
        hit = new RaycastHit();
        if (Physics.Raycast(vertHoriRays[1], out hit, 15f) && hit.collider.gameObject.layer == LayerMask.NameToLayer("Piece"))
        {
            obstacles[1] = hit.collider.gameObject.transform;
        }
        else
        {
            obstacles[1] = null;
        }

        // bottom
        hit = new RaycastHit();
        if (Physics.Raycast(vertHoriRays[2], out hit, 15f) && hit.collider.gameObject.layer == LayerMask.NameToLayer("Piece"))
        {
            obstacles[2] = hit.collider.gameObject.transform;
        }
        else
        {
            obstacles[2] = null;
        }
    }
}
