using MDM.Commons.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MDM.Models.ViewModels
{
    public class vmSlideStatus : vmViewModelbase
    {
        private ePageStatus _Status = ePageStatus.None;

        public vmSlideStatus(ePageStatus status)
        {
            this.Status = status;
        }

        public ePageStatus Status
        {
            get => _Status;
            set
            {
                _Status = value;
                InitializeDisplay();
                OnPropertyChanged(nameof(Status));
            }
        }

        public int StatusCode => this.Status.GetHashCode();
        public Brush StatusColor
        {
            get
            {
                switch (this.Status)
                {
                    case ePageStatus.Completed: return Brushes.Green;
                    case ePageStatus.Hold: return Brushes.Gold;
                    case ePageStatus.Exception: return Brushes.DimGray;
                    default: return Brushes.LightGray;
                }
            }
        }
        public string StatusText
        {
            get
            {
                switch (this.Status)
                {
                    case ePageStatus.Completed: return "완료";
                    case ePageStatus.Hold: return "보류";
                    case ePageStatus.Exception: return "예외";
                    default: return string.Empty;
                }
            }
        }




        public override void InitializeDisplay()
        {
            OnAllPropertyChanged();
            this.IsChanged = false;
            OnPropertyChanged(nameof(IsChanged));
        }

        public override void SetInitialData()
        {

        }

        public override object UpdateOriginData()
        {
            return null;
        }
    }
}
