using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;
using lab1forms.Services.Core;

namespace lab1forms.Services.Lab3
{
    public class Lab3Panel : UserControl
    {
        // ── Tab 2: Фурье ─────────────────────────────────────────────────────
        private double fourierPhase = 0;
        private System.Windows.Forms.Timer fourierTimer;

        // ── Tab 1: Редактор ──────────────────────────────────────────────────
        private PictureBox pbCanvas;
        private PictureBox pbFourier;
        private Bitmap bmpCanvas;
        private Bitmap bmpStrokes;
        private Point? lastPt;
        private Point? startPt;
        private ComboBox cmbShape;
        private Color penColor = Color.Black;
        private Color bgColor = Color.White;
        private int penWidth = 2;
        private ComboBox cmbLineStyle;
        private ComboBox cmbFourierType;
        private TrackBar trkHarmonics;

        // ── Tab 2: Фракталы ──────────────────────────────────────────────────
        private PictureBox pbTree;
        private PictureBox pbSierpinski;
        private NumericUpDown nudTreeDepth;
        private NumericUpDown nudSierpDepth;

        // ── Tab 3: Вариант 7 ─────────────────────────────────────────────────
        private PictureBox pbChart;
        private NumericUpDown nudA1;
        private NumericUpDown nudD;
        private NumericUpDown nudN;
        private TextBox txtInfo;

        public Lab3Panel()
        {
            Dock = DockStyle.Fill;
            BuildUI();
        }

        private void BuildUI()
        {
            var tabs = new TabControl { Dock = DockStyle.Fill };
            tabs.TabPages.Add(BuildEditorTab());
            tabs.TabPages.Add(BuildFourierTab());
            tabs.TabPages.Add(BuildFractalsTab());
            tabs.TabPages.Add(BuildVariantTab());
            Controls.Add(tabs);

            tabs.SelectedIndexChanged += (sender, e) =>
            {
                if (tabs.SelectedIndex == 2)
                {
                    var t1 = DrawTreeAsync();
                    var t2 = DrawSierpinskiAsync();
                }
                else if (tabs.SelectedIndex == 1 && pbFourier != null)
                {
                    pbFourier.Invalidate();
                    fourierTimer.Start();
                }
                if (tabs.SelectedIndex != 1)
                    fourierTimer.Stop();
                else if (tabs.SelectedIndex == 3 && pbChart != null)
                    pbChart.Invalidate();
            };
        }

        // ══════════════════ TAB 1 — ГРАФИЧЕСКИЙ РЕДАКТОР ════════════════════

        private TabPage BuildEditorTab()
        {
            var tab = new TabPage("Графический редактор");

            var bar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 44,
                BackColor = Color.FromArgb(50, 50, 65)
            };

            var btnNew = ToolBtn("Новый", 0);
            var btnOpen = ToolBtn("Открыть", 96);
            var btnSave = ToolBtn("Сохранить", 192);
            var btnPen = ToolBtn("Цвет линии", 288);
            var btnBg = ToolBtn("Цвет фона", 384);

            var lblW = Lbl("Толщина:", 490, 15, Color.White);
            var nudW = new NumericUpDown
            {
                Left = 567,
                Top = 10,
                Width = 52,
                Minimum = 1,
                Maximum = 20,
                Value = 2
            };

            cmbLineStyle = new ComboBox
            {
                Left = 627,
                Top = 10,
                Width = 110,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbLineStyle.Items.AddRange(new object[]
                { "Сплошная", "Штрих", "Точка", "Штрих-точка" });
            cmbLineStyle.SelectedIndex = 0;

            cmbShape = new ComboBox
            {
                Left = 745,
                Top = 10,
                Width = 120,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbShape.Items.AddRange(new object[]
                { "Произвольно", "Линия", "Прямоугольник", "Эллипс", "Треугольник" });
            cmbShape.SelectedIndex = 0;

            btnNew.Click += (sender, e) =>
            {
                int cw = Math.Max(1, pbCanvas.Width);
                int ch = Math.Max(1, pbCanvas.Height);
                bmpStrokes?.Dispose();
                bmpStrokes = new Bitmap(cw, ch, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                bmpCanvas = new Bitmap(cw, ch);
                using (var g = Graphics.FromImage(bmpCanvas))
                    g.Clear(bgColor);
                pbCanvas.Image = bmpCanvas;
            };
            btnOpen.Click += (sender, e) =>
            {
                using (var d = new OpenFileDialog
                { Filter = "Изображения|*.png;*.bmp;*.jpg" })
                {
                    if (d.ShowDialog() == DialogResult.OK)
                    {
                        bmpStrokes?.Dispose();
                        bmpCanvas = new Bitmap(d.FileName);
                        bmpStrokes = new Bitmap(bmpCanvas.Width, bmpCanvas.Height,
                            System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        pbCanvas.Image = bmpCanvas;
                    }
                }
            };
            btnSave.Click += (sender, e) =>
            {
                if (bmpCanvas == null) return;
                using (var d = new SaveFileDialog
                { Filter = "PNG|*.png|BMP|*.bmp" })
                {
                    if (d.ShowDialog() == DialogResult.OK)
                        bmpCanvas.Save(d.FileName);
                }
            };
            btnPen.Click += (sender, e) =>
            {
                using (var d = new ColorDialog())
                    if (d.ShowDialog() == DialogResult.OK) penColor = d.Color;
            };
            btnBg.Click += (sender, e) =>
            {
                using (var d = new ColorDialog())
                    if (d.ShowDialog() == DialogResult.OK)
                    {
                        bgColor = d.Color;
                        RedrawCanvas();
                        pbCanvas.BackColor = bgColor;
                    }
            };
            nudW.ValueChanged += (sender, e) => penWidth = (int)nudW.Value;

            bar.Controls.AddRange(new Control[]
                { btnNew, btnOpen, btnSave, btnPen, btnBg, lblW, nudW, cmbLineStyle, cmbShape });

            pbCanvas = new PictureBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Cursor = Cursors.Cross
            };
            pbCanvas.MouseDown += (sender, e) =>
            {
                lastPt = e.Location;
                startPt = e.Location;
            };
            pbCanvas.MouseMove += (sender, e) =>
            {
                if (e.Button != MouseButtons.Left || !lastPt.HasValue) return;
                int cw = Math.Max(1, pbCanvas.Width);
                int ch = Math.Max(1, pbCanvas.Height);
                if (bmpStrokes == null)
                    bmpStrokes = new Bitmap(cw, ch, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                if (bmpCanvas == null)
                {
                    bmpCanvas = new Bitmap(cw, ch);
                    using (var gg = Graphics.FromImage(bmpCanvas))
                        gg.Clear(bgColor);
                }

                if (cmbShape.SelectedIndex == 0)
                {
                    // Произвольно
                    using (var g = Graphics.FromImage(bmpStrokes))
                    using (var pen = new Pen(penColor, Math.Max(penWidth, 1f)))
                    {
                        switch (cmbLineStyle.SelectedIndex)
                        {
                            case 1: pen.DashPattern = new float[] { 6f, 3f }; break;
                            case 2: pen.DashPattern = new float[] { 1f, 3f }; pen.DashCap = DashCap.Round; break;
                            case 3: pen.DashPattern = new float[] { 6f, 3f, 1f, 3f }; break;
                        }
                        g.DrawLine(pen, lastPt.Value, e.Location);
                    }
                    lastPt = e.Location;
                    RedrawCanvas();
                }
                else
                {
                    // Предпросмотр фигуры
                    var preview = new Bitmap(bmpStrokes.Width, bmpStrokes.Height,
                        System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    using (var pg = Graphics.FromImage(preview))
                    {
                        pg.DrawImage(bmpStrokes, 0, 0);
                        DrawShape(pg, startPt.Value, e.Location);
                    }
                    using (var g = Graphics.FromImage(bmpCanvas))
                    {
                        g.Clear(bgColor);
                        g.DrawImage(preview, 0, 0);
                    }
                    preview.Dispose();
                    pbCanvas.Image = bmpCanvas;
                }
            };
            pbCanvas.MouseUp += (sender, e) =>
            {
                if (cmbShape.SelectedIndex != 0 && startPt.HasValue && bmpStrokes != null)
                {
                    using (var g = Graphics.FromImage(bmpStrokes))
                        DrawShape(g, startPt.Value, e.Location);
                    RedrawCanvas();
                }
                lastPt = null;
                startPt = null;
            };

            tab.Controls.Add(pbCanvas);
            tab.Controls.Add(bar);
            return tab;
        }

        private void DrawShape(Graphics g, Point p1, Point p2)
        {
            using (var pen = new Pen(penColor, Math.Max(penWidth, 1f)))
            {
                switch (cmbLineStyle.SelectedIndex)
                {
                    case 1: pen.DashPattern = new float[] { 6f, 3f }; break;
                    case 2: pen.DashPattern = new float[] { 1f, 3f }; pen.DashCap = DashCap.Round; break;
                    case 3: pen.DashPattern = new float[] { 6f, 3f, 1f, 3f }; break;
                }
                int x = Math.Min(p1.X, p2.X);
                int y = Math.Min(p1.Y, p2.Y);
                int w = Math.Abs(p2.X - p1.X);
                int h = Math.Abs(p2.Y - p1.Y);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                switch (cmbShape.SelectedIndex)
                {
                    case 1: // Линия
                        g.DrawLine(pen, p1, p2);
                        break;
                    case 2: // Прямоугольник
                        if (w > 0 && h > 0) g.DrawRectangle(pen, x, y, w, h);
                        break;
                    case 3: // Эллипс
                        if (w > 0 && h > 0) g.DrawEllipse(pen, x, y, w, h);
                        break;
                    case 4: // Треугольник
                        var tri = new Point[]
                        {
                            new Point(p1.X + (p2.X - p1.X) / 2, p1.Y),
                            new Point(p2.X, p2.Y),
                            new Point(p1.X, p2.Y)
                        };
                        g.DrawPolygon(pen, tri);
                        break;
                }
            }
        }

        private void RedrawCanvas()
        {
            if (bmpStrokes == null) return;
            if (bmpCanvas == null || bmpCanvas.Width != bmpStrokes.Width || bmpCanvas.Height != bmpStrokes.Height)
            {
                bmpCanvas?.Dispose();
                bmpCanvas = new Bitmap(bmpStrokes.Width, bmpStrokes.Height);
            }
            using (var g = Graphics.FromImage(bmpCanvas))
            {
                g.Clear(bgColor);
                g.DrawImage(bmpStrokes, 0, 0);
            }
            pbCanvas.Image = bmpCanvas;
        }

        private void DrawFourierOnG(Graphics g, int w, int h)
        {
            g.Clear(Color.FromArgb(18, 18, 28));
            if (w <= 60 || h <= 20) return;

            int harmonics = trkHarmonics != null ? trkHarmonics.Value : 7;
            int type = cmbFourierType != null ? cmbFourierType.SelectedIndex : 0;

            int pxCount = w - 50;
            var ys = new double[pxCount];
            for (int px = 0; px < pxCount; px++)
            {
                double t = px * 2 * Math.PI / pxCount + fourierPhase;
                double y = 0;
                for (int k = 1; k <= harmonics; k++)
                {
                    switch (type)
                    {
                        case 0: y += 4 / Math.PI * (1.0 / (2 * k - 1)) * Math.Sin((2 * k - 1) * t); break;
                        case 1: y += 8 / (Math.PI * Math.PI) * (Math.Pow(-1, k + 1) / Math.Pow(2 * k - 1, 2)) * Math.Sin((2 * k - 1) * t); break;
                        case 2: y += 2 / Math.PI * (Math.Pow(-1, k + 1) / k) * Math.Sin(k * t); break;
                    }
                }
                ys[px] = y;
            }

            double maxAbs = 0;
            foreach (double y in ys) if (Math.Abs(y) > maxAbs) maxAbs = Math.Abs(y);
            if (maxAbs < 1e-9) maxAbs = 1;

            float margin = 16f;
            float amp = (h / 2f - margin) / (float)maxAbs;

            using (var axisPen = new Pen(Color.FromArgb(80, 80, 80)))
            {
                g.DrawLine(axisPen, 40, h / 2, w - 10, h / 2);
                g.DrawLine(axisPen, 40, 4, 40, h - 4);
            }

            var pts = new PointF[pxCount];
            for (int px = 0; px < pxCount; px++)
                pts[px] = new PointF(40 + px, h / 2f - (float)(ys[px] * amp));

            if (pts.Length > 1)
                using (var curvePen = new Pen(Color.Cyan, 1.8f))
                    g.DrawLines(curvePen, pts);

            string[] labels = { "Прямоугольная", "Треугольная", "Пилообразная" };
            string info = string.Format("{0}  |  гармоник: {1}", labels[type], harmonics);
            g.DrawString(info, new Font("Consolas", 8f), Brushes.LightGray, 44, 3);
        }

        // ══════════════════════ TAB 2 — ФУРЬЕ ══════════════════════════════

        private TabPage BuildFourierTab()
        {
            var tab = new TabPage("Сигнал Фурье");
            tab.BackColor = Color.FromArgb(18, 18, 28);

            var ctrl = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = Color.FromArgb(30, 30, 45)
            };

            ctrl.Controls.Add(Lbl("Сигнал:", 8, 10, Color.White));
            cmbFourierType = new ComboBox
            {
                Left = 70,
                Top = 7,
                Width = 140,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbFourierType.Items.AddRange(new object[]
                { "Прямоугольная", "Треугольная", "Пилообразная" });
            cmbFourierType.SelectedIndex = 0;
            cmbFourierType.SelectedIndexChanged += (sender, e) => { if (pbFourier != null) pbFourier.Invalidate(); };

            ctrl.Controls.Add(Lbl("Гармоники:", 225, 10, Color.White));
            trkHarmonics = new TrackBar
            {
                Left = 315,
                Top = 4,
                Width = 260,
                Minimum = 1,
                Maximum = 25,
                Value = 7,
                TickFrequency = 1
            };
            trkHarmonics.ValueChanged += (sender, e) => { if (pbFourier != null) pbFourier.Invalidate(); };

            ctrl.Controls.AddRange(new Control[] { cmbFourierType, trkHarmonics });

            pbFourier = new PictureBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(18, 18, 28)
            };
            pbFourier.Paint += (sender, e) =>
                DrawFourierOnG(e.Graphics, pbFourier.Width, pbFourier.Height);
            pbFourier.Resize += (sender, e) => pbFourier.Invalidate();

            fourierTimer = new System.Windows.Forms.Timer { Interval = 16 };
            fourierTimer.Tick += (sender, e) =>
            {
                fourierPhase += 0.05;
                if (pbFourier != null) pbFourier.Invalidate();
            };

            tab.Controls.Add(pbFourier);
            tab.Controls.Add(ctrl);
            return tab;
        }

        // ══════════════════════ TAB 3 — ФРАКТАЛЫ ════════════════════════════

        private TabPage BuildFractalsTab()
        {
            var tab = new TabPage("Фракталы");
            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical
            };

            // ── Дерево Пифагора ──────────────────────────────────────────────
            var p1 = new Panel { Dock = DockStyle.Fill };
            var hdr1 = BoldLabel("Дерево Пифагора", DockStyle.Top);
            var ctrl1 = new Panel { Dock = DockStyle.Top, Height = 36 };
            ctrl1.Controls.Add(Lbl("Глубина:", 8, 10, Color.Black));
            nudTreeDepth = new NumericUpDown
            { Left = 70, Top = 6, Width = 60, Minimum = 1, Maximum = 999, Value = 9 };
            nudTreeDepth.ValueChanged += (sender, e) =>
            {
                var t = DrawTreeAsync();
            };
            ctrl1.Controls.Add(nudTreeDepth);

            pbTree = new PictureBox
            { Dock = DockStyle.Fill, BackColor = Color.FromArgb(12, 12, 22) };
            pbTree.Resize += (sender, e) =>
            {
                var t = DrawTreeAsync();
            };

            p1.Controls.Add(pbTree);
            p1.Controls.Add(ctrl1);
            p1.Controls.Add(hdr1);

            // ── Ковёр Серпинского ────────────────────────────────────────────
            var p2 = new Panel { Dock = DockStyle.Fill };
            var hdr2 = BoldLabel("Ковёр Серпинского", DockStyle.Top);
            var ctrl2 = new Panel { Dock = DockStyle.Top, Height = 36 };
            ctrl2.Controls.Add(Lbl("Глубина:", 8, 10, Color.Black));
            nudSierpDepth = new NumericUpDown
            { Left = 70, Top = 6, Width = 60, Minimum = 1, Maximum = 999, Value = 4 };
            nudSierpDepth.ValueChanged += (sender, e) =>
            {
                var t = DrawSierpinskiAsync();
            };
            ctrl2.Controls.Add(nudSierpDepth);

            pbSierpinski = new PictureBox
            { Dock = DockStyle.Fill, BackColor = Color.Black };
            pbSierpinski.Resize += (sender, e) =>
            {
                var t = DrawSierpinskiAsync();
            };

            p2.Controls.Add(pbSierpinski);
            p2.Controls.Add(ctrl2);
            p2.Controls.Add(hdr2);

            split.Panel1.Controls.Add(p1);
            split.Panel2.Controls.Add(p2);
            tab.Controls.Add(split);
            return tab;
        }

        private async Task DrawTreeAsync()
        {
            if (pbTree == null || pbTree.Width <= 0 || pbTree.Height <= 0) return;
            int depth = (int)nudTreeDepth.Value;
            int w = pbTree.Width;
            int h = pbTree.Height;

            ProcessStatus.Start("Рендеринг дерева Пифагора (ЛР №3)");
            try
            {
                var bmp = await Task.Run(() =>
                {
                    var b = new Bitmap(w, h);
                    using (var g = Graphics.FromImage(b))
                    {
                        g.Clear(Color.FromArgb(12, 12, 22));
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        DrawBranch(g, w / 2f, h - 10,
                                   w / 2f, h - 10 - h * 0.18f, depth, depth);
                    }
                    return b;
                });

                if (!IsDisposed && pbTree != null && !pbTree.IsDisposed)
                {
                    var old = pbTree.Image;
                    pbTree.Image = bmp;
                    if (old != null) old.Dispose();
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(ex, "ЛР №3 — дерево Пифагора");
            }
            finally
            {
                ProcessStatus.Stop();
            }
        }

        private static void DrawBranch(Graphics g,
            float x1, float y1, float x2, float y2, int d, int maxD)
        {
            if (d <= 0) return;
            float t = 1f - (float)d / maxD;
            var c = ColorFromHSV(t * 240, 0.85, 0.4 + t * 0.6);
            float w = Math.Max(0.8f, d / 2.5f);
            using (var pen = new Pen(c, w))
                g.DrawLine(pen, x1, y1, x2, y2);

            float dx = x2 - x1, dy = y2 - y1;
            float len = (float)Math.Sqrt(dx * dx + dy * dy) * 0.68f;
            float angle = (float)Math.Atan2(dy, dx);
            float al = angle - (float)Math.PI / 4;
            float ar = angle + (float)Math.PI / 4;

            DrawBranch(g, x2, y2,
                x2 + len * (float)Math.Cos(al), y2 + len * (float)Math.Sin(al), d - 1, maxD);
            DrawBranch(g, x2, y2,
                x2 + len * (float)Math.Cos(ar), y2 + len * (float)Math.Sin(ar), d - 1, maxD);
        }

        private async Task DrawSierpinskiAsync()
        {
            if (pbSierpinski == null ||
                pbSierpinski.Width <= 0 || pbSierpinski.Height <= 0) return;

            int depth = (int)nudSierpDepth.Value;
            int w = pbSierpinski.Width;
            int h = pbSierpinski.Height;
            int sz = Math.Min(w, h) - 20;

            ProcessStatus.Start("Рендеринг ковра Серпинского (ЛР №3)");
            try
            {
                var bmp = await Task.Run(() =>
                {
                    var b = new Bitmap(w, h);
                    using (var g = Graphics.FromImage(b))
                    {
                        g.Clear(Color.Black);
                        int ox = (w - sz) / 2, oy = (h - sz) / 2;
                        using (var br = new SolidBrush(ColorFromHSV(0, 0.7, 0.6)))
                            g.FillRectangle(br, ox, oy, sz, sz);
                        DrawCarpet(g, ox, oy, sz, depth, depth);
                    }
                    return b;
                });

                if (!IsDisposed && pbSierpinski != null && !pbSierpinski.IsDisposed)
                {
                    var old = pbSierpinski.Image;
                    pbSierpinski.Image = bmp;
                    if (old != null) old.Dispose();
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(ex, "ЛР №3 — ковёр Серпинского");
            }
            finally
            {
                ProcessStatus.Stop();
            }
        }

        private static void DrawCarpet(Graphics g,
            int x, int y, int size, int d, int maxD)
        {
            if (d <= 0 || size < 3) return;
            int s = size / 3;
            g.FillRectangle(Brushes.Black, x + s, y + s, s, s);

            float t = 1f - (float)(d - 1) / maxD;
            var c = ColorFromHSV(t * 280, 0.8, 0.7);
            using (var br = new SolidBrush(c))
            {
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                        if (!(i == 1 && j == 1))
                        {
                            g.FillRectangle(br, x + i * s, y + j * s, s, s);
                            DrawCarpet(g, x + i * s, y + j * s, s, d - 1, maxD);
                        }
            }
        }

        // ════════════════ TAB 3 — ВАРИАНТ 7: СУММА АРИФМ. ПРОГРЕССИИ ════════

        private TabPage BuildVariantTab()
        {
            var tab = new TabPage("Сумма арифм. прогрессии");

            var ctrl = new Panel
            {
                Dock = DockStyle.Top,
                Height = 52,
                BackColor = Color.FromArgb(245, 245, 252),
                Padding = new Padding(8, 4, 8, 4)
            };

            ctrl.Controls.Add(Lbl("a1:", 8, 18));
            nudA1 = Nud(38, 13, -1000, 1000, 2, 1);

            ctrl.Controls.Add(Lbl("d:", 150, 18));
            nudD = Nud(170, 13, -500, 500, 3, 1);

            ctrl.Controls.Add(Lbl("n:", 282, 18));
            nudN = Nud(302, 13, 1, 30, 10, 0);

            ctrl.Controls.AddRange(new Control[] { nudA1, nudD, nudN });

            txtInfo = new TextBox
            {
                Left = 414,
                Top = 13,
                Width = 600,
                ReadOnly = true,
                BackColor = Color.FromArgb(235, 235, 245),
                Font = new Font("Consolas", 9f)
            };
            ctrl.Controls.Add(txtInfo);

            nudA1.ValueChanged += (sender, e) => { if (pbChart != null) pbChart.Invalidate(); };
            nudD.ValueChanged += (sender, e) => { if (pbChart != null) pbChart.Invalidate(); };
            nudN.ValueChanged += (sender, e) => { if (pbChart != null) pbChart.Invalidate(); };

            pbChart = new PictureBox { Dock = DockStyle.Fill, BackColor = Color.White };
            pbChart.Paint += (sender, e) =>
                DrawChartOnG(e.Graphics, pbChart.Width, pbChart.Height);
            pbChart.Resize += (sender, e) => pbChart.Invalidate();

            tab.Controls.Add(pbChart);
            tab.Controls.Add(ctrl);
            return tab;
        }

        private static double MemberArithm(double a1, double d, int n)
        {
            if (n == 1) return a1;
            return MemberArithm(a1, d, n - 1) + d;
        }

        private static double SumArithm(double a1, double d, int n)
        {
            if (n == 1) return a1;
            return SumArithm(a1, d, n - 1) + MemberArithm(a1, d, n);
        }

        private void DrawChartOnG(Graphics g, int w, int h)
        {
            g.Clear(Color.White);
            if (w <= 100 || h <= 80 || nudA1 == null) return;

            double a1 = (double)nudA1.Value;
            double d = (double)nudD.Value;
            int n = (int)nudN.Value;

            var members = new double[n];
            var sums = new double[n];
            for (int i = 1; i <= n; i++)
            {
                members[i - 1] = MemberArithm(a1, d, i);
                sums[i - 1] = SumArithm(a1, d, i);
            }

            double formulaSum = n / 2.0 * (2 * a1 + (n - 1) * d);

            if (txtInfo != null)
                txtInfo.Text = string.Format(
                    "Рекурс.: S({0}) = {1:F4}   Формула = {2:F4}   a({0}) = {3:F4}",
                    n, sums[n - 1], formulaSum, members[n - 1]);

            int mL = 72, mR = 24, mT = 42, mB = 52;
            int cW = w - mL - mR;
            int cH = h - mT - mB;

            double yMin = 0, yMax = 0;
            foreach (double v in members) { if (v < yMin) yMin = v; if (v > yMax) yMax = v; }
            foreach (double v in sums) { if (v < yMin) yMin = v; if (v > yMax) yMax = v; }
            double range = yMax - yMin;
            if (range < 1e-9) range = 1;
            yMax += range * 0.08;
            yMin -= range * 0.08;
            range = yMax - yMin;

            using (var fnt = new Font("Segoe UI", 7.5f))
            using (var fntB = new Font("Segoe UI", 8.5f, FontStyle.Bold))
            using (var axPen = new Pen(Color.DimGray, 1.5f))
            {
                for (int i = 0; i <= 6; i++)
                {
                    double val = yMin + range * i / 6.0;
                    float yy = mT + (float)((yMax - val) / range * cH);
                    using (var gridPen = new Pen(Color.FromArgb(220, 220, 230)))
                        g.DrawLine(gridPen, mL, yy, w - mR, yy);
                    g.DrawLine(axPen, mL - 5, yy, mL, yy);
                    string lbl = string.Format("{0:F1}", val);
                    SizeF sz = g.MeasureString(lbl, fnt);
                    g.DrawString(lbl, fnt, Brushes.DimGray,
                        mL - sz.Width - 6, yy - sz.Height / 2);
                }

                if (yMin < 0 && yMax > 0)
                {
                    float y0 = mT + (float)((yMax - 0) / range * cH);
                    using (var zeroPen = new Pen(Color.Gray, 1f)
                    { DashStyle = DashStyle.Dash })
                        g.DrawLine(zeroPen, mL, y0, w - mR, y0);
                }

                g.DrawLine(axPen, mL, mT, mL, mT + cH);
                g.DrawLine(axPen, mL, mT + cH, w - mR, mT + cH);

                for (int i = 0; i < n; i++)
                {
                    float xx = mL + (i + 0.5f) * ((float)cW / n);
                    g.DrawLine(axPen, xx, mT + cH, xx, mT + cH + 4);
                    string ls = string.Format("{0}", i + 1);
                    SizeF lsz = g.MeasureString(ls, fnt);
                    g.DrawString(ls, fnt, Brushes.DimGray,
                        xx - lsz.Width / 2, mT + cH + 6);
                }

                g.DrawString("n", fntB, Brushes.DimGray, w - mR - 14, mT + cH + 7);
                var sfV = new StringFormat
                { FormatFlags = StringFormatFlags.DirectionVertical };
                g.DrawString("Значение", fntB, Brushes.DimGray, 1, mT + 4, sfV);

                g.SmoothingMode = SmoothingMode.AntiAlias;
                float barW = cW / (float)(n + 1) * 0.65f;
                float y0px = mT + (float)((yMax - 0) / range * cH);

                for (int i = 0; i < n; i++)
                {
                    float cx = mL + (i + 0.5f) * ((float)cW / n);
                    float yy = mT + (float)((yMax - members[i]) / range * cH);
                    float top = Math.Min(yy, y0px);
                    float bh = Math.Max(Math.Abs(yy - y0px), 1f);
                    float t = (float)i / Math.Max(n - 1, 1);
                    Color c1 = ColorFromHSV(200 - t * 160, 0.75, 0.9);
                    Color c2 = ColorFromHSV(200 - t * 160, 0.75, 0.45);

                    var rect = new RectangleF(cx - barW / 2, top, barW, bh);
                    using (var br = new LinearGradientBrush(
                        new RectangleF(rect.X, rect.Y - 1, rect.Width, rect.Height + 2),
                        c1, c2, LinearGradientMode.Vertical))
                        g.FillRectangle(br, rect);

                    using (var bp = new Pen(Color.FromArgb(100, 60, 60, 150), 0.6f))
                        g.DrawRectangle(bp, rect.X, rect.Y, rect.Width, rect.Height);

                    string lbl = string.Format("{0:F1}", members[i]);
                    SizeF lsz = g.MeasureString(lbl, fnt);
                    float ly = members[i] >= 0 ? yy - lsz.Height - 1 : yy + 2;
                    g.DrawString(lbl, fnt, Brushes.Black, cx - lsz.Width / 2, ly);
                }

                var sumPts = new PointF[n];
                for (int i = 0; i < n; i++)
                    sumPts[i] = new PointF(
                        mL + (i + 0.5f) * ((float)cW / n),
                        mT + (float)((yMax - sums[i]) / range * cH));

                using (var sumPen = new Pen(Color.OrangeRed, 2.5f))
                    if (n > 1) g.DrawLines(sumPen, sumPts);

                foreach (var pt in sumPts)
                {
                    g.FillEllipse(Brushes.OrangeRed, pt.X - 4.5f, pt.Y - 4.5f, 9, 9);
                    g.DrawEllipse(Pens.White, pt.X - 4f, pt.Y - 4f, 8, 8);
                }

                using (var fntTitle = new Font("Segoe UI", 8.5f, FontStyle.Bold))
                    g.DrawString(
                        string.Format("a1 = {0}   d = {1}   n = {2}", a1, d, n),
                        fntTitle, Brushes.DarkSlateBlue, mL, 6);

                int lx = mL + 8, ly2 = mT + 8;
                g.FillRectangle(new SolidBrush(ColorFromHSV(140, 0.75, 0.7)),
                    lx, ly2, 14, 14);
                g.DrawString("an = MemberArithm(a1,d,n) — рекурсия",
                    fnt, Brushes.Black, lx + 18, ly2);
                g.FillEllipse(Brushes.OrangeRed, lx + 2, ly2 + 20, 10, 10);
                g.DrawString("Sn = SumArithm(a1,d,n) — рекурсия",
                    fnt, Brushes.OrangeRed, lx + 18, ly2 + 17);
            }
        }

        // ═══════════════════════ вспомогательные ═════════════════════════════

        private static Color ColorFromHSV(double h, double s, double v)
        {
            h = ((h % 360) + 360) % 360;
            int hi = (int)(h / 60) % 6;
            double f = h / 60 - Math.Floor(h / 60);
            int V = (int)(v * 255);
            int p = (int)(v * (1 - s) * 255);
            int q = (int)(v * (1 - f * s) * 255);
            int t2 = (int)(v * (1 - (1 - f) * s) * 255);
            switch (hi)
            {
                case 0: return Color.FromArgb(V, t2, p);
                case 1: return Color.FromArgb(q, V, p);
                case 2: return Color.FromArgb(p, V, t2);
                case 3: return Color.FromArgb(p, q, V);
                case 4: return Color.FromArgb(t2, p, V);
                default: return Color.FromArgb(V, p, q);
            }
        }

        private static Button ToolBtn(string text, int left) => new Button
        {
            Text = text,
            Left = left,
            Top = 8,
            Width = 90,
            Height = 28,
            FlatStyle = FlatStyle.Flat,
            ForeColor = Color.White,
            BackColor = Color.FromArgb(80, 80, 100)
        };

        private static Label Lbl(string text, int x, int y, Color? color = null)
        {
            return new Label
            {
                Text = text,
                Left = x,
                Top = y,
                AutoSize = true,
                ForeColor = color.HasValue ? color.Value : Color.Black
            };
        }

        private static Label BoldLabel(string text, DockStyle dock) => new Label
        {
            Text = text,
            Dock = dock,
            Height = 22,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            ForeColor = Color.DarkBlue
        };

        private static NumericUpDown Nud(int x, int y,
            decimal min, decimal max, decimal val, int dec) => new NumericUpDown
            {
                Left = x,
                Top = y,
                Width = 80,
                Minimum = min,
                Maximum = max,
                Value = val,
                DecimalPlaces = dec
            };
    }
}