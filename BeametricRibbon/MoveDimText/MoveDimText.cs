using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;

using System;
using System.Collections.Generic;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;

namespace MyExternalCommand
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
                    double offsetInFeetX = 2.5;
                    double offsetInFeetY = 1.8;
                    double jointDimension = 0.0625;
                    double dimTolerance = 0.0001;

                    XYZ offsetVector = new XYZ(offsetInFeetX, 0, offsetInFeetY);

                    using (Transaction trans = new Transaction(doc, "Shift Dimension Text Up and Right"))
                    {
                        trans.Start();

                        foreach (DimensionSegment segment in dimension.Segments)
                        {
                            if(segment.Value - jointDimension <= dimTolerance)
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
