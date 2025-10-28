using System.Text.Json.Serialization;
using EventFlow.Core;
using EventFlow.Sagas;
using EventFlow.ValueObjects;

namespace BankApi.Models;

[JsonConverter(typeof(SingleValueObjectConverter))]
public class TransferId(string value) : Identity<TransferId>(value), ISagaId;