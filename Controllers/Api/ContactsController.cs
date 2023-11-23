using System.Diagnostics;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lesson_33_MVC.Data;
using Lesson_33_MVC.Data.Models;
using Lesson_33_MVC.DTO;
using Lesson_33_MVC.ViewModels;
using Lesson_33_MVC.Services.Interfaces;

namespace Lesson_33_MVC.Controllers.Api;

// REST/CRUD API controller for the Contact entity/resource

[ApiController]
[Route("api/[controller]")]
public class ContactsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IContactsBookService _contactsBookService;
    private readonly ILogger<ContactsController> _logger;

    public ContactsController(IMapper mapper, AppDbContext appDbContext, ILogger<ContactsController> logger, IContactsBookService contactsBookService)
    {
        _mapper = mapper;
        _logger = logger;

        _contactsBookService = contactsBookService;
    }

    [HttpGet("")]
    [ProducesResponseType(typeof(GetContactDto[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> AllAsync([FromQuery] int start = 0, [FromQuery] int count = 10, CancellationToken cancellationToken = default)
    {
        var all = await _contactsBookService.GetAllAsync(cancellationToken);
        return Ok(all
            .Skip(start)
            .Take(count)
            .Select(
                c => _mapper.Map<Contact, GetContactDto>(c)
            )
            .ToArray()
        );
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetContactDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(int), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ByIdAsync([FromRoute] int id, CancellationToken cancellationToken)
    {
        return Ok(_mapper.Map<Contact, GetContactDto>(await _contactsBookService.GetByIdAsync(id, cancellationToken)));
    }

    [HttpGet("{id}/avatar")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(int), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAvatarAsync([FromRoute] int id, CancellationToken cancellationToken)
    {
        var avatar = await _contactsBookService.GetAvatarAsync(id, cancellationToken);
        return File(avatar.ImageData, avatar.ImageType);
    }

    [HttpPost("")]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAsync([FromForm] CreateContactDto contactDto, CancellationToken cancellationToken)
    {
        int? avatarId = null;
        if (contactDto.AvatarFile is not null)
        {
            avatarId = await _contactsBookService.AddAvatarAsync(contactDto.AvatarFile.OpenReadStream(), contactDto.AvatarFile.ContentType, cancellationToken);
        }

        var contact = (Contact)_mapper.Map<CreateContactDto, Contact>(contactDto);
        contact.AvatarId = avatarId;

        await _contactsBookService.AddAsync(contact);

        return RedirectToAction("Index", "Home");
    }

    [HttpGet("delete")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteContactAsync([FromQuery] int id, CancellationToken cancellationToken)
    {
        return await _contactsBookService.DeleteByIdAsync(id, cancellationToken) 
            ? RedirectToAction("Index", "Home") : StatusCode(500);
    }

    [HttpPut("")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(int), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EditContactAsync([FromForm] EditContactDto contactDto, CancellationToken cancellationToken)
    {
        var contact = _mapper.Map<EditContactDto, Contact>(contactDto);
        _logger.LogWarning(contact.ToString());
        return await _contactsBookService.EditAsync(contact, cancellationToken)
            ? Ok(contact.Id) : StatusCode(500);
    }
}
