namespace SheetConverter
{
    partial class ribbon_DLENC : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public ribbon_DLENC()
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
            this.group_DataConvert = this.Factory.CreateRibbonGroup();
            this.btn_ShowConvertPanel = this.Factory.CreateRibbonButton();
            this.tab_DLENC.SuspendLayout();
            this.group_DataConvert.SuspendLayout();
            this.SuspendLayout();
            // 
            // tab_DLENC
            // 
            this.tab_DLENC.Groups.Add(this.group_DataConvert);
            this.tab_DLENC.Label = "DL E&&C";
            this.tab_DLENC.Name = "tab_DLENC";
            // 
            // group_DataConvert
            // 
            this.group_DataConvert.Items.Add(this.btn_ShowConvertPanel);
            this.group_DataConvert.Label = "데이터 변환";
            this.group_DataConvert.Name = "group_DataConvert";
            // 
            // btn_ShowConvertPanel
            // 
            this.btn_ShowConvertPanel.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btn_ShowConvertPanel.Label = "DB로 변환";
            this.btn_ShowConvertPanel.Name = "btn_ShowConvertPanel";
            this.btn_ShowConvertPanel.ShowImage = true;
            this.btn_ShowConvertPanel.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btn_ShowDBPanel_Click);
            // 
            // ribbon_DLENC
            // 
            this.Name = "ribbon_DLENC";
            this.RibbonType = "Microsoft.Excel.Workbook";
            this.Tabs.Add(this.tab_DLENC);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.ribbon_DLENC_Load);
            this.tab_DLENC.ResumeLayout(false);
            this.tab_DLENC.PerformLayout();
            this.group_DataConvert.ResumeLayout(false);
            this.group_DataConvert.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab_DLENC;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group_DataConvert;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btn_ShowConvertPanel;
    }

    partial class ThisRibbonCollection
    {
        internal ribbon_DLENC ribbon_DLENC
        {
            get { return this.GetRibbon<ribbon_DLENC>(); }
        }
    }
}
