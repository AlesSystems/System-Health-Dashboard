namespace SystemHealthDashboard.Metrics.Schedulers;

public class RingBuffer<T>
{
    private readonly T[] _buffer;
    private int _head;
    private int _count;
    private readonly int _capacity;
    private readonly object _lock = new object();

    public RingBuffer(int capacity)
    {
        if (capacity <= 0)
        {
            throw new ArgumentException("Capacity must be greater than zero.", nameof(capacity));
        }

        _capacity = capacity;
        _buffer = new T[capacity];
        _head = 0;
        _count = 0;
    }

    public void Add(T item)
    {
        lock (_lock)
        {
            _buffer[_head] = item;
            _head = (_head + 1) % _capacity;

            if (_count < _capacity)
            {
                _count++;
            }
        }
    }

    public IReadOnlyList<T> GetAll()
    {
        lock (_lock)
        {
            var result = new List<T>(_count);
            
            if (_count == 0)
            {
                return result;
            }

            int startIndex = _count < _capacity ? 0 : _head;
            
            for (int i = 0; i < _count; i++)
            {
                int index = (startIndex + i) % _capacity;
                result.Add(_buffer[index]);
            }

            return result;
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            Array.Clear(_buffer, 0, _capacity);
            _head = 0;
            _count = 0;
        }
    }

    public int Count
    {
        get
        {
            lock (_lock)
            {
                return _count;
            }
        }
    }

    public int Capacity => _capacity;
}
