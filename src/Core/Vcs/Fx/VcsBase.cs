namespace Core.Vcs.Fx
{
    public abstract class VcsBase
    {
    }

    public enum VcsAction
    {
        Pull,
    }

    public delegate void OnStatus(VcsAction action, string status);
}
