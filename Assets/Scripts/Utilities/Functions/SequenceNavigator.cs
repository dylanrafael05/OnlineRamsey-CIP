using System.Collections.Generic;
using System.Linq;

namespace Ramsey.Utilities
{
    public class SequenceNavigator<T>
    {

        //Will loop last sequence

        int current = 0;
        List<IEnumerator<T>> navigators = new();
        IEnumerable<T> lastEnumerable;

        public T Loop()
        {
            if (!navigators[current].MoveNext())
            {
                if (current != navigators.Count - 1) current++; else navigators[current] = lastEnumerable.GetEnumerator();
                navigators[current].MoveNext(); 
            }

            return navigators[current].Current;
        }

        public SequenceNavigator(IList<IEnumerable<T>> sequences)
        { sequences.Foreach(s => navigators.Add(s.GetEnumerator())); lastEnumerable = sequences.Last(); }

    }
}