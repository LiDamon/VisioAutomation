using System.Collections.Generic;
using System.Linq;
using VisioAutomation.Models.Dom;
using VisioAutomation.Models.Layouts.DirectedGraph;
using VisioAutomation.Scripting.Utilities;
using VisioAutomation.Shapes.Connectors;
using SXL = System.Xml.Linq;

namespace VisioAutomation.Scripting.DirectedGraph
{
    public class DirectedGraphBuilder
    {
        private class BuilderError
        {
            public readonly string Text;

            public BuilderError(string text)
            {
                this.Text = text;
            }

            public static BuilderError ConnectorAlreadyDefined(int pagenum, string id)
            {
                string msg = string.Format("Page {0} : Connector \"{1}\" is already defined", pagenum, id);
                return new BuilderError(msg);
            }

            public static BuilderError NodeAlreadyDefined(int pagenum, string id)
            {
                string msg = string.Format("Page {0} : Node \"{1}\" is already defined", pagenum, id);
                return new BuilderError(msg);
            }

            public static BuilderError InvalidFromNode(int pagenum, string conid, string fromid)
            {
                string msg = string.Format("Page {0} : Connector \"{1}\" references a nonexistent FROM Node \"{2}\"",
                    pagenum, conid, fromid);
                return new BuilderError( msg);
            }

            public static BuilderError InvalidToNode(int pagenum, string conid, string toid)
            {
                string msg = string.Format("Page {0} : Connector \"{1}\" references a nonexistent TO Node \"{2}\"",
                    pagenum, conid, toid);
                return new BuilderError(msg);
            }
        }

        public static List<DirectedGraphLayout> LoadFromXML(Client client, string filename)
        {
            var xmldoc = SXL.XDocument.Load(filename);
            return DirectedGraphBuilder.LoadFromXML(client, xmldoc);
        }

        private class PageData
        {
            public MsaglLayoutOptions LayoutOptions;
            public DirectedGraphLayout DirectedGraph;
            public List<ShapeInfo> ShapeInfos;
            public List<ConnectorInfo> ConnectorInfos;
            public List<BuilderError> Errors;
        }

        private static List<PageData> LoadPageDataFromXML(Client client, SXL.XDocument xmldoc)
        {
            var pagedatas = new List<PageData>();
            // LOAD and ANALYZE EACH PAGE

            int pagenum = 0;
            var page_els = xmldoc.Root.Elements("page");

            foreach (var page_el in page_els)
            {
                var node_ids = new HashSet<string>();
                var con_ids = new HashSet<string>();

                var pagedata = new PageData();
                pagedatas.Add(pagedata);
                pagedata.Errors = new List<BuilderError>();
                pagedata.LayoutOptions = new MsaglLayoutOptions();
                var renderoptions_el = page_el.Element("renderoptions");
                DirectedGraphBuilder.GetRenderOptionsFromXml(renderoptions_el, pagedata.LayoutOptions);

                pagedata.DirectedGraph = new DirectedGraphLayout();
                var shape_els = page_el.Element("shapes").Elements("shape");
                var con_els = page_el.Element("connectors").Elements("connector");

                pagedata.ShapeInfos = shape_els.Select(e => ShapeInfo.FromXml(client, e)).ToList();
                pagedata.ConnectorInfos = con_els.Select(e => ConnectorInfo.FromXml(client, e)).ToList();

                client.WriteVerbose( "Analyzing shape data for page {0}", pagenum);
                foreach (var shape_info in pagedata.ShapeInfos)
                {
                    client.WriteVerbose( "shape {0}", shape_info.ID);

                    if (node_ids.Contains(shape_info.ID))
                    {
                        pagedata.Errors.Add(BuilderError.NodeAlreadyDefined(pagenum, shape_info.ID));
                    }
                    else
                    {
                        node_ids.Add(shape_info.ID);
                    }
                }

                client.WriteVerbose( "Analyzing connector data...");
                foreach (var con_info in pagedata.ConnectorInfos)
                {
                    client.WriteVerbose( "connector {0}", con_info.ID);

                    if (con_ids.Contains(con_info.ID))
                    {
                        pagedata.Errors.Add(BuilderError.ConnectorAlreadyDefined(pagenum, con_info.ID));
                    }
                    else
                    {
                        con_ids.Add(con_info.ID);
                    }

                    if (!node_ids.Contains(con_info.From))
                    {
                        pagedata.Errors.Add(BuilderError.InvalidFromNode(pagenum, con_info.ID, con_info.From));
                    }

                    if (!node_ids.Contains(con_info.To))
                    {
                        pagedata.Errors.Add(BuilderError.InvalidToNode(pagenum, con_info.ID, con_info.To));
                    }
                }
            }

            return pagedatas;
        }

        public static List<DirectedGraphLayout> LoadFromXML(Client client, SXL.XDocument xmldoc)
        {
            var pagedatas = DirectedGraphBuilder.LoadPageDataFromXML(client, xmldoc);

            // STOP IF ANY ERRORS
            int num_errors = pagedatas.Select(pagedata => pagedata.Errors.Count).Sum();
            if (num_errors > 1)
            {
                foreach (var pagedata in pagedatas)
                {
                    foreach (var error in pagedata.Errors)
                    {
                        client.WriteVerbose( error.Text);
                    }
                    client.WriteVerbose( "Errors encountered in shape data. Stopping.");
                }
            }

            // DRAW EACH PAGE
            foreach (var pagedata in pagedatas)
            {
                client.WriteVerbose( "Creating shape AutoLayout nodes");
                foreach (var shape_info in pagedata.ShapeInfos)
                {
                    var dg_shape = pagedata.DirectedGraph.AddShape(shape_info.ID, shape_info.Label, shape_info.Stencil, shape_info.Master);
                    dg_shape.URL = shape_info.URL;
                    dg_shape.CustomProperties = new VisioAutomation.Shapes.CustomProperties.CustomPropertyDictionary();
                    foreach (var kv in shape_info.custprops)
                    {
                        dg_shape.CustomProperties[kv.Key] = kv.Value;
                    }
                }

                client.WriteVerbose( "Creating connector AutoLayout nodes");
                foreach (var con_info in pagedata.ConnectorInfos)
                {
                    var def_connector_type = ConnectorType.Curved;
                    var connectory_type = def_connector_type;

                    var from_shape = pagedata.DirectedGraph.Shapes.Find(con_info.From);
                    var to_shape = pagedata.DirectedGraph.Shapes.Find(con_info.To);

                    var def_con_color = new VisioAutomation.Drawing.ColorRGB(0x000000);
                    var def_con_weight = 1.0/72.0;
                    var def_end_arrow = 2;
                    var dg_connector = pagedata.DirectedGraph.AddConnection(con_info.ID, from_shape, to_shape, con_info.Label, connectory_type);

                    dg_connector.Cells = new ShapeCells();
                    dg_connector.Cells.LineColor = con_info.Element.AttributeAsColor("color", def_con_color).ToFormula();
                    dg_connector.Cells.LineWeight = con_info.Element.AttributeAsInches("weight", def_con_weight);
                    dg_connector.Cells.LineEndArrow = def_end_arrow;
                }
                client.WriteVerbose( "Rendering AutoLayout...");
            }
            client.WriteVerbose( "Finished rendering AutoLayout");

            var directedgraphs = pagedatas.Select(pagedata => pagedata.DirectedGraph).ToList();
            return directedgraphs;
        }

        private static void GetRenderOptionsFromXml(SXL.XElement el, MsaglLayoutOptions options)
        {
            System.Func<string, double> double_parse =
                str => double.Parse(str, System.Globalization.CultureInfo.InvariantCulture);

            options.UseDynamicConnectors = el.GetAttributeValue("usedynamicconnectors", bool.Parse);
            options.ScalingFactor = el.GetAttributeValue("scalingfactor", double_parse);
        }
    }
}