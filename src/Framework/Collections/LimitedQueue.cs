using System.Collections.Generic;
using System.Linq;

namespace Kogler.Framework
{
    /*
    * The Alphanum Algorithm is an improved sorting algorithm for strings
    * containing numbers.  Instead of sorting numbers in ASCII order like
    * a standard sort, this algorithm sorts numbers in numeric order.
    *
    * The Alphanum Algorithm is discussed at http://www.DaveKoelle.com
    *
    * Based on the Java implementation of Dave Koelle's Alphanum algorithm.
    * Contributed by Jonathan Ruckwood <jonathan.ruckwood@gmail.com>
    * 
    * Adapted by Dominik Hurnaus <dominik.hurnaus@gmail.com> to 
    *   - correctly sort words where one word starts with another word
    *   - have slightly better performance
    * 
    * This library is free software; you can redistribute it and/or
    * modify it under the terms of the GNU Lesser General Public
    * License as published by the Free Software Foundation; either
    * version 2.1 of the License, or any later version.
    *
    * This library is distributed in the hope that it will be useful,
    * but WITHOUT ANY WARRANTY; without even the implied warranty of
    * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    * Lesser General Public License for more details.
    *
    * You should have received a copy of the GNU Lesser General Public
    * License along with this library; if not, write to the Free Software
    * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
    *
    * Please compare against the latest Java version at http://www.DaveKoelle.com
    * to see the most recent modifications 
    */

    public class LimitedQueue<T> : Queue<T>
    {
        private int m_Limit = -1;

        public int Limit
        {
            get { return m_Limit; }
            set
            {
                m_Limit = value;
                CheckLimit();
            }
        }

        public LimitedQueue(int limit, IEnumerable<T> set = null)
            : base(limit)
        {
            Limit = limit;
            if (set != null)
                foreach (T entry in set.Take(limit))
                    Enqueue(entry);
        }

        public new void Enqueue(T item)
        {
            base.Enqueue(item);
            CheckLimit();
        }

        private void CheckLimit()
        {
            while (Count > Limit) Dequeue();
        }
    }
}