using Ekzakt.FileManager.Core.Contracts;
using Ekzakt.FileManager.Core.Models.EventArgs;
using Ekzakt.FileManager.Core.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Smooth.Shop.Hubs;

namespace Smooth.Shop.Controllers;

public class UploadController : Controller
{
    private readonly IEkzaktFileManager _fileManager;
    private readonly IHubContext<UploadHub> _hubContext;


    public UploadController(
        IEkzaktFileManager fileManager, 
        IHubContext<UploadHub> hubContext)
    {
        _fileManager = fileManager;
        _hubContext = hubContext;
    }


    public IActionResult Index()
    {
        return View();
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
