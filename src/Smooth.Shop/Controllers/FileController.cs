using Ekzakt.FileManager.Core.Contracts;
using Ekzakt.FileManager.Core.Models.EventArgs;
using Ekzakt.FileManager.Core.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Smooth.Shared.Configuration;
using Smooth.Shop.Application.Contracts;
using Smooth.Shop.Application.Requests;
using Smooth.Shop.Hubs;

namespace Smooth.Shop.Controllers;

public class FileController : Controller
{
    private readonly IEkzaktFileManager _fileManager;
    private readonly IHubContext<UploadHub> _hubContext;
    private readonly ISasTokenService _sasTokenService;
    private readonly AzureStorageOptions _azureStorageOptions;

    public FileController(
        IEkzaktFileManager fileManager,
        IHubContext<UploadHub> hubContext,
        ISasTokenService sasTokenService,
        IOptions<AzureStorageOptions> azureStorageOptions)
    {
        _fileManager = fileManager;
        _hubContext = hubContext;
        _sasTokenService = sasTokenService;
        _azureStorageOptions = azureStorageOptions.Value;
    }


    public IActionResult Index()
    {
        return View();
    }


    public IActionResult IndexOld()
    {
        return View("IndexOld");
    }

    [HttpGet]
    public IActionResult Sas(string fileId, CancellationToken cancellationToken)
    {
        var request = new SasTokenRequest
        {
            StorageAccountName = _azureStorageOptions.AccountName,
            StorageAccountKey = _azureStorageOptions.AccountKey,
            ContainerName = "data"
        };

        var response = _sasTokenService.GenerateSasToken(request);

        if (response.Success)
        { 
            return Ok(new { sasToken = response.SasTokon });
        }

        return new JsonResult(response);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(IFormFile file, string connectionId, string fileId, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { Message = "No file chosen." });
        }

        using var fileStream = file.OpenReadStream();

        var request = new SaveFileRequest
        {
            FileName = file.FileName,
            FileStream = fileStream,
            ProgressHandler = GetProgressHandler(connectionId, fileId),
            InitialFileSize = fileStream.Length
        };

        var result = await _fileManager.SaveFileAsync(request, cancellationToken);

        return StatusCode((int)result.Status, new
        {
            StatusCode = (int)result.Status,
            Message = result.Message ?? "Unhandled status message."
        });
    }


    #region Helpers

    public Progress<ProgressEventArgs> GetProgressHandler(string connectionId, string fileId)
    {
        return new Progress<ProgressEventArgs>(async progress =>
        {
            await _hubContext.Clients.Client(connectionId)
                .SendAsync("ReceiveProgress", new { fileId, progress.PercentageDone });
        });
    }

    #endregion Helpers
}
