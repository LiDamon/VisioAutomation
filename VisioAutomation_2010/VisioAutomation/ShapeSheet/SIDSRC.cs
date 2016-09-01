using System.Collections.Generic;
using IVisio = NetOffice.VisioApi;

namespace VisioAutomation.ShapeSheet
{
    public struct SIDSRC
    {
        public short ShapeID { get; }
        public short Section { get; }
        public short Row { get; }
        public short Cell { get; }

        public SIDSRC(
            short shape_id,
            IVisio.Enums.VisSectionIndices section,
            IVisio.Enums.VisRowIndices row,
            IVisio.Enums.VisCellIndices cell) : this(shape_id,(short)section,(short)row,(short)cell)
        {
        }

        public SIDSRC(
            short shape_id,
            short section,
            short row,
            short cell) : this()
        {
            this.ShapeID = shape_id;
            this.Section = section;
            this.Row = row;
            this.Cell = cell;
        }

        public SIDSRC(
            short shape_id,
            SRC src) : this(shape_id,src.Section,src.Row,src.Cell)
        {
        }  
        
        public override string ToString()
        {
            return string.Format("{0}({1},{2},{3},{4})", nameof(SIDSRC),this.ShapeID, this.Section, this.Row, this.Cell);
        }

        public static short [] ToStream(IList<SIDSRC> sidsrcs)
        {
            const int sidsrc_length = 4;
            var s = new short[sidsrc_length*sidsrcs.Count];
            for (int i = 0; i < sidsrcs.Count; i++)
            {
                var sidsrc = sidsrcs[i];
                int pos = i*sidsrc_length;
                s[pos + 0] = sidsrc.ShapeID;
                s[pos + 1] = sidsrc.Section;
                s[pos + 2] = sidsrc.Row;
                s[pos + 3] = sidsrc.Cell;
            }
            return s;
        }

        public SRC SRC
        {
            get { return new SRC(this.Section, this.Row, this.Cell); }
        }
    }
}