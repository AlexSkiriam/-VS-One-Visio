
namespace _VS_One__Visio
{
    partial class CustomRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public CustomRibbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomRibbon));
            this.mainTab = this.Factory.CreateRibbonTab();
            this.group1 = this.Factory.CreateRibbonGroup();
            this.group5 = this.Factory.CreateRibbonGroup();
            this.group2 = this.Factory.CreateRibbonGroup();
            this.group3 = this.Factory.CreateRibbonGroup();
            this.group4 = this.Factory.CreateRibbonGroup();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.getPhrasesMenu = this.Factory.CreateRibbonMenu();
            this.getAllPhrases = this.Factory.CreateRibbonButton();
            this.getOnlyElaboratedPhrases = this.Factory.CreateRibbonButton();
            this.button5 = this.Factory.CreateRibbonButton();
            this.setLinks = this.Factory.CreateRibbonMenu();
            this.formSet = this.Factory.CreateRibbonButton();
            this.autoSet = this.Factory.CreateRibbonButton();
            this.button2 = this.Factory.CreateRibbonButton();
            this.getResult = this.Factory.CreateRibbonButton();
            this.assFileOpen = this.Factory.CreateRibbonButton();
            this.button4 = this.Factory.CreateRibbonButton();
            this.getParametrsFromText = this.Factory.CreateRibbonButton();
            this.button3 = this.Factory.CreateRibbonButton();
            this.menu1 = this.Factory.CreateRibbonMenu();
            this.aboutBtn = this.Factory.CreateRibbonButton();
            this.graphMnBtn = this.Factory.CreateRibbonButton();
            this.addinMnBtn = this.Factory.CreateRibbonButton();
            this.update = this.Factory.CreateRibbonButton();
            this.menu2 = this.Factory.CreateRibbonMenu();
            this.group6 = this.Factory.CreateRibbonGroup();
            this.button1 = this.Factory.CreateRibbonButton();
            this.button6 = this.Factory.CreateRibbonButton();
            this.button7 = this.Factory.CreateRibbonButton();
            this.mainTab.SuspendLayout();
            this.group1.SuspendLayout();
            this.group5.SuspendLayout();
            this.group2.SuspendLayout();
            this.group3.SuspendLayout();
            this.group4.SuspendLayout();
            this.group6.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainTab
            // 
            this.mainTab.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.mainTab.Groups.Add(this.group6);
            this.mainTab.Groups.Add(this.group1);
            this.mainTab.Groups.Add(this.group5);
            this.mainTab.Groups.Add(this.group2);
            this.mainTab.Groups.Add(this.group3);
            this.mainTab.Groups.Add(this.group4);
            this.mainTab.Label = "[VS One]";
            this.mainTab.Name = "mainTab";
            // 
            // group1
            // 
            this.group1.Items.Add(this.setLinks);
            this.group1.Label = "Работа с ссылками";
            this.group1.Name = "group1";
            // 
            // group5
            // 
            this.group5.Items.Add(this.getResult);
            this.group5.Items.Add(this.button7);
            this.group5.Label = "Работа с результатами";
            this.group5.Name = "group5";
            // 
            // group2
            // 
            this.group2.Items.Add(this.assFileOpen);
            this.group2.Items.Add(this.button4);
            this.group2.Items.Add(this.getParametrsFromText);
            this.group2.Items.Add(this.button3);
            this.group2.Label = "Дополнительные функции";
            this.group2.Name = "group2";
            // 
            // group3
            // 
            this.group3.Items.Add(this.menu1);
            this.group3.Label = "Инфо";
            this.group3.Name = "group3";
            // 
            // group4
            // 
            this.group4.Items.Add(this.update);
            this.group4.Label = "Обновления";
            this.group4.Name = "group4";
            // 
            // getPhrasesMenu
            // 
            this.getPhrasesMenu.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.getPhrasesMenu.Image = ((System.Drawing.Image)(resources.GetObject("getPhrasesMenu.Image")));
            this.getPhrasesMenu.Items.Add(this.getAllPhrases);
            this.getPhrasesMenu.Items.Add(this.getOnlyElaboratedPhrases);
            this.getPhrasesMenu.Items.Add(this.button5);
            this.getPhrasesMenu.Label = "Собрать фразы";
            this.getPhrasesMenu.Name = "getPhrasesMenu";
            this.getPhrasesMenu.ShowImage = true;
            // 
            // getAllPhrases
            // 
            this.getAllPhrases.Label = "Собрать все фразы";
            this.getAllPhrases.Name = "getAllPhrases";
            this.getAllPhrases.ShowImage = true;
            this.getAllPhrases.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.getAllPhrases_Click);
            // 
            // getOnlyElaboratedPhrases
            // 
            this.getOnlyElaboratedPhrases.Label = "Собрать проработанные фразы";
            this.getOnlyElaboratedPhrases.Name = "getOnlyElaboratedPhrases";
            this.getOnlyElaboratedPhrases.ShowImage = true;
            this.getOnlyElaboratedPhrases.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.getOnlyElaboratedPhrases_Click);
            // 
            // button5
            // 
            this.button5.Label = "Собрать только новые фразы";
            this.button5.Name = "button5";
            this.button5.ShowImage = true;
            this.button5.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.button5_Click);
            // 
            // setLinks
            // 
            this.setLinks.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.setLinks.Image = ((System.Drawing.Image)(resources.GetObject("setLinks.Image")));
            this.setLinks.Items.Add(this.formSet);
            this.setLinks.Items.Add(this.autoSet);
            this.setLinks.Items.Add(this.button2);
            this.setLinks.Label = "Проставить ссылки";
            this.setLinks.Name = "setLinks";
            this.setLinks.ShowImage = true;
            // 
            // formSet
            // 
            this.formSet.Label = "Ссылки";
            this.formSet.Name = "formSet";
            this.formSet.ShowImage = true;
            this.formSet.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.formSet_Click);
            // 
            // autoSet
            // 
            this.autoSet.Label = "Авто проставление";
            this.autoSet.Name = "autoSet";
            this.autoSet.ShowImage = true;
            this.autoSet.Visible = false;
            // 
            // button2
            // 
            this.button2.Label = "Удалить ссылки";
            this.button2.Name = "button2";
            this.button2.ShowImage = true;
            this.button2.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.button2_Click_1);
            // 
            // getResult
            // 
            this.getResult.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.getResult.Image = ((System.Drawing.Image)(resources.GetObject("getResult.Image")));
            this.getResult.Label = "Собрать результаты";
            this.getResult.Name = "getResult";
            this.getResult.ShowImage = true;
            this.getResult.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.getResult_Click);
            // 
            // assFileOpen
            // 
            this.assFileOpen.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.assFileOpen.Image = ((System.Drawing.Image)(resources.GetObject("assFileOpen.Image")));
            this.assFileOpen.Label = "Работа с  .ass";
            this.assFileOpen.Name = "assFileOpen";
            this.assFileOpen.ShowImage = true;
            this.assFileOpen.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.assFileOpen_Click);
            // 
            // button4
            // 
            this.button4.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.button4.Image = ((System.Drawing.Image)(resources.GetObject("button4.Image")));
            this.button4.Label = "Создание скрипта";
            this.button4.Name = "button4";
            this.button4.ShowImage = true;
            this.button4.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.button4_Click);
            // 
            // getParametrsFromText
            // 
            this.getParametrsFromText.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.getParametrsFromText.Label = "Выгрузить параметры";
            this.getParametrsFromText.Name = "getParametrsFromText";
            this.getParametrsFromText.ShowImage = true;
            this.getParametrsFromText.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.getParametrsFromText_Click);
            // 
            // button3
            // 
            this.button3.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.button3.Label = "Сместить по x и y";
            this.button3.Name = "button3";
            this.button3.ShowImage = true;
            this.button3.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.button3_Click_1);
            // 
            // menu1
            // 
            this.menu1.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.menu1.Image = ((System.Drawing.Image)(resources.GetObject("menu1.Image")));
            this.menu1.Items.Add(this.aboutBtn);
            this.menu1.Items.Add(this.graphMnBtn);
            this.menu1.Items.Add(this.addinMnBtn);
            this.menu1.Label = "Инфо";
            this.menu1.Name = "menu1";
            this.menu1.ShowImage = true;
            // 
            // aboutBtn
            // 
            this.aboutBtn.Image = ((System.Drawing.Image)(resources.GetObject("aboutBtn.Image")));
            this.aboutBtn.Label = "О надстройке";
            this.aboutBtn.Name = "aboutBtn";
            this.aboutBtn.ShowImage = true;
            this.aboutBtn.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.aboutBtn_Click);
            // 
            // graphMnBtn
            // 
            this.graphMnBtn.Image = ((System.Drawing.Image)(resources.GetObject("graphMnBtn.Image")));
            this.graphMnBtn.Label = "Манул по блок-схемам";
            this.graphMnBtn.Name = "graphMnBtn";
            this.graphMnBtn.ShowImage = true;
            this.graphMnBtn.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.graphMnBtn_Click);
            // 
            // addinMnBtn
            // 
            this.addinMnBtn.Image = ((System.Drawing.Image)(resources.GetObject("addinMnBtn.Image")));
            this.addinMnBtn.Label = "Мануал по надстройке";
            this.addinMnBtn.Name = "addinMnBtn";
            this.addinMnBtn.ShowImage = true;
            this.addinMnBtn.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.addinMnBtn_Click);
            // 
            // update
            // 
            this.update.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.update.Image = ((System.Drawing.Image)(resources.GetObject("update.Image")));
            this.update.Label = "Обновить";
            this.update.Name = "update";
            this.update.ShowImage = true;
            this.update.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.update_Click);
            // 
            // menu2
            // 
            this.menu2.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.menu2.Image = ((System.Drawing.Image)(resources.GetObject("menu2.Image")));
            this.menu2.Items.Add(this.button1);
            this.menu2.Items.Add(this.button6);
            this.menu2.Label = "Выгрузить интенты";
            this.menu2.Name = "menu2";
            this.menu2.ShowImage = true;
            // 
            // group6
            // 
            this.group6.Items.Add(this.getPhrasesMenu);
            this.group6.Items.Add(this.menu2);
            this.group6.Label = "Работа с фразами";
            this.group6.Name = "group6";
            // 
            // button1
            // 
            this.button1.Label = "Для тест-машины";
            this.button1.Name = "button1";
            this.button1.ShowImage = true;
            this.button1.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.button1_Click);
            // 
            // button6
            // 
            this.button6.Label = "Для статистики";
            this.button6.Name = "button6";
            this.button6.ShowImage = true;
            this.button6.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.button6_Click);
            // 
            // button7
            // 
            this.button7.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.button7.Label = "Формат";
            this.button7.Name = "button7";
            this.button7.ShowImage = true;
            this.button7.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.button7_Click);
            // 
            // CustomRibbon
            // 
            this.Name = "CustomRibbon";
            this.RibbonType = "Microsoft.Visio.Drawing";
            this.Tabs.Add(this.mainTab);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.CustomRibbon_Load);
            this.mainTab.ResumeLayout(false);
            this.mainTab.PerformLayout();
            this.group1.ResumeLayout(false);
            this.group1.PerformLayout();
            this.group5.ResumeLayout(false);
            this.group5.PerformLayout();
            this.group2.ResumeLayout(false);
            this.group2.PerformLayout();
            this.group3.ResumeLayout(false);
            this.group3.PerformLayout();
            this.group4.ResumeLayout(false);
            this.group4.PerformLayout();
            this.group6.ResumeLayout(false);
            this.group6.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab mainTab;
        internal Microsoft.Office.Tools.Ribbon.RibbonMenu getPhrasesMenu;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton getAllPhrases;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton getOnlyElaboratedPhrases;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton assFileOpen;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton getResult;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group2;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group3;
        internal Microsoft.Office.Tools.Ribbon.RibbonMenu menu1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton aboutBtn;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton graphMnBtn;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton addinMnBtn;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton update;
        internal Microsoft.Office.Tools.Ribbon.RibbonMenu setLinks;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton formSet;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton autoSet;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group4;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group5;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton getParametrsFromText;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button4;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button3;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button2;
        private System.Windows.Forms.ColorDialog colorDialog1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button5;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group6;
        internal Microsoft.Office.Tools.Ribbon.RibbonMenu menu2;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button6;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button7;
    }

    partial class ThisRibbonCollection
    {
        internal CustomRibbon CustomRibbon
        {
            get { return this.GetRibbon<CustomRibbon>(); }
        }
    }
}
