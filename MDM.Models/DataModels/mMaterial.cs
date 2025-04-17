using MDM.Models.Attributes;
using MDM.Models.DataModels.ManualWorksXMLs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDM.Models.DataModels
{

    [TableName("t_Material")]
    public class mMaterial : mModelBase
    {
        [PropOrder(11)]
        [ColumnHeader("Name")]
        public string Name { get; set; } = string.Empty;

        [JsonIgnore]
        public mBook ManualworksBook { get; set; } = null;

    }
}
