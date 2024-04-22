

class Button {
    public string Color { get; set; }
    public Button(string color) {
        Color = color;
    }
}

class Game {
    private List<Button> buttons;
    private Random random;
    private int difficultyLevel;
    private int sequenceLength;
    
    public Game(int difficultyLevel) {
        buttons = new List<Button>();
        buttons.Add(new Button("Red"));
        buttons.Add(new Button("Green"));
        buttons.Add(new Button("Blue"));
        buttons.Add(new Button("Yellow"));
        random = new Random();
        this.difficultyLevel = difficultyLevel;
        sequenceLength = 4 + (difficultyLevel * 2); 
    }

    public void Start() {
        Console.WriteLine("Welcome to the Sequence Recall Game!");
        Console.WriteLine($"Difficulty Level: {difficultyLevel}");
        Console.WriteLine("Remember the sequence and press the buttons in the correct order.");

        List<Button> sequence = GenerateSequence();
        DisplaySequence(sequence);

        bool success = TakeUserInput(sequence);
        if (success) {
            Console.WriteLine("Congratulations! You remembered the sequence.");
        } else {
            Console.WriteLine("Sorry, you missed the sequence. Better luck next time!");
        }
    }

    private List<Button> GenerateSequence() {
        List<Button> sequence = new List<Button>();
        for (int i = 0; i < sequenceLength; i++) {
            int randomIndex = random.Next(0, buttons.Count);
            sequence.Add(buttons[randomIndex]);
        }
        return sequence;
    }

    private void DisplaySequence(List<Button> sequence) {
        foreach (Button button in sequence) {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write($"{button.Color} ");
            Thread.Sleep(1000); 
            Console.ResetColor();
            Thread.Sleep(500); 
        }
        Console.WriteLine();
    }

    private bool TakeUserInput(List<Button> sequence) {
        Console.WriteLine("Enter the sequence:");
        for (int i = 0; i < sequence.Count; i++) {
            string input = Console.ReadLine().Trim().ToLower();
            if (input != sequence[i].Color.ToLower()) {
                return false;
            }
        }
        return true;
    }
}

class Program {
    static void Main(string[] args) {
        Console.WriteLine("Please enter your age:");
        int age = int.Parse(Console.ReadLine());

        
        int difficultyLevel = 0;
        if (age >= 13 && age < 16) {
            difficultyLevel = 1;
        } else if (age >= 16 && age < 18) {
            difficultyLevel = 2;
        } else if (age >= 18) {
            difficultyLevel = 3;
        }

        Game game = new Game(difficultyLevel);
        game.Start();
    }
}
