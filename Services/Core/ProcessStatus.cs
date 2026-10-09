using System;

namespace lab1forms.Services.Core
{
    /// <summary>
    /// Статус запущенного процесса для строки статуса главной формы:
    /// модули лабораторных работ сообщают о начале/окончании выполнения
    /// процесса, а главная форма отображает индикатор динамики и наименование.
    /// </summary>
    public static class ProcessStatus
    {
        /// <summary>Событие: запущен процесс (наименование процесса).</summary>
        public static event Action<string> Started;

        /// <summary>Событие: процесс завершён.</summary>
        public static event Action Stopped;

        /// <summary>Сообщить о запуске процесса (индикатор строки статуса).</summary>
        public static void Start(string processName)
        {
            var handler = Started;
            if (handler != null) handler(processName);
        }

        /// <summary>Сообщить об окончании процесса.</summary>
        public static void Stop()
        {
            var handler = Stopped;
            if (handler != null) handler();
        }
    }
}
