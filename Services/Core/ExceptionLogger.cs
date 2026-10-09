using System;
using System.IO;

namespace lab1forms.Services.Core
{
    /// <summary>
    /// Центральный журнал исключений консолидированного проекта.
    /// Каждая запись содержит дату и время проявления исключения,
    /// источник (наименование формы/модуля), сообщение исключения
    /// и стек вызовов. Записи дублируются в текстовый файл
    /// <see cref="LogFileName"/> и рассылается подписчикам события
    /// <see cref="EntryLogged"/> (главная форма выводит их в textbox).
    /// </summary>
    public static class ExceptionLogger
    {
        /// <summary>Имя текстового файла журнала исключений.</summary>
        public const string LogFileName = "exceptions_log.txt";

        private static readonly object _sync = new object();

        /// <summary>Событие: сформирована новая запись журнала (текст записи).</summary>
        public static event Action<string> EntryLogged;

        /// <summary>Фиксирует исключение в файле и уведомляет подписчиков.</summary>
        /// <param name="ex">Исключение.</param>
        /// <param name="source">Источник (наименование формы/модуля).</param>
        public static void Log(Exception ex, string source)
        {
            if (ex == null) return;

            string entry = string.Format(
                "[{0:dd.MM.yyyy HH:mm:ss}]  [{1}]\n" +
                "Тип исключения: {2}\n" +
                "Сообщение: {3}\n" +
                "Стек вызовов:\n{4}\n{5}\n",
                DateTime.Now, source, ex.GetType().FullName, ex.Message,
                ex.StackTrace ?? "(стек вызовов отсутствует)", new string('─', 60));

            lock (_sync)
            {
                try { File.AppendAllText(LogFileName, entry); }
                catch { /* файл недоступен — запись всё равно уходит в UI */ }
            }

            var handler = EntryLogged;
            if (handler != null) handler(entry);
        }
    }
}
