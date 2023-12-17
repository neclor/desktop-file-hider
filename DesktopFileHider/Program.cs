using Microsoft.Win32;
using System.Diagnostics;

namespace DesktopFileHider;
static class Program
{
    static RegistryKey HiddenFilesRegistryKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", true);
    static RegistryKey DesktopFilesLocationRegistryKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\Shell\Bags\1\Desktop", true);
    static string IconLayouts = "IconLayouts";

    static string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    static string savedLayoutPath = Path.Combine(Path.GetTempPath(), "savedLayout.dat");

    static void Main()
    {
        try 
        {
            byte[] IconLayoutsValue;

            while(true)
            {
                CreateButton("-| Press ENTER to HIDE files |");

                IconLayoutsValue = (byte[])ReadRegistryValue(DesktopFilesLocationRegistryKey, IconLayouts);
                File.WriteAllBytes(savedLayoutPath, IconLayoutsValue);

                HideHiddenFiles();
                HideDesktopFiles();
                RestartExplorer();

                CreateButton("-| Press ENTER to SHOW files |");

                ShowDesktopFiles();
                ShowHiddenFiles();

                WriteRegistryValue(DesktopFilesLocationRegistryKey, IconLayouts, IconLayoutsValue);

                RestartExplorer();
            }
        } 
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
        }
    }

    static void CreateButton(string ButtonText)
    {
        Console.WriteLine("");

        int CursorCoordY = Console.CursorTop;

        Console.WriteLine(ButtonText);

        Console.SetCursorPosition(0, CursorCoordY);

        while(Console.ReadKey().Key != ConsoleKey.Enter) { }

        Console.WriteLine("");
    }

    static object ReadRegistryValue(RegistryKey Key, string ValueName)
    {
        Console.Write("Reading value from Registry");
        return DesktopFilesLocationRegistryKey.GetValue(ValueName);
    }

    static void WriteRegistryValue(RegistryKey Key, string ValueName, object Value)
    {
        Console.WriteLine("Writing a value to Registry");
        Key.SetValue(ValueName, Value, RegistryValueKind.Binary);
    }

    static void HideDesktopFiles()
    {
        Console.WriteLine("Hide desktop files");

        string[] Files = Directory.GetFiles(DesktopPath);

        foreach(string FilePath in Files)
        {
            try
            {
                File.SetAttributes(FilePath, FileAttributes.Hidden);
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }
    }

    static void ShowDesktopFiles()
    {
        Console.WriteLine("Show desktop files");

        string[] Files = Directory.GetFiles(DesktopPath);

        foreach(string FilePath in Files)
        {
            try
            {
                File.SetAttributes(FilePath, FileAttributes.Normal);
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }
    }

    static void HideHiddenFiles()
    {
        Console.WriteLine("Hide hidden files");
        HiddenFilesRegistryKey.SetValue("Hidden", 2, RegistryValueKind.DWord);
    }

    static void ShowHiddenFiles()
    {
        Console.WriteLine("Show hidden files");
        HiddenFilesRegistryKey.SetValue("Hidden", 1, RegistryValueKind.DWord);
    }

    static void RestartExplorer()
    {
        Console.Write("Restart explorer.exe");
        foreach(var process in Process.GetProcessesByName("explorer"))
        {
            process.Kill();
        }

        Thread.Sleep(100);

        Process.Start("explorer.exe", "/root");
    }
}