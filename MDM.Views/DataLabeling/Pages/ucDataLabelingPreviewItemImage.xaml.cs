﻿using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
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
using MDM.Commons;
using MDM.Helpers;
using MDM.Models.ViewModels;
using System.IO;
using Path = System.IO.Path;
using System.Diagnostics;
using System.Windows.Media.Animation;

namespace MDM.Views.DataLabeling.Pages
{
    /// <summary>
    /// ucDataLabelingPreviewItemImage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucDataLabelingPreviewItemImage : UserControl
    {
        public ucDataLabelingPreviewItemImage()
        {
            InitializeComponent();
        }

        private void txtbox_ImageTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                TextBox tb = sender as TextBox;
                if (tb == null) return;

                vmItem data = tb.DataContext as vmItem;
                if (data == null) return;

                if (tb.Text == data.Temp.Title) return;

                data.SetTitle(tb.Text);
                        
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void txtbox_ImageTitle_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_Completed_Click(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {

            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void btn_ImagePaste_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BitmapSource imgSource = Clipboard.GetImage();
                if (imgSource == null) return;

                Bitmap bitmap = ImageHelper.BitmapSourceToBitmap(imgSource);
                if (bitmap == null) return;

                Button btn = sender as Button;
                if (btn == null) return;

                vmItem data = btn.DataContext as vmItem;
                if (data == null) return;

                string dirPath = data.GetImageDirPath();
                if (string.IsNullOrEmpty(dirPath))
                {
                    string eMsg = "이미지 경로를 설정 후 다시 시도해주세요.";
                    MessageHelper.ShowErrorMessage("이미지 경로 미설정", eMsg);
                    return;
                }

                string targetPath = Path.Combine(dirPath, data.Temp.Uid + Defines.EXTENSION_IMAGE);
                if (File.Exists(targetPath))
                {
                    try
                    {
                        data.SetPreviewItem(true);
                        File.Delete(targetPath);
                    }
                    catch (Exception ee)
                    {
                        data.Temp.Uid = Guid.NewGuid().ToString();
                        targetPath = Path.Combine(dirPath, data.Temp.Uid + Defines.EXTENSION_IMAGE);
                        data.InitializeDisplay();
                    }

                }
                bitmap.Save(targetPath, ImageFormat.Png);

                this.txtblock_ImageFileName.Text = data.Temp.Uid;
                data.SetImageText();
                data.OnImageFileExistChanged();
                data.SetPreviewItem();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }

        }
        private void btn_ImageLoad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;

                vmItem data = btn.DataContext as vmItem;
                if (data == null) return;

                FileInfo fInfo = FileHelper.GetOpenFileInfo();
                if (fInfo == null) return;

                string dirPath = data.GetImageDirPath();
                if(string.IsNullOrEmpty(dirPath))
                {
                    string eMsg = "이미지 경로를 설정 후 다시 시도해주세요.";
                    MessageHelper.ShowErrorMessage("이미지 경로 미설정", eMsg);
                    return;
                }

                string targetPath = Path.Combine(dirPath, data.Temp.Uid + Defines.EXTENSION_IMAGE);
                if (File.Exists(targetPath))
                {
                    try
                    {
                        data.SetPreviewItem(true);
                        File.Delete(targetPath);
                    }
                    catch (Exception ee)
                    {
                        data.Temp.Uid = Guid.NewGuid().ToString();
                        targetPath = Path.Combine(dirPath, data.Temp.Uid + Defines.EXTENSION_IMAGE);
                        data.InitializeDisplay();
                    }

                }
                File.Copy(fInfo.FullName, targetPath);

                this.txtblock_ImageFileName.Text = data.Temp.Uid;
                data.SetImageText();
                data.OnImageFileExistChanged();
                data.SetPreviewItem();
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
                vmItem data = e.NewValue as vmItem;
                if (data == null) return;
                if (data.ItemType != Commons.Enum.eItemType.Image) return;

                data.SetPreviewItem();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_OpenDir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;

                vmItem data = btn.DataContext as vmItem;
                if (data == null) return;

                string dir = data.GetImageDirPath();
                if (string.IsNullOrEmpty(dir))
                {
                    string eMsg = "이미지 경로를 설정 후 다시 시도해주세요.";
                    MessageHelper.ShowErrorMessage("이미지 경로 미설정", eMsg);
                    return;
                }
                Process.Start(dir);
            }
            catch (Exception ee) 
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_ImageTextClipboard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vmItem data = this.DataContext as vmItem;
                if (data == null) return;
                if (data.ItemType != Commons.Enum.eItemType.Image) return;

                string text = data.Display_Text.ToString();
                Clipboard.SetText(text);

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
                    BeginTime = TimeSpan.FromSeconds(2) // 3초 후에 시작
                };
                fadeOutAnimation.Completed += (s, ee) =>
                {
                    this.grid_CopyCompleteMark.Visibility = Visibility.Collapsed;
                };
                fadeInAnimation.Completed += (s, ee) =>
                {
                    // 3초 후 페이드 아웃 애니메이션 시작
                    grid_CopyCompleteMark.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
                };

                this.grid_CopyCompleteMark.Visibility = Visibility.Visible;
                this.grid_CopyCompleteMark.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }

        }
    }
}
