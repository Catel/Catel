---
title: "Debugging Source Generators"
---
# Debugging Source Generators

When a source generator is producing unexpected output or errors, use the steps below to inspect the generated code.

## Emitting generated files to disk

By default, files created by source generators are kept in memory and are not written to disk. To write them to disk so they can be inspected, add the following to your `.csproj` file:

```xml
<PropertyGroup>
  <EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
  <CompilerGeneratedFilesOutputPath>Generated</CompilerGeneratedFilesOutputPath>
</PropertyGroup>

<ItemGroup>
  <!-- Exclude the generated files from compilation to avoid duplicate symbol errors -->
  <Compile Remove="$(CompilerGeneratedFilesOutputPath)/**/*.cs" />
</ItemGroup>
```

After adding this configuration:

1. Build the project. The generated `.cs` files will appear in the `Generated/` folder.
2. Temporarily disable the source generator package (comment it out in the `.csproj`).
3. Comment out the `<Compile Remove="..." />` line so the files in `Generated/` are compiled directly.
4. Open the generated files in Visual Studio and inspect them.

> Remember to restore the original configuration when you are done debugging.

## Common issues

### Missing parameterless constructor

**Symptom:** A XAML file fails to load because the type does not have a parameterless constructor.

**Possible causes:**

- The class is not declared as `partial`. The source generator can only add code to `partial` classes.
- The `Catel.SourceGenerators` package is not referenced in the project.
- The source generator failed silently. Enable file emission (see above) to check whether any files were generated.

### Design-time errors in the XAML designer

**Symptom:** The XAML designer throws exceptions or shows errors when opening a view.

**Explanation:** The generated `GetService<T>()` helper returns `null` at design time (`CatelEnvironment.IsInDesignMode == true`). If the base class constructor cannot handle `null` services gracefully, the designer may fail.

**Solution:** Ensure the Catel base classes used by your views support design-time instantiation. Refer to the [design-time view models](../catel-mvvm/designers/design-time-view-models.md) documentation for guidance.

### Duplicate constructor errors

**Symptom:** The compiler reports a duplicate constructor error after enabling `EmitCompilerGeneratedFiles`.

**Explanation:** When `EmitCompilerGeneratedFiles` is enabled and the `<Compile Remove="..." />` exclusion is also active, the generated files are on disk but excluded from compilation — this is the correct state. A duplicate error means the exclusion rule was removed while the generator is still active.

**Solution:** Ensure that exactly one of the following is true:

- The `<Compile Remove="..." />` line is active (normal debugging mode with files on disk).
- The source generator package reference is commented out (manual compilation from generated files).

Never have both the generator active and the files included in compilation at the same time.
