#pragma warning disable 1591 // 1591 = missing xml

using System;
using System.ComponentModel;

namespace Ricciolo.StylesExplorer.MarkupReflection
{
    public class WpfDependencyPropertyDescriptor : MarshalByRefObject, IDependencyPropertyDescriptor
    {
        private readonly DependencyPropertyDescriptor _propertyDescriptor;

        public WpfDependencyPropertyDescriptor(DependencyPropertyDescriptor propertyDescriptor)
        {
            if (propertyDescriptor == null) throw new ArgumentNullException("propertyDescriptor");
            _propertyDescriptor = propertyDescriptor;
        }

        #region IDependencyPropertyDescriptor Members

        public bool IsAttached
        {
            get { return _propertyDescriptor.IsAttached; }
        }

        #endregion

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
