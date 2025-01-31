using Ekzakt.FileManager.Core.Contracts;
using Ekzakt.FileManager.Core.Models.EventArgs;
using Ekzakt.FileManager.Core.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Smooth.Shared.Extensions;
using Smooth.Shop.Application.Contracts;
using Smooth.Shop.Application.Requests;
using Smooth.Shop.Hubs;
using System.Text.Json;
using System.Web;

namespace Smooth.Shop.Controllers;

public class FileController : Controller
{
    private readonly ILogger<FileController> _logger;
    private readonly IEkzaktFileManager _fileManager;
    private readonly IHubContext<UploadHub> _hubContext;
    private readonly ISasTokenService _sasTokenService;


    public FileController(
        ILogger<FileController> logger,
        IEkzaktFileManager fileManager,
        IHubContext<UploadHub> hubContext,
        ISasTokenService sasTokenService)
    {
        _logger = logger;
        _fileManager = fileManager;
        _hubContext = hubContext;
        _sasTokenService = sasTokenService;
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
    public IActionResult Sas(string fileName, string connectionId, CancellationToken cancellationToken)
    {
        var request = new SasTokenRequest
        {
            FileName = GetFileName(
                HttpUtility.UrlDecode(fileName),
                HttpUtility.UrlDecode(connectionId)),
        };

        var response = _sasTokenService.GenerateSasToken(request);

        if (response.Success)
        { 
            return Ok(response);
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
            FileName = GetFileName(file.Name, connectionId),
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


    [HttpPost]
    public IActionResult Confirm([FromBody] UploadConfirmRequest uploadConfirmRequest, CancellationToken cancellationToken)
    {
        var result = JsonSerializer.Serialize(uploadConfirmRequest, new JsonSerializerOptions { WriteIndented = true });

        _logger.LogInformation($"Confirming upload of file: {result}");

        return Ok();
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


    private string GetFileName(string originalFileName, string connectionId)
    {
        if (string.IsNullOrWhiteSpace(connectionId))
        {
            connectionId = "null";
        }

        var userId = User.GetUserId();
        var fileName = $"{userId}__{connectionId}__{originalFileName}";

        return fileName;
    }

    #endregion Helpers
}
