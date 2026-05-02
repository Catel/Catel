---
title: "Batch log listeners" 
---
A batch log listener is a class implementing the *IBatchLogListener* interface (and most probably deriving from *BatchLogListenerBase*). This interface adds a *Flush* method which allows a listener to be flushed. The advantage is that when a log listener writes to a slower persistence store, it will not have to access this expensive resource for every log event, but by batches.

## Flushing all listeners

When using batch log listeners, it is very important to flush the log listeners at important events such as application unhandled exceptions or when the application exits. The reason is that otherwise important log events that are currently in the batch that hasn't been written to the persistence store are lost.

To flush all flushable listeners, use the following method:

```
LogManager.FlushAll();
```

## Implementing a custom IBatchLogListener

When implementing a custom batch log listener, it is very wise to derive from the *BatchLogListenerBase* class. This brings the following advantages:

1.  The *BatchLogListenerBase* is thread-safe
2.  The *BatchLogListenerBase* automatically flushes the listener every 5 seconds
3.  You only need to implement the *WriteBatch* which actually writes the entries to the persistence store

Below is an example batch log listener:

```
public class FileLogListener : BatchLogListenerBase
{
    private readonly string _filePath;
    private readonly int _maxSizeInKiloBytes;
    public FileLogListener(string filePath, int maxSizeInKiloBytes)
    {
        Argument.IsNotNullOrWhitespace(() => filePath);
        _filePath = filePath;
        _maxSizeInKiloBytes = maxSizeInKiloBytes;
    }
    protected override void WriteBatch(System.Collections.Generic.List<LogBatchEntry> batchEntries)
    {
        try
        {
            var fileInfo = new FileInfo(_filePath);
            if (fileInfo.Exists && (fileInfo.Length / 1024 >= _maxSizeInKiloBytes))
            {
                CreateCopyOfCurrentLogFile(_filePath);
            }
            using (var fileStream = new FileStream(_filePath, FileMode.Append, FileAccess.Write, FileShare.Read))
            {
                using (var writer = new StreamWriter(fileStream))
                {
                    foreach (var batchEntry in batchEntries)
                    {
                        var message = FormatLogEvent(batchEntry.Log, batchEntry.Message, batchEntry.LogEvent, batchEntry.ExtraData);
                        writer.WriteLine(message);
                    }
                }
            }
        }
        catch (Exception)
        {
            // Swallow
        }
    }
    private void CreateCopyOfCurrentLogFile(string filePath)
    {
        for (int i = 1; i < 999; i++)
        {
            var possibleFilePath = string.Format("{0}.{1:000}", filePath, i);
            if (!File.Exists(possibleFilePath))
            {
                File.Move(filePath, possibleFilePath);
            }
        }
    }
}
```

