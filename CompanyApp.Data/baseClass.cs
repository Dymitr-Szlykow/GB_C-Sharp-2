using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CompanyApp.Data
{
    public abstract class DatabaseEntity : INotifyPropertyChanged, ICloneable
    {
        public abstract int ID { get; set; }
        public abstract string Tablename { get; }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public object Clone() => MemberwiseClone();
    }
}
