using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MDM.Models.Intefaces;

namespace MDM.Models.ViewModels
{
    public abstract class vmViewModelbase : INotifyPropertyChanged, IViewModel
    {
        public bool IsChanged { get; set; } = false;
    

        public vmViewModelbase()
        {
            SetInitialData();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public abstract void SetInitialData();
        public abstract object UpdateOriginData();
        public abstract void InitializeDisplay();


        protected void OnAllPropertyChanged()
        {
            if(PropertyChanged != null)
            {
                foreach (PropertyInfo item in this.GetType().GetProperties())
                {
                    if (string.Equals(item.Name, nameof(IsChanged))) continue;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(item.Name));
                }
                this.IsChanged = true;
                OnPropertyChanged(nameof(this.IsChanged));
            }
        }
        internal void OnPropertyChanged([CallerMemberName] string propertyName = "", bool isChanged = true)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (string.Equals(propertyName, nameof(IsChanged))) return;
            if (isChanged)
            {
                this.IsChanged = true;
                OnPropertyChanged(nameof(this.IsChanged));
            }
        }
        public void OnModifyStatusChanged(bool isChanged = false)
        {
            this.IsChanged = isChanged;
            OnPropertyChanged(nameof(this.IsChanged));
        }
    }
}
