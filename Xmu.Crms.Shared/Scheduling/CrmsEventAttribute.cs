using System;
using System.Reflection;

namespace Xmu.Crms.Shared.Scheduling
{
    [AttributeUsage(AttributeTargets.Method,
                       AllowMultiple = true)]
    public class CrmsEventAttribute : Attribute
    {
        public CrmsEventAttribute(Type table, string timeColumn, string[] paramColumns)
        {
            Table = table;
            TimeColumn = timeColumn;
            ParamColumns = paramColumns;
        }

        public Type Table { get; }

        public string TimeColumn { get; }

        public string[] WhereColumns { get; set; } = { };

        public string[] ParamColumns { get; }

        public MethodInfo Callback { get; private set; }

        public CrmsEventAttribute SetCallback(MethodInfo m)
        {
            Callback = m;
            return this;
        }
    }
}
