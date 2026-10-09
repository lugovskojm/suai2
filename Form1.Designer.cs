using lab1forms.Services.Core;

namespace lab1forms
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ExceptionLogger.EntryLogged -= OnExceptionEntry;
                ProcessStatus.Started -= OnProcessStarted;
                ProcessStatus.Stopped -= OnProcessStopped;
                clockTimer?.Dispose();
                if (components != null) components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabLab1 = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnTask2 = new System.Windows.Forms.Button();
            this.btnTask1 = new System.Windows.Forms.Button();
            this.txtN = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnBitwiseAdd = new System.Windows.Forms.Button();
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtSurname = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.lblSurname = new System.Windows.Forms.Label();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.tabLab2 = new System.Windows.Forms.TabPage();
            this.tabLab3 = new System.Windows.Forms.TabPage();
            this.tabLab4 = new System.Windows.Forms.TabPage();
            this.tabLab5 = new System.Windows.Forms.TabPage();
            this.tabControl.SuspendLayout();
            this.tabLab1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabLab1);
            this.tabControl.Controls.Add(this.tabLab2);
            this.tabControl.Controls.Add(this.tabLab3);
            this.tabControl.Controls.Add(this.tabLab4);
            this.tabControl.Controls.Add(this.tabLab5);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(2800, 1338);
            this.tabControl.TabIndex = 0;
            // 
            // tabLab1
            // 
            this.tabLab1.Controls.Add(this.splitContainer1);
            this.tabLab1.Location = new System.Drawing.Point(10, 47);
            this.tabLab1.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.tabLab1.Name = "tabLab1";
            this.tabLab1.Padding = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.tabLab1.Size = new System.Drawing.Size(2780, 1281);
            this.tabLab1.TabIndex = 0;
            this.tabLab1.Text = "Лабораторная №1";
            this.tabLab1.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(7, 7);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.txtOutput);
            this.splitContainer1.Size = new System.Drawing.Size(2766, 1267);
            this.splitContainer1.SplitterDistance = 932;
            this.splitContainer1.SplitterWidth = 9;
            this.splitContainer1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnTask2);
            this.groupBox1.Controls.Add(this.btnTask1);
            this.groupBox1.Controls.Add(this.txtN);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnBitwiseAdd);
            this.groupBox1.Controls.Add(this.txtName);
            this.groupBox1.Controls.Add(this.txtSurname);
            this.groupBox1.Controls.Add(this.lblName);
            this.groupBox1.Controls.Add(this.lblSurname);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.groupBox1.Size = new System.Drawing.Size(932, 1267);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Функции";
            // 
            // btnTask2
            // 
            this.btnTask2.Location = new System.Drawing.Point(47, 781);
            this.btnTask2.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.btnTask2.Name = "btnTask2";
            this.btnTask2.Size = new System.Drawing.Size(840, 89);
            this.btnTask2.TabIndex = 8;
            this.btnTask2.Text = "Задание 2: Найти A + B² = C² (1-20)";
            this.btnTask2.UseVisualStyleBackColor = true;
            this.btnTask2.Click += new System.EventHandler(this.btnTask2_Click);
            // 
            // btnTask1
            // 
            this.btnTask1.Location = new System.Drawing.Point(47, 625);
            this.btnTask1.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.btnTask1.Name = "btnTask1";
            this.btnTask1.Size = new System.Drawing.Size(840, 89);
            this.btnTask1.TabIndex = 7;
            this.btnTask1.Text = "Задание 1: Найти числа с N делителями";
            this.btnTask1.UseVisualStyleBackColor = true;
            this.btnTask1.Click += new System.EventHandler(this.btnTask1_Click);
            // 
            // txtN
            // 
            this.txtN.Location = new System.Drawing.Point(47, 535);
            this.txtN.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.txtN.Name = "txtN";
            this.txtN.Size = new System.Drawing.Size(835, 35);
            this.txtN.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(47, 491);
            this.label1.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(398, 29);
            this.label1.TabIndex = 5;
            this.label1.Text = "Количество делителей N (1-200):";
            // 
            // btnBitwiseAdd
            // 
            this.btnBitwiseAdd.Location = new System.Drawing.Point(47, 312);
            this.btnBitwiseAdd.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.btnBitwiseAdd.Name = "btnBitwiseAdd";
            this.btnBitwiseAdd.Size = new System.Drawing.Size(840, 89);
            this.btnBitwiseAdd.TabIndex = 4;
            this.btnBitwiseAdd.Text = "Выполнить поразрядное сложение";
            this.btnBitwiseAdd.UseVisualStyleBackColor = true;
            this.btnBitwiseAdd.Click += new System.EventHandler(this.btnBitwiseAdd_Click);
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(47, 223);
            this.txtName.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(835, 35);
            this.txtName.TabIndex = 3;
            // 
            // txtSurname
            // 
            this.txtSurname.Location = new System.Drawing.Point(47, 112);
            this.txtSurname.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.txtSurname.Name = "txtSurname";
            this.txtSurname.Size = new System.Drawing.Size(835, 35);
            this.txtSurname.TabIndex = 2;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(47, 178);
            this.lblName.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(69, 29);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "Имя:";
            // 
            // lblSurname
            // 
            this.lblSurname.AutoSize = true;
            this.lblSurname.Location = new System.Drawing.Point(47, 67);
            this.lblSurname.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lblSurname.Name = "lblSurname";
            this.lblSurname.Size = new System.Drawing.Size(129, 29);
            this.lblSurname.TabIndex = 0;
            this.lblSurname.Text = "Фамилия:";
            // 
            // txtOutput
            // 
            this.txtOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOutput.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtOutput.Location = new System.Drawing.Point(0, 0);
            this.txtOutput.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ReadOnly = true;
            this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtOutput.Size = new System.Drawing.Size(1825, 1267);
            this.txtOutput.TabIndex = 0;
            this.txtOutput.WordWrap = false;
            // 
            // tabLab2
            // 
            this.tabLab2.Location = new System.Drawing.Point(10, 47);
            this.tabLab2.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.tabLab2.Name = "tabLab2";
            this.tabLab2.Padding = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.tabLab2.Size = new System.Drawing.Size(2780, 1281);
            this.tabLab2.TabIndex = 1;
            this.tabLab2.Text = "Лабораторная №2";
            this.tabLab2.UseVisualStyleBackColor = true;
            // 
            // tabLab3
            // 
            this.tabLab3.Location = new System.Drawing.Point(10, 47);
            this.tabLab3.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.tabLab3.Name = "tabLab3";
            this.tabLab3.Size = new System.Drawing.Size(2780, 1281);
            this.tabLab3.TabIndex = 2;
            this.tabLab3.Text = "Лабораторная №3";
            this.tabLab3.UseVisualStyleBackColor = true;
            // 
            // tabLab4
            // 
            this.tabLab4.Location = new System.Drawing.Point(10, 47);
            this.tabLab4.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.tabLab4.Name = "tabLab4";
            this.tabLab4.Size = new System.Drawing.Size(2780, 1281);
            this.tabLab4.TabIndex = 3;
            this.tabLab4.Text = "Лабораторная №4";
            this.tabLab4.UseVisualStyleBackColor = true;
            // 
            // tabLab5
            // 
            this.tabLab5.Location = new System.Drawing.Point(10, 47);
            this.tabLab5.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.tabLab5.Name = "tabLab5";
            this.tabLab5.Size = new System.Drawing.Size(2780, 1281);
            this.tabLab5.TabIndex = 4;
            this.tabLab5.Text = "Лабораторная №5";
            this.tabLab5.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 29F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2800, 1338);
            this.Controls.Add(this.tabControl);
            this.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Гр. 3437к - Луговской М. С.";
            this.tabControl.ResumeLayout(false);
            this.tabLab1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabLab1;
        private System.Windows.Forms.TabPage tabLab2;
        private System.Windows.Forms.TabPage tabLab3;
        private System.Windows.Forms.TabPage tabLab4;
        private System.Windows.Forms.TabPage tabLab5;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.Label lblSurname;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtSurname;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Button btnBitwiseAdd;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtN;
        private System.Windows.Forms.Button btnTask1;
        private System.Windows.Forms.Button btnTask2;
    }
}

