using System.Collections.Generic;
using IVisio = NetOffice.VisioApi;

namespace VisioAutomation.Extensions
{
    public static class SectionMethods
    {
        public static IEnumerable<IVisio.Row> ToEnumerable(this IVisio.Section section)
        {
            return VisioAutomation.ShapeSheet.ShapeSheetHelper.ToEnumerable(section);
        }
    }
}