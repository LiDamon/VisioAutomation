using System.Collections.Generic;
using System.Linq;
using VisioAutomation.Extensions;
using IVisio = Microsoft.Office.Interop.Visio;
using VA = VisioAutomation;

namespace VisioAutomation.Scripting.Commands
{
    public class LayerCommands : CommandSet
    {
        public LayerCommands(Session session) :
            base(session)
        {

        }

        public IVisio.Layer GetLayer(string layername)
        {
            this.CheckApplication();

            if (layername == null)
            {
                throw new System.ArgumentNullException("layername");
            }

            if (layername.Length < 1)
            {
                throw new System.ArgumentException("layername");
            }

            var application = this.Session.VisioApplication;
            var page = application.ActivePage;
            IVisio.Layer layer = null;
            try
            {
                var layers = page.Layers;
                layer = layers.ItemU[layername];
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                string msg = string.Format("No such layer \"{0}\"", layername);
                throw new AutomationException(msg);
            }
            return layer;
        }

        public IList<IVisio.Layer> GetLayers()
        {
            this.CheckApplication();

            if (!this.Session.HasActiveDrawing)
            {
                new List<IVisio.Layer>(0);
            }

            var application = this.Session.VisioApplication;
            var page = application.ActivePage;
            return page.Layers.AsEnumerable().ToList();
        }

    }
}