class Programm
{
    static Snake mySnake;
    static Walls myWalls;
    static Timer myTime;
    static Food myFood;
    public static int LeftWall { get; set; }
    public static int RightWall { get; set; }
    public static int UpWall { get; set; }
    public static int DownWall { get; set; }
    public static int StartLength { get; set; }
    public static int Hight {
        get { return DownWall - UpWall - 1; } }
    public static int Width
    {
        get { return RightWall - LeftWall - 1; }
    }

    static void Main()
    {
        Console.SetWindowSize(90, 30);
        Console.SetBufferSize(90, 30);
        Console.CursorVisible = false;
        LeftWall = 5;
        UpWall = 5;
        DownWall = 28;
        RightWall = 88;
        StartLength = 6;

        myWalls = new Walls(new Point(LeftWall, UpWall, ' '), new Point(RightWall, DownWall, ' '));
        myWalls.Draw();
        mySnake = new Snake(new Point(Width / 2 - StartLength /2, Hight / 2, 'O'), StartLength);        
        myFood = new Food(new Point(LeftWall, UpWall, ' '), new Point(RightWall, DownWall, ' '));
        myFood.FoodPosition(mySnake, LeftWall, UpWall, RightWall, DownWall);
        myFood.Draw();
        myTime = new Timer(GlobalDraw, null, 0, 300);
        while (true)
        {
            if (Console.KeyAvailable)
            {
                if (!mySnake.IsRotated)
                {
                    ConsoleKeyInfo key = Console.ReadKey();
                    mySnake.Rotate(key.Key);
                }
            }
        }
    }
    static void GlobalDraw(object state)
    {
        Console.SetCursorPosition(1, 1);
        Console.Write($"Current score: {mySnake.Points.Count}");
        Point nextPoint = mySnake.NextPoint();
        if (mySnake.CrashByWalls(myWalls.Points, nextPoint) || mySnake.CrashBySnake(nextPoint))
        {
            Console.SetCursorPosition(1, 1);
            Console.Write("The end");
            myTime.Change(Timeout.Infinite, Timeout.Infinite);
        }
        else if (nextPoint.Equals(myFood))
        {
            mySnake.Growth(nextPoint);
            myFood.FoodPosition(mySnake, LeftWall, UpWall, RightWall, DownWall);
            myFood.Draw();
            mySnake.IsRotated = false;
        }
        else
        {
            mySnake.Move(nextPoint);
            mySnake.IsRotated = false;
        }
        mySnake.Draw(); 
    }
}
class Point
{
    public int X { get; set; }
    public int Y { get; set; }
    public char Symbol { get; set; }
    public Point(int x, int y, char symbol)
    {
        X = x;
        Y = y;
        Symbol = symbol;
    }
    public void Draw()
    {
        Console.SetCursorPosition(X, Y);
        Console.Write(Symbol);
    }
    public void Delete()
    {
        Console.SetCursorPosition(X, Y);
        Console.Write(' ');
    }
    public bool Equals(Point a)
    {
        return a.X == X && a.Y == Y;
    }
}
class Walls
{
    public List<Point> Points { get; set; } = new List<Point>();
    public Walls(Point startpoint,Point finalpoint)
    {
        for (int i = startpoint.Y+1; i < finalpoint.Y; i++)
        {
            Points.Add(new Point(startpoint.X, i, '|'));
            Points.Add(new Point(finalpoint.X, i, '|'));
        }
        for (int i = startpoint.X; i < finalpoint.X+1; i++)
        {
            Points.Add(new Point(i, startpoint.Y, '-'));
            Points.Add(new Point(i, finalpoint.Y, '-'));
        }
    }
    public void Draw()
    {
        foreach(var myPoint in Points)
        {
            myPoint.Draw();
        }
    }
}
class Food : Point
{
    public int startpointX, startpointY, finalpointX, finalpointY;
    public Random random;

    public Food(Point startpoint, Point finalpoint) : base(0, 0, '@')
    {
        startpointX = startpoint.X;
        startpointY = startpoint.Y;
        finalpointX = finalpoint.X;
        finalpointY = finalpoint.Y;
        random = new Random();
    }

    public void FoodPosition(Snake snake, int leftwall, int upwall, int rightwall, int downwall)
    {
        List <int> snakeToNumber = new List <int>();
        foreach (var point in snake.Points)
        {
            snakeToNumber.Add(point.X - leftwall + (point.Y - upwall) * (rightwall - leftwall - 1));
        }

        Dictionary <int, int> mapping = new Dictionary<int, int> ();
        int j = 0;
        for (int i = 1; i < (rightwall - leftwall - 1) * (downwall - upwall - 1); i++)
        {
            if (snakeToNumber.Contains(i))
            {
                j++;
                continue;
            }
            else {
                mapping.Add(i - j, i);
            }
        }
        int food = random.Next(1, (rightwall - leftwall - 1) * (downwall - upwall - 1) - snake.Points.Count);
        int finalpoint = mapping[food];
        X = (finalpoint - 1) % (rightwall - leftwall - 1) + 1 + leftwall;
        Y = finalpoint / (rightwall - leftwall - 1) + 1 + upwall;
        
    }
}
 class Snake
{
    public int length;
    public Direction currentdirection;
    public bool IsRotated { get; set; }
    public Point Head {
        get
        { 
            return Points.Last();
        }
    }
    public Point Tail { 
        get
        {
            return Points.First();
        }
     }
    public char Symbol { get; set; }
    public List<Point> Points { get; set; } = new List<Point>();
    public Snake(Point startpoint, int length)
    {
        Symbol = startpoint.Symbol;
        for (int i=0; i < length; i++)
        {
            Points.Add(new Point (startpoint.X + i, startpoint.Y, Symbol));
        }
        currentdirection = Direction.Right;
        IsRotated = false;

    }
    public void Rotate(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.LeftArrow:
                if (currentdirection != Direction.Right)
                {
                    currentdirection = Direction.Left;
                    IsRotated = true;
                }
                break;
            case ConsoleKey.RightArrow:
                if (currentdirection != Direction.Left)
                {
                    currentdirection = Direction.Right;
                    IsRotated = true;
                }
                break;
            case ConsoleKey.UpArrow:
                if (currentdirection != Direction.Down)
                {
                    currentdirection = Direction.Up;
                    IsRotated = true;
                }
                break;
            case ConsoleKey.DownArrow:
                if (currentdirection != Direction.Up)
                {
                    currentdirection = Direction.Down;
                    IsRotated = true;
                }
                break;
        }
    }
    public void Draw()
    {
        foreach (var myPoint in Points)
        {
            myPoint.Draw();
        }
    }
    public void Move(Point nextPoint)
    {
        Points.Add(nextPoint);
        Tail.Delete();
        Points.Remove(Tail);
    }
    public void Growth(Point nextPoint)
    {
        Points.Add(nextPoint);
    }
    public bool CrashByWalls(List<Point> walls, Point nextPoint)
    {
        foreach (var wall in walls)
        {
            if (nextPoint.Equals(wall))
                return true;
        }
        return false;
    }
    public bool CrashBySnake(Point nextPoint)
    {
        foreach (var point in Points)
        {
            if (nextPoint.Equals(point) && Head != point)
                return true;
        }
        return false;
    }    
    public Point NextPoint()
    {
        switch (currentdirection)
        {
            case Direction.Left:
                {
                    return new Point(Head.X - 1, Head.Y, Symbol);                  
                }
            case Direction.Right:
                {
                    return new Point(Head.X + 1, Head.Y, Symbol);                 
                }
            case Direction.Down:
                {
                    return new Point(Head.X, Head.Y + 1, Symbol);                   
                }
            case Direction.Up:
                {
                    return new Point(Head.X, Head.Y - 1, Symbol);                   
                }
            default: return null;
        }
    }
}
enum Direction
{
    Up, Right, Down, Left
}