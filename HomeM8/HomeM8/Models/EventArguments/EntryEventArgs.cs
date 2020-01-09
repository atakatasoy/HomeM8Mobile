using System;
using System.Collections.Generic;
using System.Text;

namespace HomeM8.Models.EventArguments
{
    public class EntryEventArgs : EventArgs
    {
        public string Name { get; set; }
        public EntryEventArgs(string name)
        {
            Name = name;
        }
    }
}
