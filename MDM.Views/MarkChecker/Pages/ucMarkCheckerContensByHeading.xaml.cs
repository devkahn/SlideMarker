using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using MDM.Commons;
using MDM.Helpers;
using MDM.Models.DataModels;
using MDM.Models.ViewModels;
using MDM.Views.MarkChecker.Pages.RuleSetPages;
using MDM.Views.MarkChecker.Windows;
using MenuItem = System.Windows.Controls.MenuItem;

namespace MDM.Views.MarkChecker.Pages
{
    /// <summary>
    /// ucMarkCheckerContensByHeading.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucMarkCheckerContensByHeading : UserControl
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

        private Dictionary<string, UserControl> RuleSetPages { get; set; } = new Dictionary<string, UserControl>();
        



        public ucMarkCheckerContensByHeading()
        {
            InitializeComponent();
            
            this.listbox_RuleSet.SelectedIndex = 3;
        }

        

        private ContextMenu SetContentsContextMenu()
        {
            ContextMenu output = new ContextMenu();
            output.Uid = "ucMarkCheckerContensByHeading";

            MenuItem goFirstRowToHeader = new MenuItem();
            output.Items.Add(goFirstRowToHeader);
            goFirstRowToHeader.Header = "첫 행 제목으로 만들기";
            goFirstRowToHeader.Click += menuItem_DivideLevel1_click;

            MenuItem divideByLevel1 = new MenuItem();
            output.Items.Add(divideByLevel1);
            divideByLevel1.Header = "Level1로 분할하기";
            divideByLevel1.Click += DivideByLevel1_Click; ;

            output.Items.Add(new Separator());

            MenuItem moveContent = new MenuItem();
            output.Items.Add(moveContent);
            moveContent.Header = "이동";
            moveContent.Click += MoveContent_Click;

            MenuItem removeItem = new MenuItem();
            output.Items.Add(removeItem);
            removeItem.Header = "삭제";
            removeItem.Click += RemoveItem_Click; ;


            return output;
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

                vmHeading selectedItem = e.NewValue as vmHeading;
                this.Material.CurrentHeading = selectedItem;

                foreach (vmContent con in selectedItem.Contents)
                {
                    if (con.ControlMenu.Uid == "ucMarkCheckerContensByHeading") continue;

                    con.ControlMenu = SetContentsContextMenu();
                }
            }
            catch (Exception ee)
            {
                this.IsTreeChanged = false;
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
                    item.TreeExpand(code ==1 ? false: true);
                }

                
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
                List<mHeading> rootHeadings = new List<mHeading>();
                foreach (vmHeading root in this.Material.RootHeadings)
                {
                    mHeading headingData = root.UpdateOriginData() as mHeading;
                    if(headingData != null) rootHeadings.Add(headingData);
                }

                string jsonString = JsonHelper.ToJsonString(rootHeadings);

                DateTime nowTime = DateTime.Now;
                string targetPath = System.IO.Path.Combine(this.Material.DirectoryPath, string.Format("{0}_{1}.headers", this.Material.Temp.Name , nowTime.ToString("yyMMddHHmmss")));
                if (File.Exists(targetPath))
                {
                    File.Delete(targetPath);
                }
                File.WriteAllText(targetPath, jsonString);




                this.textblock_TimeStamp.Text = nowTime.ToString("yy/MM/dd HH:mm:ss");
                DoubleAnimation fadeInAnimation = new DoubleAnimation
                {
                    From = 0,
                    To = 1,
                    Duration = TimeSpan.FromSeconds(1)
                };
                DoubleAnimation fadeOutAnimation = new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(1),
                    BeginTime = TimeSpan.FromSeconds(1) // 3초 후에 시작
                };
                fadeOutAnimation.Completed += (s, ee) =>
                {
                    this.stackPanel_SaveMessage.Visibility = Visibility.Collapsed;
                };
                fadeInAnimation.Completed += (s, ee) =>
                {
                    // 3초 후 페이드 아웃 애니메이션 시작
                    stackPanel_SaveMessage.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
                };

                this.stackPanel_SaveMessage.Visibility = Visibility.Visible;
                this.stackPanel_SaveMessage.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void listbox_RuleSet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (this.contentPresenter == null) return;

                ListBoxItem selectedRule = this.listbox_RuleSet.SelectedItem as ListBoxItem;
                if (selectedRule == null) return;

                string uid = selectedRule.Uid;
                if (!this.RuleSetPages.ContainsKey(uid))
                {
                    UserControl uc = null;
                    switch (uid)
                    {
                        case "1": uc = new ucRuleCheckTreePreprocessing(); break;
                        case "101": uc = new ucRuleCheckContentsSync(); break;
                        case "102": uc = new ucRuleCheckSameNameFinder(); break;
                        case "998": uc = new ucRuleCheckHeaderProperty(); break;
                        case "999": uc = new ucRuleCheckUserModify(); break;
                        default: break;
                    }
                    this.RuleSetPages.Add(uid, uc);
                }

                UserControl page = this.RuleSetPages[uid];
                if (page != null)
                {
                    page.DataContext = null;
                    page.DataContext = this.Material;
                }


                this.contentPresenter.Content = null;
                this.contentPresenter.Content = page;
            }
            catch (Exception ee) 
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {

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

                if (parent == null && selectedItem.Contents.Count() >0)
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

        private void btn_DeleteSelectContent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vmHeading heading = this.treeview_Header.SelectedItem as vmHeading;
                if (heading == null) return;

                List<vmContent> selectedItems = new List<vmContent>();
                foreach (vmContent item in this.listbox_ContainContents.SelectedItems)
                {
                    if (item == null) continue;
                    selectedItems.Add(item);
                }


                foreach (vmContent item in selectedItems)
                {
                    item.IsEnable = false;
                    heading.RemoveContent(item);
                    this.Material.RemoveContent(item);
                }
                
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void menuItem_MoveHeader_Click(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void menuItem_CopyHeader_Click(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void menuItem_DeleteHeader_Click(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_MoveSelectContent_Click(object sender, RoutedEventArgs e)
        {
            var selectedContents = this.listbox_ContainContents.SelectedItems;
            if (selectedContents.Count < 0)
            {
                string eMsg = "이동할 본문을 선택하세요.";
                MessageHelper.ShowErrorMessage("본문 이동", eMsg);
                return;
            }

            wndTargetHeaderSelection wndTarget = new wndTargetHeaderSelection();
            wndTarget.DataContext = this.Material;
            wndTarget.ShowDialog();

            if (wndTarget.DialogResult != true) return;

            vmHeading target = wndTarget.SelectedTargetHeader;

            List<vmContent> contents= new List<vmContent>();
            foreach (vmContent item in selectedContents)
            {
                if (item == null) continue;
                contents.Add(item);
            }
            

            foreach (vmContent item in contents)
            {
                item.ParentHeading.RemoveContent(item);
                item.SetParentHeading(target);
            }
            target.ContentsOrderBy();
           
            this.listbox_ContainContents.Items.Refresh();

        }

        private void btn_AllSelect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.listbox_ContainContents.SelectAll();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_AllUnSelect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.listbox_ContainContents.UnselectAll();
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

        private void btn_CreateNewHeader_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vmHeading selectedHeding = this.treeview_Header.SelectedItem as vmHeading;
                if (selectedHeding == null)
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
                else
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
                    newHeading.SetParent(selectedHeding);

                    //BindTree();
                }


            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_ContentMoveUp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedContents = this.listbox_ContainContents.SelectedItems;
                if (selectedContents.Count <= 0)
                {
                    string eMsg = "이동할 본문을 선택하세요.";
                    MessageHelper.ShowErrorMessage("본문 이동", eMsg);
                    return;
                }


                vmContent firstContent = selectedContents[0] as vmContent;
                if (firstContent == null) return;

                vmHeading parentHeading = firstContent.ParentHeading;

                int curIndex = parentHeading.Contents.IndexOf(firstContent);
                if (curIndex == 0) return;

                vmContent prevContent = parentHeading.Contents[curIndex - 1];
                if (prevContent == null) return;

                int order = firstContent.Temp.Temp.Order;
                firstContent.Temp.Temp.Order = prevContent.Temp.Temp.Order;
                prevContent.Temp.Temp.Order = order;

                parentHeading.ContentsOrderBy();

                

                this.listbox_ContainContents.Items.Refresh();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_ContentMoveDown_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedContents = this.listbox_ContainContents.SelectedItems;
                if (selectedContents.Count <= 0)
                {
                    string eMsg = "이동할 본문을 선택하세요.";
                    MessageHelper.ShowErrorMessage("본문 이동", eMsg);
                    return;
                }


                vmContent firstContent = selectedContents[0] as vmContent;
                if (firstContent == null) return;

                vmHeading parentHeading = firstContent.ParentHeading;

                int curIndex = parentHeading.Contents.IndexOf(firstContent);
                if (curIndex == parentHeading.Contents.Count()-1) return;

                vmContent nextContent = parentHeading.Contents[curIndex +1];
                if (nextContent == null) return;

                int order = firstContent.Temp.Temp.Order;
                firstContent.Temp.Temp.Order = nextContent.Temp.Temp.Order;
                nextContent.Temp.Temp.Order = order;

                parentHeading.ContentsOrderBy();



                this.listbox_ContainContents.Items.Refresh();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_FirstToHeader_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedContents = this.listbox_ContainContents.SelectedItems;
                if (selectedContents.Count != 1)
                {
                    string eMsg = "제목으로 변환할 본문을 1개만 선택하세요.";
                    MessageHelper.ShowErrorMessage("제목으로 만들기", eMsg);
                    return;
                }

                vmContent selectContent = selectedContents[0] as vmContent;
                if (selectContent == null) return;


                string firstLine = string.Empty;
                string remainTextLines = string.Empty;

                string text = TextHelper.RemoveNoTextLine(selectContent.Temp.Temp.LineText);
                string[] lines = TextHelper.SplitText(text);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 0) firstLine = lines[i];
                    else remainTextLines += lines[i] + "\n";
                }


                if(Defines.LIST_MARKER.Contains(firstLine.First())) firstLine = firstLine.Substring(1).Trim();
                



                mHeading heading = new mHeading();
                heading.Name = firstLine;
                vmHeading newHeading = new vmHeading(heading);
                newHeading.SetParent(selectContent.ParentHeading);

                selectContent.ParentHeading.RemoveContent(selectContent);
                selectContent.Temp.SetText(remainTextLines);
                selectContent.InitializeDisplay();
                selectContent.SetParentHeading(newHeading);

            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void menuItem_DivideLevel1_click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem menu = sender as MenuItem;
                if (menu == null) return;

                vmContent data = menu.DataContext as vmContent;
                if (data == null) return;
                
                if(data.Temp.ItemType != Commons.Enum.eItemType.Text)
                {
                    string eMsg = "[글] 속성 이외에는 제목으로 만들 수 없습니다.";
                    MessageHelper.ShowErrorMessage("제목으로 만들기", eMsg);
                    return;
                }



                string firstLine = string.Empty;
                string remainTextLines = string.Empty;

                string text = TextHelper.RemoveNoTextLine(data.Temp.Temp.LineText);
                string[] lines = TextHelper.SplitText(text);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 0) firstLine = lines[i];
                    else remainTextLines += lines[i] + "\n";
                }

                if (Defines.LIST_MARKER.Contains(firstLine.First()))
                {
                    firstLine = firstLine.Substring(1).Trim();
                }
                remainTextLines = TextHelper.RemoveLevelInText(remainTextLines);

                mHeading heading = new mHeading();
                heading.Name = firstLine;
                vmHeading newHeading = new vmHeading(heading);
                newHeading.SetParent(data.ParentHeading);

                data.ParentHeading.RemoveContent(data);
                data.Temp.SetText(remainTextLines);
                data.InitializeDisplay();
                data.SetParentHeading(newHeading);

            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void DivideByLevel1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem menu = sender as MenuItem;
                if (menu == null) return;

                vmContent data = menu.DataContext as vmContent;
                if (data == null) return;

                if (data.Temp.ItemType != Commons.Enum.eItemType.Text)
                {
                    string eMsg = "[글] 속성 이외에는 분할 할 수 없습니다.";
                    MessageHelper.ShowErrorMessage("본문 분할하기", eMsg);
                    return;
                }

                string text = TextHelper.RemoveNoTextLine(data.Temp.Temp.LineText);
                string[] lines = TextHelper.SplitText(text);
                if (lines.Length <= 1) return;

                List<string> divideText = new List<string>();

                string target = string.Empty;
                foreach (string ln in lines)
                {
                    if(string.IsNullOrEmpty(target))
                    {
                        target = ln;
                    }
                    else
                    {
                        if(char.IsWhiteSpace(ln.First()))
                        {
                            target += "\n" + ln;
                        }
                        else
                        {
                            divideText.Add(target);
                            target = ln;
                        }
                    }
                }
                divideText.Add(target);

                foreach (string newText in divideText)
                {
                    mItem item = new mItem();
                    item.ItemType = data.Temp.Temp.ItemType;
                    item.Level = data.Temp.Temp.Level;
                    item.LineText = newText;
                    item.Order = data.Temp.Temp.Order + divideText.IndexOf(newText);

                    vmItem newItem = new vmItem(item);
                    newItem.SetParent(data.Temp.ParentShape);

                    vmContent newContent = new vmContent(newItem);
                    newContent.ContentType = Commons.Enum.eContentType.UnOrderList;
                    newContent.ControlMenu = SetContentsContextMenu();
                    newContent.SetParentHeading(data.ParentHeading);
                    newContent.SetParentMaterial(this.Material);

                }

                data.IsEnable = false;
                data.ParentHeading.RemoveContent(data);
                this.Material.RemoveContent(data);

            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void MoveContent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem menu = sender as MenuItem;
                if (menu == null) return;

                vmContent data = menu.DataContext as vmContent;
                if (data == null) return;


            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem menu = sender as MenuItem;
                if (menu == null) return;

                vmContent data = menu.DataContext as vmContent;
                if (data == null) return;

                string qMsg = string.Format("선택한 본문을 삭제하시겠습니까?");

                System.Windows.Forms.DialogResult result = MessageHelper.ShowYewNoMessage("본문 삭제", qMsg);
                if(result != System.Windows.Forms.DialogResult.Yes) return;

                data.IsEnable = false;
                data.ParentHeading.RemoveContent(data);
                this.Material.RemoveContent(data);
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_MergeContent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedContents = this.listbox_ContainContents.SelectedItems;
                if (selectedContents.Count <= 1)
                {
                    string eMsg = "이동할 본문을 선택하세요.";
                    MessageHelper.ShowErrorMessage("본문 병합", eMsg);
                    return;
                }

                List<vmContent> contents = new List<vmContent>();
                foreach (vmContent item in selectedContents)
                {
                    if(item.Temp.ItemType != Commons.Enum.eItemType.Text)
                    {
                        string eMsg = "[글] 본문 이외에는 병합할 수 없습니다.";
                        MessageHelper.ShowErrorMessage("본문 병합", eMsg);
                        return;
                    }
                    contents.Add(item);
                }


                vmContent targetContent = contents.First();
                for (int i = 1; i < contents.Count; i++)
                {
                    vmContent con = contents[i];

                    string targetText = targetContent.Temp.Temp.LineText;
                    targetText += "\n" + con.Temp.Temp.LineText;
                    targetContent.Temp.SetText(targetText);

                    con.IsEnable = false;
                    con.ParentHeading.RemoveContent(con);
                    this.Material.RemoveContent(con);
                }
                targetContent.InitializeDisplay();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
    }
}
