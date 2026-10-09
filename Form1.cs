using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using lab1forms.Services.Core;
using lab1forms.Services.Lab1;
using lab1forms.Services.Lab2;
using lab1forms.Services.Lab3;
using lab1forms.Services.Lab4;
using lab1forms.Services.Lab5;

namespace lab1forms
{
    public partial class Form1 : Form
    {
        private BitwiseStringAdditionService bitwiseService;
        private DivisorCountService divisorService;
        private PythagoreanTriplesService pythagoreanService;

        // ── элементы «хрома» главной формы (меню, строка статуса, журнал) ──
        private MenuStrip menuStrip;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblProcessName;
        private ToolStripProgressBar progressBar;
        private ToolStripStatusLabel lblDateTime;
        private GroupBox grpExceptions;
        private TextBox txtExceptionLog;
        private Timer clockTimer;

        public Form1()
        {
            InitializeComponent();
            BuildChrome();
            InitializeServices();
        }

        /// <summary>
        /// Формирует главное меню, строку статуса обработки процессов
        /// (индикатор динамики, наименование процесса, текущие системные
        /// дата и время) и журнал исключений в textbox на главной форме.
        /// </summary>
        private void BuildChrome()
        {
            // ── Журнал исключений (textbox на главной форме) ────────────────
            txtExceptionLog = new TextBox
            {
                Multiline  = true,
                ReadOnly   = true,
                ScrollBars = ScrollBars.Both,
                WordWrap   = false,
                Dock       = DockStyle.Fill,
                BackColor  = Color.FromArgb(24, 24, 28),
                ForeColor  = Color.LimeGreen,
                Font       = new Font("Consolas", 8.5f)
            };

            var btnClearLog = new Button { Text = "✕ Очистить журнал", Dock = DockStyle.Bottom, Height = 24 };
            btnClearLog.Click += (s, e) => txtExceptionLog.Clear();

            grpExceptions = new GroupBox
            {
                Text    = string.Format("Журнал исключений  (файл: {0})", ExceptionLogger.LogFileName),
                Dock    = DockStyle.Bottom,
                Height  = 150,
                Padding = new Padding(6)
            };
            grpExceptions.Controls.Add(txtExceptionLog);
            grpExceptions.Controls.Add(btnClearLog);

            // ── Строка статуса обработки процессов ──────────────────────────
            statusStrip = new StatusStrip();

            lblProcessName = new ToolStripStatusLabel("Процесс: —") { AutoSize = true };

            progressBar = new ToolStripProgressBar
            {
                Width  = 140,
                Style  = ProgressBarStyle.Continuous,
                Maximum = 100
            };

            var spacer = new ToolStripStatusLabel(string.Empty) { Spring = true };

            lblDateTime = new ToolStripStatusLabel(
                DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")) { AutoSize = true };

            statusStrip.Items.Add(lblProcessName);
            statusStrip.Items.Add(progressBar);
            statusStrip.Items.Add(spacer);
            statusStrip.Items.Add(lblDateTime);

            // Часы: текущие системные дата и время, обновление раз в секунду.
            clockTimer = new Timer { Interval = 1000 };
            clockTimer.Tick += (s, e) => lblDateTime.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            clockTimer.Start();

            // ── Главное меню навигации по лабораторным работам ──────────────
            menuStrip = new MenuStrip();

            var mnuFile = new ToolStripMenuItem("&Файл");
            var mnuExit = new ToolStripMenuItem("Вы&ход", null, (s, e) => Close()) { ShortcutKeys = Keys.Alt | Keys.F4 };
            mnuFile.DropDownItems.Add(mnuExit);

            var mnuLabs = new ToolStripMenuItem("&Лабораторные работы");
            for (int i = 0; i < tabControl.TabPages.Count; i++)
            {
                int idx = i;
                string caption = "ЛР №" + (i + 1);
                var item = new ToolStripMenuItem(caption, null,
                    (s, e) => tabControl.SelectedIndex = idx)
                {
                    ShortcutKeys = Keys.Control | (Keys)(Keys.D1 + i),
                    ToolTipText  = "Перейти: " + tabControl.TabPages[i].Text
                };
                mnuLabs.DropDownItems.Add(item);
            }

            var mnuHelp = new ToolStripMenuItem("&Справка");
            var mnuAbout = new ToolStripMenuItem("О &программе", null, (s, e) =>
                MessageBox.Show(
                    "Консолидированный проект лабораторных работ\n" +
                    "«Технологии и методы программирования»\n\n" +
                    Text,
                    "О программе", MessageBoxButtons.OK, MessageBoxIcon.Information));
            mnuHelp.DropDownItems.Add(mnuAbout);

            menuStrip.Items.Add(mnuFile);
            menuStrip.Items.Add(mnuLabs);
            menuStrip.Items.Add(mnuHelp);

            // Порядок добавления важен для докинга: сначала Fill (tabControl),
            // затем нижние панели, затем верхнее меню.
            Controls.Add(grpExceptions);
            Controls.Add(statusStrip);
            Controls.Add(menuStrip);
            MainMenuStrip = menuStrip;

            // ── Подписки на журнал исключений и статус процессов ────────────
            ExceptionLogger.EntryLogged += OnExceptionEntry;
            ProcessStatus.Started += OnProcessStarted;
            ProcessStatus.Stopped += OnProcessStopped;
        }

        private void OnExceptionEntry(string entry)
        {
            if (txtExceptionLog == null || txtExceptionLog.IsDisposed) return;
            if (txtExceptionLog.InvokeRequired)
                txtExceptionLog.BeginInvoke((Action)(() => txtExceptionLog.AppendText(entry + Environment.NewLine)));
            else
                txtExceptionLog.AppendText(entry + Environment.NewLine);
        }

        private void OnProcessStarted(string processName)
        {
            lblProcessName.Text = "Выполняется: " + processName;
            progressBar.Style = ProgressBarStyle.Marquee; // индикатор динамики выполнения
        }

        private void OnProcessStopped()
        {
            lblProcessName.Text = "Процесс завершён";
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.Value = 0;
        }

        private void InitializeServices()
        {
            bitwiseService    = new BitwiseStringAdditionService();
            divisorService    = new DivisorCountService();
            pythagoreanService = new PythagoreanTriplesService();

            tabLab2.Controls.Add(new Lab2Panel());
            tabLab3.Controls.Add(new Lab3Panel());
            tabLab4.Controls.Add(new Lab4Panel());
            tabLab5.Controls.Add(new Lab5Panel());
        }

        private void btnBitwiseAdd_Click(object sender, EventArgs e)
        {
            ProcessStatus.Start("Поразрядное сложение строк (ЛР №1)");
            try
            {
                if (string.IsNullOrWhiteSpace(txtSurname.Text) || string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Пожалуйста, введите фамилию и имя!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string result = bitwiseService.Execute(txtSurname.Text, txtName.Text);
                txtOutput.Text = result;
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(ex, "ЛР №1 — поразрядное сложение строк");
            }
            finally
            {
                ProcessStatus.Stop();
            }
        }

        private void btnTask1_Click(object sender, EventArgs e)
        {
            ProcessStatus.Start("Поиск чисел с N делителями (ЛР №1)");
            try
            {
                if (string.IsNullOrWhiteSpace(txtN.Text))
                {
                    MessageBox.Show("Пожалуйста, введите количество делителей N!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string result = divisorService.Execute(txtN.Text);
                txtOutput.Text = result;
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(ex, "ЛР №1 — поиск чисел с N делителями");
            }
            finally
            {
                ProcessStatus.Stop();
            }
        }

        private void btnTask2_Click(object sender, EventArgs e)
        {
            ProcessStatus.Start("Поиск пифагоровых троек (ЛР №1)");
            try
            {
                string result = pythagoreanService.Execute();
                txtOutput.Text = result;
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(ex, "ЛР №1 — поиск пифагоровых троек");
            }
            finally
            {
                ProcessStatus.Stop();
            }
        }
    }
}
