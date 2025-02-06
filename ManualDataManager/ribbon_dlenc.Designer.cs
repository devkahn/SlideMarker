namespace ManualDataManager
{
    partial class ribbon_dlenc : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public ribbon_dlenc()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.tab_DLENC = this.Factory.CreateRibbonTab();
            this.group_MDM = this.Factory.CreateRibbonGroup();
            this.button_DataLabeling = this.Factory.CreateRibbonButton();
            this.tab_DLENC.SuspendLayout();
            this.group_MDM.SuspendLayout();
            this.SuspendLayout();
            // 
            // tab_DLENC
            // 
            this.tab_DLENC.Groups.Add(this.group_MDM);
            this.tab_DLENC.Label = "DL E&&C";
            this.tab_DLENC.Name = "tab_DLENC";
            // 
            // group_MDM
            // 
            this.group_MDM.Items.Add(this.button_DataLabeling);
            this.group_MDM.Label = "Manual Data";
            this.group_MDM.Name = "group_MDM";
            // 
            // button_DataLabeling
            // 
            this.button_DataLabeling.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.button_DataLabeling.Image = global::ManualDataManager.Properties.Resources.product_development;
            this.button_DataLabeling.Label = "Data Labeling";
            this.button_DataLabeling.Name = "button_DataLabeling";
            this.button_DataLabeling.ShowImage = true;
            this.button_DataLabeling.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.button_DataLabeling_Click);
            // 
            // ribbon_dlenc
            // 
            this.Name = "ribbon_dlenc";
            this.RibbonType = "Microsoft.PowerPoint.Presentation";
            this.Tabs.Add(this.tab_DLENC);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.ribbon_dlenc_Load);
            this.tab_DLENC.ResumeLayout(false);
            this.tab_DLENC.PerformLayout();
            this.group_MDM.ResumeLayout(false);
            this.group_MDM.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab_DLENC;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group_MDM;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button_DataLabeling;
    }

    partial class ThisRibbonCollection
    {
        internal ribbon_dlenc ribbon_dlenc
        {
            get { return this.GetRibbon<ribbon_dlenc>(); }
        }
    }
}
