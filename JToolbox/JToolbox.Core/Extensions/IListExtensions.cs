using System.Collections.Generic;

namespace JToolbox.Core.Extensions
{
    public static class IListExtensions
    {
        public static void ShiftLeft<T>(this IList<T> @this, int index)
        {
            if (@this.Count < 2)
                return;

            if (index < 1)
                return;

            var temp = @this[index - 1];
            @this.RemoveAt(index - 1);
            @this.Insert(index, temp);
        }

        public static void ShiftRight<T>(this IList<T> @this, int index)
        {
            if (@this.Count < 2)
                return;

            if (index > @this.Count - 2)
                return;

            var temp = @this[index + 1];
            @this.RemoveAt(index + 1);
            @this.Insert(index, temp);
        }

        public static void Swap<T>(this IList<T> @this, T oldValue, T newValue)
        {
            var oldIndex = @this.IndexOf(oldValue);
            while (oldIndex > 0)
            {
                @this.RemoveAt(oldIndex);
                @this.Insert(oldIndex, newValue);
                oldIndex = @this.IndexOf(oldValue);
            }
        }

        public static void Swap<T>(this IList<T> @this, int index1, int index2)
        {
            var temp = @this[index1];
            @this[index1] = @this[index2];
            @this[index2] = temp;
        }

        public static IList<T> GetRange<T>(this IList<T> @this, int index, int count)
        {
            var result = new List<T>();
            for (int i = index; i < index + count; i++)
            {
                result.Add(@this[i]);
            }
            return result;
        }
    }
}