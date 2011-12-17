﻿using System.Collections.Generic;
using IVisio = Microsoft.Office.Interop.Visio;
using VA=VisioAutomation;

namespace VisioAutomation.Extensions
{
    public static class WindowMethods
    {
        public static void Select(
            this IVisio.Window window,
            IEnumerable<IVisio.Shape> shapes,
            IVisio.VisSelectArgs selectargs)
        {
            if (shapes == null)
            {
                throw new System.ArgumentNullException("shapes");
            }

            foreach (var shape in shapes)
            {
                window.Select(shape, (short) selectargs);
            }
        }

        public static VA.Drawing.Rectangle GetViewRect(this IVisio.Window window)
        {
            double left, top, height, width;
            window.GetViewRect(out left, out top, out width, out height);
            double x0 = left;
            double x1 = left + width;
            double y0 = top - height;
            double y1 = y0 + height;

            var r = new VA.Drawing.Rectangle(x0, y0, x1, y1);
            return r;
        }

        public static System.Drawing.Rectangle GetWindowRect(this IVisio.Window window)
        {
            int left, top, height, width;
            window.GetWindowRect(out left, out top, out width, out height);
            var r = new System.Drawing.Rectangle(left, top, width, height);
            return r;
        }

        public static void SetWindowRect(
            this IVisio.Window window, 
            System.Drawing.Rectangle rect)
        {
            window.SetWindowRect(rect.Left, rect.Top, rect.Width, rect.Height);
        }

        public static void SetViewRect(
            this IVisio.Window window, 
            VA.Drawing.Rectangle rect)
        {
            window.SetViewRect(rect.Left, rect.Top, rect.Width, rect.Height);
        }
    }
}