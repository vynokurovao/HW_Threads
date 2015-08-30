using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HW_Threads_task2
{
    public class ChainDisplayer
    {
        private enum State
        {
            GenerateChainLength,
            GenerateNextChar,
            GenerateSpaceLength,
            GenerateSpace
        };

        private const char DELIMITER = ' ';

        private State _state = State.GenerateChainLength;
        private int _remainSteps = 0;

        // circular buffer
        private char[] _consoleColumn = new char[Console.WindowHeight];
        private int _bufferBegin = 0;

        private object _lockObject;
        private Random _random;
        private int _columnIndex;

        public ChainDisplayer(object lockObject, int columnIndex)
        {
            _columnIndex = columnIndex;

            for (int i = 0; i < Console.WindowHeight; i++)
            {
                _consoleColumn[i] = DELIMITER;
            }

            _random = new Random(_columnIndex);
            _lockObject = lockObject;
        }

        public void Display()
        {
            int delay = _random.Next(100, 1000);

            while (true)
            {
                processNextStep();
                DisplayChain();
                UpdateColumn();
                Thread.Sleep(delay);
            }
        }

        /*
            simple state machine implementation
        */
        private void processNextStep()
        {
            if (_state == State.GenerateChainLength)
            {
                _remainSteps = GenerateChainLength();
                _state = State.GenerateNextChar;
                processNextStep();
            }
            else if (_state == State.GenerateNextChar)
            {
                if (_remainSteps > 0)
                {
                    AddToBuffer(GenerateChar());
                    _remainSteps--;
                }
                else
                {
                    _state = State.GenerateSpaceLength;
                    processNextStep();
                }
            }
            else if (_state == State.GenerateSpaceLength)
            {
                _remainSteps = GenerateSpaceLength();
                _state = State.GenerateSpace;
                processNextStep();
            }
            else if (_state == State.GenerateSpace)
            {
                if (_remainSteps > 0)
                {
                    AddToBuffer(DELIMITER);
                    _remainSteps--;
                }
                else
                {
                    _state = State.GenerateChainLength;
                    processNextStep();
                }
            }
        }

        private void AddToBuffer(char ch)
        {
            _consoleColumn[_bufferBegin] = ch;
            _bufferBegin = (_bufferBegin + 1) % _consoleColumn.Length;
        }

        private void UpdateColumn()
        {            
            for (int i = 0; i < _consoleColumn.Length; i++)
            { 
                if (_consoleColumn[i] != DELIMITER)
                {
                    _consoleColumn[i] = GenerateChar();
                }
            }
        }

        private void DisplayChain()
        {
            lock (_lockObject)
            {
                char prevPrevChar = '\0';
                char prevChar = '\0';

                for (int i = 0; i < _consoleColumn.Length; i++)
                {
                    ConsoleColor color = ConsoleColor.DarkGreen;
                    int pos = (_bufferBegin + i) % _consoleColumn.Length;
                    char currChar = _consoleColumn[pos];

                    if (currChar != DELIMITER && prevChar == DELIMITER)
                    {
                        color = ConsoleColor.White;
                    }
                    else if (currChar != DELIMITER && prevPrevChar == DELIMITER)
                    {
                        color = ConsoleColor.Green;
                    }

                    WriteAt(currChar, Console.WindowHeight - i - 1, color);
                    prevPrevChar = prevChar;
                    prevChar = currChar;
                }
            }
        }

        private int GenerateChainLength()
        {
            return _random.Next(5, Console.WindowHeight / 2);
        }

        private int GenerateSpaceLength()
        {
            return _random.Next(Console.WindowHeight / 2, Console.WindowHeight);
        }

        private char GenerateChar()
        {
            // "pretty" alphanumerical chars range
            return (char)_random.Next(48, 90);
        }

        private void WriteAt(char ch, int row, ConsoleColor color)
        {
            try
            {
                Console.ForegroundColor = color;
                Console.SetCursorPosition(_columnIndex, row);
                Console.Write(ch);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.Clear();
                Console.WriteLine(e.Message);
            }
        }
    }
}
