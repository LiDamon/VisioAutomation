using System.Management.Automation;
using IVisio = NetOffice.VisioApi;

namespace VisioPowerShell.Commands.Get
{
    [Cmdlet(VerbsCommon.Get, VisioPowerShell.Nouns.VisioConnectionPoint)]
    public class Get_VisioConnectionPoint : VisioCmdlet
    {
        [Parameter(Mandatory = false)]
        public IVisio.IVShape[] Shapes;

        [Parameter(Mandatory = false)]
        public SwitchParameter GetCells;

        protected override void ProcessRecord()
        {
            var targets = new VisioAutomation.Scripting.TargetShapes(this.Shapes);

            var dic = this.Client.ConnectionPoint.Get(targets);

            if (this.GetCells)
            {
                this.WriteObject(dic);
                return;
            }

            foreach (var shape_points in dic)
            {
                var shape = shape_points.Key;
                var points = shape_points.Value;

                int shapeid = shape.ID;

                foreach (var point_cells in points)
                {
                    var cp = new Model.ConnectionPointValues(shapeid, point_cells);
                    this.WriteObject(cp);
                }
            }
        }

    }
}