using System;
using System.Windows.Forms;
using lab1forms.Services.Core;

namespace lab1forms
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Глобальные обработчики исключений: необработанные исключения
            // из любого потока фиксируются в центральном журнале проекта
            // (текстовый файл + textbox на главной форме).
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (s, e) => ExceptionLogger.Log(e.Exception, "UI-поток");
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                ExceptionLogger.Log((Exception)e.ExceptionObject, "AppDomain");

            Application.Run(new Form1());
        }
    }
}
