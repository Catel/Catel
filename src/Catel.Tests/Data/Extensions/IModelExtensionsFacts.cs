namespace Catel.Tests.Data
{
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

            public static readonly IPropertyData FooProperty = RegisterProperty<string>(nameof(Foo));
        }

        public class Plugin : ChildAwareModelBase
        {
            public string Name
            {
                get { return GetValue<string>(NameProperty); }
                set { SetValue(NameProperty, value); }
            }

            public static readonly IPropertyData NameProperty = RegisterProperty<string>(nameof(Name));

            public FastObservableCollection<Preset> Presets
            {
                get { return GetValue<FastObservableCollection<Preset>>(PresetsProperty); }
                set { SetValue(PresetsProperty, value); }
            }

            public static readonly IPropertyData PresetsProperty = RegisterProperty<FastObservableCollection<Preset>>(nameof(Presets), () => new FastObservableCollection<Preset>());

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

            public static readonly IPropertyData NameProperty = RegisterProperty(nameof(Name), string.Empty);

            public FastObservableCollection<Plugin> Plugins
            {
                get { return GetValue<FastObservableCollection<Plugin>>(PluginsProperty); }
                set { SetValue(PluginsProperty, value); }
            }

            public static readonly IPropertyData PluginsProperty = RegisterProperty(nameof(Plugins), () => new FastObservableCollection<Plugin>());

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
                    Assert.That(plugin.IsDirty, Is.True);

                    foreach (var preset in plugin.Presets)
                    {
                        preset.Foo = "test";
                        Assert.That(preset.IsDirty, Is.True);
                    }
                }

                pluginChangeNotifications = 0;
                presetChangeNotifications = 0;

                Assert.That(pluginContainer.IsDirty, Is.True);

                pluginContainer.ClearIsDirtyOnAllChildren();

                Assert.That(pluginChangeNotifications, Is.EqualTo(100));
                Assert.That(presetChangeNotifications, Is.EqualTo(50000));

                // Test https://github.com/Catel/Catel/issues/1262
                Assert.That(pluginContainer.IsDirty, Is.False);

                foreach (var plugin in pluginContainer.Plugins)
                {
                    Assert.That(plugin.IsDirty, Is.False);
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
                    Assert.That(plugin.IsDirty, Is.True);

                    foreach (var preset in plugin.Presets)
                    {
                        preset.Foo = "test";
                        Assert.That(preset.IsDirty, Is.True);
                    }
                }

                pluginChangeNotifications = 0;
                presetChangeNotifications = 0;

                Assert.That(pluginContainer.IsDirty, Is.True);

                pluginContainer.ClearIsDirtyOnAllChildren(true);

                Assert.That(pluginChangeNotifications, Is.EqualTo(0));
                Assert.That(presetChangeNotifications, Is.EqualTo(0));

                // Test https://github.com/Catel/Catel/issues/1262
                Assert.That(pluginContainer.IsDirty, Is.False);

                foreach (var plugin in pluginContainer.Plugins)
                {
                    Assert.That(plugin.IsDirty, Is.False);
                }
            }
        }
    }
}
