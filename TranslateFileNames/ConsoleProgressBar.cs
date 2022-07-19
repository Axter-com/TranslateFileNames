using System;

public class ConsoleProgressBar
{
    private const ConsoleColor ForeColor = ConsoleColor.Green;
    private const ConsoleColor BkColor = ConsoleColor.Gray;
    private const int DefaultWidthOfBar = 32;
    private const int TextMarginLeft = 3;

    private readonly int _total;
    private readonly int _widthOfBar;
    private string _appendedText;
    private int _cursorTopPos = 0;

    public ConsoleProgressBar(int total, string AppendedText = "", int widthOfBar = DefaultWidthOfBar)
    {
        _appendedText = AppendedText;
        _total = total;
        _widthOfBar = widthOfBar;
    }

    private bool _intited;
    public void Init()
    {
        _lastPosition = 0;

        _cursorTopPos = Console.CursorTop;
        //Draw empty progress bar
        Console.CursorVisible = false;
        Console.CursorLeft = 0;
        Console.Write("["); //start
        Console.CursorLeft = _widthOfBar;
        Console.Write("]"); //end
        Console.CursorLeft = 1;

        //Draw background bar
        for (var position = 1; position < _widthOfBar; position++) //Skip the first position which is "[".
        {
            Console.BackgroundColor = BkColor;
            Console.CursorLeft = position;
            Console.Write(" ");
        }
    }

    public void ShowProgress(int currentCount)
    {
        if (!_intited)
        {
            Init();
            _intited = true;
        }
        DrawTextProgressBar(currentCount);
    }

    private int _lastPosition = 0;

    public void DrawTextProgressBar(int currentCount)
    {
        Console.CursorTop = _cursorTopPos;
        //Draw current chunk.
        var position = currentCount * _widthOfBar / _total;
        if (position != _lastPosition)
        {
            _lastPosition = position;
            Console.BackgroundColor = ForeColor;
            Console.CursorLeft = position >= _widthOfBar ? _widthOfBar - 1 : position;
            Console.Write(" ");
        }

        //Draw totals
        Console.CursorLeft = _widthOfBar + TextMarginLeft;
        Console.BackgroundColor = ConsoleColor.Black;
        double Perc = (double)currentCount / (double)_total;
        Console.Write(currentCount + " of " + _total + " (" + Perc.ToString("P2") + ") " + _appendedText + "        "); //blanks at the end remove any excess
    }
}

