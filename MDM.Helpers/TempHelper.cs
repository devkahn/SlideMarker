using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MDM.Models.DataModels;
using MDM.Models.ViewModels;

namespace MDM.Helpers
{
    public static class TempHelper
    {
        public static vmMaterial CreateMaterial()
        {
            mMaterial material = new mMaterial();
            material.Name = "040  수행지침_철근콘크리트공사 - 데이터기반 품질관리시스템 추가";

            vmMaterial newMater = new vmMaterial(material);
            return newMater;
        }
    }
}
