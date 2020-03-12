using System;
using System.Linq;
using System.Collections.Generic;

namespace BoolForms
{
    public static class Utils
    {
        public static bool UnorderedEnumerableEquals<T>(IEnumerable<T> enumerable1, IEnumerable<T> enumerable2)
        {
            if (enumerable1 == null || enumerable2 == null || enumerable1.Count() != enumerable2.Count())
                return false;
            if (!enumerable1.Any())
                return true;
            Dictionary<T, int> lookUp = new Dictionary<T, int>();
            // create index for the first list
            foreach (T elem in enumerable1)
            {
                int count = 0;
                if (!lookUp.TryGetValue(elem, out count))
                {
                    lookUp.Add(elem, 1);
                    continue;
                }
                lookUp[elem] = count + 1;
            }
            foreach (T elem in enumerable2)
            {
                int count = 0;
                if (!lookUp.TryGetValue(elem, out count))
                {
                    // early exit as the current value in B doesn't exist in the lookUp (and not in ListA)
                    return false;
                }
                count--;
                if (count <= 0)
                    lookUp.Remove(elem);
                else
                    lookUp[elem] = count;
            }
            // if there are remaining elements in the lookUp, that means ListA contains elements that do not exist in ListB
            return lookUp.Count == 0;
        }
    }
}