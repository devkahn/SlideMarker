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
using MDM.Models.ViewModels;
using MDM.Views.MarkChecker.Pages.XMLSettings;

namespace MDM.Views.MarkChecker.Pages
{
    /// <summary>
    /// ucMarCheckerToXmlAllSetting.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucMarCheckerToXmlAllSetting : UserControl
    {
        private Dictionary<int, UserControl> SettingPages = new Dictionary<int, UserControl>();
        private vmMaterial _Material = null;
        public vmMaterial Material
        {
            get => _Material;
            set
            {
                _Material = value;
            }
        }
        public ucMarCheckerToXmlAllSetting()
        {
            InitializeComponent();
           // SetInitialPages();
        }

        private void SetInitialPages()
        {
            UserControl page = null;
            this.SettingPages.Add(210, new ucXMLSettingBook(this.Material));
            this.SettingPages.Add(211, new ucXMLSettingChapter(this.Material));
            this.SettingPages.Add(212, new ucXMLSettingHeading1(this.Material));
            this.SettingPages.Add(213, new ucXMLSettingHeading2(this.Material));
            this.SettingPages.Add(214, new ucXMLSettingHeading3(this.Material));
            this.SettingPages.Add(215, new ucXMLSettingHeading4(this.Material));
            this.SettingPages.Add(216, new ucXMLSettingHeading5(this.Material));

            this.SettingPages.Add(221, new ucXMLSettingContentsText(this.Material));
            this.SettingPages.Add(222, new ucXMLSettingContentsImages(this.Material));
            this.SettingPages.Add(223, new ucXMLSettingContentsTable(this.Material));
            this.SettingPages.Add(224, new ucXMLSettingContentsOrderedList(this.Material));
            this.SettingPages.Add(225, new ucXMLSettingContentsUnorderedList(this.Material));
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
                        case 210: page = new ucXMLSettingBook(this.Material); break;
                        case 211: page = new ucXMLSettingChapter(this.Material); break;
                        case 212: page = new ucXMLSettingHeading1(this.Material); break;
                        case 213: page = new ucXMLSettingHeading2(this.Material); break;
                        case 214: page = new ucXMLSettingHeading3(this.Material); break;
                        case 215: page = new ucXMLSettingHeading4(this.Material); break;
                        case 216: page = new ucXMLSettingHeading5(this.Material); break;

                        case 221: page = new ucXMLSettingContentsText(this.Material); break;
                        case 222: page = new ucXMLSettingContentsImages(this.Material); break;
                        case 223: page = new ucXMLSettingContentsTable(this.Material); break;
                        case 224: page = new ucXMLSettingContentsOrderedList(this.Material); break;
                        case 225: page = new ucXMLSettingContentsUnorderedList(this.Material); break;

                        case 231: page = new ucXMLSettingContentsNote(this.Material); break;
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

        public void UpdaetOptions()
        {
            foreach (int code in SettingPages.Keys)
            {
                switch (code)
                {
                    case 210: (this.SettingPages[code] as ucXMLSettingBook).SetProperty(); break;
                    case 211: (this.SettingPages[code] as ucXMLSettingChapter).SetProperty(); break;
                    case 212: (this.SettingPages[code] as ucXMLSettingHeading1).SetProperty(); break;
                    case 213: (this.SettingPages[code] as ucXMLSettingHeading2).SetProperty(); break;
                    case 214: (this.SettingPages[code] as ucXMLSettingHeading3).SetProperty(); break;
                    case 215: (this.SettingPages[code] as ucXMLSettingHeading4).SetProperty(); break;
                    case 216: (this.SettingPages[code] as ucXMLSettingHeading5).SetProperty(); break;

                    case 221: (this.SettingPages[code] as ucXMLSettingContentsText).SetProperty(); break;
                    case 222: (this.SettingPages[code] as ucXMLSettingContentsImages).SetProperty(); break;
                    case 223: (this.SettingPages[code] as ucXMLSettingContentsTable).SetProperty(); break;
                    case 224: (this.SettingPages[code] as ucXMLSettingContentsOrderedList).SetProperty(); break;
                    case 225: (this.SettingPages[code] as ucXMLSettingContentsUnorderedList).SetProperty(); break;

                    case 231: (this.SettingPages[code] as ucXMLSettingContentsNote).SetProperty(); break;
                    default:
                        break;
                }
            }
        }
    }
}
