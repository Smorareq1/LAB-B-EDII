namespace LAB_2_EDII;

public class BitWriter : IDisposable
{
    private MemoryStream _memoryStream;
    private byte _currentByte;
    private int _bitPosition;
    private bool _disposed = false;

    public BitWriter()
    {
        _memoryStream = new MemoryStream();
        _currentByte = 0;
        _bitPosition = 7;
    }

    // Write a single bit to the MemoryStream
    public void WriteBit(bool bit)
    {
        if (bit)
        {
            _currentByte |= (byte)(1 << _bitPosition);
        }
        _bitPosition--;

        // If we have filled a byte, write it to the MemoryStream
        if (_bitPosition < 0)
        {
            _memoryStream.WriteByte(_currentByte);
            _currentByte = 0;
            _bitPosition = 7;
        }
    }

    // Flush any remaining bits and return the MemoryStream
    public MemoryStream Flush()
    {
        if (_bitPosition < 7)
        {
            // Write the remaining bits as a byte
            _memoryStream.WriteByte(_currentByte);
        }
        return _memoryStream;
    }

    // Dispose of the MemoryStream
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _memoryStream.Dispose();
            }
            _disposed = true;
        }
    }
}