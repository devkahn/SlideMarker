﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDM.Models.Attributes
{
    public class InsertUseAttribute : Attribute
    {
        public bool InsertUse => true;


        public InsertUseAttribute()
        {
            
        }
    }
}
