using System.Management.Automation;

namespace VisioPowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, VisioPowerShell.Nouns.VisioLayer)]
    public class Get_VisioLayer : VisioCmdlet
    {
        [Parameter(Position = 0, Mandatory = false)]
        public string Name;

        protected override void ProcessRecord()
        {
            if (this.Name!=null || this.Name=="*")
            {
                var layer = this.Client.Layer.Get(this.Name);
                this.WriteObject(layer);
            }
            else
            {
                var layers = this.Client.Layer.Get();
                this.WriteObject(layers,false);
            }
        }
    }
}