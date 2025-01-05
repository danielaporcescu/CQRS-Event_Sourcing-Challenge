### Coding Challenge: CQRS and Event Sourcing in .NET 9 - Bank API

#### **Objective**
Build a simple bank API using CQRS and Event Sourcing principles in .NET 9. The API will support:
- Transferring money between accounts.
- Fetching account balances.
- Depositing and withdrawing money via an ATM.
- Webhooks to notify other systems of balance changes.
- Preventing accounts from going below zero.

---

#### **Requirements**

1. **Domain**
   - Accounts have unique identifiers.
   - An account's balance cannot be negative.
   - A transfer operation between accounts is atomic.

2. **Commands**
   - `CreateAccount`: Create a new account with an initial balance.
   - `TransferMoney`: Transfer money from one account to another.
   - `DepositMoney`: Deposit money into an account.
   - `WithdrawMoney`: Withdraw money from an account via an ATM.

3. **Queries**
   - `GetBalance`: Retrieve the balance of an account.
   - `GetTransactionHistory`: Fetch the event history for an account.

4. **Event Sourcing**
   - Store all events (e.g., account creation, deposits, withdrawals, transfers).
   - Reconstruct account states from the event log.

5. **CQRS**
   - Separate write operations (commands) and read operations (queries).
   - Use separate models for reading and writing.

6. **Webhook**
   - Notify a configurable URL whenever an account's balance changes.

---

#### **Challenge Steps**

1. **Set Up the Project**
   - Fork this project
   - Create a .NET 9 Web API project.
   - Add required NuGet packages for CQRS and Event Sourcing (e.g., EventFlow).

2. **Implement the Write Model (Command Side)**
   - Define commands: `CreateAccount`, `TransferMoney`, `DepositMoney`, `WithdrawMoney`.
   - Use a service or handler to process commands.
   - Store events in an in-memory event store or a database (like SQL).

3. **Implement the Read Model (Query Side)**
   - Maintain a read-optimized projection of account balances.
   - Define queries: `GetBalance`, `GetTransactionHistory`.

4. **Event Sourcing**
   - Store events such as:
     - `AccountCreated`
     - `MoneyDeposited`
     - `MoneyWithdrawn`
     - `MoneyTransferred`
     - `BalanceUpdated`
   - Replay events to reconstruct the current state of an account.

5. **Business Rules**
   - Prevent transferring or withdrawing money if the account has insufficient funds.

6. **Webhook Integration**
   - Configure a webhook URL.
   - Send a POST request with account details whenever the balance changes.

7. **Endpoints**
   - `POST /accounts`: Create a new account.
   - `POST /accounts/transfer`: Transfer money between accounts.
   - `POST /accounts/deposit`: Deposit money into an account.
   - `POST /accounts/withdraw`: Withdraw money from an account.
   - `GET /accounts/{id}/balance`: Get an account's balance.
   - `GET /accounts/{id}/history`: Get an account's event history.

8. **Testing**
   - Write unit tests for command and query handlers.
   - Simulate webhook notifications during balance updates.

---

#### **Bonus Challenges**
1. **Concurrency Handling**
   - Ensure that events are processed in the correct order to avoid race conditions.

2. **Idempotency**
   - Prevent duplicate processing of events.

3. **Resilience**
   - Retry failed webhook notifications with exponential backoff.

4. **Advanced Projections**
   - Create projections for monthly account statements.

---

#### **Evaluation Criteria**
- Clean code and adherence to CQRS and Event Sourcing principles.
- Proper separation of concerns between command and query models.
- Handling edge cases, such as insufficient funds or invalid account IDs.
- Unit tests and documentation.

