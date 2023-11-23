using System.Collections.Immutable;
using Lesson_33_MVC.Data;
using Lesson_33_MVC.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Lesson_33_MVC.Services.Interfaces;

// DDD – Domain Driven Design

public interface IContactsBookService
{
    Task<IList<Data.Models.Contact>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Data.Models.Contact> GetByNameAsync(string searchName, CancellationToken cancellationToken = default);
    Task<Data.Models.Contact> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> AddAsync(Data.Models.Contact contact, CancellationToken cancellationToken = default);
    Task<Avatar> GetAvatarAsync(int id, CancellationToken cancellationToken = default);
    Task<int> AddAvatarAsync(Stream avatarStream, string contentType, CancellationToken cancellationToken = default);
    Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> EditAsync(Data.Models.Contact contact, CancellationToken cancellationToken = default);
}

public class DefaultContactsBookService : IContactsBookService
{
    private readonly AppDbContext _appDbContext;

    public DefaultContactsBookService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<int> AddAsync(Contact contact, CancellationToken cancellationToken = default)
    {
        var entity = await _appDbContext.Contacts.AddAsync((Contact)contact);
        await _appDbContext.SaveChangesAsync(cancellationToken);

        return entity.Entity.Id;
    }

    public async Task<int> AddAvatarAsync(Stream avatarStream, string contentType, CancellationToken cancellationToken)
    {
        using var memoryStream = new MemoryStream();

        await avatarStream.CopyToAsync(memoryStream);

        var avatarEntity = await _appDbContext.Avatars.AddAsync(new Avatar
        {
            ImageType = contentType,
            ImageData = memoryStream.ToArray(),
        });

        await _appDbContext.SaveChangesAsync(cancellationToken);

        return avatarEntity.Entity.Id;
    }

    public async Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var contact = await _appDbContext.Contacts.FindAsync(id, cancellationToken);
        if (contact is null)
        {
            throw new ContactNotFoundException(id);
        }

        _appDbContext.Remove(contact);
        await _appDbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> EditAsync(Contact contact, CancellationToken cancellationToken = default)
    {
        var contactFound = await _appDbContext.Contacts.FindAsync(contact.Id, cancellationToken);
        if (contactFound is null)
        {
            throw new ContactNotFoundException(contact.Id);
        }

        if (contact.FirstName is not null)
        {
            contactFound.FirstName = contact.FirstName;
        }

        if (contact.LastName is not null)
        {
            contactFound.LastName = contact.LastName;
        }

        if (contact.Phone is not null)
        {
            contactFound.Phone = contact.Phone;
        }

        if (contact.Address is not null)
        {
            contactFound.Address = contact.Address;
        }

        //...etc

        await _appDbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<IList<Contact>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _appDbContext.Contacts.Cast<Contact>().ToListAsync();

    public async Task<Avatar> GetAvatarAsync(int id, CancellationToken cancellationToken = default)
    {
        var contact = await _appDbContext.Contacts
            .Include(c => c.Avatar)
            .SingleAsync(c => c.Id == id, cancellationToken);

        if (contact is null || contact?.AvatarId is null)
        {
            throw new ContactNotFoundException(id);
        }

        return contact.Avatar;
    }

    public async Task<Contact> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var contact = await _appDbContext.Contacts.FindAsync(id, cancellationToken);
        if (contact is null)
        {
            throw new ContactNotFoundException(id);
        }

        return contact;
    }

    public async Task<Contact> GetByNameAsync(string searchName, CancellationToken cancellationToken = default)
    {
        var contact = await _appDbContext.Contacts.Where(x => x.FullName.Contains(searchName, StringComparison.OrdinalIgnoreCase)).FirstOrDefaultAsync(cancellationToken);
        if (contact is null)
        {
            throw new ContactNotFoundException(searchName);
        }

        return contact;
    }
}