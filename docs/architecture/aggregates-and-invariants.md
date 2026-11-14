
# BookShop — Value Objects and Strongly Typed Identifiers

## 1. Value Objects

| Value Object | Bounded Context | Attributes and Types | Self-validation Rules | Immutable Domain Operation |
|---|---|---|---|---|
| Money | Orders, Payments, Product | Amount: decimal; Currency: string | Amount must be greater than zero. Currency must contain exactly three uppercase letters. | Add(Money other) returns a new Money object containing the sum. Different currencies are rejected. |
| Quantity | Orders, Inventory | Value: int | Value must be greater than zero. Zero and negative values are rejected. | IncreaseBy(Quantity other) returns a new Quantity object with the increased value. |
| BookTitle | Product / Book Catalogue | Value: string | Value cannot be null, empty or whitespace. Maximum length: 200 characters. | WithSuffix(string suffix) returns a new BookTitle object. Empty suffixes and titles longer than 200 characters are rejected. |
| PaymentRetryWindow | Payments | StartedAt: DateTime; RetryDeadline: DateTime | RetryDeadline must be later than StartedAt. Zero or negative duration is rejected. | Restart(DateTime newStart, int minutes) returns a new PaymentRetryWindow. Minutes must be greater than zero. |

## 2. Strongly Typed Identifiers

| Identifier | Entity | Bounded Context |
|---|---|---|
| CustomerId | Customer | Customer |
| ProductId | Product / Book | Product |
| InventoryItemId | Inventory Item | Inventory |
| OrderId | Order | Orders |
| PaymentId | Payment | Payments |
| ShipmentId | Shipment | Fulfilment |
| DeliveryId | Delivery | Fulfilment |

Each identifier is an immutable wrapper around a Guid. Strongly typed identifiers prevent accidentally passing an identifier of one entity where another identifier is expected.

Example:

```csharp
void LinkPayment(OrderId orderId, PaymentId paymentId);
```

## 3. Aggregate Boundaries

| Aggregate Root | Owned Objects | Bounded Context |
|---|---|---|
| Order | OrderLine | Orders |
| Payment | Payment state and retry window | Payments |
| InventoryItem | Stock level | Inventory |
| Shipment | Shipment details | Fulfilment |
| Delivery | Delivery details | Fulfilment |

The Order aggregate controls its OrderLine collection. Payment controls its own payment status. Fulfilment manages shipment and delivery independently from Orders.
