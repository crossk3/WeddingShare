using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WeddingShare.Constants;
using WeddingShare.Helpers;

namespace WeddingShare.Controllers
{
    [AllowAnonymous]
    public class RsvpController : BaseController
    {
        private readonly ISettingsHelper _settings;
        private readonly ILogger _logger;
        private readonly IStringLocalizer<Lang.Translations> _localizer;

        public RsvpController(ISettingsHelper settings, ILogger<RsvpController> logger, IStringLocalizer<Lang.Translations> localizer)
            : base()
        {
            _settings = settings;
            _logger = logger;
            _localizer = localizer;
        }

        [HttpGet]
        [Route("Rsvp")]
        [Route("Rsvp/Index")]
        public async Task<IActionResult> Index()
        {
            var model = new Views.Rsvp.IndexModel();

            try
            {
                model.EmbedUrl = await _settings.GetOrDefault(Settings.Rsvp.EmbedUrl, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading RSVP page - {ex?.Message}");
            }

            return View("~/Views/Rsvp/Index.cshtml", model);
        }
    }
}
