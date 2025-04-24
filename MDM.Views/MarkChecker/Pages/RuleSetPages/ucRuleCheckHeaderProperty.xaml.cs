using MDM.Commons.Enum;
using MDM.Helpers;
using MDM.Models.DataModels;
using MDM.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
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

namespace MDM.Views.MarkChecker.Pages.RuleSetPages
{
    /// <summary>
    /// ucRuleCheckHeaderProperty.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucRuleCheckHeaderProperty : UserControl
    {
        private vmMaterial _Material = null;
        public vmMaterial Material
        {
            get => _Material;
            set
            {
                _Material = value;
                BindHeaders();

            }
        }



        public ucRuleCheckHeaderProperty()
        {
            InitializeComponent();
        }


        private void BindHeaders()
        {
            this.listbox_headers.ItemsSource = null;
            if (this.Material == null) return;

            ObservableCollection<vmHeading> headings = new ObservableCollection<vmHeading>();

            foreach (vmHeading item in this.Material.Headings)
            {
                if (item.IsEnabled == false) continue;
                headings.Add(item);
            }

            this.listbox_headers.ItemsSource = headings;
        }
        private void BindHeaders(int level)
        {
            this.listbox_headers.ItemsSource = null;
            if (this.Material == null) return;
            if (level < 0) { BindHeaders(); return; }

            ObservableCollection<vmHeading> headings = new ObservableCollection<vmHeading>();

            foreach (vmHeading item in this.Material.Headings)
            {
                if (item.IsEnabled == false) continue;
                if (item.Temp.Level == level) headings.Add(item);
            }

            this.listbox_headers.ItemsSource = headings;
        }
        private void BindHeaders(string keyword)
        {
            this.listbox_headers.ItemsSource = null;
            if (this.Material == null) return;
            if (string.IsNullOrEmpty(keyword)){ BindHeaders(); return; }

            ObservableCollection<vmHeading> headings = new ObservableCollection<vmHeading>();

            foreach (vmHeading item in this.Material.Headings)
            {
                if (item.IsEnabled == false) continue;
                if (item.Temp.Name.ToLower().Contains(keyword.ToLower())) headings.Add(item);
            }

            this.listbox_headers.ItemsSource = headings;
        }


        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                this.Material = e.NewValue as vmMaterial;
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_Search_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;

                string uid = btn.Uid;

                switch (uid)
                {
                    case "byLevel":
                        bool isDigit = int.TryParse(this.txtbox_LevelValue.Text, out int level);
                        if (!isDigit) level = -1;
                        BindHeaders(level);
                        return;
                    case "byKeyword":
                        string keyword = this.txtbox_SearchKeyword.Text;
                        BindHeaders(keyword);
                        return;
                    default: BindHeaders(); return;
                }

            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_AllSelect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;

                string uid = btn.Uid;

                int code = int.Parse(uid);
                if (code == 1) this.listbox_headers.SelectAll();
                else this.listbox_headers.UnselectAll();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_LineControl_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;

                string uid = btn.Uid;
                foreach (vmHeading heading in this.listbox_headers.SelectedItems)
                {
                    if (heading == null) continue;

                    string output = string.Empty;
                    switch (uid)
                    {
                        case "toOne": output = ChangeNameToOne(heading.Temp.Name); break;
                        case "removeEmpty": output = RemoveNoTextLine(heading.Temp.Name); break;
                        default: output = heading.Temp.Name; break;
                    }
                    if(output != heading.Temp.Name) heading.SetName(output);
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private string RemoveNoTextLine(string input)
        {
            string output = string.Empty;

            string[] lines = TextHelper.SplitText(input);
            foreach (string ln in lines)
            {
                if (TextHelper.IsNoText(ln)) continue;
                output += ln;
            }

            return output;
        }
        private string ChangeNameToOne(string input)
        {
            string output = string.Empty;

            string[] lines = TextHelper.SplitText(input);
            foreach (string ln in lines)
            {
                output += ln;
            }

            return output;
        }

        private void btn_RemovewEmpty_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;

                string uid = btn.Uid;
                foreach (vmHeading heading in this.listbox_headers.SelectedItems)
                {
                    if (heading == null) continue;

                    string output = string.Empty;
                    switch (uid)
                    {
                        case "start": output = RemoveStartTrim(heading.Temp.Name); break;
                        case "end": output = RemoveEndTrim(heading.Temp.Name); break;
                        case "startend": output = RemoveTrim(heading.Temp.Name); break;
                        case "all": output = RemoveAllEmpty(heading.Temp.Name); break;
                        default: output = heading.Temp.Name; break;
                    }
                    if (output != heading.Temp.Name) heading.SetName(output);
                }

            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private string RemoveAllEmpty(string input)
        {
            string output = string.Empty;

            output = TextHelper.RemoveEmtpy(input);

            return output;
        }
        private string RemoveTrim(string input)
        {
            string output = string.Empty;

            output = input.Trim();

            return output;
        }
        private string RemoveEndTrim(string input)
        {
            string output = string.Empty;

            output = input.TrimEnd();

            return output;
        }
        private string RemoveStartTrim(string input)
        {
            string output = string.Empty;

            output = input.TrimStart();

            return output;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (!IsUserModity) return;

                TextBox tb = sender as TextBox;
                if (tb == null) return;

                vmHeading heading = tb.DataContext as vmHeading;
                if (heading == null) return;

                heading.SetName(tb.Text);
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                this.IsUserModity = false;

                TextBox tb = sender as TextBox;
                if (tb == null) return;

                vmHeading heading = tb.DataContext as vmHeading;
                if (heading == null) return;

                heading.SetName(tb.Text);
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        bool IsUserModity = false;

        private void TextBox_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            this.IsUserModity = true;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                RadioButton rBtn = sender as RadioButton;
                if (rBtn == null) return;


                string uid = rBtn.Uid;
                bool isDigit = int.TryParse(uid, out int code);
                if (!isDigit) code = 0;

                eHeadingType hType = (eHeadingType)code;

                foreach (vmHeading heading in this.listbox_headers.SelectedItems)
                {
                    if (heading == null) continue;

                    heading.SetHeadintType(hType);
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void RadioButton_Single_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                RadioButton rBtn = sender as RadioButton;
                if (rBtn == null) return;

                vmHeading data = rBtn.DataContext as vmHeading;
                if (data == null) return;

                string uid = rBtn.Uid;
                bool isDigit = int.TryParse(uid, out int code);
                if (!isDigit) code = 0;

                data.SetHeadintType((eHeadingType)code);
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void txtbox_SearchKeyword_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                string keyword = this.txtbox_SearchKeyword.Text;
                BindHeaders(keyword);
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_RemoveFirst_click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (vmHeading heading in this.listbox_headers.SelectedItems)
                {
                    if (heading == null) continue;

                    string output = heading.Temp.Name;
                    output = output.Substring(1);
                    if (output != heading.Temp.Name) heading.SetName(output);
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_RemoveLast_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (vmHeading heading in this.listbox_headers.SelectedItems)
                {
                    if (heading == null) continue;

                    string output = heading.Temp.Name;
                    output = output.Substring(0, output.Length-1);
                    if (output != heading.Temp.Name) heading.SetName(output);
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
    }
}
