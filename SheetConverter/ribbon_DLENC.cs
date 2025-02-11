using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools;
using Microsoft.Office.Tools.Ribbon;
using SheetConverter.Views.Pages;

namespace SheetConverter
{
    public partial class ribbon_DLENC
    {
        private ucDataToDBPanelForm HostPanel;
        private CustomTaskPane TaskPanel;
        private const string Title = "Excel To Database";
        private void ribbon_DLENC_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private void btn_ShowDBPanel_Click(object sender, RibbonControlEventArgs e)
        {
            if (this.HostPanel == null) this.HostPanel = new ucDataToDBPanelForm();
            if (this.TaskPanel == null)
            {
                this.TaskPanel = Globals.ThisAddIn.CustomTaskPanes.Add(this.HostPanel, Title);
                this.TaskPanel.Visible = true;
                this.TaskPanel.DockPosition = Microsoft.Office.Core.MsoCTPDockPosition.msoCTPDockPositionRight;
            }
            else
            {
                CustomTaskPane samePanel = null;
                try
                {
                    samePanel = Globals.ThisAddIn.CustomTaskPanes.Where(x => x.Title == Title).FirstOrDefault();
                }
                catch (Exception ee)
                {
                    // this.TaskPanel.Visible = false;
                    this.HostPanel = new ucDataToDBPanelForm();
                }

                if (samePanel == null)
                {
                    this.TaskPanel = Globals.ThisAddIn.CustomTaskPanes.Add(this.HostPanel, Title);
                    this.TaskPanel.Width = 500;
                    this.TaskPanel.Visible = true;
                    this.TaskPanel.DockPosition = Microsoft.Office.Core.MsoCTPDockPosition.msoCTPDockPositionRight;
                }
                else
                {
                    samePanel.Visible = !samePanel.Visible;
                }
            }
        }
    }
}
