namespace Catel.Tests.Data
{
    using System.Collections.Generic;
    using Catel.Collections;
    using Catel.Data;
    using NUnit.Framework;

    public class IModelExtensionsFacts
    {
        public class Preset : ModelBase
        {
            public string Foo
            {
                get { return GetValue<string>(FooProperty); }
                set { SetValue(FooProperty, value); }
            }

            public static readonly PropertyData FooProperty = RegisterProperty(nameof(Foo), typeof(string), null);
        }

        public class Plugin : ChildAwareModelBase
        {
            public string Name
            {
                get { return GetValue<string>(NameProperty); }
                set { SetValue(NameProperty, value); }
            }

            public static readonly PropertyData NameProperty = RegisterProperty(nameof(Name), typeof(string), null);

            public DispatcherFastObservableCollection<Preset> Presets
            {
                get { return GetValue<DispatcherFastObservableCollection<Preset>>(PresetsProperty); }
                set { SetValue(PresetsProperty, value); }
            }

            public static readonly PropertyData PresetsProperty = RegisterProperty(nameof(Presets), typeof(FastObservableCollection<Preset>),
                () => new DispatcherFastObservableCollection<Preset>());

            public void ClearDirty()
            {
                IsDirty = false;
            }
        }

        public class PluginContainer : ChildAwareModelBase
        {
            public string Name
            {
                get { return GetValue<string>(NameProperty); }
                set { SetValue(NameProperty, value); }
            }

            public static readonly PropertyData NameProperty = RegisterProperty(nameof(Name), typeof(string), null);

            public DispatcherFastObservableCollection<Plugin> Plugins
            {
                get { return GetValue<DispatcherFastObservableCollection<Plugin>>(PluginsProperty); }
                set { SetValue(PluginsProperty, value); }
            }

            public static readonly PropertyData PluginsProperty = RegisterProperty(nameof(Plugins), typeof(FastObservableCollection<Plugin>),
                () => new DispatcherFastObservableCollection<Plugin>());

        }

        [TestFixture]
        public class TheClearIsDirtyOnAllChildrenMethod
        {
            [Test]
            public void DoesNotSuspendNotifications()
            {
                var pluginChangeNotifications = 0;
                var presetChangeNotifications = 0;

                var pluginContainer = new PluginContainer
                {
                    Name = "test"
                };

                for (int i = 0; i < 100; i++)
                {
                    var plugin = new Plugin
                    {
                    };

                    plugin.PropertyChanged += (sender, e) =>
                    {
                        pluginChangeNotifications++;
                    };

                    for (int j = 0; j < 500; j++)
                    {
                        var preset = new Preset
                        {
                            Foo = (j + 1).ToString()
                        };

                        preset.PropertyChanged += (sender, e) =>
                        {
                            presetChangeNotifications++;
                        };

                        plugin.Presets.Add(preset);
                    }

                    pluginContainer.Plugins.Add(plugin);
                }

                // Set up the test data (change all values)
                foreach (var plugin in pluginContainer.Plugins)
                {
                    plugin.Name = "dummy";
                    Assert.IsTrue(plugin.IsDirty);

                    foreach (var preset in plugin.Presets)
                    {
                        preset.Foo = "test";
                        Assert.IsTrue(preset.IsDirty);
                    }
                }

                pluginChangeNotifications = 0;
                presetChangeNotifications = 0;

                Assert.IsTrue(pluginContainer.IsDirty);

                pluginContainer.ClearIsDirtyOnAllChildren();

                Assert.AreEqual(100, pluginChangeNotifications);
                Assert.AreEqual(50000, presetChangeNotifications);

                // Test https://github.com/Catel/Catel/issues/1262
                Assert.IsFalse(pluginContainer.IsDirty);

                foreach (var plugin in pluginContainer.Plugins)
                {
                    Assert.IsFalse(plugin.IsDirty);
                }
            }

            [Test]
            public void DoesSuspendNotifications()
            {
                var pluginChangeNotifications = 0;
                var presetChangeNotifications = 0;

                var pluginContainer = new PluginContainer
                {
                    Name = "test"
                };

                for (int i = 0; i < 100; i++)
                {
                    var plugin = new Plugin
                    {
                    };

                    plugin.PropertyChanged += (sender, e) =>
                    {
                        pluginChangeNotifications++;
                    };

                    for (int j = 0; j < 500; j++)
                    {
                        var preset = new Preset
                        {
                            Foo = (j + 1).ToString()
                        };

                        preset.PropertyChanged += (sender, e) =>
                        {
                            presetChangeNotifications++;
                        };

                        plugin.Presets.Add(preset);
                    }

                    pluginContainer.Plugins.Add(plugin);
                }

                // Set up the test data (change all values)
                foreach (var plugin in pluginContainer.Plugins)
                {
                    plugin.Name = "dummy";
                    Assert.IsTrue(plugin.IsDirty);

                    foreach (var preset in plugin.Presets)
                    {
                        preset.Foo = "test";
                        Assert.IsTrue(preset.IsDirty);
                    }
                }

                pluginChangeNotifications = 0;
                presetChangeNotifications = 0;

                Assert.IsTrue(pluginContainer.IsDirty);

                pluginContainer.ClearIsDirtyOnAllChildren(true);

                Assert.AreEqual(0, pluginChangeNotifications);
                Assert.AreEqual(0, presetChangeNotifications);

                // Test https://github.com/Catel/Catel/issues/1262
                Assert.IsFalse(pluginContainer.IsDirty);

                foreach (var plugin in pluginContainer.Plugins)
                {
                    Assert.IsFalse(plugin.IsDirty);
                }
            }
        }
    }
}
