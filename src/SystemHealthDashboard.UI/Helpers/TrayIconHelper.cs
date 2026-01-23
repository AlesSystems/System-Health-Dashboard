using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using SystemHealthDashboard.Core.Models;

namespace SystemHealthDashboard.UI.Helpers;

public static class TrayIconHelper
{
    public static Icon CreateTrayIcon(AlertSeverity severity)
    {
        var size = 16;
        using var bitmap = new Bitmap(size, size);
        using var graphics = Graphics.FromImage(bitmap);
        
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.Clear(Color.Transparent);

        var color = severity switch
        {
            AlertSeverity.Warning => Color.Orange,
            AlertSeverity.Critical => Color.Red,
            _ => Color.Green
        };

        using var brush = new SolidBrush(color);
        graphics.FillEllipse(brush, 2, 2, size - 4, size - 4);

        using var borderPen = new Pen(Color.White, 1);
        graphics.DrawEllipse(borderPen, 2, 2, size - 4, size - 4);

        var iconHandle = bitmap.GetHicon();
        var icon = Icon.FromHandle(iconHandle);
        
        return icon;
    }
}
