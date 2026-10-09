using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using lab1forms.Services.Core;
using lab1forms.Services.Lab2;

namespace lab1forms.Services.Lab2
{
    public class Lab2Panel : UserControl
    {
        private double[] _D;
        private double[] _A;

        private DataGridView  dgvD;
        private DataGridView  dgvA;
        private Label         lblD;
        private Label         lblA;
        private NumericUpDown nudSize;
        private NumericUpDown nudFreq;
        private NumericUpDown nudShowN;
        private Label        lblTimeNoThread;
        private Label        lblTimeThread;

        public Lab2Panel()
        {
            Dock = DockStyle.Fill;
            BuildUI();
        }

        private void BuildUI()
        {
            // ── панель заполнения ────────────────────────────────────────────
            var grpFill = new GroupBox
            {
                Text    = "Шаг 1 — Заполнение массива D",
                Dock    = DockStyle.Top,
                Height  = 105,
                Padding = new Padding(10, 4, 10, 4)
            };

            var lblSize = new Label { Text = "Размер n:", Left = 8,   Top = 28, AutoSize = true };
            nudSize = new NumericUpDown { Left = 80, Top = 24, Width = 75,
                                          Minimum = 2, Maximum = 100000, Value = 10 };

            var lblFreq = new Label { Text = "Частота k:", Left = 168, Top = 28, AutoSize = true };
            nudFreq = new NumericUpDown { Left = 246, Top = 24, Width = 60,
                                          Minimum = 1, Maximum = 50, Value = 5 };

            var lblShowN = new Label { Text = "Показать первые N:", Left = 322, Top = 28, AutoSize = true };
            nudShowN = new NumericUpDown { Left = 462, Top = 24, Width = 75,
                                           Minimum = 1, Maximum = 100000, Value = 50 };

            var btnRandom     = MakeBtn("Случайно",          8,   60, 128);
            var btnFreqFill   = MakeBtn("По частоте k",      144, 60, 128);
            var btnReadManual = MakeBtn("✔ Принять вручную", 280, 60, 160);

            btnRandom.Click     += BtnRandom_Click;
            btnFreqFill.Click   += BtnFreqFill_Click;
            btnReadManual.Click += BtnReadManual_Click;

            grpFill.Controls.AddRange(new Control[]
                { lblSize, nudSize, lblFreq, nudFreq, lblShowN, nudShowN,
                  btnRandom, btnFreqFill, btnReadManual });

            // ── панель обработки ─────────────────────────────────────────────
            var grpProcess = new GroupBox
            {
                Text    = "Шаг 2 — Обработка",
                Dock    = DockStyle.Bottom,
                Height  = 82,
                Padding = new Padding(10, 4, 10, 4)
            };

            var btnNoThread = MakeBtn("▶ Без потоков",  8,   22, 138, Color.FromArgb(0, 90, 180));
            var btnThread   = MakeBtn("▶▶ С потоками", 155, 22, 138, Color.FromArgb(0, 130, 80));

            lblTimeNoThread = new Label { Text = "Без потоков: —",
                Left = 8,   Top = 58, AutoSize = true, ForeColor = Color.DarkBlue };
            lblTimeThread = new Label { Text = "С потоками:  —",
                Left = 320, Top = 58, AutoSize = true, ForeColor = Color.DarkGreen };

            btnNoThread.Click += BtnNoThread_Click;
            btnThread.Click   += BtnThread_Click;

            grpProcess.Controls.AddRange(new Control[]
                { btnNoThread, btnThread, lblTimeNoThread, lblTimeThread });

            // ── два DataGridView рядом ───────────────────────────────────────
            dgvD = MakeGrid("D[i]", editable: true);
            dgvA = MakeGrid("A[i]", editable: false);

            lblD = MakeGridHeader("Массив D (исходный):");
            lblA = MakeGridHeader("Массив A (результат):");

            var split = new SplitContainer
            {
                Dock             = DockStyle.Fill,
                Orientation      = Orientation.Vertical,
                SplitterDistance = 460
            };
            split.Panel1.Controls.Add(dgvD);
            split.Panel1.Controls.Add(lblD);
            split.Panel2.Controls.Add(dgvA);
            split.Panel2.Controls.Add(lblA);

            var pnlCenter = new Panel { Dock = DockStyle.Fill };
            pnlCenter.Controls.Add(split);
            pnlCenter.Controls.Add(grpFill);

            Controls.Add(pnlCenter);
            Controls.Add(grpProcess);
        }

        // ─────────────────────── фабрики ─────────────────────────────────────
        private static Button MakeBtn(string text, int x, int y, int w, Color? bg = null)
        {
            var btn = new Button
            {
                Text      = text,
                Left      = x,
                Top       = y,
                Width     = w,
                Height    = 28,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White
            };
            btn.BackColor = bg.HasValue ? bg.Value : Color.FromArgb(70, 70, 90);
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private static DataGridView MakeGrid(string header, bool editable)
        {
            var dgv = new DataGridView
            {
                Dock                  = DockStyle.Fill,
                ColumnCount           = 1,
                RowHeadersWidth       = 46,
                AllowUserToAddRows    = editable,
                AllowUserToDeleteRows = editable,
                ReadOnly              = !editable,
                AutoSizeColumnsMode   = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode         = DataGridViewSelectionMode.CellSelect,
                BorderStyle           = BorderStyle.None
            };
            dgv.Columns[0].HeaderText = header;
            dgv.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            return dgv;
        }

        private static Label MakeGridHeader(string text)
        {
            return new Label
            {
                Text      = text,
                Dock      = DockStyle.Top,
                Height    = 22,
                TextAlign = ContentAlignment.MiddleLeft,
                Font      = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 100)
            };
        }

        // ═══════════════════════ обработчики кнопок ══════════════════════════

        private void BtnRandom_Click(object sender, EventArgs e)
        {
            try
            {
                int n = (int)nudSize.Value;
                var rnd = new Random();
                _D = new double[n];
                for (int i = 0; i < n; i++)
                    _D[i] = rnd.Next(-50, 51);
                FillGrid(dgvD, lblD, "Массив D (исходный):", _D);
            }
            catch (Exception ex) { LogException(ex); }
        }

        private void BtnFreqFill_Click(object sender, EventArgs e)
        {
            try
            {
                int n    = (int)nudSize.Value;
                int freq = (int)nudFreq.Value;
                var rnd  = new Random();
                _D = new double[n];
                for (int i = 0; i < n; i++)
                    _D[i] = rnd.Next(-50 / freq, 51 / freq) * (double)freq;
                FillGrid(dgvD, lblD, "Массив D (исходный):", _D);
            }
            catch (Exception ex) { LogException(ex); }
        }

        private void BtnReadManual_Click(object sender, EventArgs e)
        {
            try
            {
                _D = ReadGrid(dgvD);
                MessageBox.Show(string.Format("Принято {0} элементов.", _D.Length), "Готово",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { LogException(ex); }
        }

        private void BtnNoThread_Click(object sender, EventArgs e)
        {
            ProcessStatus.Start("Обработка массива D→A (без потоков)");
            try
            {
                RequireD();
                var sw = Stopwatch.StartNew();
                _A = Lab2Service.ProcessSequential(_D);
                sw.Stop();
                lblTimeNoThread.Text = string.Format(
                    "Без потоков: {0} тик.  /  {1:F4} мс",
                    sw.ElapsedTicks, sw.Elapsed.TotalMilliseconds);
                FillGrid(dgvA, lblA, "Массив A (результат):", _A);
            }
            catch (Exception ex) { LogException(ex); }
            finally { ProcessStatus.Stop(); }
        }

        private void BtnThread_Click(object sender, EventArgs e)
        {
            ProcessStatus.Start("Обработка массива D→A (с потоками)");
            try
            {
                RequireD();
                var sw = Stopwatch.StartNew();
                _A = Lab2Service.ProcessThreaded(_D);
                sw.Stop();
                lblTimeThread.Text = string.Format(
                    "С потоками:  {0} тик.  /  {1:F4} мс",
                    sw.ElapsedTicks, sw.Elapsed.TotalMilliseconds);
                FillGrid(dgvA, lblA, "Массив A (результат):", _A);
            }
            catch (Exception ex) { LogException(ex); }
            finally { ProcessStatus.Stop(); }
        }

        // ═══════════════════════ вспомогательные ═════════════════════════════

        private void RequireD()
        {
            if (_D == null || _D.Length == 0)
                throw new InvalidOperationException(
                    "Массив D не задан. Сначала заполните его одним из трёх способов.");
        }

        /// <summary>
        /// Заполняет таблицу элементами массива. Для длинных массивов
        /// отображаются только первые N элементов, где N задаёт пользователь
        /// («Показать первые N»); в заголовке таблицы это отмечается.
        /// </summary>
        private void FillGrid(DataGridView dgv, Label header, string baseText, double[] arr)
        {
            int show = Math.Min(arr.Length, (int)nudShowN.Value);

            dgv.Rows.Clear();
            for (int i = 0; i < show; i++)
            {
                dgv.Rows.Add(arr[i].ToString("F4", CultureInfo.InvariantCulture));
                dgv.Rows[i].HeaderCell.Value = (i + 1).ToString();
            }

            header.Text = arr.Length > show
                ? string.Format("{0} — показаны первые {1} из {2}", baseText, show, arr.Length)
                : baseText;
        }

        private static double[] ReadGrid(DataGridView dgv)
        {
            int n = dgv.Rows.Count - 1;
            if (n <= 0)
                throw new InvalidOperationException(
                    "Таблица D пуста. Введите значения вручную и нажмите «Принять».");

            var arr = new double[n];
            for (int i = 0; i < n; i++)
            {
                string raw = dgv.Rows[i].Cells[0].Value != null
                    ? dgv.Rows[i].Cells[0].Value.ToString()
                    : string.Empty;
                if (!double.TryParse(raw, NumberStyles.Any,
                                     CultureInfo.InvariantCulture, out arr[i]))
                    throw new FormatException(
                        string.Format("Строка {0}: «{1}» — некорректное число.", i + 1, raw));
            }
            return arr;
        }

        /// <summary>Передаёт исключение в центральный журнал проекта.</summary>
        private void LogException(Exception ex) => ExceptionLogger.Log(ex, "ЛР №2");
    }
}
