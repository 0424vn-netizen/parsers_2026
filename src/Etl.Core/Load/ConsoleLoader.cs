namespace Etl.Core.Load;

public class ConsoleLoader : Loader<ConsoleLoaderInst>
{
}

public class ConsoleLoaderInst : LoaderInst<ConsoleLoader>
{
    public override Task ProcessBatchAsync(BatchResult batch)
    {
        Console.WriteLine(batch);
        return Task.CompletedTask;
    }
}