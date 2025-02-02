using Ekzakt.FileManager.Core.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Smooth.Shared.Extensions;
using Smooth.Shop.Application.Contracts;
using Smooth.Shop.Application.Managers;
using Smooth.Shop.Application.Mappers;
using Smooth.Shop.Application.Requests;
using Smooth.Shop.Application.Services.FileNameComposer;
using Smooth.Shop.Hubs;
using System.Web;

namespace Smooth.Shop.Controllers;

public class FileController : Controller
{
    private readonly ILogger<FileController> _logger;
    private readonly IEkzaktFileManager _fileManager;
    private readonly IHubContext<UploadHub> _hubContext;
    private readonly ISasTokenService _sasTokenService;
    private readonly INewMediumRepo _newMediumRepository;
    private readonly UploadManager _uploadManager;

    public FileController(
        ILogger<FileController> logger,
        IEkzaktFileManager fileManager,
        IHubContext<UploadHub> hubContext,
        ISasTokenService sasTokenService,
        INewMediumRepo newMediumRepository,
        UploadManager uploadManager)
    {
        _logger = logger;
        _fileManager = fileManager;
        _hubContext = hubContext;
        _sasTokenService = sasTokenService;
        _newMediumRepository = newMediumRepository;
        _uploadManager = uploadManager;
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
    public IActionResult Sas(SasTokenRequest sasTokenRequest, CancellationToken cancellationToken)
    {
        var composedFileName = new UploadFileNameComposer().Compose(
            HttpUtility.UrlDecode(sasTokenRequest.FileName),
            HttpUtility.UrlDecode(sasTokenRequest.ConnectionId),
            User.GetUserId());

        var request = new SasTokenRequest { FileName = composedFileName };
        var response = _sasTokenService.GenerateSasToken(request);

        if (response.Success)
        { 
            return Ok(response);
        }

        return new JsonResult(response);
    }


    [HttpPost]
    public async Task<IActionResult> Confirm([FromBody] ConfirmUploadRequest uploadConfirmRequest, CancellationToken cancellationToken)
    {
        var confirmUploadDto = uploadConfirmRequest.ToDto();
        var result = await _uploadManager.ConfirmUploadAsync(confirmUploadDto);

        if (result.Succeeded)
        {
            return Ok(result.Data);
        }

        return BadRequest(result.Messages);

    }
}
