namespace Domain.Entidades;

public interface IEntity
{
  public Guid Id { get; }
}

public interface ILocalEntity : IEntity
{
}

public interface IExternalEntity : IEntity
{
}