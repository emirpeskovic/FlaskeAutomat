using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlaskeAutomat
{
    public static class Linq
    {

        // "Take" every nth element from an array and return a new array
        public static T[] Take<T>(this T?[] collection, Predicate<T> match)
        {
            // We make it a list to not worry about indexing
            List<T> list = new();

            // Go through every element in the collection
            for (int i = 0; i < collection.Length; i++)
            {
                var element = collection[i]!;

                // If it matches our criteria, add it to the list and remove it from the original one
                if (match(element))
                {
                    list.Add(element);
                    collection[i] = default;
                }
            }

            // Return the list as an array
            return list.ToArray();
        }
    }
}
