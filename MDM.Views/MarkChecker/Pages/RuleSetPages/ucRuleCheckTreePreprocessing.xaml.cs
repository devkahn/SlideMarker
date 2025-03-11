using System;
using System.Collections.Generic;
using System.IO;
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
using MDM.Models.DataModels;
using MDM.Models.ViewModels;
using MDM.Views.MarkChecker.Windows;

namespace MDM.Views.MarkChecker.Pages.RuleSetPages
{
    /// <summary>
    /// ucRuleCheckTreePreprocessing.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucRuleCheckTreePreprocessing : UserControl
    {
        private bool IsTreeChanged = false;

        private vmMaterial _Material = null;
        public vmMaterial Material
        {
            get => _Material;
            set
            {
                _Material = value;
                BindTree();

            }
        }


        public ucRuleCheckTreePreprocessing()
        {
            InitializeComponent();
        }

        private void BindTree()
        {
            this.treeview_Header.ItemsSource = null;
            if (this.Material == null) return;

            int minLevel = this.Material.Headings.Min(x => x.Temp.Level);
            this.treeview_Header.ItemsSource = this.Material.Headings.Where(x => x.Temp.Level == minLevel);

        }


        private void treeview_Header_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (e.NewValue == null) return;
                //    if (this.contentsPresenter == null) return;

                this.IsTreeChanged = true;

                vmHeading selectedItem = e.NewValue as vmHeading;
                if (selectedItem == null || selectedItem.Contents.Count < 0)
                {
                    this.IsTreeChanged = false;
                    return;
                }

                //   this.contentsPresenter.Content = selectedItem.Contents;

                this.IsTreeChanged = false;

            }
            catch (Exception ee)
            {
                this.IsTreeChanged = false;
                ErrorHelper.ShowError(ee);
            }
        }

        private void listbox_Children_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (this.IsTreeChanged) return;

                if (e.AddedItems == null) return;
                //    if (this.contentsPresenter == null) return;

                foreach (var item in e.AddedItems)
                {
                    vmHeading selectedItem = item as vmHeading;
                    if (selectedItem == null) return;
                    if (selectedItem.Contents.Count() < 0) return;

                    //           this.contentsPresenter.Content = selectedItem.Contents;
                }


            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_SelectionRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;

                string caption = btn.Content.ToString();

                vmHeading selectedItem = this.treeview_Header.SelectedItem as vmHeading;
                if (selectedItem == null) return;

                vmHeading parent = selectedItem.Parent;

                if (parent == null && selectedItem.Contents.Any())
                {
                    string eMsg = "선택한 제목에 포함하는 컨텐츠가 존재합니다.";
                    MessageHelper.ShowErrorMessage(caption, eMsg);
                    return;
                }

                foreach (vmHeading child in selectedItem.Children.ToList())
                {
                    child.SetParent(parent);
                }
                if (parent == null)
                {
                    this.Material.RemoveHeading(selectedItem);
                }
                else
                {
                    foreach (vmContent content in selectedItem.Contents.ToList())
                    {
                        content.SetParentHeading(parent);
                    }
                    parent.RemoveChild(selectedItem);
                }

                foreach (var item in this.Material.Headings)
                {
                    item.InitializeDisplay();
                }

                BindTree();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_ReLoad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Material.ClearHeading();
                this.Material.ClearContents();

                foreach (vmSlide slide in this.Material.Slides)
                {
                    slide.ConvertAndSetContents();
                }
                BindTree();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_MoveUp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;

                vmHeading selectedItem = this.treeview_Header.SelectedItem as vmHeading;
                if (selectedItem == null) return;

                selectedItem.Move(true);


            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_MoveDown_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;

                vmHeading selectedItem = this.treeview_Header.SelectedItem as vmHeading;
                if (selectedItem == null) return;

                selectedItem.Move(false);
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_ChildrenFold_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;

                int code = int.Parse(btn.Uid);

                string caption = btn.Content.ToString();


                vmHeading selectedItem = this.treeview_Header.SelectedItem as vmHeading;
                if (selectedItem == null) return;

                foreach (vmHeading item in selectedItem.Children)
                {
                    item.TreeExpand(code == 1 ? false : true);
                }


            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_SelectionMove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vmHeading selectedHeding = this.treeview_Header.SelectedItem as vmHeading;
                if (selectedHeding == null)
                {
                    string eMsg = "이동할 제목을 선택하세요.";
                    MessageHelper.ShowErrorMessage("새 제목 추가", eMsg);
                    return;
                }

                wndTargetHeaderSelection wndTarget = new wndTargetHeaderSelection();
                wndTarget.DataContext = this.Material;
                wndTarget.ShowDialog();

                if (wndTarget.DialogResult != true) return;

                vmHeading target = wndTarget.SelectedTargetHeader;


                selectedHeding.SetParent(target);
                selectedHeding.SetChildrenLevel();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_AddRoot_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                wndHeaderNameInput wndHi = new wndHeaderNameInput();
                wndHi.ShowDialog();

                if (wndHi.DialogResult != true) return;

                string newName = wndHi.HeaderName;

                mHeading heading = new mHeading();
                heading.Level = 0;
                heading.Name = newName;

                vmHeading newHeading = new vmHeading(heading);
                newHeading.SetParentMaterial(this.Material);
                newHeading.SetParent(null);

                BindTree();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_AddChild_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vmHeading selectedHeding = this.treeview_Header.SelectedItem as vmHeading;
                if (selectedHeding == null)
                {
                    string eMsg = "새 제목을 추가할 상위 제목을 선택하세요.";
                    MessageHelper.ShowErrorMessage("새 제목 추가", eMsg);
                    return;
                }

                wndHeaderNameInput wndHi = new wndHeaderNameInput();
                wndHi.ShowDialog();

                if (wndHi.DialogResult != true) return;

                string newName = wndHi.HeaderName;

                mHeading heading = new mHeading();
                heading.Level = 0;
                heading.Name = newName;

                vmHeading newHeading = new vmHeading(heading);
                newHeading.SetParentMaterial(this.Material);
                newHeading.SetParent(selectedHeding);

                //BindTree();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_SelectionCopy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vmHeading selectedHeding = this.treeview_Header.SelectedItem as vmHeading;
                if (selectedHeding == null)
                {
                    string eMsg = "복사할 제목을 선택하세요.";
                    MessageHelper.ShowErrorMessage("새 제목 추가", eMsg);
                    return;
                }

                wndTargetHeaderSelection wndTarget = new wndTargetHeaderSelection();
                wndTarget.DataContext = this.Material;
                wndTarget.ShowDialog();

                if (wndTarget.DialogResult != true) return;

                vmHeading target = wndTarget.SelectedTargetHeader;


                mHeading heading = new mHeading();
                heading.Level = target.Temp.Level + 1;
                heading.Name = selectedHeding.Temp.Name;

                vmHeading newHeading = new vmHeading(heading);
                newHeading.SetParentMaterial(this.Material);
                newHeading.SetParent(target);

            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
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
        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string jsonString = JsonHelper.ToJsonString(this.Material.Headings);

                string targetPath = System.IO.Path.Combine(this.Material.DirectoryPath, string.Format("headers_{0}.json", DateTime.Now.ToString("yyyyMMddHHmmss")));
                if (File.Exists(targetPath))
                {
                    File.Delete(targetPath);
                }
                File.WriteAllText(targetPath, jsonString);

                string msg = "저장 완료";
                MessageHelper.ShowMessage("", msg);
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
    }
}
