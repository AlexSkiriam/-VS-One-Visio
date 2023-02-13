using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Visio = Microsoft.Office.Interop.Visio;
using OfficeCore = Microsoft.Office.Core;
using OfficeTools = Microsoft.Office.Tools;

namespace _VS_One__Visio
{
    public partial class ThisAddIn
    {
        protected override OfficeCore.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            return Globals.Factory.GetRibbonFactory().CreateRibbonManager(
                new OfficeTools.Ribbon.IRibbonExtension[] { new CustomRibbon() });
        }

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

        #region Код, автоматически созданный VSTO

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}
