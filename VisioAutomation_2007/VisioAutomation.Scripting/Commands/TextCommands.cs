using System.Collections.Generic;
using System.Linq;
using VisioAutomation.Extensions;
using IVisio = Microsoft.Office.Interop.Visio;
using VA = VisioAutomation;

namespace VisioAutomation.Scripting.Commands
{
    public class TextCommands: CommandSet
    {
        public TextCommands(Client client) :
            base(client)
        {

        }

        public void Set(IList<IVisio.Shape> target_shapes, IList<string> texts)
        {
            this.AssertApplicationAvailable();
            this.AssertDocumentAvailable();

            if (texts == null || texts.Count<1)
            {
                // do nothing
                return;
            }
            
            var shapes = this.GetTargetShapes(target_shapes);
            if (shapes.Count<1)
            {
                return;
            }

            using (var undoscope = new VA.Application.UndoScope(this.Client.VisioApplication,"Set Shape Text"))
            {
                int numtexts = texts.Count;
                for (int i=0;i<shapes.Count;i++)
                {
                    var shape = shapes[i];
                    var text = texts[i % numtexts];
                    shape.Text = text;
                }
            }
        }

        public IList<string> Get(IList<IVisio.Shape> target_shapes)
        {
            this.AssertApplicationAvailable();
            this.AssertDocumentAvailable();
            
            var shapes = GetTargetShapes(target_shapes);
            if (shapes.Count < 1)
            {
                return new List<string>(0);
            }

            var texts = shapes.Select(s => s.Text).ToList();
            return texts;
        }

        public void ToogleCase(IList<IVisio.Shape> target_shapes)
        {
            this.AssertApplicationAvailable();
            this.AssertDocumentAvailable();
            
            var shapes = this.GetTargetShapes(target_shapes);

            if (shapes.Count < 1)
            {
                return;
            }

            using (var undoscope = new VA.Application.UndoScope(this.Client.VisioApplication,"Toggle Shape Text Case"))
            {
                var shapeids = shapes.Select(s => s.ID).ToList();

                var page = this.Client.VisioApplication.ActivePage;
                // Store all the formatting
                var formats = VA.Text.TextFormat.GetFormat(page, shapeids);

                // Change the text - this will wipe out all the character and paragraph formatting
                foreach (var shape in shapes)
                {
                    string t = shape.Text;
                    if (t.Length < 1)
                    {
                        continue;
                    }
                    shape.Text = TextCommandsUtil.toggle_case(t);
                }

                // Now restore all the formatting - based on any initial formatting from the text

                var update = new VA.ShapeSheet.Update();
                for (int i = 0; i < shapes.Count; i++)
                {
                    var format = formats[i];

                    if (format.CharacterFormats.Count>0)
                    {
                        var fmt = format.CharacterFormats[0];
                        update.SetFormulas((short) shapeids[i], fmt, 0);
                    }

                    if (format.ParagraphFormats.Count > 0)
                    {
                        var fmt = format.ParagraphFormats[0];
                        update.SetFormulas((short)shapeids[i], fmt, 0);
                    }
                }

                update.Execute(page);
            }
        }

        public void SetFont(IList<IVisio.Shape> target_shapes, string fontname)
        {
            this.AssertApplicationAvailable();
            this.AssertDocumentAvailable();

            var shapes = this.GetTargetShapes(target_shapes);
            if (shapes.Count < 1)
            {
                return;
            }
            var application = this.Client.VisioApplication;
            var active_document = application.ActiveDocument;
            var active_doc_fonts = active_document.Fonts;
            var font = active_doc_fonts[fontname];
            IVisio.VisGetSetArgs flags=0;
            var srcs = new[] {VA.ShapeSheet.SRCConstants.CharFont};
            var formulas = new[] { font.ID.ToString() };
            this.Client.ShapeSheet.SetFormula(target_shapes, srcs, formulas, flags);
        }

        public IList<VA.Text.TextFormat> GetFormat(IList<IVisio.Shape> target_shapes)
        {
            this.AssertApplicationAvailable();
            this.AssertDocumentAvailable();

            var shapes = GetTargetShapes(target_shapes);
            if (shapes.Count < 1)
            {
                return new List<VA.Text.TextFormat>(0);
            }

            var selection = this.Client.Selection.Get();
            var shapeids = selection.GetIDs();
            var application = this.Client.VisioApplication;
            var formats = VA.Text.TextFormat.GetFormat(application.ActivePage, shapeids);
            return formats;
        }
    }
}