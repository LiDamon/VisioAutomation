﻿using System.Management.Automation;
using IVisio = NetOffice.VisioApi;

namespace VisioPowerShell.Commands.Remove
{
    [Cmdlet(VerbsCommon.Remove, VisioPowerShell.Nouns.VisioUserDefinedCell)]
    public class Remove_VisioUserDefinedCell : VisioCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public string Name { get; set; }

        [Parameter(Mandatory = false)]
        public IVisio.IVShape[] Shapes;

        protected override void ProcessRecord()
        {
            var targets = new VisioAutomation.Scripting.TargetShapes(this.Shapes);
            this.Client.UserDefinedCell.Delete(targets, this.Name);
        }
    }
}