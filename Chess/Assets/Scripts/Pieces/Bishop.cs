using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The bishop class is reponsible for moving all bishop pieces on the board in a diagonal way.
/// </summary>
/// 
/// Author: Rozario (Ross) Beaudin (RB)
/// 
/// Private Variables:
/// oddMoveSum          Determine whether our bishop is to remain on dark tiles or light tiles.
/// 
public class Bishop : Pieces
{

    private bool oddMoveSum = false;
    
    /// <summary>
    /// Initialize the targets and the maximum number of chess pieces we can detect.
    /// While initializing the diagonal rays and determining if we are on a dark tile or white tile.
    /// </summary>
    /// 
    /// 2020-10-30 RB Documentaion
    /// 
    void Start()
    {
        targets = new GameObject[4];
        obstacles = new Transform[4];
        SetUpDiagonalRays();
        if((transform.position.x + transform.position.z)%2 != 0)
        {
            oddMoveSum = true;
        }
    }

    /// <summary>
    /// Update the diagonal rays and update all nearby chess pieces and possible targets.
    /// </summary>
    /// 
    /// 2020-10-30 RB Documentation
    /// 
    void FixedUpdate()
    {
        //FindPossibleMoveSums();
        UpdateDiagonalRays();
        ObstacleDetection();
        CaptureDectection();
    }

    /// <summary>
    /// The method move is responsible for moving the Bishop in one of 4 ways while determining which color tiles we can move on. 
    /// If the move has no obstacles in the way.
    /// </summary>
    /// <param name="tilePosition">The tile we want to move to.</param>
    /// <returns>True if successful move, false otherwise.</returns>
    /// 
    /// 2020-10-30 RB Documentation
    /// 
    public override bool Move(Vector3 tilePosition)
    {
        // dark tiles
        if (oddMoveSum && (tilePosition.x + tilePosition.z)%2 != 0) 
        {
            // top right obstacle
            if (obstacles[0] == null || tilePosition.x < obstacles[0].position.x && tilePosition.z < obstacles[0].position.z )
            {
                for (float i = transform.position.x, j = transform.position.z; i < 8 || j < 8; i++, j++)
                {
                    if (tilePosition.x == i && tilePosition.z == j)
                    {
                        transform.Translate(Vector3.Scale((Vector3.forward + Vector3.right), tilePosition - transform.position));
                        return true;
                    }
                }
            }
            
            // bottom right obstacle
            if(obstacles[1] == null || tilePosition.x < obstacles[1].position.x && tilePosition.z > obstacles[1].position.z)
            {
                //Debug.Log("got in 1 diagonal");
                if ((tilePosition.x + tilePosition.z == transform.position.x + transform.position.z) && (tilePosition.x - transform.position.x > 0 && tilePosition.z - transform.position.z < 0))
                {
                    transform.Translate(Vector3.Scale((Vector3.forward + Vector3.right), tilePosition - transform.position));
                    return true;
                }
            }

            // bottom left obstacle
            if(obstacles[2] == null || tilePosition.x > obstacles[2].position.x && tilePosition.z > obstacles[2].position.z)
            {
                //Debug.Log("got in 2 diagonal");
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
            if(obstacles[3] == null || tilePosition.x > obstacles[3].position.x && tilePosition.z < obstacles[3].position.z)
            {
                Debug.Log("got in 3 diagonal");
                if ((tilePosition.x + tilePosition.z == transform.position.x + transform.position.z) && (tilePosition.x - transform.position.x < 0 && tilePosition.z - transform.position.z > 0))
                {
                    transform.Translate(Vector3.Scale((Vector3.forward + Vector3.right), tilePosition - transform.position));
                    return true;
                }
            }
        }
        // light tiles
        else if(!oddMoveSum && (tilePosition.x + tilePosition.z) % 2 == 0)
        {
            // top right obstacle
            if (obstacles[0] == null || tilePosition.x < obstacles[0].position.x && tilePosition.z < obstacles[0].position.z)
            {
                //Debug.Log("got in 0 diagonal");
                for (float i = transform.position.x, j = transform.position.z; i < 8 || j < 8; i++, j++)
                {
                    if (tilePosition.x == i && tilePosition.z == j)
                    {
                        transform.Translate(Vector3.Scale((Vector3.forward + Vector3.right), tilePosition - transform.position));
                        return true;
                    }
                }
            }

            // bottom right obstacle
            if (obstacles[1] == null || tilePosition.x < obstacles[1].position.x && tilePosition.z > obstacles[1].position.z)
            {
                //Debug.Log("got in 1 diagonal");
                if ((tilePosition.x + tilePosition.z == transform.position.x + transform.position.z) && (tilePosition.x - transform.position.x > 0 && tilePosition.z - transform.position.z < 0))
                {
                    transform.Translate(Vector3.Scale((Vector3.forward + Vector3.right), tilePosition - transform.position));
                    return true;
                }
            }

            // bottom left obstacle
            if (obstacles[2] == null || tilePosition.x > obstacles[2].position.x && tilePosition.z > obstacles[2].position.z)
            {
                //Debug.Log("got in 2 diagonal");
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
            if (obstacles[3] == null || tilePosition.x > obstacles[3].position.x && tilePosition.z < obstacles[3].position.z)
            {
                //Debug.Log("got in 3 diagonal");
                if ((tilePosition.x + tilePosition.z == transform.position.x + transform.position.z) && (tilePosition.x - transform.position.x < 0 && tilePosition.z - transform.position.z > 0))
                {
                    transform.Translate(Vector3.Scale((Vector3.forward + Vector3.right), tilePosition - transform.position));
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Detects all pieces in a diagonal that are not the same color and designates them as targets.
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

        // Top right
        if (Physics.Raycast(diagonalRays[0], out hit, 15f))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Piece") && hit.collider.gameObject.GetComponent<Pieces>().color != color)
            {
                gO = hit.collider.gameObject;
                StoreTarget(gO);
                gO.GetComponent<Pieces>().SetEnemyCanCapture(true);
            }
        }

        // Top left
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

        //bottom left
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
    /// We detect all chess pieces in 4 diagonal lines from the rooks current transform.
    /// Position and stores them in the obstacles array.
    /// </summary>
    /// 
    /// 2020-10-30 RB Documentation
    /// 
    public override void ObstacleDetection()
    {
        RaycastHit hit = new RaycastHit();

        //Debug.DrawRay(transform.position, diagonalRays[0].direction * 15f, Color.red);
        //Debug.DrawRay(transform.position, diagonalRays[3].direction * 15f, Color.red);
        //Debug.DrawRay(transform.position, diagonalRays[1].direction * 15f, Color.red);
        //Debug.DrawRay(transform.position, diagonalRays[2].direction * 15f, Color.red);

        // top right
        if (Physics.Raycast(diagonalRays[0], out hit, 15f) && hit.collider.gameObject.layer == LayerMask.NameToLayer("Piece"))
        {
            obstacles[0] = hit.collider.gameObject.transform;
        }
        else
        {
            obstacles[0] = null;
        }

        // top left
        hit = new RaycastHit();
        if (Physics.Raycast(diagonalRays[3], out hit, 15f) && hit.collider.gameObject.layer == LayerMask.NameToLayer("Piece"))
        {
            obstacles[3] = hit.collider.gameObject.transform;
        }
        else
        {
            obstacles[3] = null;
        }

        // bottom right
        hit = new RaycastHit();
        if (Physics.Raycast(diagonalRays[1], out hit, 15f) && hit.collider.gameObject.layer == LayerMask.NameToLayer("Piece"))
        {
            obstacles[1] = hit.collider.gameObject.transform;
        }
        else
        {
            obstacles[1] = null;
        }

        // bottom left
        hit = new RaycastHit();
        if (Physics.Raycast(diagonalRays[2], out hit, 15f) && hit.collider.gameObject.layer == LayerMask.NameToLayer("Piece"))
        {
            obstacles[2] = hit.collider.gameObject.transform;
        }
        else
        {
            obstacles[2] = null;
        }
    }
}
