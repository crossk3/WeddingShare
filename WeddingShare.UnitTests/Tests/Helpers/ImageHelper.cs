using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using WeddingShare.Enums;
using WeddingShare.Helpers;

namespace WeddingShare.UnitTests.Tests.Helpers
{
    public class ImageHelperTests
    {
        private readonly IFileHelper _fileHelper = Substitute.For<IFileHelper>();
        private readonly ILogger<ImageHelper> _logger = Substitute.For<ILogger<ImageHelper>>();
        private readonly IStringLocalizer<Lang.Translations> _localizer = Substitute.For<IStringLocalizer<Lang.Translations>>();
        private readonly IDictionary<ImageOrientation, Image?> _imageCollection;

        private static readonly string HeicFixture = Path.Combine(TestContext.CurrentContext.TestDirectory, "Fixtures", "Gradient.heic");

        private string WorkingDirectory = string.Empty;

        public ImageHelperTests()
        {
            _imageCollection = new Dictionary<ImageOrientation, Image?>()
            {
                { ImageOrientation.Square, new Image<Rgba32>(100, 100) },
                { ImageOrientation.Landscape, new Image<Rgba32>(200, 100) },
                { ImageOrientation.Portrait, new Image<Rgba32>(100, 200) }
            };
        }

        [SetUp]
        public void Setup()
        {
            WorkingDirectory = Path.Combine(Path.GetTempPath(), $"WeddingShareTests-{Guid.NewGuid()}");
            Directory.CreateDirectory(WorkingDirectory);
        }

        [TearDown]
        public void Teardown()
        {
            if (Directory.Exists(WorkingDirectory))
            {
                Directory.Delete(WorkingDirectory, true);
            }
        }

        [TestCase(ImageOrientation.Square)]
        [TestCase(ImageOrientation.Landscape)]
        [TestCase(ImageOrientation.Portrait)]
        public void ImageHelper_GetOrientation(ImageOrientation orientation)
        {
            var image = _imageCollection[orientation];
            Assert.IsNotNull(image);

            var actual = new ImageHelper(_fileHelper, _logger, _localizer).GetOrientation(image);
            Assert.That(actual, Is.EqualTo(orientation));
        }

        [TestCase("photo.heic", true)]
        [TestCase("photo.HEIF", true)]
        [TestCase("photo.hif", true)]
        [TestCase("photo.jpg", false)]
        [TestCase("photo.mov", false)]
        public void ImageHelper_IsHeif(string filename, bool expected)
        {
            var actual = new ImageHelper(_fileHelper, _logger, _localizer).IsHeif(filename);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase("photo.heic", MediaType.Image)]
        [TestCase("photo.heif", MediaType.Image)]
        [TestCase("photo.jpg", MediaType.Image)]
        [TestCase("photo.mov", MediaType.Video)]
        [TestCase("photo.docx", MediaType.Unknown)]
        public void ImageHelper_GetMediaType(string filename, MediaType expected)
        {
            var actual = new ImageHelper(_fileHelper, _logger, _localizer).GetMediaType(filename);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public async Task ImageHelper_EnsureWebSafeFormat_ConvertsHeicToJpeg()
        {
            var source = Path.Combine(WorkingDirectory, $"{Guid.NewGuid()}.heic");
            File.Copy(HeicFixture, source);

            _fileHelper.FileExists(Arg.Any<string>()).Returns(x => File.Exists((string)x[0]));
            _fileHelper.DeleteFileIfExists(Arg.Any<string>()).Returns(x => { File.Delete((string)x[0]); return true; });

            var actual = await new ImageHelper(_fileHelper, _logger, _localizer).EnsureWebSafeFormat(source);

            Assert.Multiple(() =>
            {
                Assert.That(actual, Is.EqualTo(Path.ChangeExtension(source, ".jpg")));
                Assert.That(File.Exists(actual), Is.True, "Converted JPEG was not written");
                Assert.That(File.Exists(source), Is.False, "Source HEIC should be removed once converted");
            });

            using (var img = await Image.LoadAsync(actual))
            {
                Assert.Multiple(() =>
                {
                    Assert.That(img.Width, Is.EqualTo(800));
                    Assert.That(img.Height, Is.EqualTo(600));
                });
            }
        }

        [Test]
        public async Task ImageHelper_EnsureWebSafeFormat_LeavesNonHeifUntouched()
        {
            var source = Path.Combine(WorkingDirectory, $"{Guid.NewGuid()}.jpg");
            _fileHelper.FileExists(Arg.Any<string>()).Returns(true);

            var actual = await new ImageHelper(_fileHelper, _logger, _localizer).EnsureWebSafeFormat(source);

            Assert.That(actual, Is.EqualTo(source));
        }

        [Test]
        public async Task ImageHelper_GenerateThumbnail_Heic()
        {
            var savePath = Path.Combine(WorkingDirectory, $"{Guid.NewGuid()}.webp");
            _fileHelper.FileExists(HeicFixture).Returns(true);

            var actual = await new ImageHelper(_fileHelper, _logger, _localizer).GenerateThumbnail(HeicFixture, savePath, 240);
            Assert.That(actual, Is.True);

            using (var img = await Image.LoadAsync(savePath))
            {
                Assert.Multiple(() =>
                {
                    Assert.That(img.Width, Is.EqualTo(240));
                    Assert.That(img.Height, Is.EqualTo(180));
                });
            }
        }

        [Test]
        public async Task ImageHelper_GetOrientation_Heic()
        {
            _fileHelper.FileExists(HeicFixture).Returns(true);

            var actual = await new ImageHelper(_fileHelper, _logger, _localizer).GetOrientation(HeicFixture);
            Assert.That(actual, Is.EqualTo(ImageOrientation.Landscape));
        }
    }
}