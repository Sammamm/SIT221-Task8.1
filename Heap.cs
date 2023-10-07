using System;
using System.Collections.Generic;

namespace Heap
{
    public class Heap<K, D> where K : IComparable<K>
    {
        private class Node : IHeapifyable<K, D>
        {
            public D Data { get; set; }
            public K Key { get; set; }
            public int Position { get; set; }

            public Node(K key, D data, int position)
            {
                Data = data;
                Key = key;
                Position = position;
            }
        }

        public int Count { get; private set; }
        private List<Node> data = new List<Node>();
        private IComparer<K> comparer;

        public Heap(IComparer<K> customComparer)
        {
            comparer = customComparer;
            if (comparer == null)
                comparer = Comparer<K>.Default;

            data.Add(new Node(default(K), default(D), 0));
        }

        public IHeapifyable<K, D> GetMin()
        {
            if (Count == 0)
                throw new InvalidOperationException("The heap is empty.");
            
            return data[1];
        }

        public IHeapifyable<K, D> Insert(K key, D data)
        {
            Count++;
            Node node = new Node(key, data, Count);
            this.data.Add(node);
            UpHeap(Count);
            return node;
        }

        private void UpHeap(int startIndex)
        {
            int position = startIndex;
            while (position != 1)
            {
                if (comparer.Compare(data[position].Key, data[position / 2].Key) < 0)
                    Swap(position, position / 2);
                
                position = position / 2;
            }
        }

        private void Swap(int from, int to)
        {
            Node temp = data[from];
            data[from] = data[to];
            data[to] = temp;
            data[to].Position = to;
            data[from].Position = from;
        }

        public void Clear()
        {
            for (int i = 0; i <= Count; i++)
                data[i].Position = -1;
            
            data.Clear();
            data.Add(new Node(default(K), default(D), 0));
            Count = 0;
        }

        public override string ToString()
        {
            if (Count == 0)
                return "[]";
            
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            s.Append("[");
            
            for (int i = 0; i < Count; i++)
            {
                s.Append(data[i + 1]);
                if (i + 1 < Count)
                    s.Append(",");
            }
            
            s.Append("]");
            return s.ToString();
        }

        public IHeapifyable<K, D> Delete()
        {
            if (Count == 0)
                throw new InvalidOperationException();
            
            Node result = data[1];
            Swap(1, Count);
            data.RemoveAt(Count);
            Count--;
            DownHeap(1);
            result.Position = -1;
            return result;
        }

        private int Left(int start) => start * 2;
        private int Right(int start) => start * 2 + 1;

        private void DownHeap(int start)
        {
            int leftChild = Left(start);
            
            if (leftChild > Count)
                return;
            
            int rightChild = Right(start);
            int position = leftChild;

            if (rightChild <= Count && comparer.Compare(data[leftChild].Key, data[rightChild].Key) >= 0)
                position = rightChild;

            if (comparer.Compare(data[start].Key, data[position].Key) < 0)
                return;

            Swap(start, position);
            DownHeap(position);
        }

        public IHeapifyable<K, D>[] BuildHeap(K[] keys, D[] data)
        {
            if (Count != 0)
                throw new InvalidOperationException();
            
            if (keys.Length != data.Length)
                throw new InvalidOperationException();

            Node[] result = new Node[keys.Length];

            for (int i = 0; i < keys.Length; i++)
            {
                Node node = new Node(keys[i], data[i], ++Count);
                this.data.Add(node);
                result[i] = node;
            }
            
            Heapify();

            return result;
        }

        private void Heapify()
        {
            for (int i = Count / 2; i > 0; i--)
                DownHeap(i);
        }

        public void DecreaseKey(IHeapifyable<K, D> element, K newKey)
        {
            Node result = element as Node;
            
            if (!result.Equals(data[result.Position]))
                throw new InvalidOperationException();
            
            result.Key = newKey;
            UpHeap(result.Position);
        }

        public IHeapifyable<K, D> DeleteElement(IHeapifyable<K, D> element)
        {
            if (element is null)
                throw new ArgumentNullException();

            Node node = element as Node;
            
            if (!data[node.Position].Key.Equals(element.Key) ||
                !data[node.Position].Data.Equals(element.Data))
                throw new InvalidOperationException();

            int position = node.Position;

            Swap(position, Count);
            data.RemoveAt(Count);
            Count--;
            DownHeap(position);

            return node;
        }

        public IHeapifyable<K, D> KthMinElement(int k)
        {
            if (Count == 0)
                throw new InvalidOperationException();
            
            if (k <= 0 || k > Count)
                throw new ArgumentOutOfRangeException();

            IHeapifyable<K, D> kthMin = null;
            List<IHeapifyable<K, D>> temp = new List<IHeapifyable<K, D>>();

            for (int i = 1; i <= k; i++)
            {
                if (i == k)
                    kthMin = data[1];
                
                temp.Add(this.Delete());
            }

            foreach (var node in temp)
                this.Insert(node.Key, node.Data);

            return kthMin;
        }
    }
}
