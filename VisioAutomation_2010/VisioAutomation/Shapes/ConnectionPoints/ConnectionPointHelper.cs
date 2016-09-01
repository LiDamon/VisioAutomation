using VisioAutomation.ShapeSheet.Writers;
using IVisio = NetOffice.VisioApi;

namespace VisioAutomation.Shapes.ConnectionPoints
{
    public static class ConnectionPointHelper
    {
        public static int Add(
            IVisio.IVShape shape,
            ConnectionPointCells connection_point_cells)
        {
            if (shape == null)
            {
                throw new System.ArgumentNullException(nameof(shape));
            }

            if (!connection_point_cells.X.Formula.HasValue)
            {
                string msg = "Must provide an X Formula";
                throw new System.ArgumentException(msg, nameof(connection_point_cells));
            }

            if (!connection_point_cells.Y.Formula.HasValue)
            {
                string msg = "Must provide an Y Formula";
                throw new System.ArgumentException(msg, nameof(connection_point_cells));
            }

            var n = shape.AddRow((short)IVisio.Enums.VisSectionIndices.visSectionConnectionPts,
                                 (short)IVisio.Enums.VisRowIndices.visRowLast,
                                 (short)IVisio.Enums.VisRowTags.visTagCnnctPt);

            var writer = new FormulaWriterSRC();
            connection_point_cells.SetFormulas(writer,n);
            writer.Commit(shape);

            return n;
        }

        public static void Delete(IVisio.IVShape shape, int index)
        {
            if (shape == null)
            {
                throw new System.ArgumentNullException(nameof(shape));
            }

            if (index < 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(index));
            }

            var row = (IVisio.Enums.VisRowIndices)index;
            shape.DeleteRow( (short) IVisio.Enums.VisSectionIndices.visSectionConnectionPts, (short)row);
        }

        public static int GetCount(IVisio.IVShape shape)
        {
            if (shape == null)
            {
                throw new System.ArgumentNullException(nameof(shape));
            }

            return shape.get_RowCount((short) IVisio.Enums.VisSectionIndices.visSectionConnectionPts);
        }

        public static int Delete(IVisio.IVShape shape)
        {
            if (shape == null)
            {
                throw new System.ArgumentNullException(nameof(shape));
            }

            int n = ConnectionPointHelper.GetCount(shape);
            for (int i = n - 1; i >= 0; i--)
            {
                ConnectionPointHelper.Delete(shape, i);
            }

            return n;
        }
    }
}