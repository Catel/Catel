// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KnownTypesSerializationFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Runtime.Serialization.XmlSerialization
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Runtime.Serialization;
    using Catel.Data;
    using Catel.Runtime.Serialization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public interface IParams
    {
    }

    // Setting the known types on a base class (not interface) solves the issue
    // but is not feasible as the possible derived classes are implemented by
    // plugins which are not known in advance. Is there a way to do this dynamically,
    // by making a request to the plugin manager?
    //[KnownType(typeof(ParamsPluginA))]
    //[KnownType(typeof(ParamsPluginB))]
    [KnownType("KnownTypes")]
    public class ParamsBase : SavableModelBase<ParamsBase>, IParams
    {
        // This method returns the array of known types.

        #region Methods
        private static Type[] KnownTypes()
        {
            return new Type[] {typeof (ParamsPluginA), typeof (ParamsPluginB)};
        }
        #endregion
    }

    public class ParamsPluginA : ParamsBase
    {
        #region Property: SettingA

        #region Constants
        public static readonly PropertyData SettingAProperty =
            RegisterProperty("SettingA", typeof (String));
        #endregion

        #region Properties
        public String SettingA
        {
            get { return GetValue<String>(SettingAProperty); }
            set { SetValue(SettingAProperty, value); }
        }
        #endregion

        #endregion
    }

    public class ParamsPluginB : ParamsBase
    {
        #region Property: SettingB

        #region Constants
        public static readonly PropertyData SettingBProperty =
            RegisterProperty("SettingB", typeof (String));
        #endregion

        #region Properties
        public String SettingB
        {
            get { return GetValue<String>(SettingBProperty); }
            set { SetValue(SettingBProperty, value); }
        }
        #endregion

        #endregion
    }

    public class Container : SavableModelBase<Container>
    {
        #region Property: Parameters

        #region Constants
        public static readonly PropertyData ParametersProperty =
            RegisterProperty("Parameters", typeof (ObservableCollection<IParams>),
                new ObservableCollection<IParams>());
        #endregion

        #region Properties
        public ObservableCollection<IParams> Parameters
        {
            get { return GetValue<ObservableCollection<IParams>>(ParametersProperty); }
            set { SetValue(ParametersProperty, value); }
        }
        #endregion

        #endregion
    }

    [TestClass]
    public class Serialization
    {
        #region Methods
        [TestMethod]
        public void Save()
        {
            var c = new Container();

            var pA = new ParamsPluginA();
            pA.SettingA = "TestA";
            c.Parameters.Add(pA);

            var pB = new ParamsPluginB();
            pB.SettingB = "TestB";
            c.Parameters.Add(pB);

            using (var memoryStream = new MemoryStream())
            {
                c.Save(memoryStream, SerializationMode.Xml);

                memoryStream.Position = 0L;

                var c2 = Container.Load(memoryStream, SerializationMode.Xml);

                Assert.AreEqual(c, c2);   
            }
        }
        #endregion
    }
}