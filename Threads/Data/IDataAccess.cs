using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Threads.Data
{
    public interface IDataAccess
    {
        bool InsertData(int num, string message);
        ObservableCollection<ThreadInfo> GetLatest20Data();
    }
}
