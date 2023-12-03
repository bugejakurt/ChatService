using System.Collections.Concurrent;

namespace ChatService.Models
{
    public class FixedSizedQueue<T> : ConcurrentQueue<T>
    {
        private readonly object lockObj = new object();

        public int Size { get; private set; }

        public FixedSizedQueue(int size)
        {
            Size = size;
        }

        public new void Enqueue(T obj)
        {
            if (!TryEnqueue(obj))
                throw new Exception("Queue is full");
        }

        public bool TryEnqueue(T obj)
        {
            lock (lockObj)
            {
                if (Count >= Size)
                    return false;

                base.Enqueue(obj);
                return true;
            }
        }

        public IEnumerable<T> Dequeue(int maxItems)
        {
            for (int i = 0; i < maxItems; i++)
            {
                if (TryDequeue(out T? obj))
                    yield return obj;
            }
        }

        public void Resize(int size)
        {
            lock (lockObj)
            {
                if (Count > size)
                    throw new Exception($"Cannot be resized less than {Count}");

                Size = size;
            }
        }
    }
}
