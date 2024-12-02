﻿using System;
using System.Text;
using System.Text.RegularExpressions;

// Reprezentuje pojedynczą figurę szachową, w tym jej typ (np. pion, wieża)
// oraz kolor (biały lub czarny).


public class CommandManager
{
    private readonly Stack<ICommand> executedCommands = new();
    private readonly Stack<ICommand> undoneCommands = new();
    private readonly ChessBoard board;
    private readonly List<string> history = new();

    public CommandManager(ChessBoard board)
    {
        this.board = board;
    }

    public void ExecuteCommand(ICommand command)
    {
        command.Execute();
        executedCommands.Push(command);
        undoneCommands.Clear();
        history.Add(board.SaveState());
    }

    public void Undo()
    {
        if (executedCommands.TryPop(out var command))
        {
            command.Undo();
            undoneCommands.Push(command);
            history.Add(board.SaveState());
        }
    }

    public void Redo()
    {
        if (undoneCommands.TryPop(out var command))
        {
            command.Execute();
            executedCommands.Push(command);
            history.Add(board.SaveState());
        }
    }

    public void Replay()
    {
        foreach (var state in history)
        {
            Console.Clear();
            board.LoadState(state);
            Console.WriteLine(board);
            Thread.Sleep(1000); // 1-sekundowa pauza
        }
    }
}





public interface ICommand
{
    void Execute();
    void Undo();
}

public class MoveCommand: ICommand
{
    private readonly ChessBoard board;
    private readonly int fromRow,fromCol,toRow,toCol;
    private ChessPiece movedPiece;
    private ChessPiece capturedPiece;

    public MoveCommand(ChessBoard board, int fromRow, int fromCol, int toRow, int toCol)
    {
        this.board = board;
        this.fromRow = fromRow;
        this.fromCol = fromCol;
        this.toRow = toRow;
        this.toCol = toCol;
    }

    public void Execute()
    {
        movedPiece = board.GetPiece(fromRow, fromCol);
        board.MovePiece(fromRow, fromCol, toRow, toCol, out capturedPiece);
    }

    public void Undo()
    {
        board.SetPiece(fromRow, fromCol, movedPiece);
        board.SetPiece(toRow, toCol, capturedPiece);
    }





}

public class ChessPiece
{
    public enum PieceType { Pawn, Rook, Knight, Bishop, Queen, King }
    public enum PieceColor { White, Black }

    public PieceType Type { get; }
    public PieceColor Color { get; }

    public ChessPiece(PieceType type, PieceColor color)
    {
        Type = type;
        Color = color;
    }

    // Zwraca kod literowy figury, używany do wyświetlania jej na planszy.
    public char GetCode()
    {
        // Dopasowanie litery do typu figury
        char baseCode = Type switch
        {
            PieceType.Pawn => 'P',
            PieceType.Rook => 'R',
            PieceType.Knight => 'N',
            PieceType.Bishop => 'B',
            PieceType.Queen => 'Q',
            PieceType.King => 'K',
            _ => '?'
        };

        // Zwrócenie wielkiej litery dla białych figur, małej dla czarnych
        return Color == PieceColor.White ? baseCode : char.ToLower(baseCode);
    }

    // Tworzy obiekt figury szachowej na podstawie kodu literowego.
    public static ChessPiece FromCode(char code)
    {
        // Określenie koloru figury na podstawie wielkości litery
        var color = char.IsUpper(code) ? PieceColor.White : PieceColor.Black;

        // Dopasowanie typu figury do litery
        var type = char.ToUpper(code) switch
        {
            'P' => PieceType.Pawn,
            'R' => PieceType.Rook,
            'N' => PieceType.Knight,
            'B' => PieceType.Bishop,
            'Q' => PieceType.Queen,
            'K' => PieceType.King,
            _ => throw new ArgumentException($"Nieznany kod figury: {code}")
        };

        // Zwrócenie nowej figury
        return new ChessPiece(type, color);
    }
}

// Reprezentuje szachownicę, przechowując stan wszystkich pól i figur.
// Umożliwia ustawianie, usuwanie i przesuwanie figur na planszy.
public class ChessBoard
{
    private readonly ChessPiece[,] board;

    public ChessBoard()
    {
        board = new ChessPiece[8, 8];
    }

    public ChessPiece GetPiece(int row, int col)
    {
        return board[row, col];
    }

    public void SetPiece(int row, int col, ChessPiece piece)
    {
        board[row, col] = piece;
    }

    public void RemovePiece(int row, int col)
    {
        board[row, col] = null;
    }

    public void MovePiece(int fromRow, int fromCol, int toRow, int toCol, out ChessPiece capturedPiece)
    {
        ChessPiece piece = GetPiece(fromRow, fromCol);
        capturedPiece = GetPiece(toRow, toCol);

        RemovePiece(fromRow, fromCol);
        SetPiece(toRow, toCol, piece);
    }

    public string SaveState()
    {
        var sb = new StringBuilder();
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                ChessPiece piece = board[row, col];
                if (piece != null)
                {
                    sb.Append(piece.GetCode());
                    sb.Append((char)('a' + col));
                    sb.Append(8 - row);
                    sb.Append(" ");
                }
            }
        }
        return sb.ToString().Trim();
    }

    public void LoadState(string state)
    {
        InitializeFromString(state);
    }

    public void InitializeFromString(string notation)
    {
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                board[row, col] = null;
            }
        }

        string[] pieces = Regex.Split(notation.Trim(), "\\s+");
        foreach (string pieceNotation in pieces)
        {
            char code = pieceNotation[0];
            char colChar = pieceNotation[1];
            char rowChar = pieceNotation[2];

            int col = colChar - 'a';
            int row = 8 - (rowChar - '0');

            ChessPiece piece = ChessPiece.FromCode(code);
            board[row, col] = piece;
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine("  a b c d e f g h");

        for (int row = 0; row < 8; row++)
        {
            sb.Append(8 - row + " ");
            for (int col = 0; col < 8; col++)
            {
                ChessPiece piece = board[row, col];
                sb.Append(piece != null ? piece.GetCode() + " " : ". ");
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        // Tworzymy nową szachownicę i ustawiamy początkowy (domyślny) układ
        ChessBoard board = new ChessBoard();
        string defaultNotation = @"
            ra8 nb8 bc8 qd8 ke8 bf8 ng8 rh8
            pa7 pb7 pc7 pd7 pe7 pf7 pg7 ph7
            Pa2 Pb2 Pc2 Pd2 Pe2 Pf2 Pg2 Ph2
            Ra1 Nb1 Bc1 Qd1 Ke1 Bf1 Ng1 Rh1
        ";
        board.InitializeFromString(defaultNotation);

        // Wyświetlenie początkowego stanu planszy
        Console.WriteLine(board);

        // Pętla gry: użytkownik wprowadza ruchy w formacie "e2 e4"
        while (true)
        {
            Console.Write("Wprowadź ruch (np. e2 e4 lub 'exit' aby zakończyć): ");
            string input = Console.ReadLine();

            // Wyjście z gry
            if ("exit".Equals(input, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Gra zakończona.");
                break;
            }

            try
            {
                // Rozdzielenie ruchu na pole startowe i końcowe
                string[] parts = input.Split(' ');
                if (parts.Length != 2)
                {
                    Console.WriteLine("Nieprawidłowy format. Użyj formatu 'e2 e4'.");
                    continue;
                }

                string from = parts[0]; // Pole startowe
                string to = parts[1];   // Pole docelowe

                // Konwersja notacji szachowej na indeksy tablicy
                int fromRow = 8 - (from[1] - '0');
                int fromCol = from[0] - 'a';
                int toRow = 8 - (to[1] - '0');
                int toCol = to[0] - 'a';

                // Wykonanie ruchu i aktualizacja planszy
                if (board.MovePiece(fromRow, fromCol, toRow, toCol))
                {
                    Console.WriteLine(board);
                }
                else
                {
                    Console.WriteLine("Nie można wykonać ruchu.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Błąd podczas przetwarzania ruchu: {e.Message}");
            }
        }
    }
}
