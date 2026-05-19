namespace Etl.Core.Load;

public abstract class Loader
{
    protected internal abstract Type InstanceType { get; }
}

public abstract class Loader<TInst> : Loader
    where TInst : ILoaderInst
{
    protected internal override Type InstanceType => typeof(TInst);
}

//IMPORTANT: ILoaderArgs is transient, it is created with workflow.

public interface ILoaderInst
{
    void Initalize(object definition, LoaderArgs args);
    Task ProcessBatchAsync(BatchResult batch);
    void OnCompleted();
}

public abstract class LoaderInst<TDef> : ILoaderInst
    where TDef : Loader
{
    
    void ILoaderInst.Initalize(object definition, LoaderArgs args) => Initalize((TDef)definition, args);
    protected virtual void Initalize(TDef definition, LoaderArgs args) { }

    public abstract Task ProcessBatchAsync(BatchResult batch);

    void ILoaderInst.OnCompleted() => OnCompleted();
    protected virtual void OnCompleted() { }
}
