using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The knight class is responsible for moving and capture pieces with the knight piece in an L direction.
/// </summary>
/// 
/// Author: Rozario (Ross) Beaudin (RB)
/// 
/// 
public class Knight : Pieces
{

    /// <summary>
    /// Update all possible targets every fixed frame
    /// </summary>
    /// 
    /// 2020-10-30 RB Documentation
    /// 
    void FixedUpdate()
    {

        CaptureDectection();
    }

    /// <summary>
    /// Move the Knight in an L direction if it is not occupied.
    /// </summary>
    /// <param name="tilePosition">The tile we want to move to.</param>
    /// <returns>True if it is a successful move. False otherwise.</returns>
    /// 
    /// 2020-10-30 RB Documentation
    /// 
    public override bool Move(Vector3 tilePosition)
    {
        if (tilePosition.x == transform.position.x + 1 && tilePosition.z == transform.position.z + 2)
        {
            transform.Translate(Vector3.Scale((Vector3.forward + Vector3.right), tilePosition - transform.position));
            return true;
        }
        else if (tilePosition.x == transform.position.x + 2 && tilePosition.z == transform.position.z + 1)
        {
            transform.Translate(Vector3.Scale((Vector3.forward + Vector3.right), tilePosition - transform.position));
            return true;
        }
        else if (tilePosition.x == transform.position.x + 2 && tilePosition.z == transform.position.z - 1)
        {
            transform.Translate(Vector3.Scale((Vector3.forward + Vector3.right), tilePosition - transform.position));
            return true;
        }
        else if (tilePosition.x == transform.position.x + 1 && tilePosition.z == transform.position.z - 2)
        {
            transform.Translate(Vector3.Scale((Vector3.forward + Vector3.right), tilePosition - transform.position));
            return true;
        }
        else if (tilePosition.x == transform.position.x - 1 && tilePosition.z == transform.position.z - 2)
        {
            transform.Translate(Vector3.Scale((Vector3.forward + Vector3.right), tilePosition - transform.position));
            return true;
        }
        else if (tilePosition.x == transform.position.x - 2 && tilePosition.z == transform.position.z - 1)
        {
            transform.Translate(Vector3.Scale((Vector3.forward + Vector3.right), tilePosition - transform.position));
            return true;
        }
        else if (tilePosition.x == transform.position.x - 2 && tilePosition.z == transform.position.z + 1)
        {
            transform.Translate(Vector3.Scale((Vector3.forward + Vector3.right), tilePosition - transform.position));
            return true;
        }
        else if (tilePosition.x == transform.position.x - 1 && tilePosition.z == transform.position.z + 2)
        {
            transform.Translate(Vector3.Scale((Vector3.forward + Vector3.right), tilePosition - transform.position));
            return true;
        }

        return false;
    }

    /// <summary>
    /// Check if any L position are taken by a piece and store them as a target if they are not the same color.
    /// </summary>
    /// <returns>True if we have a target. False otherwise.</returns>
    /// 
    /// 2020-10-30 RB Documentation
    /// 
    public override bool CaptureDectection()
    {
        GameObject gO;
        targets = new GameObject[8];

        RaycastHit hit = new RaycastHit();
        if(Physics.BoxCast(transform.position + Vector3.right + Vector3.forward, transform.localScale/4, Vector3.forward, out hit))
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
        if (Physics.BoxCast(transform.position + Vector3.right + Vector3.forward, transform.localScale / 4, Vector3.right, out hit))
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
        if (Physics.BoxCast(transform.position + Vector3.right + Vector3.back, transform.localScale / 4, Vector3.right, out hit))
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
        if (Physics.BoxCast(transform.position + Vector3.right + Vector3.back, transform.localScale / 4, Vector3.back, out hit))
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
        if (Physics.BoxCast(transform.position + Vector3.left + Vector3.back, transform.localScale / 4, Vector3.back, out hit))
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
        if (Physics.BoxCast(transform.position + Vector3.left + Vector3.back, transform.localScale / 4, Vector3.left, out hit))
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
        if (Physics.BoxCast(transform.position + Vector3.left + Vector3.forward, transform.localScale / 4, Vector3.left, out hit))
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
        if (Physics.BoxCast(transform.position + Vector3.left + Vector3.forward, transform.localScale / 4, Vector3.forward, out hit))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Piece") && hit.collider.gameObject.GetComponent<Pieces>().color != color)
            {
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



    // Debugging purposes ONLY
    //private void OnDrawGizmos()
    //{
    //    //transform.localScale / 3, Vector3.forward * 2 + Vector3.right)
    //    Gizmos.color = Color.green;
    //    //Gizmos.DrawSphere(new Vector3(transform.position.x + 1, transform.position.y, transform.position.z + 2), 0.5f);
    //    if(Physics.BoxCast(transform.position + Vector3.right + Vector3.forward, transform.localScale/4, (Vector3.forward )))
    //    {
    //        Gizmos.color = Color.red;
    //        Gizmos.DrawWireCube(transform.position + Vector3.right + Vector3.forward + (Vector3.forward), transform.localScale/4);
    //    }
    //    else
    //    {
    //        Gizmos.DrawWireCube(transform.position + Vector3.right + Vector3.forward + (Vector3.forward), transform.localScale / 4);
    //    }

    //}
}
