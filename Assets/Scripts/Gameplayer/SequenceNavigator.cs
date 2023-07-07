using System.Collections.Generic;
using System.Linq;
using Ramsey.Board;

namespace Ramsey.Gameplayer
{
    public class SequenceNavigator<T>
    {
        //Will loop last sequence

        public delegate IEnumerable<T> Initializer(GameState state);

        bool shouldReset;
        int current;
        List<IEnumerator<T>> navigators;
        
        readonly IList<Initializer> sequences;

        public T Loop(GameState state)
        {
            if(shouldReset)
            {
                PerformReset(state);
                shouldReset = false;
            }

            if (!navigators[current].MoveNext())
            {
                if (current != navigators.Count - 1) current++; else navigators[current] = sequences.Last()(state).GetEnumerator();
                navigators[current].MoveNext(); 
            }

            return navigators[current].Current;
        }

        public SequenceNavigator(params Initializer[] sequences)
        {
            this.sequences = sequences;
            Reset();
        }

        private void PerformReset(GameState state) 
        {
            navigators = sequences.Select(s => s(state).GetEnumerator()).ToList();
            current = 0;
        }

        public void Reset() 
        {
            shouldReset = true;
        }
    }
}