namespace VolumeKsharp;

public interface IMode
{
    public bool IncomingCommands(InputCommands command);
    public void Compute();
}