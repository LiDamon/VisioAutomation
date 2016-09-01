using System.Management.Automation;
using IVisio = NetOffice.VisioApi;

namespace VisioPowerShell.Commands.Get
{
    [Cmdlet(VerbsCommon.Get, VisioPowerShell.Nouns.VisioCustomProperty)]
    public class Get_VisioCustomProperty : VisioCmdlet
    {
        [Parameter(Mandatory = false)]
        public IVisio.Shape[] Shapes;

        [Parameter(Mandatory = false)]
        public SwitchParameter GetCells;

        protected override void ProcessRecord()
        {
            var targets = new VisioAutomation.Scripting.TargetShapes(this.Shapes);
            var dic = this.Client.CustomProp.Get(targets);

            if (this.GetCells)
            {
                this.WriteObject(dic);                
                return;
            }

            foreach (var shape_propdic_pair in dic)
            {
                var shape = shape_propdic_pair.Key;
                var propdic = shape_propdic_pair.Value;
                int shape_id = shape.ID;
                foreach (var propname_propcells_pair in propdic)
                {
                    string propname = propname_propcells_pair.Key;
                    var propcells = propname_propcells_pair.Value;
                    var cpf = new Model.CustomPropertyValues(shape_id, propname, propcells);
                    this.WriteObject(cpf);
                }
            }
        }
    }
}