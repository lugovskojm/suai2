using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LogParserLib.Parsers
{
    /// <summary>
    /// Парсер для поиска словоформ dataGridView* (вариант 11, без учёта регистра).
    /// Каждый парсер — отдельный класс в DLL (требование задания).
    /// </summary>
    public class DataGridViewParser
    {
        private static readonly Regex _regex = new Regex(
            @"dataGridView\w*",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Синхронный парсинг текста.
        /// </summary>
        public ParseResult Parse(string text, int skipCount)
        {
            var sw = Stopwatch.StartNew();
            var list = new List<(int Index, int Length)>();
            foreach (Match m in _regex.Matches(text ?? string.Empty))
                list.Add((m.Index, m.Length));
            sw.Stop();
            return new ParseResult(list, sw.Elapsed, skipCount);
        }

        /// <summary>
        /// Асинхронный парсинг текста (фоновый поток).
        /// </summary>
        public async Task<ParseResult> ParseAsync(string text, int skipCount)
        {
            return await Task.Run(() => Parse(text, skipCount));
        }

        /// <summary>
        /// Асинхронный парсинг нескольких текстов одновременно.
        /// Возвращает результаты в том же порядке, что и входные тексты.
        /// </summary>
        public async Task<ParseResult[]> ParseManyAsync(string[] texts, int skipCount)
        {
            var tasks = texts.Select(t => ParseAsync(t, skipCount)).ToArray();
            return await Task.WhenAll(tasks);
        }
    }
}
