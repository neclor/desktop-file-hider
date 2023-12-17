using Microsoft.Win32;
using System.Diagnostics;

namespace DesktopFileHider;
static class Program
{
    static RegistryKey HiddenFilesRegistryKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", true);
    static RegistryKey DesktopFilesLocationRegistryKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\Shell\Bags\1\Desktop", true);
    static string IconLayouts = "IconLayouts";
    static string savedLayoutPath = Path.Combine(Path.GetTempPath(), "savedLayout.dat");

    static void Main()
    {
        try {
            byte[] iconLayouts;

            while(true)
            {
                CreateButton("-| Press ENTER to HIDE files |");

                iconLayouts = (byte[])ReadRegistryValue(DesktopFilesLocationRegistryKey, IconLayouts);
                File.WriteAllBytes(savedLayoutPath, iconLayouts);

                HideHiddenFiles();
                RestartExplorer();

                CreateButton("-| Press ENTER to SHOW files |");

                WriteRegistryValue(DesktopFilesLocationRegistryKey, IconLayouts, iconLayouts);

                ShowHiddenFiles();
                RestartExplorer();
            }
        } catch (Exception ex)
        {
            Console.Error.WriteLine("error: " + ex.Message);
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
        Console.Write("Reading value from Registry ... ");
        return DesktopFilesLocationRegistryKey.GetValue(ValueName);
    }

    static void WriteRegistryValue(RegistryKey Key, string ValueName, object Value)
    {
        Console.Write("Writing a value to Registry ... ");
        Key.SetValue(ValueName, Value, RegistryValueKind.Binary);
    }

    static void HideHiddenFiles()
    {
        Console.Write("Hide hidden files ... ");

        HiddenFilesRegistryKey.SetValue("Hidden", 2, RegistryValueKind.DWord);
    }

    static void ShowHiddenFiles()
    {
        Console.Write("Show hidden files ... ");
        HiddenFilesRegistryKey.SetValue("Hidden", 1, RegistryValueKind.DWord);
    }

    static void RestartExplorer()
    {
        Console.Write("Restart explorer.exe ... ");
        foreach(var process in Process.GetProcessesByName("explorer"))
        {
            process.Kill();
        }

        Thread.Sleep(100);

        Process.Start("explorer.exe");

        Console.WriteLine("successfully");
    }
}