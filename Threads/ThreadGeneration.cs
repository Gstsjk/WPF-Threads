using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Threads.Data;

namespace Threads
{
    public class ThreadGeneration
    {
        private static readonly Random random = new();
        private readonly int _minLength;
        private readonly int _maxLength;
        private readonly IDataAccess _dataAccess;
        private bool _isGenerating;
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private Dispatcher _UIThread;
        public ThreadGeneration(IDataAccess dataAccess, Dispatcher dispatcher, int min, int max)
        {
            _isGenerating = false;
            _dataAccess = dataAccess;
            _UIThread = dispatcher;
            _minLength = min;
            _maxLength = max;
        }

        public void Start(int n, Action<ThreadInfo> onThreadInfoAdded)
        {
            if(!_isGenerating)
            {
                _isGenerating = true;
                for (int i = 1; i <= n; i++)
                {
                    int threadNum = i;

                    Thread thread = new Thread(() =>
                    {
                        while (_isGenerating)
                        {
                            string generatedString = GenerateRandomString(_minLength, _maxLength);
                            ThreadInfo newThreadInfo = new ThreadInfo(threadNum, generatedString);

                            if (SaveToDatabase(threadNum, generatedString))
                            {
                                _UIThread.Invoke(() => onThreadInfoAdded?.Invoke(newThreadInfo));
                            }
                            else
                            {
                                Console.WriteLine("Failed to Save");
                            }

                            Thread.Sleep(random.Next(500, 2001));
                        }
                    });
                    thread.Start();
                }

            }
        }

        public void Stop()
        {
            _isGenerating = false;
        }

        private string GenerateRandomString(int minLength, int maxLength)
        {
            int stringLength = random.Next(minLength, maxLength+1);

            return new string(Enumerable.Repeat(Chars, stringLength)
                            .Select(s => s[random.Next(s.Length)]).ToArray());

        }

        private bool SaveToDatabase(int threadNum, string message)
        {
           return _dataAccess.InsertData(threadNum, message);
        }
         
    }

}
