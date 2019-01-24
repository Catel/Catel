// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Binding.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if XAMARIN

namespace Catel.MVVM
{
    using System;
    using System.Globalization;
    using Converters;
    using Logging;

    /// <summary>
    /// Binding class for platforms not supporting bindings.
    /// </summary>
    public class Binding : BindingBase
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private BindingParty _source;
        private BindingParty _target;

        private bool _isUpdatingBinding;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Binding"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="sourcePropertyName">Name of the source property.</param>
        /// <param name="target">The target.</param>
        /// <param name="targetPropertyName">Name of the target property.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="converter">The converter.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="target"/> is <c>null</c>.</exception>
        public Binding(object source, string sourcePropertyName, object target, string targetPropertyName, BindingMode mode = BindingMode.TwoWay,
            IValueConverter converter = null)
            : this(new BindingParty(source, sourcePropertyName), new BindingParty(target, targetPropertyName), mode, converter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Binding"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="converter">The converter.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="target"/> is <c>null</c>.</exception>
        public Binding(BindingParty source, BindingParty target, BindingMode mode = BindingMode.TwoWay, IValueConverter converter = null)
        {
            Argument.IsNotNull("source", source);
            Argument.IsNotNull("target", target);

            Mode = mode;
            Converter = converter;

            _source = source;
            _target = target;

            Initialize();
        }

        #region Properties
        /// <summary>
        /// Gets or sets the converter.
        /// </summary>
        /// <value>The converter.</value>
        public IValueConverter Converter { get; set; }

        /// <summary>
        /// Gets or sets the converter parameter.
        /// </summary>
        /// <value>The converter parameter.</value>
        public object ConverterParameter { get; set; }

        /// <summary>
        /// Gets the binding mode.
        /// </summary>
        /// <value>The mode.</value>
        public BindingMode Mode { get; private set; }

        /// <summary>
        /// Gets the binding source.
        /// </summary>
        /// <value>The source.</value>
        public BindingParty Source
        {
            get { return _source; }
        }

        /// <summary>
        /// Gets the binding target.
        /// </summary>
        /// <value>The target.</value>
        public BindingParty Target
        {
            get { return _target; }
        }

        /// <summary>
        /// Gets the value of the binding source.
        /// </summary>
        /// <value>The value.</value>
        public object Value
        {
            get { return (_source != null) ? _source.GetPropertyValue() : null; }
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the value of the binding has changed.
        /// </summary>
        public event EventHandler<EventArgs> ValueChanged;
        #endregion

        #region Methods
        /// <summary>
        /// Determines the value to use in the <see cref="BindingBase.ToString"/> method.
        /// </summary>
        /// <returns>The string to use.</returns>
        protected override string DetermineToString()
        {
            return string.Format("{0} => {1}", _source, _target);
        }

        private void Initialize()
        {
            _source.ValueChanged += OnSourceValueChanged;
            _target.ValueChanged += OnTargetValueChanged;

            Log.Debug("Initialized binding '{0}'", this);

            // First time, initialize binding
            if (Mode != BindingMode.OneWayToSource)
            {
                UpdateBinding(_source, _target, false);
            }
        }

        /// <summary>
        /// Uninitializes this binding.
        /// </summary>
        protected override void Uninitialize()
        {
            _source.ValueChanged -= OnSourceValueChanged;
            _source.Dispose();
            _source = null;

            _target.ValueChanged -= OnTargetValueChanged;
            _target.Dispose();
            _target = null;

            Log.Debug("Uninitialized binding '{0}'", this);
        }

        private void OnSourceValueChanged(object sender, EventArgs e)
        {
            TransferValueFromSourceToTarget();
        }

        private void OnTargetValueChanged(object sender, EventArgs e)
        {
            TransferValueFromTargetToSource();
        }

        private void UpdateBinding(BindingParty source, BindingParty target, bool useConvertBack)
        {
            if (_isUpdatingBinding)
            {
                return;
            }

            if (!EnsureBindingLifetime())
            {
                Uninitialize();
                return;
            }

            try
            {
                _isUpdatingBinding = true;

                Log.Debug("Updating binding '{0}' => '{1}'", source, target);

                var newValue = source.GetPropertyValue();

                var converter = Converter;
                if (converter != null)
                {
                    if (useConvertBack)
                    {
                        newValue = converter.ConvertBack(newValue, typeof (object), ConverterParameter, CultureInfo.CurrentCulture);
                    }
                    else
                    {
                        newValue = converter.Convert(newValue, typeof(object), ConverterParameter, CultureInfo.CurrentCulture);
                    }
                }

                if (ReferenceEquals(newValue, ConverterHelper.UnsetValue))
                {
                    Log.Debug("Skipping update because new value is 'ConverterHelper.UnsetValue'");
                    return;
                }

                target.SetPropertyValue(newValue);

                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to update binding");
            }
            finally
            {
                _isUpdatingBinding = false;
            }
        }

        /// <summary>
        /// Ensures the binding lifetime by checking if both the source and target are still alive.
        /// </summary>
        /// <returns><c>true</c> if the binding is still valid; otherwise, <c>false</c>.</returns>
        private bool EnsureBindingLifetime()
        {
            if (_source is null || _source.Instance is null)
            {
                return false;
            }

            if (_target is null || _target.Instance is null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Transfers the value from the source to target.
        /// <para />
        /// Note that this method will check the <see cref="BindingMode"/> to see if the transfer is possible.
        /// </summary>
        public void TransferValueFromSourceToTarget()
        {
            if (Mode == BindingMode.OneWay || Mode == BindingMode.TwoWay)
            {
                UpdateBinding(_source, _target, false);
            }
        }

        /// <summary>
        /// Transfers the value from the target to source.
        /// <para />
        /// Note that this method will check the <see cref="BindingMode"/> to see if the transfer is possible.
        /// </summary>
        public void TransferValueFromTargetToSource()
        {
            if (Mode == BindingMode.OneWayToSource || Mode == BindingMode.TwoWay)
            {
                UpdateBinding(_target, _source, true);
            }
        }
        #endregion
    }
}

#endif
