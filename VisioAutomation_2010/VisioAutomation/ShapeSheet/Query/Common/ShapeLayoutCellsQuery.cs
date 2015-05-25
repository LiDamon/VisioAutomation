namespace VisioAutomation.ShapeSheet.Query.Common
{
    class ShapeLayoutCellsQuery : CellQuery
    {
        public Query.CellColumn ConFixedCode { get; set; }
        public Query.CellColumn ConLineJumpCode { get; set; }
        public Query.CellColumn ConLineJumpDirX { get; set; }
        public Query.CellColumn ConLineJumpDirY { get; set; }
        public Query.CellColumn ConLineJumpStyle { get; set; }
        public Query.CellColumn ConLineRouteExt { get; set; }
        public Query.CellColumn ShapeFixedCode { get; set; }
        public Query.CellColumn ShapePermeablePlace { get; set; }
        public Query.CellColumn ShapePermeableX { get; set; }
        public Query.CellColumn ShapePermeableY { get; set; }
        public Query.CellColumn ShapePlaceFlip { get; set; }
        public Query.CellColumn ShapePlaceStyle { get; set; }
        public Query.CellColumn ShapePlowCode { get; set; }
        public Query.CellColumn ShapeRouteStyle { get; set; }
        public Query.CellColumn ShapeSplit { get; set; }
        public Query.CellColumn ShapeSplittable { get; set; }
        public Query.CellColumn DisplayLevel { get; set; }
        public Query.CellColumn Relationships { get; set; }

        public ShapeLayoutCellsQuery() :
            base()
        {
            this.ConFixedCode = this.AddCell(ShapeSheet.SRCConstants.ConFixedCode, nameof(ShapeSheet.SRCConstants.ConFixedCode));
            this.ConLineJumpCode = this.AddCell(ShapeSheet.SRCConstants.ConLineJumpCode, nameof(ShapeSheet.SRCConstants.ConLineJumpCode));
            this.ConLineJumpDirX = this.AddCell(ShapeSheet.SRCConstants.ConLineJumpDirX, nameof(ShapeSheet.SRCConstants.ConLineJumpDirX));
            this.ConLineJumpDirY = this.AddCell(ShapeSheet.SRCConstants.ConLineJumpDirY, nameof(ShapeSheet.SRCConstants.ConLineJumpDirY));
            this.ConLineJumpStyle = this.AddCell(ShapeSheet.SRCConstants.ConLineJumpStyle, nameof(ShapeSheet.SRCConstants.ConLineJumpStyle));
            this.ConLineRouteExt = this.AddCell(ShapeSheet.SRCConstants.ConLineRouteExt, nameof(ShapeSheet.SRCConstants.ConLineRouteExt));
            this.ShapeFixedCode = this.AddCell(ShapeSheet.SRCConstants.ShapeFixedCode, nameof(ShapeSheet.SRCConstants.ShapeFixedCode));
            this.ShapePermeablePlace = this.AddCell(ShapeSheet.SRCConstants.ShapePermeablePlace, nameof(ShapeSheet.SRCConstants.ShapePermeablePlace));
            this.ShapePermeableX = this.AddCell(ShapeSheet.SRCConstants.ShapePermeableX, nameof(ShapeSheet.SRCConstants.ShapePermeableX));
            this.ShapePermeableY = this.AddCell(ShapeSheet.SRCConstants.ShapePermeableY, nameof(ShapeSheet.SRCConstants.ShapePermeableY));
            this.ShapePlaceFlip = this.AddCell(ShapeSheet.SRCConstants.ShapePlaceFlip, nameof(ShapeSheet.SRCConstants.ShapePlaceFlip));
            this.ShapePlaceStyle = this.AddCell(ShapeSheet.SRCConstants.ShapePlaceStyle, nameof(ShapeSheet.SRCConstants.ShapePlaceStyle));
            this.ShapePlowCode = this.AddCell(ShapeSheet.SRCConstants.ShapePlowCode, nameof(ShapeSheet.SRCConstants.ShapePlowCode));
            this.ShapeRouteStyle = this.AddCell(ShapeSheet.SRCConstants.ShapeRouteStyle, nameof(ShapeSheet.SRCConstants.ShapeRouteStyle));
            this.ShapeSplit = this.AddCell(ShapeSheet.SRCConstants.ShapeSplit, nameof(ShapeSheet.SRCConstants.ShapeSplit));
            this.ShapeSplittable = this.AddCell(ShapeSheet.SRCConstants.ShapeSplittable, nameof(ShapeSheet.SRCConstants.ShapeSplittable));
            this.DisplayLevel = this.AddCell(ShapeSheet.SRCConstants.DisplayLevel, nameof(ShapeSheet.SRCConstants.DisplayLevel));
            this.Relationships = this.AddCell(ShapeSheet.SRCConstants.Relationships, nameof(ShapeSheet.SRCConstants.Relationships));



        }

        public Shapes.Layout.ShapeLayoutCells GetCells(System.Collections.Generic.IList<ShapeSheet.CellData<double>> row)
        {
            var cells = new Shapes.Layout.ShapeLayoutCells();
            cells.ConFixedCode = Extensions.CellDataMethods.ToInt(row[this.ConFixedCode]);
            cells.ConLineJumpCode = Extensions.CellDataMethods.ToInt(row[this.ConLineJumpCode]);
            cells.ConLineJumpDirX = Extensions.CellDataMethods.ToInt(row[this.ConLineJumpDirX]);
            cells.ConLineJumpDirY = Extensions.CellDataMethods.ToInt(row[this.ConLineJumpDirY]);
            cells.ConLineJumpStyle = Extensions.CellDataMethods.ToInt(row[this.ConLineJumpStyle]);
            cells.ConLineRouteExt = Extensions.CellDataMethods.ToInt(row[this.ConLineRouteExt]);
            cells.ShapeFixedCode = Extensions.CellDataMethods.ToInt(row[this.ShapeFixedCode]);
            cells.ShapePermeablePlace = Extensions.CellDataMethods.ToInt(row[this.ShapePermeablePlace]);
            cells.ShapePermeableX = Extensions.CellDataMethods.ToInt(row[this.ShapePermeableX]);
            cells.ShapePermeableY = Extensions.CellDataMethods.ToInt(row[this.ShapePermeableY]);
            cells.ShapePlaceFlip = Extensions.CellDataMethods.ToInt(row[this.ShapePlaceFlip]);
            cells.ShapePlaceStyle = Extensions.CellDataMethods.ToInt(row[this.ShapePlaceStyle]);
            cells.ShapePlowCode = Extensions.CellDataMethods.ToInt(row[this.ShapePlowCode]);
            cells.ShapeRouteStyle = Extensions.CellDataMethods.ToInt(row[this.ShapeRouteStyle]);
            cells.ShapeSplit = Extensions.CellDataMethods.ToInt(row[this.ShapeSplit]);
            cells.ShapeSplittable = Extensions.CellDataMethods.ToInt(row[this.ShapeSplittable]);
            cells.DisplayLevel = Extensions.CellDataMethods.ToInt(row[this.DisplayLevel]);
            cells.Relationships = Extensions.CellDataMethods.ToInt(row[this.Relationships]);
            return cells;
        }
    }
}