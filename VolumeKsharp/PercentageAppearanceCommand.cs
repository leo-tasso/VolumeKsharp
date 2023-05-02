using System;

namespace VolumeKsharp;

public class PercentageAppearanceCommand : IAppearanceCommand
{
    public string? Message { get; }

    public PercentageAppearanceCommand(int percentage)
    {
        if (percentage < 0 || percentage > 100)
        {
            throw new ArgumentException("Percentage must be between 0 and 100.");
        }
        Message = $"p{percentage}";
    }

}