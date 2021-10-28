using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is responsible for most of the make up of all chess pieces like having all the raycasts or having the obstacle array.
/// </summary>
/// 
/// Author: Rozario (Ross) Beaudin (RB)
/// 
/// Editor variables:
/// currentState        The current state of our chess piece.
/// color               The color of our piece.
/// 
/// Protected Variables:
/// diagonalRays        the ray casts that will be made in a diagonal line
/// vertHoriRays        the ray casts that will be made in vertical and horizontal directions.
/// boxCol              our box collider
/// enemyCanCapture     In an enemy attack range.
/// targets             The targets we can capture in our immediate attack range.
/// obstacles           transforms of other pieces
/// firstMove           is this our pieces first move.
/// 
[RequireComponent(typeof(BoxCollider))]
public abstract class Pieces : MonoBehaviour
{
    public enum State
    {
        WAITING, IDLE, SELECTED
    }
    
    public enum TheColor
    {
        DARK, WHITE
    }

    public State currentState = State.IDLE;
    public TheColor color = TheColor.WHITE;

    protected List<Ray> diagonalRays = new List<Ray>();
    protected List<Ray> vertHoriRays = new List<Ray>();

    protected BoxCollider boxCol;

    protected bool enemyCanCapture = false;
    protected GameObject[] targets = new GameObject[8];
    protected Transform[] obstacles = new Transform[8];

    protected bool firstMove = true;


    /// <summary>
    /// This method will be responsible for moving the chess pieces
    /// </summary>
    /// <param name="tilePosition">The position we want our piece to move to.</param>
    /// <returns>True if the move was successful. False otherwise.</returns>
    /// 
    /// 2020-10-29 RB Documentation
    /// 
    public abstract bool Move(Vector3 tilePosition);

    /// <summary>
    /// Thie method is called when we want our piece to capture one of the targets.
    /// </summary>
    /// <param name="tilePosition">The targets position</param>
    /// <returns>True if capture was successful. False otherwise.</returns>
    public bool Capture(Vector3 tilePosition)
    {
        //Debug.Log(targets[0]);
        if (targets == null)
        {
            return false;
        }
        
        foreach (GameObject gO in targets)
        {
            if (gO != null && gO.transform.position == tilePosition)
            {
                transform.position = new Vector3(gO.transform.position.x, transform.position.y, gO.transform.position.z);
                Debug.Log("capture gO = " + gO);
                Destroy(gO);
                firstMove = false;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Detects all possible pieces on the opposite color and stores all targets that we can capture.
    /// </summary>
    /// <returns></returns>
    /// 
    /// 2020-10-29 RB Documentation
    /// 
    public abstract bool CaptureDectection();

    /// <summary>
    /// Detects all possible pieces regardless of color.
    /// </summary>
    /// 
    /// 2020-10-29 RB Documentation
    /// 
    public virtual void ObstacleDetection()
    {
        return;
    }

    /// <summary>
    /// Set the color of our piece.
    /// </summary>
    /// <param name="color">The color we want to change our piece to.</param>
    /// 
    /// 2020-10-29 RB Documentation
    /// 
    public void SetTheColor(TheColor color)
    {
        this.color = color;
    }

    /// <summary>
    /// Sets up all diagonal ray casts.
    /// </summary>
    /// 
    /// 2020-10-29 RB Documentation
    /// 
    public void SetUpDiagonalRays()
    {
        // index 0 will be top right
        diagonalRays.Add(new Ray(transform.position, new Vector3(1f, 0, 1f)));

        // index 1 will be bottom right
        diagonalRays.Add(new Ray(transform.position, new Vector3(1f, 0, -1f)));

        // index 2 will be bottom left
        diagonalRays.Add(new Ray(transform.position, new Vector3(-1f, 0, -1f)));
        
        // index 3 will be top left
        diagonalRays.Add(new Ray(transform.position, new Vector3(-1f, 0, 1f)));

    }

    /// <summary>
    /// Updates all of the diagonal ray casts.
    /// </summary>
    /// 
    /// 2020-10-29 RB Documentation
    /// 
    protected void UpdateDiagonalRays()
    {
        diagonalRays[0] = new Ray(transform.position, new Vector3(1f, 0, 1f));
        diagonalRays[1] = new Ray(transform.position, new Vector3(1f, 0, -1f));
        diagonalRays[2] = new Ray(transform.position, new Vector3(-1f, 0, -1f));
        diagonalRays[3] = new Ray(transform.position, new Vector3(-1f, 0, 1f));
    }

    /// <summary>
    /// Sets up all vertical and horizontal ray casts.
    /// </summary>
    /// 
    /// 2020-10-29 RB Documentation
    /// 
    protected void SetUpVertHoriRays()
    {
        // index 0 will be forward
        vertHoriRays.Add(new Ray(transform.position, Vector3.forward));

        // index 1 will be left
        vertHoriRays.Add(new Ray(transform.position, Vector3.right));

        // index 2 will be back
        vertHoriRays.Add(new Ray(transform.position, Vector3.back));

        // index 3 will be right
        vertHoriRays.Add(new Ray(transform.position, Vector3.left));
    }

    /// <summary>
    /// Updates all vertical and horizontal ray casts.
    /// </summary>
    /// 
    /// 2020-10-29 RB Documentation
    ///
    protected void UpdateVertHoriRays()
    {
        vertHoriRays[0] = new Ray(transform.position, Vector3.forward);
        vertHoriRays[1] = new Ray(transform.position, Vector3.right);
        vertHoriRays[2] = new Ray(transform.position, Vector3.back);
        vertHoriRays[3] = new Ray(transform.position, Vector3.left);
    }

    /// <summary>
    /// Stores a detected target in one of the valid target spots.
    /// </summary>
    /// <param name="gO">The gameobject we want to store.</param>
    /// 
    /// 2020-10-29 RB Documentation
    /// 
    protected void StoreTarget(GameObject gO)
    {
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] == null)
            {
                targets[i] = gO;
                return;
            }
        }
    }

    /// <summary>
    /// Set the box collider.
    /// </summary>
    /// <param name="boxCol">the box collider we want to change to.</param>
    /// 
    /// 2020-10-29 RB Documentation
    /// 
    protected void SetBoxCol(BoxCollider boxCol)
    {
        this.boxCol = boxCol;
    }

    /// <summary>
    /// Set enemyCanCapture
    /// </summary>
    /// <param name="capture">The bool we are changing to.</param>
    /// 
    /// 2020-10-29 RB Documentation
    /// 
    public void SetEnemyCanCapture(bool capture)
    {
        enemyCanCapture = capture;
    }

    /// <summary>
    /// Gets enemyCanCapture
    /// </summary>
    /// <returns>enemyCanCapture</returns>
    /// 
    /// 2020-10-29 RB Documentation
    /// 
    public bool GetEnemyCanCapture()
    {
        return enemyCanCapture;
    }

    /// <summary>
    /// Gets the firstMove
    /// </summary>
    /// <returns>firstMove</returns>
    /// 
    /// 2020-10-29 RB Documentation
    /// 
    public bool GetFirstMove()
    {
        return firstMove;
    }
}
