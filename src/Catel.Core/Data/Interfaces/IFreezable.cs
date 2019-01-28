namespace Catel.Data
{
    public interface IFreezable
    {
        bool IsFrozen { get; }

        void Freeze();

        void Unfreeze();
    }
}
