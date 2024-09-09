using System.Collections.Concurrent;

namespace Backend.Helpers;

public class SemaphoreManager
{
    private readonly SemaphoreSlim _semaphore;
    private readonly Dictionary<string, DateTime> _semaphoreOwners;
    private readonly ConcurrentQueue<string> _queue;

    public SemaphoreManager(int maxCount)
    {
        _semaphore = new SemaphoreSlim(maxCount);
        _semaphoreOwners = new Dictionary<string, DateTime>();
        _queue = new ConcurrentQueue<string>();
    }

    public void EnqueuSession(string sessionId) => _queue.Enqueue(sessionId);

    public bool TryPeekQueue(out string? sessionId) => _queue.TryPeek(out sessionId);

    public bool TryDequeue(out string? sessionId) => _queue.TryDequeue(out sessionId);

    public bool Contains(string sessionId) => _queue.Contains(sessionId);

    public async Task<bool> TryAcquireSemaphoreAsync(string sessionId)
    {
        if (await _semaphore.WaitAsync(0))
        {
            Console.WriteLine($"Client '{sessionId}' has the semaphore");
            _semaphoreOwners.Add(sessionId, DateTime.UtcNow);
            return true;
        }
        return false;
    }

    public void ReleaseSemaphore(string sessionId)
    {
        if (_semaphoreOwners.Remove(sessionId, out _))
        {
            Console.WriteLine($"Client '{sessionId}' has LOST the semaphore");
            _semaphore.Release();
        }
    }

    public void ReleaseExpiredSemaphores(TimeSpan expirationTime)
    {
        Console.WriteLine("--- Performing semaphore cleanup ---");
        var expiredSessions = _semaphoreOwners
            .Where(kv => DateTime.UtcNow - kv.Value > expirationTime)
            .Select(kv => kv.Key)
            .ToList();

        foreach (var session in expiredSessions)
        {
            Console.Write($"\tCleaning ");
            ReleaseSemaphore(session);
        }
    }

    public bool ClientHasSemaphore(string sessionId) => _semaphoreOwners.ContainsKey(sessionId);
}
