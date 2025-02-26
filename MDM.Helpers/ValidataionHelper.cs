using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using MDM.Commons.Enum;
using MDM.Models.ViewModels;

namespace MDM.Helpers
{
    public static class SlideValidataionHelper
    {

    }

    public static class HeaderValidationHelper
    {
        // 


    }

    public static class TextValidationHelper
    {

    }
    public static class ImageValidationHelper
    {
        public static Dictionary<int, bool> IsImageFilesExist(vmSlide parentSlide)
        {
            Dictionary<int, bool> output = new Dictionary<int, bool>();

            foreach (vmItem item in parentSlide.Items)
            {
                if (item.ItemType != Commons.Enum.eItemType.Image) continue;

                int index = parentSlide.Items.IndexOf(item);
                output.Add(index, true);

                string imageDirPath = item.GetImageDirPath();
                string imageFilePath = Path.Combine(imageDirPath, item.Temp.Uid + ".png");
                output[index] = File.Exists(imageFilePath);
            }

            return output;
        }
        public static Dictionary<int, bool> IsImageTextsValid(vmSlide parentSlide)
        {
            Dictionary<int, bool> output = new Dictionary<int, bool>();

            foreach(vmItem item in parentSlide.Items)
            {
                if (item.ItemType != eItemType.Image) continue;

                int index = parentSlide.Items.IndexOf(item);
                output.Add(index, true);

                string text = item.Temp.LineText;
                Match match = TextHelper.IsImageMarkdown(text);
                output[index] = match.Success;
            }

            return output;
        }
    }
    public static class TableValidationHelper
    {
        public static Dictionary<int, bool> IsTableStringValid(vmSlide parentSlide)
        {
            Dictionary<int, bool> output = new Dictionary<int, bool>();



            return output;
        }

    }


}
