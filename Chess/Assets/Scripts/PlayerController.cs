using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The playercontroller is responsible for controlling all chess pieces through mouse input
/// given that it is the correct turn.
/// </summary>
/// 
/// Author: Rozario (Ross) Beaudin
/// 
/// Editor Variables:
/// chessBoard          The reference to our current chess board
/// 
/// Private variables:
/// currentTurn             The current turn of the chess pieces
/// currentState            The current state of the player controller selecting pieces
/// selectedPiece           The chess piece we selected from mouse selection
/// enPassantReady          When we can perform enPassant
/// previouslyMovedPawn     A record of a previously moved pawn
/// whiteKing               reference to the white king on the board
/// darkKing                reference to the dark king on the board
/// 
/// Private enum
/// State           Dictates what the player controller will be doing depending on the state.
/// 

public class PlayerController : MonoBehaviour
{
    private enum State
    {
        PIECE_SELECTED, NO_SELECTION, WAITING
    }

    private Pieces.TheColor currentTurn = Pieces.TheColor.WHITE;
    private State currentState = State.NO_SELECTION;

    private Vector3 mousePosition;
    private Pieces selectedPiece = null;

    private bool enPassantReady = false;

    private Pawn previouslyMovedPawn;

    public ChessBoard chessBoard;
    private King whiteKing;
    private King darkKing;

    /// <summary>
    /// Initialize the white king and the dark king.
    /// </summary>
    /// 
    /// 2020-10-30 RB Documentation
    /// 
    void Start()
    {
        whiteKing = chessBoard.GetWhiteKing().GetComponent<King>();
        darkKing = chessBoard.GetDarkKing().GetComponent<King>();
    }     

    /// <summary>
    /// Depending on the state the playercontroller will do different things. 
    /// If we pressing the mouse down we handle piece selected and no selection.
    /// if we are waiting we simply wait to change state.
    /// </summary>
    /// 
    /// 2020-10-30 RB Documentation
    /// 
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
            switch (currentState)
            {
                case (State.PIECE_SELECTED):
                    HandleSelected();
                    break;
                case (State.NO_SELECTION):
                    HandleNoSelection();
                    break;
            }
        }

        if(currentState == State.WAITING)
        {
            HandleWaiting();   
        }
    }

    /// <summary>
    /// If we have already selected a chess piece we will look to see if we are performing a move or capture a target.
    /// We also check here if we can preform a pawn promotion.
    /// </summary>
    /// 
    /// 2020-10-30 RB Documentation
    /// 
    private void HandleSelected()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            GameObject gO = hit.collider.gameObject;
            if (gO.layer == LayerMask.NameToLayer("Tile"))
            {
                    
                if (selectedPiece.Move(gO.transform.position))
                {
                    if (enPassantReady && previouslyMovedPawn != null)
                    {
                        previouslyMovedPawn.SetUsedDoubleMove(false);
                        previouslyMovedPawn = null;
                    }
                    currentTurn = currentTurn == Pieces.TheColor.WHITE ? Pieces.TheColor.DARK : Pieces.TheColor.WHITE;
                    
                    
                    if (previouslyMovedPawn == null && selectedPiece.GetComponent<Pawn>() != null)
                    {
                        previouslyMovedPawn = selectedPiece.GetComponent<Pawn>();
                        enPassantReady = previouslyMovedPawn.GetUsedDoubleMove();
                        
                        currentState = State.NO_SELECTION;
                        return;
                    }
                    enPassantReady = false;
                }
                
                
            }
            else if (gO.layer == LayerMask.NameToLayer("Piece"))
            {
                if(gO.GetComponent<Rook>() != null && selectedPiece.GetComponent<King>() != null)
                {
                    selectedPiece.GetComponent<King>().Castling(gO);
                    currentTurn = currentTurn == Pieces.TheColor.WHITE ? Pieces.TheColor.DARK : Pieces.TheColor.WHITE;
                }
                else if (!selectedPiece.Equals(gO.GetComponent<Pieces>()) && selectedPiece.Capture(gO.transform.position))
                {
                    currentTurn = currentTurn == Pieces.TheColor.WHITE ? Pieces.TheColor.DARK : Pieces.TheColor.WHITE;
                }

                enPassantReady = false;
            }
            if (chessBoard.PawnPromotionCheck())
            {
                currentState = State.WAITING;
            }
            else
            {
                currentState = State.NO_SELECTION;
            }
            
        }
    }

    /// <summary>
    /// This method handles when we currently have not selected any piece on the board. 
    /// We can select a piece from this method given that it is the correct turn.
    /// </summary>
    /// 
    /// 2020-10-30 RB Documentation
    /// 
    private void HandleNoSelection()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Piece") && hit.collider.gameObject.GetComponent<Pieces>().color == currentTurn)
            {
                currentState = State.PIECE_SELECTED;
                selectedPiece = hit.collider.gameObject.GetComponent<Pieces>();
            }

            Debug.Log("You selected the " + hit.transform.name);
        }
    }

    /// <summary>
    /// This method handles when we are in the waiting state. 
    /// This state will only triger when we are promoting a chess piece.
    /// </summary>
    /// 
    /// 2020-10-30 RB Documentation
    /// 
    private void HandleWaiting()
    {
        if (!chessBoard.PawnPromotionCheck())
            currentState = State.NO_SELECTION;
    }
}

