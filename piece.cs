using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class piece : NetworkBehaviour
{
    public bool iswhite;
    public bool isking;
    

    public bool isForceToMove(piece[,] board, int x, int y)
    {
       
        // White piece
        if (iswhite)
        {
            // Check top left
            if (x >= 2 && y <= 5)
            {
                piece p = board[x - 1, y + 1];
                //if there is a piece and its not the same color 
                if (p != null && p.iswhite != iswhite)
                {
                    //check if its possible lo land after jump
                    if (board[x - 2, y + 2] == null)
                    {
                        return true;
                    }
                }
            }

            // Check top right
            if (x <= 5 && y <= 5)
            {
                piece p = board[x + 1, y + 1];
                //if there is a piece and its not the same color 
                if (p != null && p.iswhite != iswhite)
                {
                    //check if its possible lo land after jump
                    if (board[x + 2, y + 2] == null)
                    {
                        return true;
                    }
                }
            }

            // Check bottom left
            if (x >= 2 && y >= 2)
            {
                piece p = board[x - 1, y - 1];
                //if there is a piece and its not the same color 
                if (p != null && p.iswhite != iswhite)
                {
                    //check if its possible lo land after jump
                    if (board[x - 2, y - 2] == null)
                    {
                        return true;
                    }
                }
            }

            // Check bottom right
            if (x <= 5 && y >= 2)
            {
                piece p = board[x + 1, y - 1];
                //if there is a piece and its not the same color 
                if (p != null && p.iswhite != iswhite)
                {
                    //check if its possible lo land after jump
                    if (board[x + 2, y - 2] == null)
                    {
                        return true;
                    }
                }
            }
        }

        // Black piece
        if (!iswhite)
        {
            // Check top left
            if (x >= 2 && y <= 5)
            {
                piece p = board[x - 1, y + 1];
                //if there is a piece and its not the same color 
                if (p != null && p.iswhite != iswhite)
                {
                    //check if its possible lo land after jump
                    if (board[x - 2, y + 2] == null)
                    {
                        return true;
                    }
                }
            }

            // Check top right
            if (x <= 5 && y <= 5)
            {
                piece p = board[x + 1, y + 1];
                //if there is a piece and its not the same color 
                if (p != null && p.iswhite != iswhite)
                {
                    //check if its possible lo land after jump
                    if (board[x + 2, y + 2] == null)
                    {
                        return true;
                    }
                }
            }

            // Check bottom left
            if (x >= 2 && y >= 2)
            {
                piece p = board[x - 1, y - 1];
                //if there is a piece and its not the same color 
                if (p != null && p.iswhite != iswhite)
                {
                    //check if its possible lo land after jump
                    if (board[x - 2, y - 2] == null)
                    {
                        return true;
                    }
                }
            }

            // Check bottom right
            if (x <= 5 && y >= 2)
            {
                piece p = board[x + 1, y - 1];
                //if there is a piece and its not the same color 
                if (p != null && p.iswhite != iswhite)
                {
                    //check if its possible lo land after jump
                    if (board[x + 2, y - 2] == null)
                    {
                        return true;
                    }                   
                }
                
            }
        }

        // White piece king
        if (iswhite && isking)
        {
            int copyX = x;
            int copyY = y;

            // Check top left
            while (x >= 2 && y <= 5)
            {
                int placeToGOX = x - 2;
                int placeToGOY = y + 2;
                piece p = board[x - 1, y + 1];
                //if there is a piece and its not the same color
                if (p != null && p.iswhite != iswhite)
                {
                    if(placeToGOX >= 0 && placeToGOY <= 7)
                    {
                        piece toGo = board[placeToGOX, placeToGOY];
                        if (toGo == null)
                        {
                            Debug.Log("you need to kill the piece it the index: " + (x - 1) + ", " + (y + 1));
                            return true;
                        }
                        // placeToGOX--;
                        //placeToGOY++;
                        return false;
                    }
                    
                }
                
                x--;
                y++;
            }

            x = copyX;
            y = copyY;
            // Check top right
            while (x <= 5 && y <= 5)
            {
                int placeToGOX = x + 2;
                int placeToGOY = y + 2;
                piece p = board[x + 1, y + 1];
                //if there is a piece and its not the same color
                if (p != null && p.iswhite != iswhite)
                {
                    if(placeToGOX <= 7 && placeToGOY <= 7)
                    {
                        piece toGo = board[placeToGOX, placeToGOY];
                        if (toGo == null)
                        {
                            Debug.Log("you need to kill the piece it the index: " + (x + 1) + ", " +  (y + 1));
                            return true;
                        }
                        //  placeToGOX++;
                        //placeToGOY++;
                        return false;
                    }
                }
                
              x++;
              y++;
            }

            x = copyX;
            y = copyY;
            // Check bottum left
            while (x >= 2 && y >= 2)
            {
                int placeToGOX = x - 2;
                int placeToGOY = y - 2;
                piece p = board[x - 1, y - 1];
                //if there is a piece and its not the same color
                if (p != null && p.iswhite != iswhite)
                {
                    if(placeToGOX >= 0 && placeToGOY >= 0)
                    {
                        piece toGo = board[placeToGOX, placeToGOY];
                        if (toGo == null)
                        {
                            Debug.Log("you need to kill the piece it the index: " + (x - 1) + ", " +  (y - 1));
                            return true;
                        }
                        //  placeToGOX--;
                        // placeToGOY--;
                        return false;
                    }
                   
                }
               
                x--;
                y--;
            }

            x = copyX;
            y = copyY;
            // Check buttom right
            while (x <= 5 && y >= 2)
            {
                int placeToGOX = x + 2;
                int placeToGOY = y - 2;
                piece p = board[x + 1, y - 1];
                //if there is a piece and its not the same color
                if (p != null && p.iswhite != iswhite)
                {
                    if(placeToGOX <= 7 && placeToGOY >= 0)
                    {
                        piece toGo = board[placeToGOX, placeToGOY];
                        if (toGo == null)
                        {
                            Debug.Log("you need to kill the piece it the index: " + (x + 1) + ", " +  (y - 1));
                            return true;
                        }
                        
                      //  placeToGOX++;
                      //  placeToGOY--;
                        return false;
                    }
                   
                    
                }
                
                
                x++;
                y--;
            }
            
        }

        
        // black piece king
        if (!iswhite && isking)
        {
            int copyX = x;
            int copyY = y;

            // Check top left
            while (x >= 2 && y <= 5)
            {
                int placeToGOX = x - 2;
                int placeToGOY = y + 2;
                piece p = board[x - 1, y + 1];
                //if there is a piece and its not the same color
                if (p != null && p.iswhite != iswhite)
                {
                    if(placeToGOX >= 0 && placeToGOY <= 7)
                    {
                        piece toGo = board[placeToGOX, placeToGOY];
                        if (toGo == null)
                        {
                            Debug.Log("you need to kill the piece it the index: " + (x - 1) + ", " + (y + 1));
                            return true;
                        }
                        // placeToGOX--;
                        //placeToGOY++;
                        return false;
                    }
                    
                }
                
                x--;
                y++;
            }

            x = copyX;
            y = copyY;
            // Check top right
            while (x <= 5 && y <= 5)
            {
                int placeToGOX = x + 2;
                int placeToGOY = y + 2;
                piece p = board[x + 1, y + 1];
                //if there is a piece and its not the same color
                if (p != null && p.iswhite != iswhite)
                {
                    if(placeToGOX <= 7 && placeToGOY <= 7)
                    {
                        piece toGo = board[placeToGOX, placeToGOY];
                        if (toGo == null)
                        {
                            Debug.Log("you need to kill the piece it the index: " + (x + 1) + ", " +  (y + 1));
                            return true;
                        }
                        //  placeToGOX++;
                        //placeToGOY++;
                        return false;
                    }
                }
                
              x++;
              y++;
            }

            x = copyX;
            y = copyY;
            // Check bottum left
            while (x >= 2 && y >= 2)
            {
                int placeToGOX = x - 2;
                int placeToGOY = y - 2;
                piece p = board[x - 1, y - 1];
                //if there is a piece and its not the same color
                if (p != null && p.iswhite != iswhite)
                {
                    if(placeToGOX >= 0 && placeToGOY >= 0)
                    {
                        piece toGo = board[placeToGOX, placeToGOY];
                        if (toGo == null)
                        {
                            Debug.Log("you need to kill the piece it the index: " + (x - 1) + ", " +  (y - 1));
                            return true;
                        }
                        //  placeToGOX--;
                        // placeToGOY--;
                        return false;
                    }
                   
                }
               
                x--;
                y--;
            }

            x = copyX;
            y = copyY;
            // Check buttom right
            while (x <= 5 && y >= 2)
            {
                int placeToGOX = x + 2;
                int placeToGOY = y - 2;
                piece p = board[x + 1, y - 1];
                //if there is a piece and its not the same color
                if (p != null && p.iswhite != iswhite)
                {
                    if(placeToGOX <= 7 && placeToGOY >= 0)
                    {
                        piece toGo = board[placeToGOX, placeToGOY];
                        if (toGo == null)
                        {
                            Debug.Log("you need to kill the piece it the index: " + (x + 1) + ", " +  (y - 1));
                            return true;
                        }
                        
                      //  placeToGOX++;
                      //  placeToGOY--;
                        return false;
                    }
                   
                    
                }
                
                
                x++;
                y--;
            }
            
        }

        return false;
    }

    public bool validMove(piece[,] board, int x1, int y1, int x2, int y2)
    {
        //if you are moving on top of another piece
        if (board[x2, y2] != null) return false;

        int deltaMoveX = x1 - x2;
        int deltaMoveY = y2 - y1;
        //white pieces
        if (iswhite || isking)
        {
            if (deltaMoveX == 1 || deltaMoveX == -1)
            {
                if (deltaMoveY == 1)
                {
                    return true;
                }
            }
            else if (deltaMoveX == 2 || deltaMoveX == -2)
            {
                if (deltaMoveY == 2 || deltaMoveY == -2)
                {
                    piece p = board[(x1 + x2) / 2, (y1 + y2) / 2];

                    if (p != null && p.iswhite != iswhite)
                    {
                        return true;
                    }
                }


            }

        }
        //black pieces
        if (!iswhite || isking)
        {
            if (deltaMoveX == 1 || deltaMoveX == -1)
            {
                if (deltaMoveY == -1)
                {
                    return true;
                }
            }
            else if (deltaMoveX == 2 || deltaMoveX == -2)
            {
                if (deltaMoveY == 2 || deltaMoveY == -2)
                {
                    piece p = board[(x1 + x2) / 2, (y1 + y2) / 2];

                    if (p != null && p.iswhite != iswhite)
                    {
                        return true;
                    }
                }


            }

        }

         if(iswhite && isking)
         {
            int deltaX = Mathf.Abs(x2 - x1);
            int deltaY = Mathf.Abs(y2 - y1);

            // Check if it's a valid diagonal move
            if (deltaX == deltaY)
            {
                // Kings can move in any diagonal direction
                
                    // Check if the diagonal path is clear
                    int stepX = (x2 > x1) ? 1 : -1;
                    int stepY = (y2 > y1) ? 1 : -1;
                    int currentX = x1 + stepX;
                    int currentY = y1 + stepY;

                    while (currentX != x2 && currentY != y2 )
                    {
                    if (board[currentX, currentY] != null)
                    {
                        // If a piece is encountered while moving, check if it can be captured
                        piece capturedPiece = board[currentX, currentY];
                        if (capturedPiece.iswhite != iswhite)
                        {
                            // Capture the piece and continue the move
                            int nextX = currentX + stepX;
                            int nextY = currentY + stepY;
                            currentX += stepX;
                            currentY += stepY;
                            if(board[nextX, nextY] != null)
                            {
                                return false;
                            }
                            // Continue moving until the target position is reached
                            while (currentX != x2 && currentY != y2)
                            {
                                if (board[currentX, currentY] != null)
                                    return false; // Path is not clear

                                currentX += stepX;
                                currentY += stepY;
                            }

                            
                            return true;
                        }
                        else
                        {
                            return false; // Path is blocked by own piece
                        }
                    }


                        currentX += stepX;
                        currentY += stepY;
                    }



                return true;
                
                
            }
         }

        if (!iswhite && isking)
        {
            int deltaX = Mathf.Abs(x2 - x1);
            int deltaY = Mathf.Abs(y2 - y1);

            // Check if it's a valid diagonal move
            if (deltaX == deltaY)
            {
                // Kings can move in any diagonal direction

                // Check if the diagonal path is clear
                int stepX = (x2 > x1) ? 1 : -1;
                int stepY = (y2 > y1) ? 1 : -1;
                int currentX = x1 + stepX;
                int currentY = y1 + stepY;

                while (currentX != x2 && currentY != y2)
                {
                    if (board[currentX, currentY] != null)
                    {
                        // If a piece is encountered while moving, check if it can be captured
                        piece capturedPiece = board[currentX, currentY];
                        if (capturedPiece.iswhite != iswhite)
                        {
                            // Capture the piece and continue the move
                            int nextX = currentX + stepX;
                            int nextY = currentY + stepY;
                            currentX += stepX;
                            currentY += stepY;
                            if (board[nextX, nextY] != null)
                            {
                                return false;
                            }
                            // Continue moving until the target position is reached
                            while (currentX != x2 && currentY != y2)
                            {
                                if (board[currentX, currentY] != null)
                                    return false; // Path is not clear

                                currentX += stepX;
                                currentY += stepY;
                            }


                            return true;
                        }
                        else
                        {
                            return false; // Path is blocked by own piece
                        }
                    }


                    currentX += stepX;
                    currentY += stepY;
                }



                return true;


            }
        }

        return false;
    }

}
