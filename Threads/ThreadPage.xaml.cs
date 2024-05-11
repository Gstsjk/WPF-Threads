using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace Threads
{
    public partial class ThreadPage : Window
    {
        public ThreadPage()
        {
            InitializeComponent();
            DataContext = new ThreadViewModel();
        }

    }

}
