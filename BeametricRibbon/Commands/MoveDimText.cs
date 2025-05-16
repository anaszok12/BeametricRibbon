using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;

using System;
using BeametricRibbon.Forms;

namespace CmdMoveDimText
{
    [Transaction(TransactionMode.Manual)]
    public class MoveDimText : IExternalCommand
    {
        public class DimPickFilter : ISelectionFilter //ISelection filter only allow dimension
        {
            public bool AllowElement(Element elem)
            {
                if (elem.Category?.Id?.IntegerValue == (int)BuiltInCategory.OST_Dimensions) //compare integer 
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public bool AllowReference(Reference reference, XYZ position)
            {
                return false;
            }
        }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet e)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                Reference pickedref;

                Selection sel = uiapp.ActiveUIDocument.Selection;

                DimPickFilter selFilter = new DimPickFilter();
                pickedref = sel.PickObject(ObjectType.Element, selFilter, "Please select a dimension");

                Element elem = doc.GetElement(pickedref);
                Dimension dimension = elem as Dimension;

                if (dimension != null)
                {
                    // Initialize WPF form
                    OffsetInputForm window = new OffsetInputForm();
                    bool? dialogResult = window.ShowDialog();

                    if (dialogResult == true)
                    {
                        double offsetInFeetX;
                        double offsetInFeetY;
                        double offsetInFeetZ;

                        string OffsetXText = window.OffsetXValue;
                        string OffsetYText = window.OffsetYValue;
                        string OffsetZText = window.OffsetZValue;

                        bool parseXSuccess = double.TryParse(OffsetXText, out offsetInFeetX);
                        bool parseYSuccess = double.TryParse(OffsetYText, out offsetInFeetY);
                        bool parseZSuccess = double.TryParse(OffsetZText, out offsetInFeetZ);

                        if (parseXSuccess && parseYSuccess && parseZSuccess)
                        {
                            double jointDimension = 0.0625;
                            double dimTolerance = 0.0001;

                            XYZ offsetVector = new XYZ(offsetInFeetX, offsetInFeetY, offsetInFeetZ);

                            using (Transaction trans = new Transaction(doc, "Shift Dimension Text Up and Right"))
                            {
                                trans.Start();

                                foreach (DimensionSegment segment in dimension.Segments)
                                {
                                    if (segment.Value - jointDimension <= dimTolerance)
                                    {
                                        XYZ currentTextPosition = segment.TextPosition;
                                        XYZ newTextPosition = currentTextPosition.Add(offsetVector);
                                        segment.TextPosition = newTextPosition;
                                    }
                                    else { }
                                }
                                trans.Commit();
                            }

                        }
                    }
                }
            return Result.Succeeded;
            }

            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }

            catch (Exception ex)
            {
                TaskDialog.Show("Error. ", " An error occured " + ex.Message);
                return Result.Failed;
            }
        }

    }
}
