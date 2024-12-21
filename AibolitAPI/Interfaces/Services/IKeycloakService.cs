using Newtonsoft.Json;

namespace AibolitAPI.Services;

public interface IKeycloakService
{
    Task ProcessTokenAsync(string token);

    Task<IEnumerable<TDto>> GetEntitiesWithSSOAsync<TDto, TEntity>(IEnumerable<TEntity> entities,
        Func<TEntity, string> keycloakIdSelector) where TEntity : class;

    Task<TDto> GetEntityWithSSOAsync<TDto, TEntity>(TEntity entity,
        Func<TEntity, string> keycloakIdSelector) where TEntity : class;

    Task<string> GetAdminTokenAsync();

    string Serialize(object obj, int maxDepth = 2,
        ReferenceLoopHandling referenceLoopHandling = ReferenceLoopHandling.Ignore,
        Formatting formatting = Formatting.Indented);
}