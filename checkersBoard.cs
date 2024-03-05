using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine;
using Unity.Netcode;

public class checkersBoard : NetworkBehaviour
{
    public static checkersBoard instance { set; get; }

    public piece[,] pieces = new piece[8, 8];
    public GameObject whitePiecePrefab;
    public GameObject blackPiecePrefab;

    public Transform chatMessageContainer;
    public GameObject messagePrefab;

    public GameObject highlightsContianer;

    public CanvasGroup alertCanves;
    private float lastAlert;
    private bool alertAvtive;

    public bool isWhite;
    public bool isWhiteTurn;
    private bool hasKilled;

    private int noKillRounds = 0;
    private const int maxNoKillRounds = 20;

    private piece selectedPiece;
    private List<piece> forcedPieces;

    public float whitePieceTotalTime = 180.0f; // 3 minutes for player 1
    public float blackPieceTotalTime = 180.0f; // 3 minutes for player 2
   
    private bool isPaused;

    public TextMeshProUGUI whitePieceTimerText;
    public TextMeshProUGUI blackPieceTimerText;

    private Vector3 boardOfSet = new Vector3(-4f, 0, -4f);
    private Vector3 pieceOfSet = new Vector3(0.5f, 0.125f, 0.5f);

    private Vector2 mouseOver;
    private Vector2 startDrag;
    private Vector2 endDrag;

    private client client;

    public void Start()
    {

        instance = this;

        client = FindObjectOfType<client>();      
        
        foreach(Transform t in highlightsContianer.transform)
        {
            t.position = Vector3.down * 100;
        }

        if(client)
        {
            isWhite = client.isHost;
            alert(client.players[0].name + " VS " + client.players[1].name);
        }
        else
        {
            alert("White player's turn");
        }

        isWhiteTurn = true;
        forcedPieces = new List<piece>();
        isPaused = false;

        UpdateTimerDisplay();
        generateBoard();
        
        
    }


    private void Update()
    {
        foreach (Transform t in highlightsContianer.transform)
        {
            t.Rotate(Vector3.up * 90 * Time.deltaTime);
        }

        updateAlert();
        updtaeMouse();
        if((isWhite) ? isWhiteTurn:!isWhiteTurn)
        {
            int x = (int)mouseOver.x;
            int y = (int)mouseOver.y;

            if(selectedPiece != null )
            {
                updatePieceDrag(selectedPiece);
            }

            if(Input.GetMouseButtonDown(0))
            {
                selectPiece(x, y);
            }

            if(Input.GetMouseButtonUp(0)) 
            {
                tryMove((int)startDrag.x, (int)startDrag.y, x, y);     
            }

        }

        if (!isPaused)
        {
            if (isWhiteTurn)
            {
                whitePieceTotalTime -= Time.deltaTime;
                whitePieceTotalTime = Mathf.Max(0, whitePieceTotalTime);
            }
            else
            {
                blackPieceTotalTime -= Time.deltaTime;
                blackPieceTotalTime = Mathf.Max(0, blackPieceTotalTime);
            }

            UpdateTimerDisplay();

            if (whitePieceTotalTime <= 0 || blackPieceTotalTime <= 0)
            {
                // Technical loss: Handle the end of the game
                checkVictory();
            }
        }
    }
    private void selectPiece(int x, int y)
    {
        //out of bounds
        if(x < 0 || x >= 8 || y < 0 || y >= 8)
        {
            return;
        }

        piece p = pieces[x, y];

        if(p != null && p.iswhite == isWhite)
        {
            if (forcedPieces.Count == 0)
            {
                selectedPiece = p;
                startDrag = mouseOver;
            }
            else
            {
                //look for the piece under our forced pieces list
                if(forcedPieces.Find(fp => fp == p) == null)
                {
                    return;
                }
                selectedPiece = p;
                startDrag = mouseOver;
            }
        }
    }

    public void tryMove(int x1, int y1, int x2, int y2)
    {
        forcedPieces = scanForPossibleMove();
        //multiplayer support
        startDrag = new Vector2(x1, y1);
        endDrag = new Vector2(x2, y2);
        selectedPiece = pieces[x1 ,y1];

        //out of bounds
        if (x2 < 0 || x2 > 8 || y2 < 0 || y2 >8)
        {
            if(selectedPiece != null)
            {
                movePiece(selectedPiece, x1, y1);
            }

            startDrag = Vector2.zero;
            selectedPiece = null;
            hightlight();
            return;
        }

        if (selectedPiece != null)
        {
            //if it didnt move
            if (endDrag == startDrag)
            {
                movePiece(selectedPiece, x1, y1);
                startDrag = Vector2.zero;
                selectedPiece = null;
                hightlight();
                return;
            }
            //check if its a valid move
            if (selectedPiece.validMove(pieces, x1, y1, x2, y2))
            {
                //if the piece is not a king 
                if (!selectedPiece.isking)
                {
                    //did we kill
                    //if this is a kill
                    if (Mathf.Abs(x2 - x1) == 2)
                    {
                        //hasKilled = forcedPieces.Count;

                        piece p = pieces[(x1 + x2) / 2, (y1 + y2) / 2];
                        if (p != null)
                        {
                            pieces[(x1 + x2) / 2, (y1 + y2) / 2] = null;
                            Destroy(p.gameObject);
                            hasKilled = true;
                        }
                    }
                }
                //if the piece is a king 
                if (selectedPiece.isking)
                {
                    //did we kill
                    //if this is a kill
                    int stepX = (x2 > x1) ? 1 : -1;
                    int stepY = (y2 > y1) ? 1 : -1;
                    int currentX = x1 + stepX;
                    int currentY = y1 + stepY;

                    while (currentX != x2 && currentY != y2)
                    {
                        piece board = pieces[currentX, currentY];
                        if (board != null)
                        {
                             
                            pieces[currentX, currentY] = null;
                            Destroy(board.gameObject);
                            hasKilled = true;

                        }
                        currentX += stepX;
                        currentY += stepY;
                    }
                    
                }
                //were we suposed to kill anything?
                if (forcedPieces.Count != 0 && !hasKilled)
                {
                    movePiece(selectedPiece, x1, y1);
                    startDrag = Vector2.zero;
                    selectedPiece = null;
                    hightlight();
                    return;
                }

                if(!hasKilled)// checks if there was a round without a kill
                {
                    noKillRounds++;
                }
                else
                {
                    noKillRounds = 0;
                }


                pieces[x2, y2] = selectedPiece;
                pieces[x1, y1] = null;
                movePiece(selectedPiece, x2, y2);

                endTurn();
            }
            else
            {
                movePiece(selectedPiece, x1, y1);
                startDrag = Vector2.zero;
                selectedPiece = null;
                hightlight();
                return;
            }
        

        }
        
    }

    private void endTurn()
    {

        int x1 = (int)startDrag.x;
        int x2 = (int)endDrag.x;
        int y1 = (int)startDrag.y;
        int y2 = (int)endDrag.y;
        // check for king 
        if(selectedPiece != null)
        {
            if (selectedPiece.iswhite && !selectedPiece.isking && y2 == 7)
            {
                selectedPiece.isking = true;
                selectedPiece.transform.Rotate(Vector3.right * 180);
            }
            else if (!selectedPiece.iswhite && !selectedPiece.isking && y2 == 0)
            {
                selectedPiece.isking = true;
                selectedPiece.transform.Rotate(Vector3.right * 180);
            }
        }
        if (client)
        {
            string msg = "CMOV|";
            msg = msg + startDrag.x.ToString() + "|";
            msg = msg + startDrag.y.ToString() + "|";
            msg = msg + endDrag.x.ToString() + "|";
            msg = msg + endDrag.y.ToString();

            client.send(msg);
        }
        selectedPiece = null;
        startDrag = Vector2.zero;

        if (scanForPossibleMove(selectedPiece, x2 ,y2).Count != 0 && hasKilled)
        {           
            hasKilled = false;
            return;           
        }

        isWhiteTurn = !isWhiteTurn;
        //isWhite = !isWhite;
        hasKilled = false;
        checkVictory();

        if(!client)
        {
            isWhite = !isWhite;
            if(isWhite)
            {
                alert("White player's turn");
            }
            else
            {
                alert("Black player's turn");
            }
        }
        else
        {
            if(isWhite)
            {
                alert(client.players[0].name + "'s turn");
            }
            else
            {
                alert(client.players[1].name + "'s turn");
            }
        }

        scanForPossibleMove();
    }

    private bool HasLost(bool isWhite) //check if a player lost because he doesnt have any valid move to do 
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                piece p = pieces[x, y];

                if (p != null && p.iswhite != isWhite)
                {
                    for (int x2 = 0; x2 < 8; x2++)
                    {
                        for (int y2 = 0; y2 < 8; y2++)
                        {
                            if (p.validMove(pieces, x, y, x2, y2))
                            {                                
                                return false; // Player has a valid move.
                                
                            }
                        }
                    }
                }
            }
        }     
        return true; // Player has no valid moves and has lost.
    }

    private void checkVictory()
    {
        bool whiteLost = HasLost(true);
        bool blackLost = HasLost(false);

        if (CountRemainingPieces(true) == 0)
        {
            victory(false);
           // Debug.Log("no black pieces left");
        }
        if(CountRemainingPieces(false) == 0)
        {
            victory(true);
           // Debug.Log("no white pieces left");
        }       

        if(blackLost)
        {
            victory(false);
             Debug.Log("no valid move");
        }
        if (whiteLost)
        {
            victory(true);
            Debug.Log("no valid move");
        }
        if(blackLost && whiteLost)
        {
            Debug.Log("its a draw");
        }

        if (noKillRounds >= maxNoKillRounds)
        {
            Debug.Log("The game is a draw due to 20 rounds without any kills. 10 rounds for each player");
        }

        if(blackPieceTotalTime <= 0)
        {
            isPaused = true;
            victory(true);
        }
        if(whitePieceTotalTime <= 0)
        {
            isPaused = true;
            victory(false);
        }

    }

    private void victory(bool isWhite)
    {
        if(isWhite)
        {
            Debug.Log("white team has won");
        }
        else
        {
            Debug.Log("black team has won");
        }
    }

    private int CountRemainingPieces(bool isWhite)
    {
        int count = 0;

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                piece p = pieces[x, y];
                if (p != null && p.iswhite == isWhite)
                {
                    count++;
                }
            }
        }

        return count;
    }


    private List<piece> scanForPossibleMove(piece p, int x, int y)
    {
        forcedPieces = new List<piece>();

        if (pieces[x, y].isForceToMove(pieces, x, y))
        {
            forcedPieces.Add(pieces[x,y]);
        }

        hightlight();
        return forcedPieces;
    }
    private List<piece> scanForPossibleMove()
    {
        forcedPieces = new List<piece>();

        //check all the pieces
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (pieces[i, j] != null && pieces[i, j].iswhite == isWhiteTurn)
                {
                    if (pieces[i, j].isForceToMove(pieces, i, j))
                    {
                        forcedPieces.Add(pieces[i, j]);
                    }
                }
            }
        }
        hightlight();
        return forcedPieces;
    }

    private void hightlight()
    {
        foreach (Transform t in highlightsContianer.transform)
        {
            t.position = Vector3.down * 100;
        }

        if (forcedPieces.Count > 0)
        {
            highlightsContianer.transform.GetChild(0).transform.position = forcedPieces[0].transform.position + Vector3.down * 0.1f;
        }
        if (forcedPieces.Count > 1)
        {
            highlightsContianer.transform.GetChild(1).transform.position = forcedPieces[1].transform.position + Vector3.down * 0.1f;
        }
    }

    private void updatePieceDrag(piece p)
    {
        if (!Camera.main)
        {
            Debug.Log("unable to find main camera");
        }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25f, LayerMask.GetMask("board")))
        {
            p.transform.position = hit.point + Vector3.up;
        }
        
    }

    private void updtaeMouse()
    {
         
        if(!Camera.main)
        {
            Debug.Log("unable to find main camera");
        }

        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25f, LayerMask.GetMask("board")))
        {
            mouseOver.x = (int)(hit.point.x - boardOfSet.x);
            mouseOver.y = (int)(hit.point.z - boardOfSet.z);
        }
        else
        {
            mouseOver.x = -1;
            mouseOver.y = -1;
        }
    }


    private void generateBoard()
    {
        
            //generate white team
            for (int y = 0; y < 3; y++)
            {
                bool oddRaw = (y % 2 == 0);

                for (int x = 0; x < 8; x = x + 2)
                {
                    // generate the pieces
                    generatePiece((oddRaw) ? x : x + 1, y);
                }
            }
        
        
            //generate black team
            for (int y = 7; y > 4; y--)
            {
                bool oddRaw = (y % 2 == 0);

                for (int x = 0; x < 8; x = x + 2)
                {
                    // generate the pieces
                    generatePiece((oddRaw) ? x : x + 1, y);
                }
            
        }
    }
    private void generatePiece(int x, int y)
    {
        bool isPieceWhite = (y > 3) ? false : true;
        GameObject go = Instantiate((isPieceWhite) ? whitePiecePrefab : blackPiecePrefab) as GameObject;
        if (IsServer)
        {
            go.transform.SetParent(transform);
        }
        piece p = go.GetComponent<piece>();
        pieces[x, y] = p;
        movePiece(p, x, y);
    }
    private void movePiece(piece p, int x, int y)
    {
        p.transform.position = (Vector3.right * x) + (Vector3.forward * y) + boardOfSet + pieceOfSet;
    }

    private void UpdateTimerDisplay()
    {
        whitePieceTimerText.text = "Player 1 Time: " + FormatTime(whitePieceTotalTime);
        blackPieceTimerText.text = "Player 2 Time: " + FormatTime(blackPieceTotalTime);
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    public void alert(string text)
    {
        alertCanves.GetComponentInChildren<TextMeshProUGUI>().text = text;
        alertCanves.alpha = 1;
        lastAlert = Time.time;
        alertAvtive = true;
    }
    public void updateAlert()
    {
        if(alertAvtive)
        {
            if(Time.time - lastAlert > 1.5f)
            {
                alertCanves.alpha = 1 - ((Time.time - lastAlert) - 1.5f);

                if(Time.time - lastAlert > 2.5)
                {
                    alertAvtive = false;
                }
            }


        }
    }

    public void chatMessage(string msg)
    {
        GameObject go = Instantiate(messagePrefab); 
        go.transform.SetParent(chatMessageContainer);

        go.GetComponentInChildren<TMP_Text>().text = msg;
    }
    public void sendChatMessage()
    {
        TMP_InputField i = GameObject.Find("messageInput").GetComponent<TMP_InputField>();

        if(i.text == "")
        {
            return;
        }

        client.send("CMSG| " + i.text);

        i.text = "";
    }
}


