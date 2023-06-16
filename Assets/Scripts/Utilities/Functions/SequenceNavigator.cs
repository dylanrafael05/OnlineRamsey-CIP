using System.Collections.Generic;
using System.Linq;

namespace Ramsey.Utilities
{
    public class SequenceNavigator<T>
    {
        //Will loop last sequence

        int current;
        List<IEnumerator<T>> navigators;
        IList<IEnumerable<T>> sequences;

        public T Loop()
        {
            if (!navigators[current].MoveNext())
            {
                if (current != navigators.Count - 1) current++; else navigators[current] = sequences.Last().GetEnumerator();
                navigators[current].MoveNext(); 
            }

            return navigators[current].Current;
        }

        public SequenceNavigator(params IEnumerable<T>[] sequences)
        {
            this.sequences = sequences;
            Reset();
        }

        public void Reset() 
        {
            navigators = sequences.Select(s => s.GetEnumerator()).ToList();
            current = 0;
        }
    }
}