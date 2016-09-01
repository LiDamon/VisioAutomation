﻿using VisioAutomation.ShapeSheet.Writers;
using IVisio = NetOffice.VisioApi;

namespace VisioAutomation.Scripting
{
    public class ShapeSheetWriter
    {
        internal readonly FormulaWriterSIDSRC formula_writer;
        internal readonly ResultWriterSIDSRC  result_writer;
        public Client Client;
        public IVisio.IVPage TargetPage;
        public bool BlastGuards;
        public bool TestCircular;

        public ShapeSheetWriter(Client client, IVisio.IVPage page)
        {
            this.Client = client;
            this.TargetPage = page;
            this.formula_writer = new FormulaWriterSIDSRC();
            this.result_writer = new ResultWriterSIDSRC();
        }

        public void SetFormula(short id, ShapeSheet.SRC src, string formula)
        {
            var sidsrc = new ShapeSheet.SIDSRC(id, src);
            this.formula_writer.SetFormula(sidsrc, formula);
        }

        public void SetResult(short id, ShapeSheet.SRC src, double result)
        {
            var sidsrc = new ShapeSheet.SIDSRC(id, src);
            this.result_writer.SetResult(sidsrc, result, IVisio.Enums.VisUnitCodes.visNumber);
        }

        public void SetResult(short id, ShapeSheet.SRC src, int result)
        {
            var sidsrc = new ShapeSheet.SIDSRC(id, src);
            this.result_writer.SetResult(sidsrc, result, IVisio.Enums.VisUnitCodes.visNumber);
        }

        public void SetResult(short id, ShapeSheet.SRC src, string result)
        {
            var sidsrc = new ShapeSheet.SIDSRC(id, src);
            this.result_writer.SetResult(sidsrc, result, IVisio.Enums.VisUnitCodes.visNumber);
        }

        public void Commit()
        {
            var application = this.Client.Application.Get();
            using (var undoscope = this.Client.Application.NewUndoScope("Modify ShapeSheet"))
            {
                this.formula_writer.BlastGuards = this.BlastGuards;
                this.formula_writer.TestCircular = this.TestCircular;
                this.formula_writer.Commit(this.TargetPage);

                this.result_writer.BlastGuards = this.BlastGuards;
                this.result_writer.TestCircular = this.TestCircular;
                this.result_writer.Commit(this.TargetPage);

            }
        }
    }
}