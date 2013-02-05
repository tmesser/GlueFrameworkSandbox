using System;
using System.Linq;

namespace Domain.Switching
{
    /// <summary>
    /// Adapted from http://blogs.msdn.com/b/jaredpar/archive/2008/05/16/switching-on-types.aspx
    /// </summary>
    public static class TypeSwitch
    {
        public static void Do(object source, params CaseInfo[] cases)
        {
            var type = source.GetType();
            Do(type, cases);
        }

        public static void Do(Type source, params CaseInfo[] cases)
        {
            foreach (var entry in cases.Where(entry => entry.IsDefault || entry.Target.IsAssignableFrom(source)))
            {
                entry.Action(source);
                break;
            }
        }

        public static void Do(object firstSource, object secondSource, params DualCaseInfo[] cases)
        {
            var firstType = firstSource.GetType();
            var secondType = secondSource.GetType();
            Do(firstType, secondType, cases);
        }

        public static void Do(Type firstSource, Type secondSource, params DualCaseInfo[] cases)
        {
            foreach (var entry in cases.Where(entry =>
                (entry.IsDefault || entry.FirstTarget.IsAssignableFrom(firstSource)) &&
                (entry.IsDefault || entry.SecondTarget.IsAssignableFrom(secondSource))))
            {
                entry.Action(firstSource, secondSource);
                break;
            }
        }

        public static CaseInfo Case<T>(Action action)
        {
            return new CaseInfo
            {
                Action = x => action(),
                Target = typeof(T)
            };
        }

        public static CaseInfo Case<T>(Action<T> action)
        {
            return new CaseInfo
            {
                Action = x => action((T)x),
                Target = typeof(T)
            };
        }

        /// <summary>
        /// This is mostly a 'syntactic sugar' adaptation to make MappingFactories look prettier.  DualCaseInfo is kind of ugly on the whole, but that is not the point right now.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TS">The type of the S.</typeparam>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public static DualCaseInfo Case<T, TS>(Action action)
        {
            return new DualCaseInfo
            {
                Action = (x, y) => action(),
                FirstTarget = typeof(T),
                SecondTarget = typeof(TS)
            };
        }

        /// <summary>
        /// This is mostly a 'syntactic sugar' adaptation to make MappingFactories look prettier.  DualCaseInfo is kind of ugly on the whole, but that is not the point right now.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TS">The type of the S.</typeparam>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public static DualCaseInfo Case<T, TS>(Action<T, TS> action)
        {
            return new DualCaseInfo
            {
                Action = (x, y) => action((T)x, (TS)y),
                FirstTarget = typeof(T),
                SecondTarget = typeof(TS)
            };
        }

        public static CaseInfo Default(Action action)
        {
            return new CaseInfo
            {
                Action = x => action(),
                IsDefault = true
            };
        }
    }
}
