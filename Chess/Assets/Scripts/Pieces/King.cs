using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The king piece is responsible the king on the board in an octal direction fashion. 
/// It will also be responsible for castling.
/// </summary>
/// 
/// Author: Rozario (Ross) Beaudin 
/// 
/// Private Variables:
/// rooks           The rooks that we can castle with.
/// 
public class King : Pieces
{
    
    private GameObject [] rooks = new GameObject[2];

    /// <summary>
    /// Set up the Diagonal ray casts and the vertical and horizontal rays.
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
    /// Update the vertical, horizontal and diagonal rays. 
    /// While also detected for chess pieces nearby and possible targets.
    /// </summary>
    /// 
    /// 2020-10-30 RB Documentation
    /// 
    void FixedUpdate()
    {
        UpdateDiagonalRays();
        UpdateVertHoriRays();
        CaptureDectection();
        ObstacleDetection();
    }

    /// <summary>
    /// The method move is responsible for moving the King in one of 8 ways If the move is safe.
    /// </summary>
    /// <param name="tilePosition">The tile we want to move to.</param>
    /// <returns>True if successful move, false otherwise.</returns>
    /// 
    /// 2020-10-30 RB Documentation
    /// 
    public override bool Move(Vector3 tilePosition)
    {
        Vector3 originalPosition = transform.position;

        // right obstacle
        if (tilePosition.x == transform.position.x && tilePosition.z == (transform.position.z + 1))
        {
            //Debug.Log("got in 0 diagonal");
            transform.Translate(Vector3.Scale((Vector3.forward + Vector3.right), tilePosition - transform.position));
            firstMove = false;
            return true;
        }

        // right obstacle
        if (tilePosition.x == transform.position.x + 1 && tilePosition.z == transform.position.z)
        {
            //Debug.Log("got in 1 diagonal");
            transform.Translate(Vector3.Scale((Vector3.forward + Vector3.right), tilePosition - transform.position));
            firstMove = false;
            return true;
        }

        // bottom obstacle
        if (tilePosition.x == transform.position.x && tilePosition.z == transform.position.z - 1)
        {
            //Debug.Log("got in 2 diagonal");
            transform.Translate(Vector3.Scale((Vector3.forward + Vector3.right), tilePosition - transform.position));
            firstMove = false;
            return true;
        }

        // left obstacle
        if (tilePosition.x == transform.position.x - 1 && tilePosition.z == transform.position.z)
        {
            //Debug.Log("got in 3 diagonal");
            transform.Translate(Vector3.Scale((Vector3.forward + Vector3.right), tilePosition - transform.position));
            firstMove = false;
            return true;
        }


        // top right obstacle
        if (tilePosition.x == transform.position.x + 1 && tilePosition.z == transform.position.z + 1)
        {
            //Debug.Log("got in 4 diagonal");
            transform.Translate(Vector3.Scale((Vector3.forward + Vector3.right), tilePosition - transform.position));
            firstMove = false;
            return true;

        }

        // bottom right obstacle
        if (tilePosition.x == transform.position.x + 1 && tilePosition.z == transform.position.z - 1)
        {
            //Debug.Log("got in 5 diagonal");
            transform.Translate(Vector3.Scale((Vector3.forward + Vector3.right), tilePosition - transform.position));
            firstMove = false;
            return true;
        }

        // bottom left obstacle
        if (tilePosition.x == transform.position.x - 1 && tilePosition.z == transform.position.z - 1)
        {
            //Debug.Log("got in 6 diagonal");
            transform.Translate(Vector3.Scale((Vector3.forward + Vector3.right), tilePosition - transform.position));
            firstMove = false;
            return true;
        }

        // top left obstacle
        if (tilePosition.x == transform.position.x - 1 && tilePosition.z == transform.position.z + 1)
        {
            //Debug.Log("got in 7 diagonal");
            transform.Translate(Vector3.Scale((Vector3.forward + Vector3.right), tilePosition - transform.position));
            firstMove = false;
            return true;
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

        //Debug.DrawRay(transform.position, vertHoriRays[0].direction * 1f, Color.red);
        //Debug.DrawRay(transform.position, vertHoriRays[3].direction * 1f, Color.red);
        //Debug.DrawRay(transform.position, vertHoriRays[1].direction * 1f, Color.red);
        //Debug.DrawRay(transform.position, vertHoriRays[2].direction * 1f, Color.red);
        //Debug.DrawRay(transform.position, diagonalRays[0].direction * 1f, Color.red);
        //Debug.DrawRay(transform.position, diagonalRays[3].direction * 1f, Color.red);
        //Debug.DrawRay(transform.position, diagonalRays[1].direction * 1f, Color.red);
        //Debug.DrawRay(transform.position, diagonalRays[2].direction * 1f, Color.red);
        
        // top
        if (Physics.Raycast(vertHoriRays[0], out hit, 1f))
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
        if (Physics.Raycast(vertHoriRays[1], out hit, 1f))
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
        if (Physics.Raycast(vertHoriRays[2], out hit, 1f))
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
        if (Physics.Raycast(vertHoriRays[3], out hit, 1f))
        {

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Piece") && hit.collider.gameObject.GetComponent<Pieces>().color != color)
            {
                gO = hit.collider.gameObject;
                StoreTarget(gO);
                gO.GetComponent<Pieces>().SetEnemyCanCapture(true);
            }
        }


        // top right
        if (Physics.Raycast(diagonalRays[0], out hit, 1f))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Piece") && hit.collider.gameObject.GetComponent<Pieces>().color != color)
            {
                gO = hit.collider.gameObject;
                StoreTarget(gO);
                gO.GetComponent<Pieces>().SetEnemyCanCapture(true);
                Debug.Log("we in here gO = " + gO);
            }
        }

        // bottom right
        hit = new RaycastHit();
        if (Physics.Raycast(diagonalRays[1], out hit, 1f))
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
        if (Physics.Raycast(diagonalRays[2], out hit, 1f))
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
        if (Physics.Raycast(diagonalRays[3], out hit, 1f))
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
    /// Detects if the we can no longer move.
    /// </summary>
    /// <returns> true if in checkmate. False otherwise</returns>
    /// 
    /// 2020-10-30 RB Documentation
    /// 
    public bool CheckmateDetection()
    {
        if (!enemyCanCapture)
            return false;
        Vector3 originalPosition = transform.position;
        int numOfNonValidMoves = 0;


        if(numOfNonValidMoves == 8)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Allows the king to castle with the selected rook.
    /// </summary>
    /// <param name="rook">The rook we want to castle with.</param>
    /// 
    /// 2020-10-30 RB Documentation
    /// 
    public void Castling(GameObject rook)
    {
        if (!firstMove)
            return;
        if (!rook.GetComponent<Rook>().GetFirstMove())
            return;
        Vector3 originalPosition = transform.position;
        if(rooks[0] != null && rook.Equals(rooks[0]))
        {
            
            transform.Translate(Vector3.right * 2);
            if (enemyCanCapture)
            {
                transform.position = originalPosition;
                return;
            }
            rooks[0].transform.position = transform.position + Vector3.left;
        }
        else if(rooks[1] != null && rook.Equals(rooks[1]))
        {
            transform.Translate(Vector3.left * 2);
            if (enemyCanCapture)
            {
                transform.position = originalPosition;
                return;
            }
            rooks[1].transform.position = transform.position + Vector3.right;
        }
        firstMove = false;
    }

    /// <summary>
    /// Detects only left and right from the king for possible rooks we want to castle with.
    /// </summary>
    /// 
    /// 2020-10-30 RB Documentation
    /// 
    public override void ObstacleDetection()
    {
        RaycastHit hit = new RaycastHit();
        GameObject gO;
        
        
        if (Physics.Raycast(vertHoriRays[1], out hit, 15f))
        {
            gO = hit.collider.gameObject;
            if (gO.layer == LayerMask.NameToLayer("Piece") && gO.GetComponent<Rook>() != null)
            {
                rooks[0] = gO;
            }
            else
            {
                rooks[0] = null;
            }
        }


        hit = new RaycastHit();
        if (Physics.Raycast(vertHoriRays[3], out hit, 15f))
        {
            gO = hit.collider.gameObject;
            if (gO.layer == LayerMask.NameToLayer("Piece") && gO.GetComponent<Rook>() != null)
            {
                rooks[1] = gO;
            }
            else
            {
                rooks[1] = null;
            }
        }

    }
}
