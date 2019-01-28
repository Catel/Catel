namespace Catel.Data
{
    public partial class ModelBase : IFreezable
    {
        private bool _isFrozen;

        bool IFreezable.IsFrozen
        {
            get
            {
                return _isFrozen;
            }
        }

        void IFreezable.Freeze()
        {
            _isFrozen = true;
        }

        void IFreezable.Unfreeze()
        {
            _isFrozen = false;
        }
    }
}
