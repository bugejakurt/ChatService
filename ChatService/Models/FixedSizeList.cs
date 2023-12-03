namespace ChatService.Models
{
    public class FixedSizeList<T> : List<T>
    {
        public int Size { get; private set; }

        public FixedSizeList(int size)
        {
            Size = size;
        }

        public new void Add(T obj)
        {
            if (!TryAdd(obj))
                throw new Exception("List is full");
        }

        public bool TryAdd(T obj)
        {
            if (Count >= Size)
                return false;

            base.Add(obj);
            return true;
        }
    }
}
