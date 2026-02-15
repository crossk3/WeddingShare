using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WeddingShare.Views.Rsvp
{
    public class IndexModel : PageModel
    {
        public IndexModel()
        {
            this.EmbedUrl = string.Empty;
        }

        public string EmbedUrl { get; set; }

        public void OnGet()
        {
        }
    }
}
