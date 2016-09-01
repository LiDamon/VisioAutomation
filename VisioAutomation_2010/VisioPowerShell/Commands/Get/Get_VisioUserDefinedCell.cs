using System.Management.Automation;
using IVisio = NetOffice.VisioApi;

namespace VisioPowerShell.Commands.Get
{
    [Cmdlet(VerbsCommon.Get, VisioPowerShell.Nouns.VisioUserDefinedCell)]
    public class Get_VisioUserDefinedCell : VisioCmdlet
    {
        [Parameter(Mandatory = false)]
        public IVisio.Shape[] Shapes;

        [Parameter(Mandatory = false)]
        public SwitchParameter GetCells;

        protected override void ProcessRecord()
        {
            var targets = new VisioAutomation.Scripting.TargetShapes(this.Shapes);
            var dic = this.Client.UserDefinedCell.Get(targets);

            if (this.GetCells)
            {
                this.WriteObject(dic);
                return;
            }

            foreach (var kv in dic)
            {
                int shapeid = kv.Key.ID;
                foreach (var udc in kv.Value)
                {
                    var udcell_vals = new Model.UserDefinedCellvalues(shapeid, udc.Name, udc.Value.Formula.Value,udc.Prompt.Formula.Value);
                    this.WriteObject(udcell_vals);
                }
            }
        }
    }
}