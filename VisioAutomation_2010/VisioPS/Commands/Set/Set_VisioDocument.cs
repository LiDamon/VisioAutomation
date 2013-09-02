using VAS=VisioAutomation.Scripting;
using VA = VisioAutomation;
using SMA = System.Management.Automation;
using IVisio = Microsoft.Office.Interop.Visio;

namespace VisioPS.Commands
{
    [SMA.Cmdlet(SMA.VerbsCommon.Set, "VisioDocument")]
    public class Set_VisioDocument : VisioPSCmdlet
    {
        [SMA.Parameter(Position = 0, Mandatory = true, ParameterSetName = "Name")]
        [SMA.ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [SMA.Parameter(Position = 0, Mandatory = true, ParameterSetName = "Doc")]
        [SMA.ValidateNotNull]
        public IVisio.Document Document  { get; set; }
        
        protected override void ProcessRecord()
        {
            if (this.Name != null)
            {
                this.ScriptingSession.Document.Activate(this.Name);
            }
            else if (this.Document != null)
            {
                this.ScriptingSession.Document.Activate(this.Document);
            }
        }
    }
}