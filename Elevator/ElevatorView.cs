using System.Drawing.Drawing2D;
using System.Reflection;

namespace Elevator
{
    // Draws one elevator. As an observer, it redraws whenever the ElevatorUnit changes.
    public class ElevatorView : IElevatorObserver
    {
        public ElevatorUnit Unit { get; }
        private readonly Control shaft;
        private readonly PictureBox car;
        private readonly Label display;
        private readonly Button btnFloor1, btnFloor2;
        private readonly Func<int, int> floorToY;

        private Panel doorL, doorR;                              // car doors
        private readonly Panel[] hallDoorL = new Panel[3];       // shaft doors, indexed by floor (1, 2)
        private readonly Panel[] hallDoorR = new Panel[3];

        // last drawn values, so we only repaint on change
        private string lastIndicator = "";
        private string lastStatus = "";

        public ElevatorView(ElevatorUnit unit, Control shaft, PictureBox car,
                            Label display, Button btnFloor1, Button btnFloor2)
        {
            Unit = unit;
            this.shaft = shaft;
            this.car = car;
            this.display = display;
            this.btnFloor1 = btnFloor1;
            this.btnFloor2 = btnFloor2;
            floorToY = unit.FloorToY;

            // DoubleBuffered is protected, so set it by reflection to stop the shaft flickering
            typeof(Control).GetProperty("DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(shaft, true);

            car.Paint += Car_Paint;
            shaft.Paint += Shaft_Paint;
            BuildDoors();

            car.Top = unit.Y;
            unit.Attach(this);
            Update(unit, null);
        }

        public void Update(ElevatorUnit e, string eventText)
        {
            if (car.Top != e.Y) { car.Top = e.Y; shaft.Invalidate(); }   // cable follows the car
            ApplyDoors();

            string indicator = e.DirectionArrow + e.CurrentFloor;
            if (indicator != lastIndicator)
            {
                lastIndicator = indicator;
                display.Text = indicator;
                car.Invalidate();                                         // inside floor indicator
            }
            if (e.StatusText != lastStatus) { lastStatus = e.StatusText; shaft.Invalidate(); }

            btnFloor1.BackColor = e.IsPending(1) ? Color.Orange : Color.Silver;
            btnFloor2.BackColor = e.IsPending(2) ? Color.Orange : Color.Silver;
        }

        private Panel MakeDoor(Control parent, Color color)
        {
            var p = new Panel { BackColor = color, BorderStyle = BorderStyle.FixedSingle };
            p.Paint += Door_Paint;
            p.Resize += (s, ev) => p.Invalidate();
            parent.Controls.Add(p);
            return p;
        }

        private void BuildDoors()
        {
            doorL = MakeDoor(car, Color.Silver);
            doorR = MakeDoor(car, Color.Silver);

            for (int f = 1; f <= 2; f++)
            {
                hallDoorL[f] = MakeDoor(shaft, Color.SlateGray);
                hallDoorR[f] = MakeDoor(shaft, Color.SlateGray);
                hallDoorL[f].BringToFront();
                hallDoorR[f].BringToFront();
            }
            ApplyDoors();
        }

        private static void SetLeaves(Panel left, Panel right, int x, int y, int totalW, int h, int leafW)
        {
            left.SetBounds(x, y, leafW, h);
            right.SetBounds(x + totalW - leafW, y, leafW, h);
            left.Visible = right.Visible = leafW > 0;
        }

        private void ApplyDoors()
        {
            int half = (car.Width + 1) / 2;
            int leaf = half - Unit.DoorOpen;

            SetLeaves(doorL, doorR, 0, 0, car.Width, car.Height, leaf);

            for (int f = 1; f <= 2; f++)
            {
                int w = (f == Unit.CurrentFloor) ? leaf : half;     // only the floor the car is at opens
                SetLeaves(hallDoorL[f], hallDoorR[f], car.Left, floorToY(f), car.Width, car.Height, w);
            }
        }

        private static void Door_Paint(object sender, PaintEventArgs e)
        {
            var p = (Panel)sender;
            if (p.ClientSize.Width <= 0 || p.ClientSize.Height <= 0) return;   // gradient brush throws on an empty rectangle
            using (var b = new LinearGradientBrush(p.ClientRectangle,
                   ControlPaint.Light(p.BackColor), ControlPaint.Dark(p.BackColor), LinearGradientMode.Horizontal))
                e.Graphics.FillRectangle(b, p.ClientRectangle);
        }

        private void Car_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int w = car.ClientSize.Width, h = car.ClientSize.Height;
            int ceilH = Math.Max(8, h / 12);
            int floorH = h / 6;
            int side = w / 10;

            using (var wall = new LinearGradientBrush(new Rectangle(0, 0, w, h),
                   Color.FromArgb(238, 228, 208), Color.FromArgb(185, 168, 140), LinearGradientMode.Vertical))
                g.FillRectangle(wall, 0, 0, w, h);

            using (var left = new LinearGradientBrush(new Rectangle(0, 0, side, h),
                   Color.FromArgb(130, 0, 0, 0), Color.FromArgb(0, 0, 0, 0), LinearGradientMode.Horizontal))
                g.FillRectangle(left, 0, 0, side, h);
            using (var right = new LinearGradientBrush(new Rectangle(w - side, 0, side, h),
                   Color.FromArgb(0, 0, 0, 0), Color.FromArgb(130, 0, 0, 0), LinearGradientMode.Horizontal))
                g.FillRectangle(right, w - side, 0, side, h);

            g.FillRectangle(Brushes.DimGray, 0, 0, w, ceilH);
            using (var light = new SolidBrush(Color.FromArgb(255, 255, 240)))
                g.FillRectangle(light, w / 4, ceilH / 4, w / 2, Math.Max(2, ceilH / 2));

            // floor indicator
            var disp = new Rectangle(w / 2 - 16, ceilH + 3, 32, 16);
            g.FillRectangle(Brushes.Black, disp);
            g.DrawRectangle(Pens.Silver, disp);
            using (var f = new Font("Segoe UI", 8, FontStyle.Bold))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                g.DrawString(Unit.DirectionArrow + Unit.CurrentFloor, f, Brushes.LimeGreen, disp, sf);

            // mirror
            var mirror = new Rectangle(w / 4, disp.Bottom + 6, w / 2, (int)(h * 0.30));
            using (var mb = new LinearGradientBrush(mirror, Color.FromArgb(225, 238, 245),
                   Color.FromArgb(140, 165, 180), LinearGradientMode.ForwardDiagonal))
                g.FillRectangle(mb, mirror);
            g.DrawRectangle(Pens.Silver, mirror);

            // handrail
            int railY = (int)(h * 0.66);
            using (var rail = new Pen(Color.Silver, 4))
                g.DrawLine(rail, side + 2, railY, w - side - 2, railY);
            using (var shadow = new Pen(Color.FromArgb(90, 0, 0, 0), 2))
                g.DrawLine(shadow, side + 2, railY + 3, w - side - 2, railY + 3);

            // tiled floor
            var floorRect = new Rectangle(0, h - floorH, w, floorH);
            using (var fb = new LinearGradientBrush(floorRect, Color.FromArgb(90, 90, 95),
                   Color.FromArgb(45, 45, 50), LinearGradientMode.Vertical))
                g.FillRectangle(fb, floorRect);
            using (var tile = new Pen(Color.FromArgb(120, 0, 0, 0)))
            {
                for (int x = 0; x < w; x += 18) g.DrawLine(tile, x, h - floorH, x, h);
                g.DrawLine(tile, 0, h - floorH / 2, w, h - floorH / 2);
            }
        }

        private void Shaft_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int w = shaft.ClientSize.Width, h = shaft.ClientSize.Height;

            using (var bg = new LinearGradientBrush(new Rectangle(0, 0, w, h),
                   Color.FromArgb(75, 79, 88), Color.FromArgb(38, 41, 48), LinearGradientMode.Horizontal))
                g.FillRectangle(bg, 0, 0, w, h);

            // guide rails
            using (var rail = new Pen(Color.FromArgb(150, 150, 155), 3))
            {
                g.DrawLine(rail, car.Left - 12, 0, car.Left - 12, h);
                g.DrawLine(rail, car.Right + 11, 0, car.Right + 11, h);
            }

            // cable
            using (var cable = new Pen(Color.Black, 2))
                g.DrawLine(cable, car.Left + car.Width / 2, 0, car.Left + car.Width / 2, car.Top);

            // door frames and floor number plates
            for (int f = 1; f <= 2; f++)
            {
                int y = floorToY(f);
                var frame = new Rectangle(car.Left - 8, y - 8, car.Width + 16, car.Height + 12);
                using (var fb = new SolidBrush(Color.FromArgb(185, 185, 190)))
                    g.FillRectangle(fb, frame);

                var plate = new Rectangle(car.Left + car.Width / 2 - 12, y - 24, 24, 14);
                using (var pb = new SolidBrush(Color.FromArgb(200, 170, 90)))
                    g.FillRectangle(pb, plate);
                using (var font = new Font("Segoe UI", 8, FontStyle.Bold))
                using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    g.DrawString(f.ToString(), font, Brushes.Black, plate, sf);
            }

            // elevator name and current status
            using (var font = new Font("Segoe UI", 8, FontStyle.Bold))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                g.DrawString($"ELEVATOR {Unit.Name}  -  {Unit.StatusText}", font, Brushes.Gainsboro,
                             new Rectangle(0, h - 20, w, 18), sf);
        }
    }
}