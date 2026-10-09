using System;
using System.Collections.Generic;

namespace LogParserLib.Parsers
{
    /// <summary>
    /// Результат парсинга: список вхождений, время выполнения, число пропущенных.
    /// </summary>
    public class ParseResult
    {
        /// <summary>Все найденные вхождения (index, length).</summary>
        public IReadOnlyList<(int Index, int Length)> Matches { get; }

        /// <summary>Время выполнения парсинга.</summary>
        public TimeSpan Elapsed { get; }

        /// <summary>Количество пропущенных вхождений (i).</summary>
        public int SkipCount { get; }

        /// <summary>Всего найдено вхождений.</summary>
        public int TotalFound => Matches.Count;

        /// <summary>Выделено (после i-го).</summary>
        public int Highlighted => Math.Max(0, TotalFound - SkipCount);

        public ParseResult(
            IReadOnlyList<(int Index, int Length)> matches,
            TimeSpan elapsed,
            int skipCount)
        {
            Matches   = matches;
            Elapsed   = elapsed;
            SkipCount = skipCount;
        }
    }
}
