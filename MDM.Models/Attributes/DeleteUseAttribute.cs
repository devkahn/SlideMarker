using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDM.Models.Attributes
{
    public class DeleteUseAttribute : Attribute
    {
        public bool DeleteUse => true;


        public DeleteUseAttribute()
        {

        }
    }
}