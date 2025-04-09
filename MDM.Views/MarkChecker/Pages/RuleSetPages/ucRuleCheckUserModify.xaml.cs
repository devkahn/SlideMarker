using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Emit;
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
using MDM.Models.DataModels;
using MDM.Models.ViewModels;
using MDM.Views.MarkChecker.Windows;

namespace MDM.Views.MarkChecker.Pages.RuleSetPages
{
    /// <summary>
    /// ucRuleCheckUserModify.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucRuleCheckUserModify : UserControl
    {
        private bool IsTreeChanged = false;

        private vmMaterial _Material = null;
        public vmMaterial Material
        {
            get => _Material;
            set
            {
                _Material = value;
            }
        }

        public vmHeading SelectedParentHeading { get; set; } = null;

        public ucRuleCheckUserModify()
        {
            InitializeComponent();
        }

        private void Grid_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                vmHeading selectedHeading = e.NewValue as vmHeading;
                if (selectedHeading == null) return;

                this.SelectedParentHeading = selectedHeading;

                List<vmHeadingModify> list = new List<vmHeadingModify>();
                foreach (vmHeading child in selectedHeading.Children)
                {
                    list.Add(new vmHeadingModify(child));
                }

                this.datagrid_Children.ItemsSource = null;
                this.datagrid_Children.ItemsSource = list;
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_Cut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<vmHeadingModify> list = new List<vmHeadingModify>();
                if(this.datagrid_Children.SelectedItems.Count == 0)
                {
                    list = this.datagrid_Children.ItemsSource as List<vmHeadingModify>; 
                }
                else
                {
                    foreach (vmHeadingModify item in this.datagrid_Children.SelectedItems)
                    {
                        if(item == null) continue;  
                        list.Add(item);
                    }
                }

                int startIndex = int.Parse(this.txtbox_CutCopyStartIndex.Text);
                int endIndex = int.Parse(this.txtbox_CutCopyEndIndex.Text);

                foreach (vmHeadingModify item in list)
                {
                    string origin = item.Display_TargetName.ToString();

                    string cutString = origin.Substring(startIndex, endIndex - startIndex);
                    item.TempString = cutString;

                    string targetString = origin.Substring(0, startIndex);
                    targetString += origin.Substring(endIndex);

                    item.Display_TargetName = targetString;
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_Trim_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;

                string uid = btn.Uid;
                int code = string.IsNullOrEmpty(uid) ? -1 : int.Parse(uid);

                List<vmHeadingModify> list = new List<vmHeadingModify>();
                if (this.datagrid_Children.SelectedItems.Count == 0)
                {
                    list = this.datagrid_Children.ItemsSource as List<vmHeadingModify>;
                }
                else
                {
                    foreach (vmHeadingModify item in this.datagrid_Children.SelectedItems)
                    {
                        if (item == null) continue;
                        list.Add(item);
                    }
                }

                foreach (vmHeadingModify item in list)
                {
                    switch (code)
                    {
                        case 0: item.Display_TargetName = item.Display_TargetName.ToString().Trim(); break;
                        case 10: item.Display_TargetName = item.Display_TargetName.ToString().TrimStart(); break;
                        case 20: item.Display_TargetName = item.Display_TargetName.ToString().TrimEnd(); break;
                        case 100: item.Display_TargetName = TextHelper.RemoveEmtpy(item.Display_TargetName.ToString()); break;
                        case 101:
                            string output = string.Empty;
                            string[] lines = TextHelper.SplitText(item.Display_TargetName.ToString());
                            foreach (string ln in lines)
                            {
                                if(TextHelper.IsNoText(ln)) continue;
                                output += string.Format(" {0}", ln.Trim());
                            }
                            item.Display_TargetName = output;
                            break;
                        default: break;
                    }
                }
               

            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_paste_click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<vmHeadingModify> list = GetSelectedHeaderList();

                int insertIndex = int.Parse(this.txtbox_PasteStartIndex.Text);

                foreach (vmHeadingModify item in list)
                {
                    string front = this.txtbox_FrontAddText.Text;
                    string value = item.TempString;
                    string back = this.txtbox_BackAddText.Text;

                    string insertString = string.Format("{0}{1}{2}", front, value, back);


                    item.Display_TargetName = item.Display_TargetName.ToString().Insert(insertIndex, insertString);
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private List<vmHeadingModify> GetSelectedHeaderList()
        {
            List<vmHeadingModify> list =  new List<vmHeadingModify>();
            if (this.datagrid_Children.SelectedItems.Count == 0)
            {
                list = this.datagrid_Children.ItemsSource as List<vmHeadingModify>;
            }
            else
            {
                foreach (vmHeadingModify item in this.datagrid_Children.SelectedItems)
                {
                    if (item == null) continue;
                    list.Add(item);
                }
            }

            return list;
        }

        private void btn_Reset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<vmHeadingModify> items = GetSelectedHeaderList();
                foreach (vmHeadingModify item in items)
                {
                    item.Display_TargetName = item.Display_OriginName;
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_RemoveChar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<vmHeadingModify> list = GetSelectedHeaderList();

                int count = int.Parse(this.txtbox_RemoveCharCount.Text);

                foreach (vmHeadingModify item in list)
                {
                    item.Display_TargetName = item.Display_TargetName.ToString().Substring(count);
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_AddChar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<vmHeadingModify> list = GetSelectedHeaderList();
                int insertIndex = int.Parse(this.txtbox_AddCharIndex.Text);
                string addedString = this.txtbox_AddText.Text;
                foreach (vmHeadingModify item in list)
                {
                    item.Display_TargetName = item.Display_TargetName.ToString().Insert(insertIndex, addedString);
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_Change_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<vmHeadingModify> list = GetSelectedHeaderList();
                foreach (vmHeadingModify item in list)
                {
                    item.Display_OriginName = item.Display_TargetName;
                    item.Origin.SetName(item.Display_TargetName.ToString());
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_ChangeChar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<vmHeadingModify> list = GetSelectedHeaderList();

                int startIndex = int.Parse(this.txtbox_ChangeCharStartIndex.Text);
                int endIndex = int.Parse(this.txtbox_ChangeCharEndIndex.Text);

                string originChar = this.txtbox_OriginChar.Text;
                string targetChar = this.txtbox_TargetChar.Text;

                foreach (vmHeadingModify item in list)
                {
                    string output = string.Empty;
                    for (int i = 0; i < item.Display_TargetName.ToString().Length; i++)
                    {
                        
                        char value = item.Display_TargetName.ToString()[i];
                        if(startIndex <= i && i <= endIndex)
                        {
                            if(value.ToString() == originChar)
                            {
                                output += targetChar;
                            }
                            else
                            {
                                output += value;
                            }
                        }
                        else
                        {
                            output += value;
                        }
                    }
                    item.Display_TargetName = output;
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_MergeSameHeader_click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<vmHeadingModify> list = this.datagrid_Children.ItemsSource as List<vmHeadingModify>;

                List<vmHeading> newList = new List<vmHeading>();
                foreach (vmHeadingModify item in list)
                {
                    vmHeading sameNameHeader = newList.Where(x => x.Temp.Name == item.Origin.Temp.Name).FirstOrDefault();
                    if(sameNameHeader == null)
                    {
                        newList.Add(item.Origin);
                    }
                    else
                    {
                        foreach (vmContent content in item.Origin.Contents.ToList())
                        {
                            item.Origin.RemoveContent(content);
                            content.SetParentHeading(sameNameHeader);
                        }
                        this.SelectedParentHeading.RemoveChild(item.Origin);
                    }
                }

                List<vmHeadingModify> modiList = new List<vmHeadingModify>();
                foreach (vmHeading item in newList)
                {
                    item.ContentsOrderBy();
                    vmHeadingModify newModify = new vmHeadingModify(item);
                    modiList.Add(newModify);
                }
                this.datagrid_Children.ItemsSource = null;
                this.datagrid_Children.ItemsSource = modiList;

            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }

        }
    }


    internal class vmHeadingModify : vmViewModelbase
    {
        private vmHeading _Origin = null;

        private object _Display_OriginName = null;
        private object _Display_TargetName = null;

        internal vmHeadingModify(vmHeading heading)
        {
            this.Origin = heading;
        }

        public vmHeading Origin
        {
            get => _Origin;
            set
            {
                _Origin = value;
                this.Display_OriginName = value.Display_Name;
                this.Display_TargetName = value.Display_Name;
            }
        }
        public string TempString { get; set; } = string.Empty;

        public object Display_OriginName
        {
            get => _Display_OriginName;
            set
            {
                _Display_OriginName = value;
                OnPropertyChanged(nameof(Display_OriginName));
            }
        }
        public object Display_TargetName
        {
            get => _Display_TargetName;
            set
            {
                _Display_TargetName = value;
                OnPropertyChanged(nameof(Display_TargetName));
            }
        }

        public override void InitializeDisplay()
        {
            
        }

        public override void SetInitialData()
        {
            
        }

        public override object UpdateOriginData()
        {
            return null;
        }
    }

}
