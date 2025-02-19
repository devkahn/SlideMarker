using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MDM.Helpers;
using MDM.Views.MarkChecker.Pages.XMLSettings;

namespace MDM.Views.MarkChecker.Pages
{
    /// <summary>
    /// ucMarCheckerToXmlAllSetting.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucMarCheckerToXmlAllSetting : UserControl
    {
        Dictionary<int, UserControl> SettingPages = new Dictionary<int, UserControl>();

        public ucMarCheckerToXmlAllSetting()
        {
            InitializeComponent();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                RadioButton rBtn = sender as RadioButton;
                if (rBtn == null) return;

                string uid = rBtn.Uid;
                bool isCodeValid = int.TryParse(uid, out int code);
                if (!isCodeValid) code = -1;

                if (!this.SettingPages.ContainsKey(code))
                {
                    UserControl page = null;
                    switch (code)
                    {
                        case 210: page = new ucXMLSettingBook(); break;
                        case 211: page = new ucXMLSettingChapter(); break;
                        case 212: page = new ucXMLSettingHeading1(); break;
                        case 213: page = new ucXMLSettingHeading2(); break;
                        case 214: page = new ucXMLSettingHeading3(); break;
                        case 215: page = new ucXMLSettingHeading4(); break;
                        case 216: page = new ucXMLSettingHeading5(); break;

                        case 221: page = new ucXMLSettingContentsText(); break;
                        case 222: page = new ucXMLSettingContentsImages(); break;
                        case 223: page = new ucXMLSettingContentsTable(); break;
                        default:
                            break;
                    }
                    this.SettingPages[code] = page;
                }

                this.contentpresenter.Content = this.SettingPages[code];

            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
    }
}
