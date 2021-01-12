using System;

namespace Aicup2020.Game.A
{
    public sealed class PriorityQueue<T>
    {
        private PriorityQueueNode[] _heap;
        private int _count;

        public PriorityQueue(int capacity)
        {
            _heap = new PriorityQueueNode[capacity];
            _count = 0;
        }

        public T First
        {
            get
            {
                if (_count == 0)
                {
                    throw new InvalidOperationException("Queue is empty");
                }
                return _heap[0].Item;
            }
        }

        public void Enqueue(T item, int priority)
        {
            EnsureCapacity();

            int index = _count;
            _count++;
            _heap[index] = new PriorityQueueNode(item, priority);
            HeapifyUp(index);
        }

        private void HeapifyUp(int index)
        {
            while (index != 0)
            {
                int parent = GetParent(index);
                if (_heap[parent].Priority <= _heap[index].Priority)
                {
                    return;
                }

                Swap(parent, index);
                index = parent;
            }
        }

        private void EnsureCapacity()
        {
            int capacity = _heap.Length;

            if (_count < capacity)
            {
                return;
            }

            var resized = new PriorityQueueNode[capacity * 2];
            Array.Copy(_heap, 0, resized, 0, _count);
            _heap = resized;
        }

        public bool TryDequeue(out T item)
        {
            if (_count == 0)
            {
                item = default;
                return false;
            }

            item = _heap[0].Item;

            _heap[0] = _heap[_count - 1];
            _count--;

            if (_count > 1)
            {
                HeapifyDown(0);
            }

            return true;
        }

        private void HeapifyDown(int index)
        {
            while (true)
            {
                int left = GetLeftChild(index);
                int right = GetRightChild(index);
                int min = index;

                if (left < _count && _heap[left].Priority < _heap[min].Priority)
                {
                    min = left;
                }

                if (right < _count && _heap[right].Priority < _heap[min].Priority)
                {
                    min = right;
                }

                if (min == index)
                {
                    return;
                }

                Swap(min, index);
                index = min;
            }
        }

        private void Swap(int i, int j)
        {
            PriorityQueueNode temp = _heap[i];
            _heap[i] = _heap[j];
            _heap[j] = temp;
        }

        private static int GetLeftChild(int index) => 2 * index + 1;

        private static int GetRightChild(int index) => 2 * index + 2;

        private static int GetParent(int index) => (index - 1) / 2;

        public void Clear()
        {
            Array.Clear(_heap, 0, _count); // GC;
            _count = 0;
        }

        private readonly struct PriorityQueueNode
        {
            public readonly T Item;
            public readonly int Priority;

            public PriorityQueueNode(T item, int priority)
            {
                Item = item;
                Priority = priority;
            }
        }
    }
}