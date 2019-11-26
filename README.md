# AutoComposer
Automatically composes multiple objects into a single object using annotations,
or flattens a composable type into its component types.

# Usage
```
Composer composer = new Composer();

// Compose a top level object from multiple sub objects.
Object[] objects = { new Top(), new Middle(), new Bottom(), new Middle(), new Bottom() };
Top top = composer.Compose<Top>(objects);

// Flatten the type out into its children
Type[] types = composer.FlattenComposableType<Top>();

private class Top
{
    [Composable(2)]
    public Middle Middle2 { get; set; }

    [Composable(1)]
    public Middle Middle { get; set; }
}

private class Middle
{
    [Composable]
    public Bottom Bottom { get; set; }
}

private class Bottom
{ }
```
