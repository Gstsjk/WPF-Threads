using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Threads.Data;
namespace Threads
{
    public class ThreadViewModel : INotifyPropertyChanged
    {
        private Dispatcher Dispatcher;

        private IDataAccess _dataAccess;
        private ThreadGeneration _threadGeneration;
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<ThreadInfo> _threadsInfo;
        public ObservableCollection<ThreadInfo> ThreadsInfo
        {
            get { return _threadsInfo; }
            set
            {
                _threadsInfo = value;
                OnPropertyChanged(nameof(ThreadsInfo));
            }
        }
        //BUTTON
        private bool _generating = false;
        public bool Generating
        {
            get { return _generating; }
            set
            {
                _generating = value;
                OnPropertyChanged(nameof(Generating)); 
                OnPropertyChanged(nameof(GenerateText));
            }
        }

        public string GenerateText
        {
            get { return _generating ? "Stabdyti" : "Vykdyti"; }
        }

        public ICommand GenerateCommand { get; set; }
        //TEXTBOX
        private int _threadCount = 2;
        public int ThreadCount
        {
            get { return _threadCount; }
        }

        public string ThreadCountString
        {
            get { return _threadCount.ToString(); }
            set
            {
                if (ValidateThreadCount(value))
                {
                    _threadCount = int.Parse(value);
                    OnPropertyChanged(nameof(ThreadCountString));
                    OnPropertyChanged(nameof(ThreadCount));
                }
                else
                {
                    MessageBox.Show("Thread skaičius turi būti tarp 2 ir 15.", "Neteisinga įvestis", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool ValidateThreadCount(string input)
        {
            if (!int.TryParse(input, out int value))
            {
                return false;
            }
            return value >= 2 && value <= 15;
        }



        public ThreadViewModel()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            //_dataAccess = new SqlDataAccess(@"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=ThreadDB.mdf;Connect Timeout=30;");
            _dataAccess = new AccDataAccess(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=DB.accdb;");
            _threadGeneration = new ThreadGeneration(_dataAccess, Dispatcher,5,10);
            _threadsInfo = _dataAccess.GetLatest20Data();
            GenerateCommand = new RelayCommand(ToggleGeneration);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ToggleGeneration()
        {
            Generating = !_generating;
            if (_generating)
            {
                try
                {
                    _threadGeneration.Start(ThreadCount, newThreadInfo =>
                    {
                        _threadsInfo.Insert(0, newThreadInfo);
                        if (ThreadsInfo.Count > 20)
                        {
                            ThreadsInfo.RemoveAt(_threadsInfo.Count - 1);
                        }
                        OnPropertyChanged(nameof(ThreadsInfo));
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error starting thread generation: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Generating = false;
                }
            }
            else
            {
                _threadGeneration.Stop();
            }
        }
    }
}
