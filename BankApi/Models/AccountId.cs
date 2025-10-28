using System.Text.Json.Serialization;
using EventFlow.Core;
using EventFlow.ValueObjects;

namespace BankApi.Models;

[JsonConverter(typeof(SingleValueObjectConverter))]
public class AccountId(string value) : Identity<AccountId>(value);