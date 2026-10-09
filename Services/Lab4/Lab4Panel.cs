using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using lab1forms.Services.Core;
using LogParserLib.Parsers;

namespace lab1forms.Services.Lab4
{
    /// <summary>
    /// Лабораторная работа №4, Вариант 11.
    /// UI-оболочка над DataGridViewParser из LogParserLib.dll.
    /// Парсинг: dataGridView* (без учёта регистра),
    /// вхождения до i-го — серые, после i-го — жёлтые.
    /// </summary>
    public class Lab4Panel : UserControl
    {
        // ─ Парсер из DLL ────────────────────────────────────────
        private readonly DataGridViewParser _parser = new DataGridViewParser();

        // ─ UI ──────────────────────────────────────────────────
        private RichTextBox    richLog;
        private NumericUpDown  nudSkipCount;
        private Button         btnLoadFile;
        private Button         btnHighlight;
        private Button         btnClear;
        private Label          lblTime;
        private Label          lblCount;
        private Label          lblFound;

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

        public Lab4Panel()
        {
            Dock = DockStyle.Fill;
            BuildUI();
        }

        private void BuildUI()
        {
            // ── Топ-панель ──────────────────────────────────────────────
            var topPanel = new Panel
            {
                Dock = DockStyle.Top, Height = 56,
                BackColor = Color.FromArgb(45, 45, 60),
                Padding   = new Padding(8, 6, 8, 4)
            };

            var lblI = new Label
            {
                Text = "Пропустить первые i вхождений (dataGridView*):",
                ForeColor = Color.White,
                Left = 8, Top = 18, AutoSize = true
            };

            nudSkipCount = new NumericUpDown
            {
                Left = 380, Top = 14, Width = 80,
                Minimum = 0, Maximum = 99999, Value = 0,
                Font = new Font("Segoe UI", 10f)
            };

            btnLoadFile = new Button
            {
                Text = "Загрузить лог",
                Left = 475, Top = 12, Width = 140, Height = 34,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(60, 100, 160)
            };

            btnHighlight = new Button
            {
                Text = "Выделить (async)",
                Left = 625, Top = 12, Width = 165, Height = 34,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(60, 150, 80)
            };

            btnClear = new Button
            {
                Text = "Очистить",
                Left = 800, Top = 12, Width = 110, Height = 34,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(160, 60, 60)
            };

            topPanel.Controls.AddRange(new Control[]
                { lblI, nudSkipCount, btnLoadFile, btnHighlight, btnClear });

            // ── Статус-бар ──────────────────────────────────────────────
            var statusPanel = new Panel
            {
                Dock = DockStyle.Bottom, Height = 30,
                BackColor = Color.FromArgb(35, 35, 50)
            };

            lblCount = new Label { Text = "Всего: —",          ForeColor = Color.LightGray,  Left =   8, Top = 6, AutoSize = true };
            lblFound = new Label { Text = "Выделено: —",       ForeColor = Color.Yellow,     Left = 200, Top = 6, AutoSize = true };
            lblTime  = new Label { Text = "Время (async): —", ForeColor = Color.LightGreen, Left = 420, Top = 6, AutoSize = true };

            statusPanel.Controls.AddRange(new Control[] { lblCount, lblFound, lblTime });

            // ── RichTextBox ──────────────────────────────────────────────
            richLog = new RichTextBox
            {
                Dock       = DockStyle.Fill,
                Font       = new Font("Consolas", 10f),
                BackColor  = DefaultBack,
                ForeColor  = DefaultFore,
                ScrollBars = RichTextBoxScrollBars.Both,
                WordWrap   = false
            };

            Controls.Add(richLog);
            Controls.Add(topPanel);
            Controls.Add(statusPanel);

            // ── События ───────────────────────────────────────────────
            richLog.TextChanged    += async (s, e) => { if (!_isHighlighting) await HighlightAsync(); };
            nudSkipCount.ValueChanged += async (s, e) => await HighlightAsync();
            btnLoadFile.Click      += async (s, e) => await LoadFileAsync();
            btnHighlight.Click     += async (s, e) => await HighlightAsync();
            btnClear.Click         += (s, e) =>
            {
                _isHighlighting = true;
                richLog.Clear();
                _isHighlighting = false;
                lblCount.Text = "Всего: —";
                lblFound.Text = "Выделено: —";
                lblTime.Text  = "Время (async): —";
            };
        }

        // ── Загрузка файла ──────────────────────────────────────────
        private async Task LoadFileAsync()
        {
            using (var dlg = new OpenFileDialog
            {
                Filter = "Лог-файлы|*.log;*.txt|Все файлы|*.*",
                Title  = "Выберите лог-журнал"
            })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;

                ProcessStatus.Start("Загрузка лог-файла (ЛР №4)");
                try
                {
                    string text = await Task.Run(() =>
                        System.IO.File.ReadAllText(dlg.FileName, System.Text.Encoding.UTF8));
                    _isHighlighting = true;
                    richLog.Text    = text;
                    _isHighlighting = false;
                    await HighlightAsync();
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log(ex, "ЛР №4 — загрузка лог-файла");
                }
                finally
                {
                    ProcessStatus.Stop();
                }
            }
        }

        // ── Асинх подсветка через DLL ────────────────────────────────
        private async Task HighlightAsync()
        {
            if (richLog == null || string.IsNullOrEmpty(richLog.Text)) return;
            if (_isHighlighting) return;
            _isHighlighting = true;
            ProcessStatus.Start("Парсинг лога (ЛР №4, async)");
            try
            {
                int    skipCount = (int)nudSkipCount.Value;
                string fullText  = richLog.Text;

                // Вызываем асинхронный парсинг из DLL
                ParseResult result = await _parser.ParseAsync(fullText, skipCount);

                lblCount.Text = $"Всего: {result.TotalFound}";
                lblFound.Text = $"Выделено (после i={skipCount}): {result.Highlighted}";
                lblTime.Text  = $"Время (async): {result.Elapsed.TotalMilliseconds:F2} мс";

                if (result.TotalFound == 0) return;

                // Применяем подсветку в UI-потоке
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
                ExceptionLogger.Log(ex, "ЛР №4 — парсинг лога");
            }
            finally
            {
                _isHighlighting = false;
                ProcessStatus.Stop();
            }
        }
    }
}
