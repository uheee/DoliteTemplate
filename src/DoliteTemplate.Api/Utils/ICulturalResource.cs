using Microsoft.Extensions.Localization;

namespace DoliteTemplate.Api.Utils;

public interface ICulturalResource<TResource>
{
    IStringLocalizer<TResource> Localizer { get; init; }
}