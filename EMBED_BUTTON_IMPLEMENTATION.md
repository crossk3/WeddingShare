# Embed URL Button Implementation - Complete

## Summary

Added an "Embed" button in the admin panel gallery list that displays the carousel embed URL and iframe code for easy sharing and embedding of galleries.

## Features

- **One-click embed code generation** - Click the embed button to get both URL and iframe code
- **Copy to clipboard** - Quick copy buttons for both URL and iframe code
- **Readonly key preference** - Automatically uses readonly key if available, otherwise falls back to secret key
- **Responsive modal** - Clean popup dialog with formatted embed code
- **Multi-language support** - Localized in English and Turkish (4 variants)

## Files Modified

### 1. Admin Panel Gallery List
**File:** `WeddingShare/Views/Account/Partials/GalleriesList.cshtml`

Added embed button after "Open Gallery" button:
```razor
<i class="btnEmbedGallery btn btn-outline-info fa-solid fa-code" alt="Embed" ...></i>
```

- Uses info color (`btn-outline-info`) to differentiate from other action buttons
- Uses Font Awesome `fa-code` icon
- Only visible to users with `GalleryPermissions.View`
- Disabled for galleries with no items

### 2. JavaScript Handler
**File:** `WeddingShare/wwwroot/js/account.js`

Added click handler for `.btnEmbedGallery` (after line 967):
- Extracts gallery identifier, readonly key, and secret key from table row
- Prefers readonly key over secret key for better security
- Builds carousel embed URL with `mode=4` parameter
- Generates iframe code with responsive dimensions (800x600)
- Displays popup modal with:
  - **Embed URL section**: Direct link with copy button
  - **iFrame code section**: HTML code with copy button
  - Both sections have click-to-select functionality
  - Copy buttons show confirmation message on click

### 3. Localization Strings

Added to all `.resx` and JavaScript language files:

#### Resource Files (.resx)
- `WeddingShare/Resources/Lang/Translations.en-GB.resx`
- `WeddingShare/Resources/Lang/Translations.tr-CY.resx`
- `WeddingShare/Resources/Lang/Translations.tr-TR.resx`
- `WeddingShare/Resources/Lang/Translations.tr.resx`

New keys added:
- `Copied_To_Clipboard` - "Copied to clipboard" / "Panoya kopyalandı"
- `Embed_Gallery` - "Embed Gallery" / "Galeriyi Göm"
- `Embed_URL` - "Embed URL" / "Gömme URL'si"
- `Embed_URL_Description` - Description of the embed URL
- `Embed_IFrame_Code` - "iFrame Embed Code" / "iFrame Gömme Kodu"
- `Embed_IFrame_Description` - Description of the iframe code

#### JavaScript Files
- `WeddingShare/wwwroot/js/lang/en.js`
- `WeddingShare/wwwroot/js/lang/en-GB.js`
- `WeddingShare/wwwroot/js/lang/tr.js`
- `WeddingShare/wwwroot/js/lang/tr-CY.js`
- `WeddingShare/wwwroot/js/lang/tr-TR.js`

Same keys added with proper translations for client-side localization.

## Usage

### Admin Panel Access

1. Navigate to `/Account` and go to the **Galleries** tab
2. Each gallery row shows action buttons
3. Click the blue **code icon** (📄) button to get embed code
4. A modal appears with two sections:

### Embed URL Section
```
https://your-domain/Gallery/Index?id=gallery-name&key=readonly_key&mode=4
```
- Direct link to carousel view
- Uses readonly key for security
- Click URL to select all
- Click copy button to copy to clipboard

### iFrame Code Section
```html
<iframe src="https://your-domain/Gallery/Index?id=gallery-name&key=readonly_key&mode=4"
        width="800" height="600" frameborder="0" loading="lazy"></iframe>
```
- Ready-to-paste HTML code
- Responsive iframe with lazy loading
- Click textarea to select all
- Click copy button to copy to clipboard

## Button Location

The embed button appears in the actions column between:
- **Open Gallery** (left)
- **Embed** (new - blue code icon)
- **Download** (right)
- **Edit** (right)
- **Wipe** (right)
- **Delete** (right)

## Security Features

1. **Readonly Key Priority**: Uses readonly key if available to prevent unauthorized uploads
2. **Permission Check**: Only visible to users with `GalleryPermissions.View`
3. **Empty Gallery Protection**: Disabled for galleries with 0 items
4. **Secure Key Handling**: Keys are properly encoded in URLs

## User Experience

- **Visual Feedback**: Copy buttons show "Copied to clipboard" message
- **Click-to-Select**: Click input/textarea to automatically select all text
- **Responsive Design**: Modal adapts to screen size
- **Clear Instructions**: Description text explains what each section is for
- **Icon Consistency**: Uses Font Awesome icons matching the rest of the admin panel

## Example URLs Generated

### Standard Gallery
```
https://wedding-share.example.com/Gallery/Index?id=summer-wedding&key=abc123readonly&mode=4
```

### Gallery with Spaces
```
https://wedding-share.example.com/Gallery/Index?id=john%20and%20jane&key=xyz789readonly&mode=4
```

## Testing Checklist

### Functionality
- [ ] Click embed button - modal appears
- [ ] Copy URL button - copies to clipboard
- [ ] Copy iframe button - copies to clipboard
- [ ] Click URL input - text is selected
- [ ] Click iframe textarea - text is selected
- [ ] Close button - modal closes
- [ ] Test with readonly key gallery
- [ ] Test with secret key only gallery
- [ ] Test with empty gallery (button disabled)

### Localization
- [ ] English (en-GB) - all strings display correctly
- [ ] English (en) - all strings display correctly
- [ ] Turkish (tr) - all strings display correctly
- [ ] Turkish Cyprus (tr-CY) - all strings display correctly
- [ ] Turkish Turkey (tr-TR) - all strings display correctly

### Permissions
- [ ] User with GalleryPermissions.View - can see button
- [ ] User without view permission - button hidden

### URL Generation
- [ ] Gallery identifier is correctly encoded
- [ ] Readonly key is used when available
- [ ] Secret key is fallback when no readonly key
- [ ] mode=4 parameter is present
- [ ] URL is valid and accessible

### Embed Testing
- [ ] Copy iframe code to test HTML file
- [ ] Open HTML file in browser
- [ ] Verify carousel displays correctly in iframe
- [ ] Test iframe responsiveness
- [ ] Verify navigation works within iframe

## Browser Compatibility

- ✅ Chrome/Edge (Chromium) - `navigator.clipboard.writeText` supported
- ✅ Firefox - `navigator.clipboard.writeText` supported
- ✅ Safari 13.1+ - `navigator.clipboard.writeText` supported
- ⚠️  Older browsers - may require manual copy (click-to-select still works)

## Integration with Carousel View

The embed button generates URLs with `mode=4`, which corresponds to the `ViewMode.Carousel` enum value implemented in the carousel view mode feature. This provides:

- 3 images per slide on desktop
- Responsive layout (2 on tablet, 1 on mobile)
- Previous/Next navigation arrows
- Carousel indicator dots
- Clean embed-friendly interface (no title, banner, or review counter)

## Future Enhancements

Possible improvements for future versions:

1. **Customizable iframe dimensions** - Let admins specify width/height
2. **Theme selection** - Generate embed code with light/dark mode parameter
3. **Auto-advance option** - Add parameter for automatic slideshow
4. **QR code generation** - Show QR code for mobile sharing
5. **Social media sharing** - Direct share buttons for Facebook, Twitter, etc.
6. **Preview embed** - Show live preview of embed before copying
7. **Embed analytics** - Track how many times embed code is viewed
8. **Custom CSS injection** - Allow custom styling for embedded galleries

## Files Summary

### Created
- None (all modifications to existing files)

### Modified (5 files)
1. `WeddingShare/Views/Account/Partials/GalleriesList.cshtml`
2. `WeddingShare/wwwroot/js/account.js`
3. `WeddingShare/Resources/Lang/Translations.*.resx` (4 files)
4. `WeddingShare/wwwroot/js/lang/*.js` (5 files)

Total: 10 files modified

## Implementation Date
2026-02-15

## Related Features
- Carousel View Mode (ViewMode.Carousel = 4)
- Readonly Secret Keys (for secure embedded access)
- Gallery Permissions System
- Multi-language Localization
