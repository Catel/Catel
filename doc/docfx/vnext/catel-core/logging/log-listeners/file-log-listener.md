---
title: "FileLogListener" 
---
Catel also supports very lightweight listeners to allow external logging libraries to hook on. To create a listener, first create a new class that implements the *ILogListener* interface. Next, register it in the *LogManager* using the *LogManager.AddListener* method.

The ILogListener has a separate method for each LogEvent, but also has a shared method that is called for each log event. For example, if a debug message is written to the log, both the Write and Debug methods are invoked on the ILogListener.

For an example which writes to disk in batches, see the [batch log event listeners](Batch_log_listeners)

Note that Catel already contains a FileLogListener and there is no need to reproduce this class. It only acts as an example that is easy to understand

## Specifying the path

The defaultÂ *FileLogListener* that ships with Catel allows the customization of the file name and the size of the log file. If no size is specified, the max log file size will default to 10 MB.Â 

There are several constants inside the file name that can be used:

*{AppData}*

The application data directoryÂ that is used by all users.

Based on the entry assembly. For example `%ProgramData%\[company]\[product]\.`

**Note: Currently this writes to `%ProgramData%` only if HttpContext.Current evaluates to an object (e.g. a web app); otherwise it uses %AppData%*.
*

*{AppDataLocal}*

The local application data directory that is used by the current, non-roaming user.

Based on the entry assembly. For example `%LocalAppData%\[company]\product]\`.

*{AppDataRoaming}*

The roaming application data directory that is used by the current roaming user.

Based on the entry assembly. For example `%AppData%\[company]\[product]\`.

*{AppDataMachine}*

The application data directoryÂ that is used by all users.

Based on the entry assembly. For example `%ProgramData%\[company]\[product]\`.

*{AppDir}*

The directory of the current application (i.e. `AppDomain.CurrentDomain.BaseDirectory`).

*{AssemblyCompany}*

The assembly company.

*{AssemblyName}*

The assembly name.

*{AssemblyProduct}*

The assembly product.

*{AutoLogFileName}*

Creates a log file with the following format: `{`*`AssemblyName}_{Date}_{Time}_{ProcessId}`.*

*{Date}*

The date as *`yyyy-MM-dd`.*

*{Time}*

The time as *`HHmmss`.*

*{ProcessId}*

The process id.

*{WorkDir}*

The application execution directory.Â  Calls `Directory.GetCurrentDirectory()`, internally.

A good example would be:

```
var fileLogListener = new FileLogListener();
fileLogListener.FilePath = "{AppDir}\{AutoLogFileName}";
Â 
LogManager.AddListener(fileLogListener);
```

Note that the default path of the *FileLogListener* is *{AppData}\\{AutoLogFileName}*

## Creating a custom listener

A listener can be created by creating a new class deriving from LogListenerBase.

```
public class FileLogListener : LogListenerBase
{
    private readonly TextWriter _textWriter;

    public FileLogListener(string fileName)
    {
        Argument.IsNotNullOrWhitespace("fileName", fileName);
        FileName = fileName;

        _textWriter = new StreamWriter(fileName, true);
    }

    public string FileName { get; private set; }

    public override void Write(ILog log, string message, LogEvent logEvent)
    {
        _textWriter.WriteLine(message);
    }
}
```

## Registering the listener

Â Last but not least, it is important to register the listener:

```
LogManager.AddListener(new FileLogListener("<log_file_path>"));
```

