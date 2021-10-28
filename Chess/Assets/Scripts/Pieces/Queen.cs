using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Queen piece is a motor controller for all queen chess pieces on the board. 
/// Handling capturing and movement that the queen can perform.
/// </summary>
/// 
/// Author: Rozario (Ross) Beaudin
/// 
public class Queen : Pieces
{

    /// <summary>
    /// Initialize the horizontal, vertical and diagonal ways we can move.
    /// </summary>
    /// 
    /// 2020-10-30 RB Documentation
    /// 
    void Start()
    {
        SetUpDiagonalRays();
        SetUpVertHoriRays();
    }

    /// <summary>
    /// Update all diagonal, vertical and horizontal rays 
    /// while updating the possible targets and chess pieces nearby.
    /// </summary>
    /// 
    /// 2020-10-30 RB Documentation
    /// 
    void FixedUpdate()
    {
        UpdateDiagonalRays();
        UpdateVertHoriRays();
        ObstacleDetection();
        CaptureDectection();
    }

    /// <summary>
    /// The method move is responsible for moving the Queen in one of 8 ways 
    /// if the move has no obstacles in the way.
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
            //Debug.Log("got in 0 diagonal");
            //Debug.Log(obstacles[0]);
            if (tilePosition.x - transform.position.x == 0 && tilePosition.z - transform.position.z > 0)
            {
                transform.Translate(Vector3.Scale((Vector3.forward + Vector3.right), tilePosition - transform.position));
                return true;
            }
        }

        // right obstacle
        if (obstacles[1] == null || tilePosition.x < obstacles[1].position.x && tilePosition.z == obstacles[1].position.z)
        {
            //Debug.Log("got in 1 diagonal");
            if (tilePosition.x - transform.position.x > 0 && tilePosition.z - transform.position.z == 0)
            {
                transform.Translate(Vector3.Scale((Vector3.forward + Vector3.right), tilePosition - transform.position));
                return true;
            }
        }

        // bottom obstacle
        if (obstacles[2] == null || tilePosition.x == obstacles[2].position.x && tilePosition.z > obstacles[2].position.z)
        {
            //Debug.Log("got in 2 diagonal");
            if (tilePosition.x - transform.position.x == 0 && tilePosition.z - transform.position.z < 0)
            {
                transform.Translate(Vector3.Scale((Vector3.forward + Vector3.right), tilePosition - transform.position));
                return true;
            }
        }

        // left obstacle
        if (obstacles[3] == null || tilePosition.x > obstacles[3].position.x && tilePosition.z == obstacles[3].position.z)
        {
            //Debug.Log("got in 3 diagonal");
            if (tilePosition.x - transform.position.x < 0 && tilePosition.z - transform.position.z == 0)
            {
                transform.Translate(Vector3.Scale((Vector3.forward + Vector3.right), tilePosition - transform.position));
                return true;
            }
        }


        // top right obstacle
        if (obstacles[4] == null || tilePosition.x < obstacles[4].position.x && tilePosition.z < obstacles[4].position.z)
        {
            //Debug.Log("got in 4 diagonal");
            for (float i = transform.position.x, j = transform.position.z; i < 8 || j < 8; i++, j++)
            {
                if(tilePosition.x == i && tilePosition.z == j)
                {
                    transform.Translate(Vector3.Scale((Vector3.forward + Vector3.right), tilePosition - transform.position));
                    return true;
                }
            }
        }

        // bottom right obstacle
        if (obstacles[5] == null || tilePosition.x < obstacles[5].position.x && tilePosition.z > obstacles[5].position.z)
        {
            //Debug.Log("got in 5 diagonal");
            if ((tilePosition.x + tilePosition.z == transform.position.x + transform.position.z) && 
                (tilePosition.x - transform.position.x > 0 && tilePosition.z - transform.position.z < 0))
            {
                transform.Translate(Vector3.Scale((Vector3.forward + Vector3.right), tilePosition - transform.position));
                return true;
            }
        }

        // bottom left obstacle
        if (obstacles[6] == null || tilePosition.x > obstacles[6].position.x && tilePosition.z > obstacles[6].position.z)
        {
            //Debug.Log("got in 6 diagonal" + obstacles[6].ToString());
            for (float i = transform.position.x, j = transform.position.z; i >= 0 || j >= 0; i--, j--)
            {
                if (tilePosition.x == i && tilePosition.z == j)
                {
                    transform.Translate(Vector3.Scale((Vector3.forward + Vector3.right), tilePosition - transform.position));
                    return true;
                }
            }
        }

        // top left obstacle
        if (obstacles[7] == null || tilePosition.x > obstacles[7].position.x && tilePosition.z < obstacles[7].position.z)
        {
            //Debug.Log("got in 7 diagonal");
            if ((tilePosition.x + tilePosition.z == transform.position.x + transform.position.z) && 
                (tilePosition.x - transform.position.x < 0 && tilePosition.z - transform.position.z > 0))
            {
                transform.Translate(Vector3.Scale((Vector3.forward + Vector3.right), tilePosition - transform.position));
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Detects all pieces that are not the same color and designates them as targets in all 8 directions.
    /// </summary>
    /// <returns>True if we have a target. False otherwise.</returns>
    /// 
    /// 2020-10-30 RB Documentation
    /// 
    public override bool CaptureDectection()
    {
        RaycastHit hit = new RaycastHit();
        GameObject gO;
        targets = new GameObject[8];

        // top
        if (Physics.Raycast(vertHoriRays[0], out hit, 15f))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Piece") && hit.collider.gameObject.GetComponent<Pieces>().color != color)
            {
                gO = hit.collider.gameObject;
                StoreTarget(gO);
                gO.GetComponent<Pieces>().SetEnemyCanCapture(true);
            }
        }
        
        // right
        hit = new RaycastHit();
        if (Physics.Raycast(vertHoriRays[1], out hit, 15f))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Piece") && hit.collider.gameObject.GetComponent<Pieces>().color != color)
            {
                gO = hit.collider.gameObject;
                StoreTarget(gO);
                gO.GetComponent<Pieces>().SetEnemyCanCapture(true);
            }
        }
        
        // bottom
        hit = new RaycastHit();
        if (Physics.Raycast(vertHoriRays[2], out hit, 15f))
        {

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Piece") && hit.collider.gameObject.GetComponent<Pieces>().color != color)
            {
                gO = hit.collider.gameObject;
                StoreTarget(gO);
                gO.GetComponent<Pieces>().SetEnemyCanCapture(true);
            }
        }

        // left
        hit = new RaycastHit();
        if (Physics.Raycast(vertHoriRays[3], out hit, 15f))
        {

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Piece") && hit.collider.gameObject.GetComponent<Pieces>().color != color)
            {
                gO = hit.collider.gameObject;
                StoreTarget(gO);
                gO.GetComponent<Pieces>().SetEnemyCanCapture(true);
            }
        }


        // top right
        if (Physics.Raycast(diagonalRays[0], out hit, 15f))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Piece") && hit.collider.gameObject.GetComponent<Pieces>().color != color)
            {
                gO = hit.collider.gameObject;
                StoreTarget(gO);
                gO.GetComponent<Pieces>().SetEnemyCanCapture(true);
            }
        }
        
        // bottom right
        hit = new RaycastHit();
        if (Physics.Raycast(diagonalRays[1], out hit, 15f))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Piece") && hit.collider.gameObject.GetComponent<Pieces>().color != color)
            {
                gO = hit.collider.gameObject;
                StoreTarget(gO);
                gO.GetComponent<Pieces>().SetEnemyCanCapture(true);
            }
        }

        // bottom left
        hit = new RaycastHit();
        if (Physics.Raycast(diagonalRays[2], out hit, 15f))
        {

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Piece") && hit.collider.gameObject.GetComponent<Pieces>().color != color)
            {
                gO = hit.collider.gameObject;
                StoreTarget(gO);
                gO.GetComponent<Pieces>().SetEnemyCanCapture(true);
            }
        }

        // top left
        hit = new RaycastHit();
        if (Physics.Raycast(diagonalRays[3], out hit, 15f))
        {

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Piece") && hit.collider.gameObject.GetComponent<Pieces>().color != color)
            {
                gO = hit.collider.gameObject;
                StoreTarget(gO);
                gO.GetComponent<Pieces>().SetEnemyCanCapture(true);
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
    /// We detect all chess pieces in 8 straight lines from the queen's current 
    /// transform.Position and stores them in the obstacles array.
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
        //Debug.DrawRay(transform.position, diagonalRays[0].direction * 15f, Color.red);
        //Debug.DrawRay(transform.position, diagonalRays[3].direction * 15f, Color.red);
        //Debug.DrawRay(transform.position, diagonalRays[1].direction * 15f, Color.red);
        //Debug.DrawRay(transform.position, diagonalRays[2].direction * 15f, Color.red);

        // top
        if (Physics.Raycast(vertHoriRays[0], out hit, 15f) && hit.collider.gameObject.layer == LayerMask.NameToLayer("Piece"))
        {
            obstacles[0] = hit.collider.gameObject.transform;
        }
        else
        {
            obstacles[0] = null;
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

        

        hit = new RaycastHit();
        // top right
        if (Physics.Raycast(diagonalRays[0], out hit, 15f) && hit.collider.gameObject.layer == LayerMask.NameToLayer("Piece"))
        {
            obstacles[4] = hit.collider.gameObject.transform;
        }
        else
        {
            obstacles[4] = null;
        }

        // bottom right
        hit = new RaycastHit();
        if (Physics.Raycast(diagonalRays[1], out hit, 15f) && hit.collider.gameObject.layer == LayerMask.NameToLayer("Piece"))
        {
            obstacles[5] = hit.collider.gameObject.transform;
        }
        else
        {
            obstacles[5] = null;
        }

        // bottom left
        hit = new RaycastHit();
        if (Physics.Raycast(diagonalRays[2], out hit, 15f) && hit.collider.gameObject.layer == LayerMask.NameToLayer("Piece"))
        {
            obstacles[6] = hit.collider.gameObject.transform;
        }
        else
        {
            obstacles[6] = null;
        }
        
        // top left
        hit = new RaycastHit();
        if (Physics.Raycast(diagonalRays[3], out hit, 15f) && hit.collider.gameObject.layer == LayerMask.NameToLayer("Piece"))
        {
            obstacles[7] = hit.collider.gameObject.transform;
        }
        else
        {
            obstacles[7] = null;
        }
    }
}
