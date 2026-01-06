using System.Linq;
using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Core;
using Color = Microsoft.Maui.Graphics.Color;

namespace Lummo.Mobile.Controls;

public class CustomStatusBarBehavior : Behavior<Page>
{
    public static readonly BindableProperty StatusBarColorProperty =
        BindableProperty.CreateAttached(
            "StatusBarColor",
            typeof(Color),
            typeof(CustomStatusBarBehavior),
            Colors.Transparent,
            propertyChanged: OnStatusBarPropertyChanged);

    public static readonly BindableProperty StatusBarStyleProperty =
        BindableProperty.CreateAttached(
            "StatusBarStyle",
            typeof(StatusBarStyle),
            typeof(CustomStatusBarBehavior),
            StatusBarStyle.Default,
            propertyChanged: OnStatusBarPropertyChanged);

    public static Color GetStatusBarColor(BindableObject view)
    {
        return (Color)view.GetValue(StatusBarColorProperty);
    }

    public static void SetStatusBarColor(BindableObject view, Color value)
    {
        view.SetValue(StatusBarColorProperty, value);
    }

    public static StatusBarStyle GetStatusBarStyle(BindableObject view)
    {
        return (StatusBarStyle)view.GetValue(StatusBarStyleProperty);
    }

    public static void SetStatusBarStyle(BindableObject view, StatusBarStyle value)
    {
        view.SetValue(StatusBarStyleProperty, value);
    }

    private static void OnStatusBarPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not Page page)
            return;

        // Get both current property values from the page
        var color = GetStatusBarColor(page);
        var style = GetStatusBarStyle(page);

        // Remove existing StatusBarBehavior if present
        var existingBehavior = page.Behaviors.OfType<StatusBarBehavior>().FirstOrDefault();
        if (existingBehavior != null)
        {
            page.Behaviors.Remove(existingBehavior);
        }

        // Add new StatusBarBehavior with current values from the page
        page.Behaviors.Add(new StatusBarBehavior
        {
            StatusBarColor = color,
            StatusBarStyle = style
        });
    }
}
