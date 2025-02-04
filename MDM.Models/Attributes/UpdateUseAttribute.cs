using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDM.Models.Attributes
{
    internal class UpdateUseAttribute : Attribute
    {
        public bool UpdateUse => true;


        public UpdateUseAttribute()
        {

        }
    }
}
