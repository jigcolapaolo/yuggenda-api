using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Yuggenda.Application.DTOs.Customers;

public class UpdateCustomerRequest
{
    private string? _firstName;
    private string? _lastName;
    private string? _email;
    private string? _phone;

    [MaxLength(100)]
    public string? FirstName
    {
        get => _firstName;
        set { _firstName = value; FirstNameProvided = true; }
    }

    [MaxLength(100)]
    public string? LastName
    {
        get => _lastName;
        set { _lastName = value; LastNameProvided = true; }
    }

    [EmailAddress]
    [MaxLength(255)]
    public string? Email
    {
        get => _email;
        set { _email = value; EmailProvided = true; }
    }

    [MaxLength(50)]
    public string? Phone
    {
        get => _phone;
        set { _phone = value; PhoneProvided = true; }
    }

    [JsonIgnore]
    public bool FirstNameProvided { get; private set; }

    [JsonIgnore]
    public bool LastNameProvided { get; private set; }

    [JsonIgnore]
    public bool EmailProvided { get; private set; }

    [JsonIgnore]
    public bool PhoneProvided { get; private set; }
}