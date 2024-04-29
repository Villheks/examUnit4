using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;

namespace SimonGame
{
    class Program
    {
        private static GpioController s_GpioController;
        private static GpioPin[] leds = new GpioPin[4];
        private static GpioPin[] buttons = new GpioPin[4];
        private static Color[] sequence;
        private static int sequenceIndex = 0;
        private static Timer timer;
        private static bool acceptingInput = false;
        private static int round = 1;
        private static bool gameOver = false;
        private static bool debounceActive = false;
        private static int buttonsPressed = 0;
        private static DateTime gameStartTime;

        static void Main()
        {
            s_GpioController = new GpioController();

            
            int[] ledPins = { 12, 4, 15, 26 }; 
            int[] buttonPins = { 14, 16, 2, 25 }; 

            
            for (int i = 0; i < 4; i++)
            {
                leds[i] = s_GpioController.OpenPin(ledPins[i], PinMode.Output);
                buttons[i] = s_GpioController.OpenPin(buttonPins[i], PinMode.InputPullUp);

                leds[i].Write(PinValue.Low); 
            }

            
            Log("Game Started");
            gameStartTime = DateTime.UtcNow;

            
            for (int i = 0; i < 4; i++)
            {
                int buttonIndex = i;
                buttons[i].ValueChanged += (sender, args) => Button_ValueChanged(leds[buttonIndex], args, (Color)buttonIndex);
            }

            
            StartGame();

            Thread.Sleep(Timeout.Infinite);
        }

        private static void StartGame()
        {
            gameOver = false;
            buttonsPressed = 0;

            
            if (round == 1)
            {
                sequence = GenerateRandomSequence(50); 
            }

            
            Log($"Round {round} Started");

            
            DisplaySequence(round);
        }

        private static void DisplaySequence(int round)
        {
            sequenceIndex = 0;
            acceptingInput = false;

            timer = new Timer(TurnNextLedOn, round, 1000, Timeout.Infinite);
        }

        private static void TurnNextLedOn(object state)
        {
            int currentRound = (int)state;
            int sequenceLength = currentRound;

            if (sequenceIndex < sequenceLength)
            {
                leds[(int)sequence[sequenceIndex]].Write(PinValue.High);
                Thread.Sleep(1000);
                leds[(int)sequence[sequenceIndex]].Write(PinValue.Low);
                sequenceIndex++;
                timer.Change(1000, Timeout.Infinite);
            }
            else
            {
                
                sequenceIndex = 0; 
                acceptingInput = true;
            }
        }

        private static void Button_ValueChanged(GpioPin led, PinValueChangedEventArgs e, Color color)
        {
            if (acceptingInput && e.ChangeType == PinEventTypes.Falling && !gameOver && !debounceActive)
            {
                debounceActive = true;
                
                if (color == sequence[sequenceIndex])
                {
                    
                    led.Write(PinValue.High);
                    Thread.Sleep(500);
                    led.Write(PinValue.Low);
                    sequenceIndex++;

                    Log($"Button {color} Pressed - Correct");
                    buttonsPressed++;

                    int sequenceLength = round;
                    if (sequenceIndex >= sequenceLength)
                    {
                        
                        round++; 
                        Log($"Round {round - 1} Finished. Buttons Pressed: {buttonsPressed}");
                        StartGame(); 
                    }
                }
                else
                {
                    
                    Log($"Button {color} Pressed - Incorrect");
                    Log("Game Over");
                    gameOver = true;
                    OutputSummary(); 
                }
                // Start debounce timer
                Timer debounceTimer = new Timer((state) => debounceActive = false, null, 200, Timeout.Infinite);
            }
        }

        private static Color[] GenerateRandomSequence(int length)
        {
            Random random = new Random();
            Color[] sequence = new Color[length];
            for (int i = 0; i < length; i++)
            {
                sequence[i] = (Color)random.Next(4); 
            }
            return sequence;
        }

        private static void Log(string message)
        {
            
            TimeSpan elapsedTime = DateTime.UtcNow - gameStartTime;
            string elapsedTimeString = $"{elapsedTime.TotalSeconds:F0}s";

            
            Debug.WriteLine($"[{elapsedTimeString}] {message}");
        }

        private static void OutputSummary()
        {
            Log($"Game Summary - Rounds Finished: {round - 1}, Buttons Pressed: {buttonsPressed}");
        }

        
        private enum Color
        {
            Blue,
            Red,
            Yellow,
            Green
        }
    }
}
