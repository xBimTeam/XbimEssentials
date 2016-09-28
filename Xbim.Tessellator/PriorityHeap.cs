/*
** SGI FREE SOFTWARE LICENSE B (Version 2.0, Sept. 18, 2008) 
** Copyright (C) 2011 Silicon Graphics, Inc.
** All Rights Reserved.
**
** Permission is hereby granted, free of charge, to any person obtaining a copy
** of this software and associated documentation files (the "Software"), to deal
** in the Software without restriction, including without limitation the rights
** to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
** of the Software, and to permit persons to whom the Software is furnished to do so,
** subject to the following conditions:
** 
** The above copyright notice including the dates of first publication and either this
** permission notice or a reference to http://oss.sgi.com/projects/FreeB/ shall be
** included in all copies or substantial portions of the Software. 
**
** THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
** INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
** PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL SILICON GRAPHICS, INC.
** BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
** TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE
** OR OTHER DEALINGS IN THE SOFTWARE.
** 
** Except as contained in this notice, the name of Silicon Graphics, Inc. shall not
** be used in advertising or otherwise to promote the sale, use or other dealings in
** this Software without prior written authorization from Silicon Graphics, Inc.
*/
/*
** Original Author: Eric Veach, July 1994.
** libtess2: Mikko Mononen, http://code.google.com/p/libtess2/.
** LibTessDotNet: Remi Gillig, https://github.com/speps/LibTessDotNet
*/

using System;
using System.Diagnostics;

namespace Xbim.Tessellator
{
    public struct PqHandle
    {
        public static readonly int Invalid = 0x0fffffff;
        internal int Handle;
    }

    public class PriorityHeap<TValue> where TValue : class
    {
        public delegate bool LessOrEqual(TValue lhs, TValue rhs);

        protected class HandleElem
        {
            internal TValue Key;
            internal int Node;
        }

        private LessOrEqual _leq;
        private int[] _nodes;
        private HandleElem[] _handles;
        private int _size, _max;
        private int _freeList;
        private bool _initialized;

        public bool Empty { get { return _size == 0; } }

        public PriorityHeap(int initialSize, LessOrEqual leq)
        {
            _leq = leq;

            _nodes = new int[initialSize + 1];
            _handles = new HandleElem[initialSize + 1];

            _size = 0;
            _max = initialSize;
            _freeList = 0;
            _initialized = false;

            _nodes[1] = 1;
            _handles[1] = new HandleElem { Key = null };
        }

        private void FloatDown(int curr)
        {
            int child;
            int hCurr, hChild;

            hCurr = _nodes[curr];
            while (true)
            {
                child = curr << 1;
                if (child < _size && _leq(_handles[_nodes[child + 1]].Key, _handles[_nodes[child]].Key))
                {
                    ++child;
                }

                Debug.Assert(child <= _max);

                hChild = _nodes[child];
                if (child > _size || _leq(_handles[hCurr].Key, _handles[hChild].Key))
                {
                    _nodes[curr] = hCurr;
                    _handles[hCurr].Node = curr;
                    break;
                }

                _nodes[curr] = hChild;
                _handles[hChild].Node = curr;
                curr = child;
            }
        }

        private void FloatUp(int curr)
        {
            int parent;
            int hCurr, hParent;

            hCurr = _nodes[curr];
            while (true)
            {
                parent = curr >> 1;
                hParent = _nodes[parent];
                if (parent == 0 || _leq(_handles[hParent].Key, _handles[hCurr].Key))
                {
                    _nodes[curr] = hCurr;
                    _handles[hCurr].Node = curr;
                    break;
                }
                _nodes[curr] = hParent;
                _handles[hParent].Node = curr;
                curr = parent;
            }
        }

        public void Init()
        {
            for (int i = _size; i >= 1; --i)
            {
                FloatDown(i);
            }
            _initialized = true;
        }

        public PqHandle Insert(TValue value)
        {
            int curr = ++_size;
            if ((curr * 2) > _max)
            {
                _max <<= 1;
                Array.Resize(ref _nodes, _max + 1);
                Array.Resize(ref _handles, _max + 1);
            }

            int free;
            if (_freeList == 0)
            {
                free = curr;
            }
            else
            {
                free = _freeList;
                _freeList = _handles[free].Node;
            }

            _nodes[curr] = free;
            if (_handles[free] == null)
            {
                _handles[free] = new HandleElem { Key = value, Node = curr };
            }
            else
            {
                _handles[free].Node = curr;
                _handles[free].Key = value;
            }

            if (_initialized)
            {
                FloatUp(curr);
            }

            Debug.Assert(free != PqHandle.Invalid);
            return new PqHandle { Handle = free };
        }

        public TValue ExtractMin()
        {
            Debug.Assert(_initialized);

            int hMin = _nodes[1];
            TValue min = _handles[hMin].Key;

            if (_size > 0)
            {
                _nodes[1] = _nodes[_size];
                _handles[_nodes[1]].Node = 1;

                _handles[hMin].Key = null;
                _handles[hMin].Node = _freeList;
                _freeList = hMin;

                if (--_size > 0)
                {
                    FloatDown(1);
                }
            }

            return min;
        }

        public TValue Minimum()
        {
            Debug.Assert(_initialized);
            return _handles[_nodes[1]].Key;
        }

        public void Remove(PqHandle handle)
        {
            Debug.Assert(_initialized);

            int hCurr = handle.Handle;
            Debug.Assert(hCurr >= 1 && hCurr <= _max && _handles[hCurr].Key != null);

            int curr = _handles[hCurr].Node;
            _nodes[curr] = _nodes[_size];
            _handles[_nodes[curr]].Node = curr;

            if (curr <= --_size)
            {
                if (curr <= 1 || _leq(_handles[_nodes[curr >> 1]].Key, _handles[_nodes[curr]].Key))
                {
                    FloatDown(curr);
                }
                else
                {
                    FloatUp(curr);
                }
            }

            _handles[hCurr].Key = null;
            _handles[hCurr].Node = _freeList;
            _freeList = hCurr;
        }
    }
}
