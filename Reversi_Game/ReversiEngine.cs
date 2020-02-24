using System;

namespace Reversi_Game
{
    public class ReversiEngine
    {
        public int WidthBoard { get ; set; }
        public int HeightBoard { get; set; }

        private int[,] board;
        public int numberPlayerOfNextRound { get; private set; } = 1;

        private static int opponentNumber(int playerNumber)
        {
            return (playerNumber == 1) ? 2 : 1;
        }

        private bool isFieldCorrect(int horizontal, int vertical)
        {
            return horizontal >= 0 && horizontal < WidthBoard &&
                   vertical >= 0 && vertical < HeightBoard;
        }

        public int fieldState(int horizontal, int vertical)
        {
            if (!isFieldCorrect(horizontal, vertical))
                throw new Exception("Jestes poza polem!!!");
            return board[horizontal, vertical];
        }
    
        private void clearBoard()
        {
            for (int i = 0; i < WidthBoard; i++)
                for (int j = 0; j < HeightBoard; j++)
                    board[i, j] = 0;
            int centerWidth = WidthBoard / 2;
            int centerHeight = HeightBoard / 2;

            board[centerWidth - 1, centerHeight - 1] = board[centerWidth, centerHeight] = 1;
            board[centerWidth - 1, centerHeight] = board[centerWidth, centerHeight] = 2;
        }

        public ReversiEngine(int numberFirstPlayer, int widthBoard = 8, int heightBoard = 8)
        {
            if (numberFirstPlayer < 1 || numberFirstPlayer > 2)
                throw new Exception("Zły numer gracza! Musisz wybrać 1");

            board = new int[widthBoard, heightBoard];
            WidthBoard = widthBoard;
            HeightBoard = heightBoard;

            clearBoard();

            numberPlayerOfNextRound = numberFirstPlayer;
            calculateNumbersOfFields();
        }

        private void changeCurrentPlayer()
        {
            numberPlayerOfNextRound = opponentNumber(numberPlayerOfNextRound);
        }

        protected int putStone(int horizontal, int vertical, bool onlyTest)
        {
            if (!isFieldCorrect(horizontal, vertical))
                throw new Exception("Jestes poza polem!!!");

            if (board[horizontal, vertical] != 0)
                return -1;

            int howManyFields = 0;

            for (int directionHorizontal = -1; directionHorizontal <= 1; directionHorizontal++)
            {
                for(int directionVertical = -1; directionVertical <= 1; directionVertical++)
                {
                    if (directionHorizontal == 0 && directionVertical == 0) continue;

                    int i = horizontal;
                    int j = vertical;
                    bool findStoneOpponent = false;
                    bool findStonePlayer = false;
                    bool findEmptyField = false;
                    bool findEndOfBoard = false;

                    do
                    {
                        i += directionHorizontal;
                        j += directionVertical;
                        if (!isFieldCorrect(i, j))
                            findEndOfBoard = true;

                        if (!findEndOfBoard)
                        {
                            if (board[i, j] == numberPlayerOfNextRound)
                                findStonePlayer = true;
                            if (board[i, j] == 0)
                                findEmptyField = true;
                            if (board[i, j] == opponentNumber(numberPlayerOfNextRound))
                                findStoneOpponent = true;
                        }
                    }
                    while (!(findEndOfBoard || findStonePlayer || findEmptyField));

                    bool canPutStone = findStoneOpponent && findStonePlayer && !findEmptyField;

                    if (canPutStone)
                    {
                        int max_index = Math.Max(Math.Abs(i - horizontal), Math.Abs(j - vertical));

                        if (!onlyTest)
                        {
                            for (int index = 0; index < max_index; index++)
                                board[horizontal + index * directionHorizontal, vertical + index * directionVertical] = numberPlayerOfNextRound;
                        }

                        howManyFields += max_index - 1;
                    }
                }

            }

            if (howManyFields > 0 && !onlyTest)
            {
                changeCurrentPlayer();
                calculateNumbersOfFields();
            }


            return howManyFields;

        }

        public bool putStone(int horizontal, int vertical)
        {
            return putStone(horizontal, vertical, false) > 0;
        }

        private int[] numberFields = new int[3];

        private void calculateNumbersOfFields()
        {
            for (int i = 0; i < numberFields.Length; ++i) numberFields[i] = 0;
            for(int i = 0; i < WidthBoard; ++i)
                for (int j = 0; j < HeightBoard; ++j)
                    numberFields[board[i, j]]++;
        }

        public int numberEmptyFields { get { return numberFields[0]; }}
        public int numberFieldsOfPlayer1 { get { return numberFields[1]; } }
        public int numberFieldsOfPlayer2 { get { return numberFields[2]; } }

        private bool canCurrentPlayerMakeMove()
        {
            int numberOfCorrectFields = 0;
            for (int i = 0; i < WidthBoard; i++)
                for (int j = 0; j < HeightBoard; j++)
                    if (board[i, j] == 0 && putStone(i, j, true) > 0)
                        numberOfCorrectFields++;

            return numberOfCorrectFields > 0;
        }

        public void Pas()
        {
            if (canCurrentPlayerMakeMove())
                throw new Exception("Gracz nie może oddać spasować, jeżeli istnieje możliwość ruchu.");
            changeCurrentPlayer();
        }

        public enum situationOnBoard
        {
            MoveIsPossible,
            CurrentPlayerCanNotMove,
            BothPlayersCanNotMove,
            AllFieldsAreBusy
        }

        public situationOnBoard checkSituationOnBoard()
        {
            if (numberEmptyFields == 0) return situationOnBoard.AllFieldsAreBusy;

            bool canMove = canCurrentPlayerMakeMove();
            if (canMove) return situationOnBoard.MoveIsPossible;
            else
            {
                changeCurrentPlayer();
                bool canOpponentMove = canCurrentPlayerMakeMove();
                changeCurrentPlayer();
                if (canOpponentMove) return situationOnBoard.CurrentPlayerCanNotMove;
                else return situationOnBoard.BothPlayersCanNotMove;
            }
        }

        public int numberPlayerWithAdvantage
        {
            get
            {
                if (numberFieldsOfPlayer1 == numberFieldsOfPlayer2) return 0;
                else return (numberFieldsOfPlayer1 > numberFieldsOfPlayer2) ? 1 : 2;
            }
        }
    }
}
