using System;
using System.Text.Json.Serialization;
using EventFlow.Aggregates;
using EventFlow.Core;

namespace BankApi.Models
{
    public class Transaction
    {
        [JsonConstructor]
        public Transaction(
            IAggregateEvent aggregateEvent,
            IIdentity aggregateIdentity,
            int aggregateSequenceNumber,
            Type eventType,
            IMetadata metadata,
            DateTimeOffset timestamp)
        {
            AggregateEvent = aggregateEvent;
            AggregateIdentity = aggregateIdentity;
            EventType = eventType;
            AggregateSequenceNumber = aggregateSequenceNumber;
            Metadata = metadata;
            Timestamp = timestamp;
        }
        [JsonPropertyName("AggregateEvent")]
        public IAggregateEvent AggregateEvent { get; set; }

        [JsonPropertyName("AggregateIdentity")]
        public IIdentity AggregateIdentity { get; set; }
        
        [JsonPropertyName("AggregateSequenceNumber")]
        public int AggregateSequenceNumber { get; set; }

        [JsonPropertyName("EventType")]
        public Type EventType { get; set; }
        
        [JsonPropertyName("Metadata")]
        public IMetadata Metadata { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }
}