using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Prism.Commands;
using ReviyAPITrainingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPITrainingCountUI
{
    public class MainViewViewModel
    {
        private ExternalCommandData _commandData;

        public DelegateCommand SelectPipe { get; }
        public DelegateCommand SelectWalls { get; }
        public DelegateCommand SelectDoors { get; }

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            SelectPipe = new DelegateCommand(OnSelectPipe);
            SelectWalls = new DelegateCommand(OnSelectWalls);
            SelectDoors = new DelegateCommand(OnSelectDoors);
        }
     
        public event EventHandler HideRequest;
        private void RaiseHideRequest()
        {
            HideRequest?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler ShowRequest;
        private void RaiseShowRequest()
        {
            ShowRequest?.Invoke(this, EventArgs.Empty);
        }

        private void OnSelectPipe()
        {
            RaiseHideRequest();

            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var pipes = new FilteredElementCollector(doc)
                .OfClass(typeof(Pipe))
                .Cast<Pipe>()
                .ToList();

            TaskDialog.Show("Pipe info", pipes.Count.ToString());
            
            RaiseShowRequest();
        }

        private void OnSelectWalls()
        {
            RaiseHideRequest();

            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            IList<Reference> selectedElementRefList = uidoc.Selection.PickObjects(ObjectType.Element, new WallFilter(), "Выберете элементы по грани");
            var wallList = new List<double>();           
            double sumVolume = 0;           
            foreach (var selectedElement in selectedElementRefList)
            {
                Wall owall = doc.GetElement(selectedElement) as Wall;
                Parameter volume = owall.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED);
                double volumeValue = UnitUtils.ConvertFromInternalUnits(volume.AsDouble(), DisplayUnitType.DUT_CUBIC_METERS);
                sumVolume += volumeValue;
            }
            TaskDialog.Show("Объем стен", sumVolume.ToString());

            RaiseShowRequest();
        }
        private void OnSelectDoors()
        {
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            List<FamilyInstance> fInstances = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Doors)
                .WhereElementIsNotElementType()
                .Cast<FamilyInstance>()
                .ToList();

            TaskDialog.Show("Pipe info", fInstances.Count.ToString());
        }
    }
}
