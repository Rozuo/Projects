using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The pawn is responsible for moving the pawn pieces and capturing pieces in a diagonal and capture other pawns via En Passent
/// </summary>
/// 
/// Author: Rozario (Ross) Beaudin
/// 
/// Private Variables:
/// usedDoubleMove      if the pawn utilized the double move on they're first turn.
/// 
public class Pawn : Pieces
{

    private bool usedDoubleMove = false;

    /// <summary>
    /// Initialize the targets, set up the box collider, make sure the first move is true, 
    /// Set up diagonal, vertical and horizontal rays and initialize all targets we can capture.
    /// </summary>
    /// 
    /// 2020-10-30 RB Documentation
    /// 
    void Start()
    {
        targets = new GameObject[3];
        SetBoxCol(GetComponent<BoxCollider>());
        firstMove = true;
        SetUpDiagonalRays();
        SetUpVertHoriRays();
        CaptureDectection();
    }

    /// <summary>
    /// Update the vertical, horizontal and diagonal rays. 
    /// While also detected for chess pieces that are possible targets.
    /// </summary>
    /// 
    /// 2020-10-30 RB Documentation
    /// 
    void FixedUpdate()
    {
        UpdateDiagonalRays();
        UpdateVertHoriRays();
        CaptureDectection();
    }

    /// <summary>
    /// The method move is responsible for moving the pawn in one way forward. 
    /// While determining the correct way to move.
    /// </summary>
    /// <param name="tilePosition">The tile we want to move to.</param>
    /// <returns>True if successful move, false otherwise.</returns>
    /// 
    /// 2020-10-30 RB Documentation
    /// 
    public override bool Move(Vector3 tilePosition)
    {
        switch (color)
        {
            case (TheColor.WHITE):
                if (tilePosition.x == transform.position.x)
                {
                    if (firstMove && tilePosition.z == transform.position.z + 2)
                    {
                        transform.Translate(0, 0, tilePosition.z - transform.position.z);
                        usedDoubleMove = true;
                    }
                    else if (tilePosition.z == transform.position.z + 1)
                    {
                        transform.Translate(0, 0, tilePosition.z - transform.position.z);
                        usedDoubleMove = false;
                    }
                    return true;
                }
                else if(tilePosition.z == transform.position.z + 1 && tilePosition.x == transform.position.x + 1 && EnPassant(true))
                {
                    
                    transform.Translate(tilePosition.x - transform.position.x, 0, tilePosition.z - transform.position.z);
                    usedDoubleMove = false;
                    firstMove = false;
                    return true;
                }
                else if (tilePosition.z == transform.position.z + 1 && tilePosition.x == transform.position.x - 1 && EnPassant(false))
                {
                    
                    transform.Translate(tilePosition.x - transform.position.x, 0, tilePosition.z - transform.position.z);
                    usedDoubleMove = false;
                    return true;
                }

                break;

            case (TheColor.DARK):
                if (tilePosition.x == transform.position.x)
                {
                    if (firstMove && tilePosition.z == transform.position.z - 2)
                    {
                        transform.Translate(0, 0, (tilePosition.z - transform.position.z));
                        usedDoubleMove = true;
                    }
                    else if (tilePosition.z == transform.position.z - 1)
                    {
                        transform.Translate(0, 0, (tilePosition.z - transform.position.z));
                        usedDoubleMove = false;
                    }

                    firstMove = false;
                    return true;
                }
                else if (tilePosition.z == transform.position.z - 1 && tilePosition.x == transform.position.x + 1 && EnPassant(true))
                {
                    transform.Translate(tilePosition.x - transform.position.x, 0, tilePosition.z - transform.position.z);
                    usedDoubleMove = false;
                    firstMove = false;
                    return true;
                }
                else if (tilePosition.z == transform.position.z - 1 && tilePosition.x == transform.position.x - 1 && EnPassant(false))
                {
                    transform.Translate(tilePosition.x - transform.position.x, 0, tilePosition.z - transform.position.z);
                    usedDoubleMove = false;
                    return true;
                }
                break;
        }
        Debug.Log(tilePosition);

        return false;
    }

    /// <summary>
    /// Detects all pieces that are not the same color and designates them as targets in 2 directions.
    /// </summary>
    /// <returns>True if we have a target. False otherwise.</returns>
    /// 
    /// 2020-10-30 RB Documentation
    /// 
    public override bool CaptureDectection()
    {
        RaycastHit hit = new RaycastHit();
        GameObject gO;
        targets = new GameObject[3];
        switch (color)
        {
            case (TheColor.WHITE):
                if (Physics.Raycast(diagonalRays[0], out hit, 1f))
                {
                    //Debug.Log("In attack range = " + hit.collider.gameObject.name);
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Piece") && hit.collider.gameObject.GetComponent<Pieces>().color != color)
                    {
                        gO = hit.collider.gameObject;
                        StoreTarget(gO);
                        gO.GetComponent<Pieces>().SetEnemyCanCapture(true);
                        //Debug.Log("we in here gO = " + gO);
                    }
                }
                hit = new RaycastHit();
                if (Physics.Raycast(diagonalRays[3], out hit, 1f))
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
                //foreach(GameObject gameObj in targets)
                //{
                //    if(gameObj != null)
                //    {
                //        return true;
                //    }
                //}
                break;
            case (TheColor.DARK):
                //Debug.DrawRay(transform.position, diagonalRays[0].direction + Vector3.forward + Vector3.right, Color.red);
                //Debug.DrawRay(transform.position, diagonalRays[3].direction + Vector3.forward + Vector3.left, Color.red);
                //Debug.DrawRay(transform.position, diagonalRays[1].direction, Color.red);
                //Debug.DrawRay(transform.position, diagonalRays[2].direction, Color.red);
                //Debug.Log(diagonalRays[0]);
                if (Physics.Raycast(diagonalRays[1], out hit, 1f))
                {
                    //Debug.Log("In attack range = " + hit.collider.gameObject.name);
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Piece") && hit.collider.gameObject.GetComponent<Pieces>().color != color)
                    {
                        gO = hit.collider.gameObject;
                        StoreTarget(gO);
                        gO.GetComponent<Pieces>().SetEnemyCanCapture(true);
                        //Debug.Log("we in here gO = " + gO);
                    }
                }
                hit = new RaycastHit();
                if (Physics.Raycast(diagonalRays[2], out hit, 1f))
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

                break;
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
    /// Detects if there is a pawn on the left or the right. If so we perform En Passent and capture the piece.
    /// </summary>
    /// <param name="rightDirection">Determines if we want to perform en passent on the right or the left.</param>
    /// <returns>True if successful. False otherwise.</returns>
    /// 
    /// 2020-10-30 RB Documentation
    /// 
    private bool EnPassant(bool rightDirection)
    {
        RaycastHit hit = new RaycastHit();
        GameObject gO;
        // right detection
        if (rightDirection && Physics.Raycast(vertHoriRays[1], out hit, 1f))
        {
            if (hit.collider.gameObject.GetComponent<Pawn>() != null && hit.collider.gameObject.GetComponent<Pawn>().color != color && hit.collider.gameObject.GetComponent<Pawn>().usedDoubleMove)
            {
                //Debug.DrawLine(transform.position, hit.point, Color.red);
                gO = hit.collider.gameObject;
                Destroy(gO);
                //Debug.Log("we in here gO = " + gO);
                return true;
            }
        }
        else if (!rightDirection && Physics.Raycast(vertHoriRays[3], out hit, 1f))
        {
            if (hit.collider.gameObject.GetComponent<Pawn>() != null && hit.collider.gameObject.GetComponent<Pawn>().color != color && hit.collider.gameObject.GetComponent<Pawn>().usedDoubleMove)
            {
                //Debug.DrawLine(transform.position, hit.point, Color.red);
                gO = hit.collider.gameObject;
                Destroy(gO);
                //Debug.Log("we in here gO = " + gO);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Get the variable usedDoubleMove.
    /// </summary>
    /// <returns>usedDoubleMove</returns>
    /// 
    /// 2020-10-30 RB Documentation
    /// 
    public bool GetUsedDoubleMove()
    {
        return usedDoubleMove;
    }

    /// <summary>
    /// Set the variable usedDoubleMove
    /// </summary>
    /// <param name="usedDoubleMove">What we want to set used double move as.</param>
    /// 
    /// 2020-10-30 RB Documetation
    /// 
    public void SetUsedDoubleMove(bool usedDoubleMove)
    {
        this.usedDoubleMove = usedDoubleMove;
    }
}
