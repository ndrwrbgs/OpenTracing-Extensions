namespace OpenTracing.Contrib.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Extensions for <see cref="ISpan"/>, for logging caller info
    /// </summary>
    public static partial class SpanExtensions
    {
        /// <summary>
        /// Logs information about the location of the log line as obtained by
        /// <see cref="CallerMemberNameAttribute"/> and <see cref="CallerLineNumberAttribute"/>.
        /// </summary>
        public static ISpan LogMemberInfo(
            this ISpan span,
            CallerInfoLogFlags logFlags = CallerInfoLogFlags.AllInformation,
            string memberNameLogKey = Constants.CallerMemberNameLogKey,
            string lineNumberLogKey = Constants.CallerMemberLineNumberLogKey,
            [CallerMemberName] string memberName = null,
            [CallerLineNumber] int lineNumber = -1)
        {
            var fields = CreateFields(
                logFlags,
                memberNameLogKey,
                lineNumberLogKey,
                memberName,
                lineNumber);

            return span
                .Log(fields);
        }

        /// <summary>
        /// Logs <see cref="fields"/> with information about the location of the log line
        /// as obtained by <see cref="CallerMemberNameAttribute"/> and <see cref="CallerLineNumberAttribute"/>.
        /// </summary>
        public static ISpan LogWithMemberInfo(
            this ISpan span,
            IEnumerable<KeyValuePair<string, object>> fields,
            CallerInfoLogFlags logFlags = CallerInfoLogFlags.AllInformation,
            string memberNameLogKey = Constants.CallerMemberNameLogKey,
            string lineNumberLogKey = Constants.CallerMemberLineNumberLogKey,
            [CallerMemberName] string memberName = null,
            [CallerLineNumber] int lineNumber = -1)
        {
            var memberFields = CreateFields(
                logFlags,
                memberNameLogKey,
                lineNumberLogKey,
                memberName,
                lineNumber);

            return span
                .Log(fields.Concat(memberFields));
        }

        /// <summary>
        /// Single log-value version of <see cref="LogWithMemberInfo(ISpan,IEnumerable{KeyValuePair{string,object}},CallerInfoLogFlags,string,string,string,int)"/>
        /// </summary>
        public static ISpan LogWithMemberInfo(
            this ISpan span,
            string fieldKey,
            object fieldValue,
            CallerInfoLogFlags logFlags = CallerInfoLogFlags.AllInformation,
            string memberNameLogKey = Constants.CallerMemberNameLogKey,
            string lineNumberLogKey = Constants.CallerMemberLineNumberLogKey,
            [CallerMemberName] string memberName = null,
            [CallerLineNumber] int lineNumber = -1)
        {
            var fields = new[]
            {
                new KeyValuePair<string, object>(fieldKey, fieldValue),
            };

            var memberFields = CreateFields(
                logFlags,
                memberNameLogKey,
                lineNumberLogKey,
                memberName,
                lineNumber);

            return span
                .Log(fields.Concat(memberFields));
        }

        private static KeyValuePair<string, object>[] CreateFields(
            CallerInfoLogFlags logFlags,
            string memberNameLogKey,
            string lineNumberLogKey,
            string memberName,
            int lineNumber)
        {
            switch (logFlags)
            {
                case CallerInfoLogFlags.None:
                    return new KeyValuePair<string, object>[]{ };
                case CallerInfoLogFlags.CallerMemberName:
                    return new[]
                    {
                        new KeyValuePair<string, object>(memberNameLogKey, memberName),
                    };
                case CallerInfoLogFlags.CallerLineNumber:
                    return new[]
                    {
                        new KeyValuePair<string, object>(lineNumberLogKey, lineNumber),
                    };
                case CallerInfoLogFlags.CallerMemberName | CallerInfoLogFlags.CallerLineNumber:
                    return new[]
                    {
                        new KeyValuePair<string, object>(memberNameLogKey, memberName),
                        new KeyValuePair<string, object>(lineNumberLogKey, lineNumber),
                    };
                default:
                    throw new ArgumentOutOfRangeException(nameof(logFlags), logFlags, null);
            }
        }
    }
}