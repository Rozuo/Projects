using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The Chess board class is responsible for creating our chess board placing all pieces. It will also be responsible with spawning all chess pieces.
/// </summary>
/// 
/// Author: Rozario (Ross) Beaudin (RB)
/// 
/// Editor Variables:
/// lightTile           the prefab for our light tiles
/// darkTile            the prefab for our dark tiles
/// whitePieces         an array for all white pieces
/// darkPieces          an array for all dark pieces
/// promotionText       text that will appear when a promotion is possible
/// 
/// 
/// Private Variables:
/// whitePiecesInPlay       a array record of all current white pieces.
/// darkPiecesInPlay        a array record of all current dark pieces.
/// tiles                   a array record of all tiles.
/// startingX               the x we start instantiating our objects
/// startingZ               the z we start instantiating our objects
/// currentX                the current x we are instantiating at.
/// currentZ                the current z we are instantiating at.
/// 
/// 
/// Private enums:
/// PieceColor              Dictates what color is a piece.
/// ChessPiece              Dictates what type of chess piece is instantiated.
/// 
public class ChessBoard : MonoBehaviour
{
    private enum PieceColor
    {
        Dark, White
    }

    private enum ChessPiece
    {
        Rook, Knight, Bishop, King, Queen, Pawn
    }

    public GameObject lightTile;
    public GameObject darkTile;
    public GameObject[] whitePieces = new GameObject[6];
    public GameObject[] darkPieces = new GameObject[6];

    public Text promotionText;

    private GameObject[] whitePiecesInPlay = new GameObject[16];
    private GameObject[] darkPiecesInPlay = new GameObject[16];
    private GameObject[,] tiles = new GameObject[8, 8];

    private GameObject whiteKing;
    private GameObject darkKing;

    private int startingX = 0;
    private int startingZ = 0;
    private int currentX;
    private int currentZ;
    

    /// <summary>
    /// Initialize our currentX and currentZ and then setup the chess board.
    /// </summary>
    /// 
    /// 2020-10-29 RB Documentation
    /// 
    void Start()
    {
        currentX = startingX;
        currentZ = startingZ;
        CreateChessBoard();
    }

    /// <summary>
    /// This method is responsible for creating our chess board and placing our pieces on the board.
    /// </summary>
    /// 
    /// 2020-10-29 RB Documentation
    /// 
    private void CreateChessBoard()
    {
        int lastDark = 0;
        float pieceY = 0.5f;
        GameObject gO;
        for (int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                // tile placement
                switch (lastDark)
                {
                    case (0):
                        gO = Instantiate(lightTile, transform, false);
                        gO.transform.position = new Vector3(currentX, 0, currentZ);
                        tiles[i, j] = gO;
                        lastDark = 1;
                        break;
                    case (1):
                        gO = Instantiate(darkTile, transform, false);
                        gO.transform.position = new Vector3(currentX, 0, currentZ);
                        tiles[i, j] = gO;
                        lastDark = 0;
                        break;
                }

                // chess piece placement
                switch (i)
                {
                    case (0):
                        if(j == 0 | j == 7)
                        {
                            whitePiecesInPlay[j] = SpawnChessPiece(new Vector3(currentX, pieceY, currentZ), PieceColor.White, ChessPiece.Rook);
                        }
                        else if (j == 1 | j == 6)
                        {
                            whitePiecesInPlay[j] = SpawnChessPiece(new Vector3(currentX, pieceY, currentZ), PieceColor.White, ChessPiece.Knight);
                        }
                        else if (j == 2 | j == 5)
                        {
                            whitePiecesInPlay[j] = SpawnChessPiece(new Vector3(currentX, pieceY, currentZ), PieceColor.White, ChessPiece.Bishop);
                        }
                        else if(j == 3)
                            whitePiecesInPlay[j] = SpawnChessPiece(new Vector3(currentX, pieceY, currentZ), PieceColor.White, ChessPiece.King);
                        else if(j == 4)
                            whitePiecesInPlay[j] = SpawnChessPiece(new Vector3(currentX, pieceY, currentZ), PieceColor.White, ChessPiece.Queen);
                        
                        break;
                    case (1):
                        whitePiecesInPlay[j+7] = SpawnPawn(new Vector3(currentX, 0, currentZ), PieceColor.White);
                        break;

                    case (6):
                        darkPiecesInPlay[j + 7] = SpawnPawn(new Vector3(currentX, pieceY, currentZ), PieceColor.Dark);
                        break; 
                    case (7):
                        if (j == 0 | j == 7)
                        {
                            darkPiecesInPlay[j] = SpawnChessPiece(new Vector3(currentX, pieceY, currentZ), PieceColor.Dark, ChessPiece.Rook);
                        }
                        else if (j == 1 | j == 6)
                        {
                            darkPiecesInPlay[j] = SpawnChessPiece(new Vector3(currentX, pieceY, currentZ), PieceColor.Dark, ChessPiece.Knight);
                        }
                        else if (j == 2 | j == 5)
                        {
                            darkPiecesInPlay[j] = SpawnChessPiece(new Vector3(currentX, pieceY, currentZ), PieceColor.Dark, ChessPiece.Bishop);
                        }
                        else if (j == 3)
                            darkPiecesInPlay[j] = SpawnChessPiece(new Vector3(currentX, pieceY, currentZ), PieceColor.Dark, ChessPiece.King);
                        else if (j == 4)
                            darkPiecesInPlay[j] = SpawnChessPiece(new Vector3(currentX, pieceY, currentZ), PieceColor.Dark, ChessPiece.Queen);
                        
                        break;
                }

                currentX += 1;
            }
            lastDark = lastDark == 0 ? 1 : 0;
            currentX = startingX;
            currentZ += 1;
        }
    }

    /// <summary>
    /// This method is responsible for instatiating a specific chess piece (not including the pawn).
    /// </summary>
    /// <param name="position">The position we want the piece to be placed</param>
    /// <param name="color">The color of the piece</param>
    /// <param name="piece">The type of chess piece</param>
    /// <returns>The created chess pieces</returns>
    /// 
    /// 2020-10-29 RB Documentation
    /// 
    private GameObject SpawnChessPiece(Vector3 position, PieceColor color, ChessPiece piece)
    {
        GameObject gO = null;
        float yOffset = 0.0f;
        switch (piece)
        {
            case (ChessPiece.Rook):
                switch (color)
                {
                    case (PieceColor.White):
                        gO = Instantiate(whitePieces[0], transform, false);
                        
                        break;
                    case (PieceColor.Dark):
                        gO = Instantiate(darkPieces[0], transform, false);
                        break;
                }
                yOffset = 0.45f;
                break;
            case (ChessPiece.Knight):
                switch (color)
                {
                    case (PieceColor.White):
                        gO = Instantiate(whitePieces[1], transform, false);
                        break;
                    case (PieceColor.Dark):
                        gO = Instantiate(darkPieces[1], transform, false);
                        break;
                }
                yOffset = 0.5f;
                break;
            case (ChessPiece.Bishop):
                switch (color)
                {
                    case (PieceColor.White):
                        gO = Instantiate(whitePieces[2], transform, false);
                        break;
                    case (PieceColor.Dark):
                        gO = Instantiate(darkPieces[2], transform, false);
                        break;
                }
                yOffset = 0.56f;
                break;
            case (ChessPiece.Queen):
                switch (color)
                {
                    case (PieceColor.White):
                        gO = Instantiate(whitePieces[3], transform, false);
                        break;
                    case (PieceColor.Dark):
                        gO = Instantiate(darkPieces[3], transform, false);
                        break;
                }
                yOffset = 0.87f;
                break;
            case (ChessPiece.King):
                switch (color)
                {
                    case (PieceColor.White):
                        gO = Instantiate(whitePieces[4], transform, false);
                        whiteKing = gO;
                        break;
                    case (PieceColor.Dark):
                        gO = Instantiate(darkPieces[4], transform, false);
                        darkKing = gO;
                        break;
                }
                yOffset = 0.8f;
                break;
        }
        position.Set(position.x, yOffset, position.z);
        gO.transform.position = position;
        return gO;
    }

    /// <summary>
    /// This method will create only pawns. This is it's seperate method since there is a lot of pawns to be placed.
    /// </summary>
    /// <param name="position">The position we place the pawn.</param>
    /// <param name="color">The color we want the pawn.</param>
    /// <returns>The created pawn</returns>
    /// 
    /// 2020-10-29 RB Documentation
    /// 
    private GameObject SpawnPawn(Vector3 position, PieceColor color)
    {
        GameObject gO = null;
        float yOffset = 0.34f;
        switch (color)
        {
            case (PieceColor.White):
                gO = Instantiate(whitePieces[5], transform, false);
                gO.GetComponent<Pieces>().SetTheColor(Pieces.TheColor.WHITE);
                break;
            case (PieceColor.Dark):
                gO = Instantiate(darkPieces[5], transform, false);
                gO.GetComponent<Pieces>().SetTheColor(Pieces.TheColor.DARK);
                break;
        }

        position.Set(position.x, yOffset, position.z);
        gO.transform.position = position;
        return gO;
    }

    /// <summary>
    /// This method is responsible for causing the pawn promotion when the pawn reaches the end of the board.
    /// </summary>
    /// <returns>True if we can promote. False if we can not or no longer promote.</returns>
    /// 
    /// 2020-10-29 RB Documentation
    /// 
    public bool PawnPromotionCheck()
    {
        int index = 0;
        GameObject promotingPawn = null;
        PieceColor color = PieceColor.White;
        for(int i = 7; i < whitePiecesInPlay.Length; i++)
        {
            if(whitePiecesInPlay[i] != null && whitePiecesInPlay[i].GetComponent<Pawn>() != null && whitePiecesInPlay[i].transform.position.z == 7)
            {
                promotingPawn = whitePiecesInPlay[i];
                index = i;
                color = PieceColor.White;
            }
        }

        if(promotingPawn == null)
        {
            for(int i = 7; i < darkPiecesInPlay.Length; i++)
            {
                if (darkPiecesInPlay[i] != null && darkPiecesInPlay[i].GetComponent<Pawn>() != null && darkPiecesInPlay[i].transform.position.z == 0)
                {
                    promotingPawn = darkPiecesInPlay[i];
                    index = i;
                    color = PieceColor.Dark;
                }
            }
        }

        if(promotingPawn != null)
        {
            promotionText.enabled = true;
            GameObject gO;
            // queen promotion
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                switch (color)
                {
                    case (PieceColor.White):
                        gO = Instantiate(whitePieces[3], transform, false);
                        gO.transform.position = new Vector3(promotingPawn.transform.position.x, 0.87f, promotingPawn.transform.position.z);
                        Destroy(promotingPawn);
                        whitePiecesInPlay[index] = gO;
                        break;
                    case (PieceColor.Dark):
                        gO = Instantiate(darkPieces[3], transform, false);
                        gO.transform.position = new Vector3(promotingPawn.transform.position.x, 0.87f, promotingPawn.transform.position.z);
                        Destroy(promotingPawn);
                        darkPiecesInPlay[index] = gO;
                        break;
                }

                promotionText.enabled = false;
                return false;
            }

            // Bishop promotion
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                switch (color)
                {
                    case (PieceColor.White):
                        gO = Instantiate(whitePieces[2], transform, false);
                        gO.transform.position = new Vector3(promotingPawn.transform.position.x, 0.56f, promotingPawn.transform.position.z);
                        Destroy(promotingPawn);
                        whitePiecesInPlay[index] = gO;
                        break;
                    case (PieceColor.Dark):
                        gO = Instantiate(darkPieces[2], transform, false);
                        gO.transform.position = new Vector3(promotingPawn.transform.position.x, 0.56f, promotingPawn.transform.position.z);
                        Destroy(promotingPawn);
                        darkPiecesInPlay[index] = gO;
                        break;
                }

                promotionText.enabled = false;
                return false;
            }

            // Rook promotion
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                switch (color)
                {
                    case (PieceColor.White):
                        gO = Instantiate(whitePieces[0], transform, false);
                        gO.transform.position = new Vector3(promotingPawn.transform.position.x, 0.45f, promotingPawn.transform.position.z);
                        Destroy(promotingPawn);
                        whitePiecesInPlay[index] = gO;
                        break;
                    case (PieceColor.Dark):
                        gO = Instantiate(darkPieces[0], transform, false);
                        gO.transform.position = new Vector3(promotingPawn.transform.position.x, 0.45f, promotingPawn.transform.position.z);
                        Destroy(promotingPawn);
                        darkPiecesInPlay[index] = gO;
                        break;
                }

                promotionText.enabled = false;
                return false;
            }

            // Knight promotion
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                switch (color)
                {
                    case (PieceColor.White):
                        gO = Instantiate(whitePieces[1], transform, false);
                        gO.transform.position = new Vector3(promotingPawn.transform.position.x, 0.5f, promotingPawn.transform.position.z);
                        Destroy(promotingPawn);
                        whitePiecesInPlay[index] = gO;
                        break;
                    case (PieceColor.Dark):
                        gO = Instantiate(darkPieces[1], transform, false);
                        gO.transform.position = new Vector3(promotingPawn.transform.position.x, 0.5f, promotingPawn.transform.position.z);
                        Destroy(promotingPawn);
                        darkPiecesInPlay[index] = gO;
                        break;
                }

                promotionText.enabled = false;
                return false;
            }

            return true;
        }
        return false;
    }

    /// <summary>
    /// Gets the white king
    /// </summary>
    /// <returns>the white king object</returns>
    public GameObject GetWhiteKing()
    {
        return whiteKing;
    }

    /// <summary>
    /// Gets the dark king
    /// </summary>
    /// <returns>the dark king object</returns>
    public GameObject GetDarkKing()
    {
        return darkKing;
    }
}
