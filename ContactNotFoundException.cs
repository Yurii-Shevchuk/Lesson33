using System.Runtime.Serialization;

namespace Lesson_33_MVC;

[Serializable]
internal class ContactNotFoundException : Exception
{
    private int id;

    public ContactNotFoundException()
    {
    }

    public ContactNotFoundException(int id)
    {
        this.id = id;
    }

    public ContactNotFoundException(string? message) : base(message)
    {
    }

    public ContactNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected ContactNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}