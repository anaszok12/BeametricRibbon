using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Media.Imaging;

namespace BeametricRibbon
{
    class App : IExternalApplication

    {
        static void AddRibbonPanel(UIControlledApplication application)
        {
            // create custom ribbon tab
            String tabName = "Beametric";
            application.CreateRibbonTab(tabName);

            // Add a new ribbon panel
            RibbonPanel ribbonPanel = application.CreateRibbonPanel(tabName,"Tools");

            // get .dll assembly path
            string thisAssembplyPath = Assembly.GetExecutingAssembly().Location;

            // create push button
            PushButtonData b1Data = new PushButtonData(
                "cmdMoveDimText",
                " Move Dimension Text",
                thisAssembplyPath,
                "MyExternalCommand.MoveDimText");

            PushButton pb1 = ribbonPanel.AddItem(b1Data) as PushButton;
            pb1.ToolTip = "Select a dimension to move the joint text";
            BitmapImage pb1Imange = new BitmapImage(new Uri("pack://application:,,,/BeametricRibbon;component/Resources/Move Dim Text 32-Light.png"));
            pb1.LargeImage = pb1Imange;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            // do nothing
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            // call our method creating ribbon panel
           AddRibbonPanel(application);
            return Result.Succeeded;
        }
    }
}
