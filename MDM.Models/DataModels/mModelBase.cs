using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel;
using MDM.Models.Attributes;

namespace MDM.Models.DataModels
{
    public class mModelBase
    {
        [PropOrder(1)]
        [SelectUse][UpdateUse][DeleteUse]
        [ColumnHeader("Idx")]
        public int Idx { get; set; } = -1;

        [PropOrder(2)]
        [InsertUse][SelectUse]
        [ColumnHeader("Uid")]
        public string Uid { get; set; } = Guid.NewGuid().ToString();

        [PropOrder(3)]
        [InsertUse][SelectUse][UpdateUse]
        [ColumnHeader("ParentUid")]
        public string ParentUid { get; set; } = string.Empty;

        [PropOrder(4)]
        [InsertUse][SelectUse][UpdateUse][DeleteUse]
        [ColumnHeader("IsUsed")]
        public bool IsUsed { get; set; } = true;

        [PropOrder(5)]
        [InsertUse][SelectUse]
        [ColumnHeader("CreateDate")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [PropOrder(6)]
        [InsertUse][SelectUse][UpdateUse]
        [ColumnHeader("UpdateDate")]
        public DateTime? UpdateDate { get; set; } = null;

        [PropOrder(7)]
        [SelectUse][UpdateUse][DeleteUse]
        [ColumnHeader("DeleteDate")]
        public DateTime? DeletedDate { get; set; } = null;


        public T Copy<T>() where T : new()
        {
            T output = new T();

            PropertyInfo[] prorperties = typeof(T).GetProperties();
            foreach (var property in prorperties)
            {
                if (property.CanRead && property.CanWrite)
                {
                    property.SetValue(output, property.GetValue(this));
                }
            }

            return output;
        }
    }
}
