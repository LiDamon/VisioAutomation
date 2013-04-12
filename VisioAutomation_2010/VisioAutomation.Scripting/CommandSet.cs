﻿using System.Collections.Generic;
using IVisio = Microsoft.Office.Interop.Visio;
using VA=VisioAutomation;
using System.Linq;

namespace VisioAutomation.Scripting
{
    public class CommandSet
    {
        // Keep a reference back to the parent session. This gives access to all other commands
        // for a the current context
        protected readonly Session Session;

        public CommandSet(Session session)
        {
            this.Session = session;
        }


        internal static IEnumerable<System.Reflection.MethodInfo> GetCommandMethods(System.Type mytype)
        {
            var cmdsettype = typeof(VA.Scripting.CommandSet);

            if (!cmdsettype.IsAssignableFrom(mytype))
            {
                string msg = string.Format("{0} must derive from {1}", mytype.Name, cmdsettype.Name);
            }

            var methods = mytype.GetMethods().Where(m => m.IsPublic && !m.IsStatic);

            foreach (var method in methods)
            {
                if (method.Name == "ToString" || method.Name == "GetHashCode" || method.Name == "GetType" || method.Name == "Equals")
                {
                    continue;
                }

                yield return method;
            }
        }

        protected void CheckApplication()
        {
            var has_app = this.Session.HasApplication();
            if (!has_app)
            {
                throw new VA.AutomationException("No Visio Application is bound to to this session");
            }
        }

        protected IList<IVisio.Shape> GetTargetShapes(IList<IVisio.Shape> shapes)
        {
            if (shapes == null)
            {
                // If no collection of shapes were passed in then use the selection
                this.Session.WriteVerbose("GetTargetShapes: Targeting shapes from active selection");
                var out_shapes = this.Session.Selection.GetShapes();
                this.Session.WriteVerbose("GetTargetShapes: Number of shapes = {0}", out_shapes.Count);
                return out_shapes;
            }
            this.Session.WriteVerbose("GetTargetShapes: Targeting specified shapes ");
            this.Session.WriteVerbose("GetTargetShapes: Number of shapes = {0}", shapes.Count);
            return shapes;
        }

        protected int GetTargetSelection(IList<IVisio.Shape> shapes)
        {
            if (shapes == null)
            {
                this.Session.WriteVerbose("GetTargetSelection: Targeting shapes from active selection");
                int n = this.Session.Selection.Count();
                this.Session.WriteVerbose("GetTargetSelection: Number of shapes = {0}", n);
                return n;
            }

            this.Session.WriteVerbose("GetTargetSelection: Targeting specified shapes");
            this.Session.WriteVerbose("GetTargetSelection: Number of shapes specified = {0}", shapes.Count);
            this.Session.WriteVerbose("GetTargetSelection: Clearing Selection");
            this.Session.Selection.SelectNone();
            this.Session.WriteVerbose("GetTargetSelection: Setting selection");
            this.Session.Selection.Select(shapes);
            int n2 = this.Session.Selection.Count();
            this.Session.WriteVerbose("GetTargetSelection: Selection contains {0} shapes", n2);
            return n2;
        }

        protected IVisio.Shape GetTargetShape( IVisio.Shape shape)
        {
            if (shape == null)
            {
                this.Session.WriteVerbose("GetTargetShape: Targeting single shape from active selection");
                // If no collection of shapes were passed in then use the selection
                var out_shapes = this.Session.Selection.GetShapes();
                int n = out_shapes.Count;
                this.Session.WriteVerbose("GetTargetShape: Number of shapes from selection = {0}", n);
                if (out_shapes.Count > 0)
                {
                    this.Session.WriteVerbose("GetTargetShape: More than 1 shape in selection, targeting the first one");                    
                    return out_shapes[0];
                }
                else
                {
                    this.Session.WriteVerbose("GetTargetShape: No shapes in selection, targeting none");
                    return null;
                }
            }
            else
            {
                this.Session.WriteVerbose("GetTargetShape: Targeting specified shape");
                return shape;
            }
        }
    }
}