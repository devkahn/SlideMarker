namespace MarkChecker
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
            this.tab_dlenc = this.Factory.CreateRibbonTab();
            this.group1 = this.Factory.CreateRibbonGroup();
            this.button1 = this.Factory.CreateRibbonButton();
            this.tab_dlenc.SuspendLayout();
            this.group1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tab_dlenc
            // 
            this.tab_dlenc.Groups.Add(this.group1);
            this.tab_dlenc.Label = "DL ENC";
            this.tab_dlenc.Name = "tab_dlenc";
            // 
            // group1
            // 
            this.group1.Items.Add(this.button1);
            this.group1.Label = "Checker Checker";
            this.group1.Name = "group1";
            // 
            // button1
            // 
            this.button1.Label = "Finder";
            this.button1.Name = "button1";
            this.button1.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.button1_Click);
            // 
            // ribbon_dlenc
            // 
            this.Name = "ribbon_dlenc";
            this.RibbonType = "Microsoft.PowerPoint.Presentation";
            this.Tabs.Add(this.tab_dlenc);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.ribbon_dlenc_Load);
            this.tab_dlenc.ResumeLayout(false);
            this.tab_dlenc.PerformLayout();
            this.group1.ResumeLayout(false);
            this.group1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab_dlenc;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button1;
    }

    partial class ThisRibbonCollection
    {
        internal ribbon_dlenc ribbon_dlenc
        {
            get { return this.GetRibbon<ribbon_dlenc>(); }
        }
    }
}
