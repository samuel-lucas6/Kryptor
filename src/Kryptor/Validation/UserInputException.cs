using System;

namespace Kryptor;

[Serializable]
public class UserInputException : Exception
{
    public UserInputException()
    {
    }

    public UserInputException(string message) : base(message)
    {
    }

    public UserInputException(string message, Exception innerException) : base(message, innerException)
    {
    }
}