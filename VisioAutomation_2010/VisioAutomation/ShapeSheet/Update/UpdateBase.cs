using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IVisio= Microsoft.Office.Interop.Visio;

namespace VisioAutomation.ShapeSheet.Update
{
    public class UpdateBase : IEnumerable<UpdateRecord>
    {
        public bool BlastGuards { get; set; }
        public bool TestCircular { get; set; }

        private UpdateRecord? _first_update;
        private readonly List<UpdateRecord> _updates;

        public void Clear()
        {
            this._updates.Clear();
            this._first_update = null;
        }

        public UpdateBase()
        {
            this._updates = new List<UpdateRecord>();
        }

        public UpdateBase(int capacity)
        {
            this._updates = new List<UpdateRecord>(capacity);
        }

        protected IVisio.VisGetSetArgs ResultFlags
        {
            get
            {
                var flags = this.get_common_flags();
                if ((flags & IVisio.VisGetSetArgs.visSetFormulas) > 0)
                {
                    flags = (IVisio.VisGetSetArgs) ((short) flags | (short)IVisio.VisGetSetArgs.visSetUniversalSyntax);
                }
                return flags;
            }
        }

        protected IVisio.VisGetSetArgs FormulaFlags
        {
            get
            {
                var common_flags = this.get_common_flags();
                var formula_flags = (short)IVisio.VisGetSetArgs.visSetUniversalSyntax;
                var combined_flags = (short) common_flags | formula_flags;
                return (IVisio.VisGetSetArgs) combined_flags;
            }
        }

        private IVisio.VisGetSetArgs get_common_flags()
        {
            IVisio.VisGetSetArgs f_bg = this.BlastGuards ? IVisio.VisGetSetArgs.visSetBlastGuards : 0;
            IVisio.VisGetSetArgs f_tc = this.TestCircular ? IVisio.VisGetSetArgs.visSetTestCircular : 0;

            var flags = (short) f_bg | (short) f_tc;
            return (IVisio.VisGetSetArgs) flags;
        }


        private void CheckFormulaIsNotNull(string formula)
        {
            if (formula == null)
            {
                throw new AutomationException("Null not allowed for formula");
            }
        }

        private void _add_update(UpdateRecord update)
        {
            // This block ensures that only homogeneous updates are constructed
            if (!this._first_update.HasValue)
            {
                this._first_update = update;
            }
            else
            {
                // first validate the stream types
                if (this._first_update.Value.StreamType != update.StreamType)
                {
                    throw new AutomationException("Cannot contain both SRC and SIDSRC updates");
                }

                // Now ensure that we aren't mixing formulas and results
                // Keep in mind that we can mix differnt types of results (strings and numerics)
                if (this._first_update.Value.UpdateType == UpdateType.Formula && update.UpdateType != UpdateType.Formula)
                {
                    if (update.UpdateType != UpdateType.Formula)
                    {
                        throw new AutomationException("Cannot contain both Formula and Result updates");
                    }
                }
                else if (this._first_update.Value.UpdateType == UpdateType.ResultNumeric ||
                         this._first_update.Value.UpdateType == UpdateType.ResultString)
                {
                    if (update.UpdateType == UpdateType.Formula)
                    {
                        throw new AutomationException("Cannot contain both Formula and Result updates");
                    }
                }
            }

            // Now that it is safe, add the record
            this._updates.Add(update);

        }

        protected void _SetFormula(SIDSRC streamitem, FormulaLiteral formula)
        {
            this.CheckFormulaIsNotNull(formula.Value);
            var rec = new UpdateRecord(StreamType.SIDSRC, streamitem, formula.Value);
            this._add_update(rec);
        }
        protected void _SetFormula(SRC streamitem, FormulaLiteral formula)
        {
            this.CheckFormulaIsNotNull(formula.Value);
            var rec = new UpdateRecord(StreamType.SRC, new SIDSRC(-1, streamitem), formula.Value);
            this._add_update(rec);
        }

        protected void _SetFormulaIgnoreNull(SIDSRC streamitem, FormulaLiteral formula)
        {
            if (formula.HasValue)
            {
                this._SetFormula(streamitem, formula);
            }
        }

        protected void _SetFormulaIgnoreNull(SRC streamitem, FormulaLiteral formula)
        {
            if (formula.HasValue)
            {
                this._SetFormula(streamitem, formula);
            }
        }

        protected void _SetResult(SIDSRC streamitem, double value, IVisio.VisUnitCodes unitcode)
        {
            var rec = new UpdateRecord(StreamType.SIDSRC, streamitem, value, unitcode);
            this._add_update(rec);
        }

        protected void _SetResult(SRC streamitem, double value, IVisio.VisUnitCodes unitcode)
        {
            var rec = new UpdateRecord(StreamType.SRC, new SIDSRC(-1, streamitem), value, unitcode);
            this._add_update(rec);
        }

        protected void _SetResult(SIDSRC streamitem, string value, IVisio.VisUnitCodes unitcode)
        {
            var rec = new UpdateRecord(StreamType.SIDSRC, streamitem, value, unitcode);
            this._add_update(rec);
        }

        protected void _SetResult(SRC streamitem, string value, IVisio.VisUnitCodes unitcode)
        {
            var rec = new UpdateRecord(StreamType.SRC, new SIDSRC(-1,streamitem), value, unitcode);
            this._add_update(rec);
        }

        public IEnumerator<UpdateRecord> GetEnumerator()
        {
            foreach (var i in this._updates)
            {
                yield return i;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            // keeps it hidden.
            return this.GetEnumerator();
        }

        public void Execute(IVisio.Page page)
        {
            var surface = new ShapeSheetSurface(page);
            this._Execute(surface);
        }

        public void Execute(IVisio.Shape shape)
        {
            var surface = new ShapeSheetSurface(shape);
            this._Execute(surface);
        }

        public void Execute(ShapeSheetSurface surface)
        {
            this._Execute(surface);
        }

        private void _Execute(ShapeSheetSurface surface)
        {
            // Do nothing if there aren't any updates
            if (this._updates.Count < 1)
            {
                return;
            }

            if (surface.Target.Shape != null)
            {
                if (this._first_update.Value.StreamType == StreamType.SIDSRC)
                {
                    throw new AutomationException("Contains a SIDSRC updates. Need SRC updates");
                }
            }
            else if (surface.Target.Master != null)
            {
                if (this._first_update.Value.StreamType == StreamType.SIDSRC)
                {
                    throw new AutomationException("Contains a SIDSRC updates. Need SRC updates");
                }
            }
            else if (surface.Target.Page != null)
            {
                if (this._first_update.Value.StreamType == StreamType.SRC)
                {
                    throw new AutomationException("Contains a SRC updates. Need SIDSRC updates");
                }
            }

            var stream = this.build_stream();

            if (this._first_update.Value.UpdateType == UpdateType.ResultNumeric ||
                this._first_update.Value.UpdateType == UpdateType.ResultString)
            {
                // Set Results

                // Create the unitcodes and results arrays
                var unitcodes = new object[this._updates.Count];
                var results = new object[this._updates.Count];
                int i = 0;
                foreach (var update in this._updates)
                {
                    unitcodes[i] = update.UnitCode;
                    if (update.UpdateType == UpdateType.ResultNumeric)
                    {
                        results[i] = update.ResultNumeric;
                    }
                    else if (update.UpdateType == UpdateType.ResultString)
                    {
                        results[i] = update.ResultString;
                    }
                    else
                    {
                        throw new AutomationException("Unhandled update type");
                    }
                    i++;
                }

                var flags = this.ResultFlags;

                if (this._first_update.Value.UpdateType == UpdateType.ResultNumeric)
                {
                }
                else if (this._first_update.Value.UpdateType == UpdateType.ResultString)
                {
                    flags |= IVisio.VisGetSetArgs.visGetStrings;
                }

                surface.SetResults(stream, unitcodes, results, (short) flags);
            }
            else
            {
                // Set Formulas

                // Create the formulas array
                var formulas = new object[this._updates.Count];
                int i = 0;
                foreach (var rec in this._updates)
                {
                    formulas[i] = rec.Formula;
                    i++;
                }

                var flags = this.FormulaFlags;

                int c = surface.SetFormulas(stream, formulas, (short) flags);
            }
        }

        private short[] build_stream()
        {
            var st = this._first_update.Value.StreamType;

            if (st == StreamType.SRC)
            {
                var streamb = new List<SRC>(this._updates.Count);
                streamb.AddRange(this._updates.Select(i => i.SIDSRC.SRC));
                return SRC.ToStream(streamb);
            }
            else
            {
                var streamb = new List<SIDSRC>(this._updates.Count);
                streamb.AddRange(this._updates.Select(i => i.SIDSRC));
                return SIDSRC.ToStream(streamb);
            }
        }
    }
}