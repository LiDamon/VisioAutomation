using Microsoft.VisualStudio.TestTools.UnitTesting;
using VACXN = VisioAutomation.Shapes.Connections;
using VA = VisioAutomation;

namespace TestVisioAutomation
{
    [TestClass]
    public class ScriptingConnectionPointTests : VisioAutomationTest
    {
        [TestMethod]
        public void Scripting_ConnectionPoints_Scenarios()
        {
            var ss = GetScriptingClient();
            ss.Document.New();
            ss.Page.New(new VA.Drawing.Size(4, 4), false);

            var s1 = ss.Draw.Rectangle(1, 1, 1.25, 1.5);

            var s2 = ss.Draw.Rectangle(2, 3, 2.5, 3.5);

            var s3 = ss.Draw.Rectangle(4.5, 2.5, 6, 3.5);

            ss.Selection.None();
            ss.Selection.Select(s1);
            ss.Selection.Select(s2);
            ss.Selection.Select(s3);

            var indices0 = ss.ConnectionPoint.Add("0", "Width*0.67",
                                                 VACXN.ConnectionPointType.Outward);
            Assert.AreEqual(3, indices0.Count);
            Assert.AreEqual(0, indices0[0]);
            Assert.AreEqual(0, indices0[1]);
            Assert.AreEqual(0, indices0[2]);

            var dic = ss.ConnectionPoint.Get(null);
            Assert.AreEqual(3, dic.Count);
            Assert.AreEqual("Width*0.67", dic[s1][0].Y.Formula);
            Assert.AreEqual("Width*0.67", dic[s2][0].Y.Formula);
            Assert.AreEqual("Width*0.67", dic[s2][0].Y.Formula);

            ss.ConnectionPoint.Delete(null,0);
            ss.Document.Close(true);
        }
    }
}