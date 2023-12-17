using Microsoft.Win32;
using System.Diagnostics;

namespace DesktopFileHider;
static class Program
{
    static RegistryKey HiddenFilesRegistryKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", true);
    static RegistryKey DesktopFilesLocationRegistryKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\Shell\Bags\1\Desktop", true);
    static string IconLayouts = "IconLayouts";

    static void Main()
    {
        object DesktopFilesLocationValue;

        while(true)
        {
            CreateButton("-| Press ENTER to HIDE files |");

            DesktopFilesLocationValue = ReadRegistryValue(DesktopFilesLocationRegistryKey, IconLayouts);

            HideHiddenFiles();
            RestartExplorer();

            CreateButton("-| Press ENTER to SHOW files |");

            WriteRegistryValue(DesktopFilesLocationRegistryKey, IconLayouts, DesktopFilesLocationValue);

            ShowHiddenFiles();
            RestartExplorer();
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
        Console.Write("Reading value from a register ... ");

        try
        {   object value = DesktopFilesLocationRegistryKey.GetValue(ValueName);

            Console.WriteLine("successfuly");

            return value;
        }
        catch(Exception ex)
        {
            Console.WriteLine("error: " + ex.Message);
        }

        return null;
    }

    static void WriteRegistryValue(RegistryKey Key, string ValueName, object Value)
    {
        Console.Write("Writing a value to a register ... ");
        try
        {
            Key.SetValue(ValueName, Value, RegistryValueKind.Binary);

            Console.WriteLine("successfuly");
        }
        catch(Exception ex)
        {
            Console.WriteLine("error: " + ex.Message);
        }
    }

    static void HideHiddenFiles()
    {
        Console.Write("Hide hidden files ... ");

        try
        {
            HiddenFilesRegistryKey.SetValue("Hidden", 2, RegistryValueKind.DWord);

            Console.WriteLine("successfully");
        }
        catch(Exception ex)
        {
            Console.WriteLine("error: " + ex.Message);
        }
    }

    static void ShowHiddenFiles()
    {
        Console.Write("Show hidden files ... ");

        try
        {
            HiddenFilesRegistryKey.SetValue("Hidden", 1, RegistryValueKind.DWord);

            Console.WriteLine("successfully");
        }
        catch(Exception ex)
        {
            Console.WriteLine("error: " + ex.Message);
        }
    }

    static void RestartExplorer()
    {
        Console.Write("Restart explorer.exe ... ");
        try
        {
            foreach(var process in Process.GetProcessesByName("explorer"))
            {
                process.Kill();
            }

            Thread.Sleep(100);

            Process.Start("explorer.exe");

            Console.WriteLine("successfully");
        }
        catch(Exception ex)
        {
            Console.WriteLine("error: " + ex.Message);
        }
    }
}