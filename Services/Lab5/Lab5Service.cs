using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using lab1forms.Services.Core;
using LogParserLib.Parsers;

namespace lab1forms.Services.Lab5
{
    public class Lab5Panel : UserControl
    {
        private readonly DataGridViewParser _parser = new DataGridViewParser();

        private ListBox       lstFiles;
        private Button        btnAddFiles;
        private Button        btnRunAll;
        private Button        btnClear;
        private NumericUpDown nudSkipCount;
        private RichTextBox   richLog;
        private DataGridView  dgvResults;
        private Label         lblTotal;

        private bool _isHighlighting;

        private static readonly Color HighlightColor = Color.Yellow;
        private static readonly Color SkippedColor   = Color.LightGray;
        private static readonly Color DefaultBack    = Color.FromArgb(25, 25, 35);
        private static readonly Color DefaultFore    = Color.LightGray;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        private const int WM_SETREDRAW = 0x000B;
        private void BeginRedraw() => SendMessage(richLog.Handle, WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);
        private void EndRedraw()   { SendMessage(richLog.Handle, WM_SETREDRAW, new IntPtr(1), IntPtr.Zero); richLog.Invalidate(); }

        public Lab5Panel()
        {
            Dock = DockStyle.Fill;
            BuildUI();
        }

        private void BuildUI()
        {
            // ── Топ-панель
            var topPanel = new Panel
            {
                Dock = DockStyle.Top, Height = 56,
                BackColor = Color.FromArgb(45, 45, 60),
                Padding = new Padding(8, 6, 8, 4)
            };
            btnAddFiles = new Button { Text = "Добавить лог-файлы...", Left = 8,   Top = 12, Width = 180, Height = 34, FlatStyle = FlatStyle.Flat, ForeColor = Color.White, BackColor = Color.FromArgb(60, 100, 160) };
            var lblI    = new Label  { Text = "Пропустить i:", ForeColor = Color.White, Left = 200, Top = 18, AutoSize = true };
            nudSkipCount = new NumericUpDown { Left = 300, Top = 14, Width = 80, Minimum = 0, Maximum = 99999, Value = 0, Font = new Font("Segoe UI", 10f) };
            btnRunAll   = new Button { Text = "Запустить все (async)",  Left = 395, Top = 12, Width = 180, Height = 34, FlatStyle = FlatStyle.Flat, ForeColor = Color.White, BackColor = Color.FromArgb(60, 150, 80) };
            btnClear    = new Button { Text = "Очистить",               Left = 585, Top = 12, Width = 110, Height = 34, FlatStyle = FlatStyle.Flat, ForeColor = Color.White, BackColor = Color.FromArgb(160, 60, 60) };
            topPanel.Controls.AddRange(new Control[] { btnAddFiles, lblI, nudSkipCount, btnRunAll, btnClear });

            // ── Статус-бар
            var statusPanel = new Panel { Dock = DockStyle.Bottom, Height = 30, BackColor = Color.FromArgb(35, 35, 50) };
            lblTotal = new Label { Text = "Общее время всех задач: —", ForeColor = Color.LightGreen, Left = 8, Top = 6, AutoSize = true };
            statusPanel.Controls.Add(lblTotal);

            // ── Левая панель (фикс. ширина 320) — БЕЗ SplitContainer
            var leftPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 320,
                BackColor = Color.FromArgb(30, 30, 45)
            };

            // Вертикальный сплиттер между lstFiles и dgvResults
            var hSplitter = new Splitter
            {
                Dock = DockStyle.Top,
                Height = 5,
                BackColor = Color.FromArgb(60, 60, 90)
            };

            // ── Список файлов (верхняя половина левой панели)
            lstFiles = new ListBox
            {
                Dock = DockStyle.Top,
                Height = 150,
                BackColor = Color.FromArgb(30, 30, 45),
                ForeColor = Color.LightCyan,
                Font = new Font("Consolas", 9f)
            };

            // ── Таблица результатов (нижняя половина левой панели)
            dgvResults = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.FromArgb(30, 30, 45),
                ForeColor = Color.White,
                GridColor = Color.FromArgb(60, 60, 80),
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Font = new Font("Segoe UI", 9f)
            };
            dgvResults.DefaultCellStyle.BackColor                = Color.FromArgb(30, 30, 45);
            dgvResults.DefaultCellStyle.ForeColor                = Color.White;
            dgvResults.ColumnHeadersDefaultCellStyle.BackColor  = Color.FromArgb(45, 45, 60);
            dgvResults.ColumnHeadersDefaultCellStyle.ForeColor  = Color.White;
            dgvResults.EnableHeadersVisualStyles                 = false;
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "File",        HeaderText = "Файл" });
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "Total",       HeaderText = "Всего" });
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "Highlighted", HeaderText = "Выделено" });
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "TimeMs",      HeaderText = "Время async (мс)" });
            dgvResults.SelectionChanged += async (s, e) => await ShowSelectedFileAsync();

            // порядок добавления в leftPanel важен: Fill-элемент должен быть добавлен первым
            leftPanel.Controls.Add(dgvResults);   // Fill
            leftPanel.Controls.Add(hSplitter);    // Top (splitter между)
            leftPanel.Controls.Add(lstFiles);     // Top

            // ── Гориз. сплиттер между левой панелью и richLog
            var vSplitter = new Splitter
            {
                Dock = DockStyle.Left,
                Width = 5,
                BackColor = Color.FromArgb(60, 60, 90)
            };

            // ── RichTextBox
            richLog = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 10f),
                BackColor = DefaultBack,
                ForeColor = DefaultFore,
                ScrollBars = RichTextBoxScrollBars.Both,
                WordWrap = false,
                ReadOnly = false
            };
            richLog.TextChanged += async (s, e) =>
            {
                if (!_isHighlighting) await HighlightCurrentAsync();
            };

            // Порядок добавления в UserControl: Fill -> Left-splitter -> Left -> Top -> Bottom
            Controls.Add(richLog);       // Fill
            Controls.Add(vSplitter);     // Left
            Controls.Add(leftPanel);     // Left
            Controls.Add(topPanel);      // Top
            Controls.Add(statusPanel);   // Bottom

            btnAddFiles.Click += OnAddFilesClick;
            btnRunAll.Click   += async (s, e) => await RunAllAsync();
            btnClear.Click    += OnClearClick;
        }

        private void OnAddFilesClick(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog
            {
                Filter = "Лог-файлы|*.log;*.txt|Все файлы|*.*",
                Title = "Выберите лог-журналы",
                Multiselect = true
            })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;
                foreach (var f in dlg.FileNames)
                    if (!lstFiles.Items.Contains(f))
                        lstFiles.Items.Add(f);
            }
        }

        private void OnClearClick(object sender, EventArgs e)
        {
            lstFiles.Items.Clear();
            dgvResults.Rows.Clear();
            _isHighlighting = true;
            richLog.Clear();
            _isHighlighting = false;
            lblTotal.Text = "Общее время всех задач: —";
        }

        private async Task RunAllAsync()
        {
            if (lstFiles.Items.Count == 0)
            {
                MessageBox.Show("Добавьте лог-файлы!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ProcessStatus.Start("Обработка всех лог-файлов (ЛР №5)");
            try
            {
                int skipCount = (int)nudSkipCount.Value;
                dgvResults.Rows.Clear();

                var files = lstFiles.Items.Cast<string>().ToArray();

                var readTasks = files.Select(f =>
                    Task.Run(() => System.IO.File.ReadAllText(f, System.Text.Encoding.UTF8))
                ).ToArray();
                string[] texts = await Task.WhenAll(readTasks);

                var parseTasks = texts.Select(t => _parser.ParseAsync(t, skipCount)).ToArray();
                ParseResult[] results = await Task.WhenAll(parseTasks);

                double totalMs = 0;
                for (int i = 0; i < files.Length; i++)
                {
                    var r  = results[i];
                    var ms = r.Elapsed.TotalMilliseconds;
                    totalMs += ms;
                    dgvResults.Rows.Add(
                        System.IO.Path.GetFileName(files[i]),
                        r.TotalFound,
                        r.Highlighted,
                        ms.ToString("F2")
                    );
                }

                lblTotal.Text = $"Общее время всех задач: {totalMs:F2} мс | файлов: {files.Length}";

                if (dgvResults.Rows.Count > 0)
                    dgvResults.Rows[0].Selected = true;
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(ex, "ЛР №5 — обработка всех файлов");
            }
            finally
            {
                ProcessStatus.Stop();
            }
        }

        private async Task ShowSelectedFileAsync()
        {
            if (dgvResults.SelectedRows.Count == 0) return;
            int rowIdx = dgvResults.SelectedRows[0].Index;
            if (rowIdx < 0 || rowIdx >= lstFiles.Items.Count) return;

            string path = lstFiles.Items[rowIdx].ToString();

            ProcessStatus.Start("Загрузка лог-файла (ЛР №5)");
            try
            {
                string text = await Task.Run(() =>
                    System.IO.File.ReadAllText(path, System.Text.Encoding.UTF8));

                _isHighlighting = true;
                richLog.Text    = text;
                _isHighlighting = false;
                await HighlightCurrentAsync();
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(ex, "ЛР №5 — загрузка лог-файла");
            }
            finally
            {
                ProcessStatus.Stop();
            }
        }

        private async Task HighlightCurrentAsync()
        {
            if (richLog == null || string.IsNullOrEmpty(richLog.Text)) return;
            if (_isHighlighting) return;
            _isHighlighting = true;
            ProcessStatus.Start("Парсинг лога (ЛР №5, async)");
            try
            {
                int    skipCount = (int)nudSkipCount.Value;
                string fullText  = richLog.Text;

                ParseResult result = await _parser.ParseAsync(fullText, skipCount);
                if (result.TotalFound == 0) return;

                richLog.SuspendLayout();
                BeginRedraw();
                richLog.SelectAll();
                richLog.SelectionBackColor = DefaultBack;
                richLog.SelectionColor     = DefaultFore;
                richLog.DeselectAll();

                for (int i = 0; i < result.Matches.Count; i++)
                {
                    var (idx, len) = result.Matches[i];
                    richLog.Select(idx, len);
                    if (i < result.SkipCount)
                    {
                        richLog.SelectionBackColor = SkippedColor;
                        richLog.SelectionColor     = Color.Gray;
                    }
                    else
                    {
                        richLog.SelectionBackColor = HighlightColor;
                        richLog.SelectionColor     = Color.DarkRed;
                    }
                }

                richLog.DeselectAll();
                EndRedraw();
                richLog.ResumeLayout();
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(ex, "ЛР №5 — парсинг лога");
            }
            finally
            {
                _isHighlighting = false;
                ProcessStatus.Stop();
            }
        }
    }
}
