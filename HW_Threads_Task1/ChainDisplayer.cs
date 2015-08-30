using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HW_Threads_Task1
{
    public class ChainDisplayer
    {
        private const char DELIMITER = ' ';
        private int _chainLength;
        private object _lockObject;
        private Random _random;
        private int _columnIndex;

        public ChainDisplayer(object lockObject, int columnIndex)
        {
            _columnIndex = columnIndex;
            _random = new Random(_columnIndex);
            _lockObject = lockObject;
            GenerateChainLength();
        }

        public void Display()
        {
            int delay = _random.Next(100, 200);
            while(true)
            {
                for (int i = 0; i < Console.WindowHeight + _chainLength; i++)
                {
                    DisplayChain(i, GenerateChain());
                    Thread.Sleep(delay);
                }
            }
        }

        private void DisplayChain(int row, char[] chain)
        {
            lock (_lockObject)
            {
                for (int i = 0; i < chain.Length; i++)
                {
                    int pos = row - i;
                    if (pos < 0 || pos >= Console.WindowHeight)
                    {
                        continue;
                    }                   

                    WriteAt(chain[chain.Length - i - 1], pos, GetColorByIndex(i));
                }

                int oldLastPos = row - chain.Length;
                if (oldLastPos >= 0)
                {
                    WriteAt(DELIMITER, oldLastPos, ConsoleColor.White);
                }
            }
        }

        private void GenerateChainLength()
        {
            _chainLength = _random.Next(5, Console.WindowHeight / 2);
        }

        private char[] GenerateChain()
        {
            char[] chain = new char[_chainLength];

            for (int i = 0; i < _chainLength; i++)
            {
                // alphanumerical chars range
                chain[i] = (char)_random.Next(48, 90);
            }

            return chain;
        }

        private ConsoleColor GetColorByIndex(int index)
        {
            switch (index)
            {
                case 0:
                    return ConsoleColor.White;
                case 1:
                    return ConsoleColor.Green;
                default:
                    return ConsoleColor.DarkGreen;
            }
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
