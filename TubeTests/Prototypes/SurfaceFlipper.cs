using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Glue.Prototypes
{
    public class TestForm : Form
    {
        public Timer RedrawTimer { get => redrawTimer; set => redrawTimer = value; }

        private Timer redrawTimer;
        private int tickCount = 0;

        public TestForm()
        {
            RedrawTimer = new System.Windows.Forms.Timer
            {
                Interval = 1,
            };
            RedrawTimer.Tick += new System.EventHandler(OnTick);
            RedrawTimer.Start();
        }

        protected void OnTick(object sender, EventArgs e)
        {
            tickCount++;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.DrawString("Count: " + tickCount, Font, Brushes.Black, 50, 75);            

            base.OnPaint(e);
        }
    }

    [TestClass]
    public class SurfaceFlipper
    {
        // Uncomment to run surface prototype test  
        // DO NOT SUBMIT UNCOMMENTED
        // [TestMethod]
        public void TestForm()
        {
            Application.Run(new TestForm());
        }
    }
}
