using System;

namespace VolumeKsharp;

public class PercentageApperanceCommand : IApperanceCommand
{
    public string? Message { get; }

    public PercentageApperanceCommand(int percentage)
    {
        if (percentage < 0 || percentage > 100)
        {
            throw new ArgumentException("Percentage must be between 0 and 100.");
        }
        Message = $"p{percentage}";
    }

}