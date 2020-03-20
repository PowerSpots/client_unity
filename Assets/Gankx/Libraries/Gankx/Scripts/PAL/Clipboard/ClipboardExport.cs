using Gankx;
using Gankx.PAL;

public class ClipboardExport
{
    public static void SetClipboard(string content)
    {
        UniClipboard.SetText(content);
    }

    public static string GetClipboard()
    {
        return UniClipboard.GetText();
    }
}