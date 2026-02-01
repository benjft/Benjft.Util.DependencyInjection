using Benjft.Util.DependencyInjection.Attributes;

namespace Benjft.Util.DependencyInjection.Example.Invalid;

public interface IInvalidService;

[Service<IInvalidService>]
public class InvalidService;


public class InvalidGenericService<T>;