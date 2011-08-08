﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.Interop.Visio;
using VisioAutomation.Drawing;
using VA=VisioAutomation;
using VisioAutomation.Extensions;
using IVisio = Microsoft.Office.Interop.Visio;

namespace VisioAutomation.Infographics
{
    public class SingleValuePieChartGrid : Block
    {
        public IList<DataPoint> DataPoints;
        public string FontName;
        public VA.Drawing.ColorRGB CellRectColor = new ColorRGB(0xe0e0e0);
        public VA.Drawing.ColorRGB LineColor = new ColorRGB(0xc0c0c0);
        public VA.Drawing.ColorRGB ValueColor = new ColorRGB(0xa0a0a0);
        public VA.Drawing.ColorRGB NonValueColor = new ColorRGB(0xffffff);
        public VA.Drawing.ColorRGB BKColor = new ColorRGB(0xf0f0f0);

        public override Size Render(Page page, Point upperleft)
        {
            var datapoints = this.DataPoints;

            int rows = 2;
            int cols = 3;
            // ensure rows and colums >= 1

            int max_items = rows * cols;
            // ensure max_items >= datapoints

            var doc = page.Document;
            var fonts = doc.Fonts;

            var myfont = VA.Text.TextHelper.TryGetFont(fonts, this.FontName);
            if (myfont == null)
            {
                myfont = fonts["Arial"];
            }

            var margin = new VA.Drawing.Size(0.25, 0.25);

            int allocrows = System.Math.Max(1, datapoints.Count / cols);
            int alloccols = System.Math.Max(1, cols);
            var cellsize = new VA.Drawing.Size(2.0, 1.5);

            var bkrect = new VA.Drawing.Rectangle(0, -(allocrows * cellsize.Height + margin.Height + margin.Height), alloccols * cellsize.Width + margin.Width + margin.Width, 0).Add(upperleft);

            var bkshape = page.DrawRectangle(bkrect);

            var bkfmt = new VA.Format.ShapeFormatCells();
            bkfmt.FillForegnd = this.BKColor.ToFormula();
            bkfmt.LinePattern = 0;
            bkfmt.LineWeight = 0;//  VA.Convert.PointsToInches(1.0);
            bkfmt.LineColor = 0; //this.LineColor.ToFormula();

            var tb_fmt = new VA.Text.TextBlockFormatCells();
            tb_fmt.VerticalAlign = 0;
            var origin = bkrect.UpperLeft.Add(margin.Width, -margin.Height);

            var cellfmt = new VA.Format.ShapeFormatCells();
            cellfmt.FillForegnd = this.CellRectColor.ToFormula();
            cellfmt.LinePattern = 0;
            cellfmt.LineWeight = 0.0;

            var cellcharfmt = new VA.Text.CharacterFormatCells();
            cellcharfmt.Font = myfont.ID;

            var valfmt = new VA.Format.ShapeFormatCells();
            valfmt.FillForegnd = this.ValueColor.ToFormula();
            valfmt.LinePattern = 1;
            valfmt.LineWeight = VA.Convert.PointsToInches(1.0);
            valfmt.LineColor = this.LineColor.ToFormula();

            var nonvalfmt = new VA.Format.ShapeFormatCells();
            nonvalfmt.FillForegnd = this.NonValueColor.ToFormula();
            nonvalfmt.LinePattern = 1;
            nonvalfmt.LineWeight = VA.Convert.PointsToInches(1.0);
            nonvalfmt.LineColor = this.LineColor.ToFormula();

            var rect_shapes = new List<IVisio.Shape>(datapoints.Count());


            var value_shapes = new List<IVisio.Shape>(datapoints.Count());
            var nonvalue_shapes = new List<IVisio.Shape>(datapoints.Count());

            foreach (int row in Enumerable.Range(0, rows))
            {
                foreach (int col in Enumerable.Range(0, cols))
                {
                    int dp_index = (row * cols) + col;
                    if (dp_index < datapoints.Count())
                    {
                        // Get datapoint
                        var dp = datapoints[dp_index];

                        // Handle background cell
                        var ul = origin.Add(col * cellsize.Width, -row * cellsize.Height);
                        var ll = ul.Add(0, -cellsize.Height);
                        var ur = ll.Add(cellsize.Width, cellsize.Height);
                        var cellrect = new VA.Drawing.Rectangle(ll, ur);
                        var cellshape = page.DrawRectangle(cellrect);
                        cellshape.Text = dp.Label;
                        rect_shapes.Add(cellshape);

                        // draw background
                        var piecenter = cellrect.Center;
                        var pieradius = System.Math.Min(cellrect.Width, cellrect.Height) / 4.0;
                        var piedata = new[] { dp.Value, 100.0 - dp.Value };
                        var shapes = VA.Layout.LayoutHelper.DrawPieSlices(page, piecenter, pieradius, piedata);
                        var value_shape = shapes[0];
                        var nonvalue_shape = shapes[1];

                        value_shapes.Add(value_shape);
                        nonvalue_shapes.Add(nonvalue_shape);

                    }
                }
            }

            var update = new VA.ShapeSheet.Update.SIDSRCUpdate();

            // format cells rects
            foreach (var shape in rect_shapes)
            {
                short shapeid = shape.ID16;
                tb_fmt.Apply(update, shapeid);
                cellfmt.Apply(update, shapeid);
                cellcharfmt.Apply(update, shapeid, 0);
            }

            foreach (var shape in value_shapes)
            {
                short shapeid = shape.ID16;
                valfmt.Apply(update, shapeid);
            }

            foreach (var shape in nonvalue_shapes)
            {
                short shapeid = shape.ID16;
                nonvalfmt.Apply(update, shapeid);
            }

            bkfmt.Apply(update, bkshape.ID16);

            update.Execute(page);

            return bkrect.Size;

        }
    }
}
