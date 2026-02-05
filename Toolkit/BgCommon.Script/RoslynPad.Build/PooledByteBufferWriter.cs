using System.Buffers;
using System.Diagnostics;
using System.IO;

namespace RoslynPad.Build;

internal sealed class PooledByteBufferWriter : IBufferWriter<byte>, IDisposable
{
    private const int MinimumBufferSize = 256;
    private byte[] rentedBuffer;
    private int index;

    public PooledByteBufferWriter(int initialCapacity = MinimumBufferSize)
    {
        Debug.Assert(initialCapacity > 0);

        rentedBuffer = ArrayPool<byte>.Shared.Rent(initialCapacity);
        index = 0;
    }

    public ReadOnlyMemory<byte> WrittenMemory
    {
        get
        {
            Debug.Assert(rentedBuffer != null);
            Debug.Assert(index <= rentedBuffer.Length);
            return rentedBuffer.AsMemory(0, index);
        }
    }

    public int WrittenCount
    {
        get
        {
            Debug.Assert(rentedBuffer != null);
            return index;
        }
    }

    public int Capacity
    {
        get
        {
            Debug.Assert(rentedBuffer != null);
            return rentedBuffer.Length;
        }
    }

    public int FreeCapacity
    {
        get
        {
            Debug.Assert(rentedBuffer != null);
            return rentedBuffer.Length - index;
        }
    }

    public void Reset() => index = 0;

    public void Clear() => ClearHelper();

    private void ClearHelper()
    {
        Debug.Assert(rentedBuffer != null);
        Debug.Assert(index <= rentedBuffer.Length);

        rentedBuffer.AsSpan(0, index).Clear();
        index = 0;
    }

    // Returns the rented buffer back to the pool
    public void Dispose()
    {
        if (rentedBuffer == null)
        {
            return;
        }

        ClearHelper();
        byte[] toReturn = rentedBuffer;
        rentedBuffer = null!;
        ArrayPool<byte>.Shared.Return(toReturn);
    }

    public void Advance(int count)
    {
        Debug.Assert(rentedBuffer != null);
        Debug.Assert(count >= 0);
        Debug.Assert(index <= rentedBuffer.Length - count);

        index += count;
    }

    public Memory<byte> GetMemory(int sizeHint = 0)
    {
        CheckAndResizeBuffer(sizeHint);
        return rentedBuffer.AsMemory(index);
    }

    public Span<byte> GetSpan(int sizeHint = 0)
    {
        CheckAndResizeBuffer(sizeHint);
        return rentedBuffer.AsSpan(index);
    }

    internal ValueTask WriteToStreamAsync(Stream destination, CancellationToken cancellationToken) =>
        destination.WriteAsync(WrittenMemory, cancellationToken);

    internal void WriteToStream(Stream destination) => destination.Write(WrittenMemory.Span);

    private void CheckAndResizeBuffer(int sizeHint)
    {
        Debug.Assert(rentedBuffer != null);
        Debug.Assert(sizeHint >= 0);

        if (sizeHint == 0)
        {
            sizeHint = MinimumBufferSize;
        }

        int availableSpace = rentedBuffer.Length - index;

        if (sizeHint > availableSpace)
        {
            int currentLength = rentedBuffer.Length;
            int growBy = Math.Max(sizeHint, currentLength);

            int newSize = currentLength + growBy;

            if ((uint)newSize > int.MaxValue)
            {
                newSize = currentLength + sizeHint;
                if ((uint)newSize > int.MaxValue)
                {
                    throw new InsufficientMemoryException("BufferMaximumSizeExceeded: " + (uint)newSize);
                }
            }

            byte[] oldBuffer = rentedBuffer;

            rentedBuffer = ArrayPool<byte>.Shared.Rent(newSize);

            Debug.Assert(oldBuffer.Length >= index);
            Debug.Assert(rentedBuffer.Length >= index);

            Span<byte> previousBuffer = oldBuffer.AsSpan(0, index);
            previousBuffer.CopyTo(rentedBuffer);
            previousBuffer.Clear();
            ArrayPool<byte>.Shared.Return(oldBuffer);
        }

        Debug.Assert(rentedBuffer.Length - index > 0);
        Debug.Assert(rentedBuffer.Length - index >= sizeHint);
    }
}
