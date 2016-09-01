using System.Management.Automation;
using IVisio = NetOffice.VisioApi;

namespace VisioPowerShell.Commands.Get
{
    [Cmdlet(VerbsCommon.Get, VisioPowerShell.Nouns.VisioMaster)]
    public class Get_VisioMaster : VisioCmdlet
    {
        [Parameter(Position = 0, Mandatory = false)]
        public string Name;

        [Parameter(Position = 1, Mandatory = false)]
        public IVisio.IVDocument Document;

        protected override void ProcessRecord()
        {
            bool master_specified = this.Name !=null;
            bool doc_specified = this.Document !=null;

            if (master_specified)
            {
                // master name is specified
                if (doc_specified)
                {
                    ((Cmdlet) this).WriteVerbose("Get master from specified document");
                    var masters = this.Client.Master.GetMastersByName(this.Name, this.Document);
                    this.WriteObject(masters,true);
                }
                else
                {
                    ((Cmdlet) this).WriteVerbose("Get master from active document");
                    var masters = this.Client.Master.GetMastersByName(this.Name);
                    this.WriteObject(masters,true);
                }
            }
            else
            {
                // master name is not specified
                if (doc_specified)
                {
                    ((Cmdlet) this).WriteVerbose("Get all masters from specified document");
                    var masters = this.Client.Master.Get(this.Document);
                    this.WriteObject(masters, false);                    
                }
                else
                {
                    ((Cmdlet) this).WriteVerbose("Get all masters from active document");
                    var masters = this.Client.Master.Get();
                    this.WriteObject(masters, false);                   
                }
            }
        }
    }
}