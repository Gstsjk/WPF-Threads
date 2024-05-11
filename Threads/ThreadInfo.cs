using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Threads
{
    public class ThreadInfo : EventArgs
    {
        public int Num {  get; set; }

        public string Message { get; set; }
        
        public ThreadInfo(int num, string message)
        {
            Num = num;
            Message = message;
        }
    }


}
