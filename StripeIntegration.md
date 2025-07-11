# Stripe Integration Guide for SnacksPOS

## Overview
This guide explains how to integrate Stripe payments into your SnacksPOS application and optionally connect to a Stripe MCP server.

## Prerequisites
1. Stripe account and API keys
2. Node.js (for MCP server)
3. .NET 8 SDK (already have)

## Option 1: Direct .NET Stripe Integration

### 1. Install Stripe.net NuGet Package
```bash
dotnet add SnacksPOS.Web package Stripe.net
```

### 2. Configure Stripe in appsettings.json
```json
{
  "Stripe": {
    "PublishableKey": "pk_test_...",
    "SecretKey": "sk_test_..."
  }
}
```

### 3. Add Stripe Service Registration
In `Program.cs`:
```csharp
// Configure Stripe
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];
services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
```

### 4. Create Payment Intent for Checkout
```csharp
// In your checkout endpoint
var options = new PaymentIntentCreateOptions
{
    Amount = (long)(total * 100), // Convert to cents
    Currency = "usd",
    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
    {
        Enabled = true,
    },
};

var service = new PaymentIntentService();
var paymentIntent = await service.CreateAsync(options);
```

## Option 2: Stripe MCP Server Setup

### 1. Install MCP Server
```bash
npm install -g @modelcontextprotocol/server-stripe
```

### 2. Configuration File
Create a `.mcpconfig` file:
```json
{
  "mcpServers": {
    "stripe": {
      "command": "mcp-server-stripe",
      "env": {
        "STRIPE_API_KEY": "sk_test_your_key_here"
      }
    }
  }
}
```

### 3. Available MCP Tools
The Stripe MCP server provides these tools:
- `create_payment_intent`: Create payment intents
- `confirm_payment_intent`: Confirm payments
- `list_customers`: Retrieve customer data
- `create_customer`: Create new customers
- `list_payment_methods`: Get payment methods
- `create_subscription`: Set up subscriptions

### 4. Example MCP Usage
```typescript
// Example of calling MCP tools
const paymentIntent = await mcpClient.call("create_payment_intent", {
  amount: 2000, // $20.00 in cents
  currency: "usd",
  customer: "cus_customer_id"
});
```

## Integration Points in SnacksPOS

### Current Payment Flow
Your app currently has:
- `PayBalanceCommand.cs` - Marks entries as paid
- `CheckoutCommand.cs` - Creates ledger entries
- `/api/cart/checkout` endpoint

### Recommended Integration
1. **Replace the checkout endpoint** to create Stripe Payment Intents
2. **Add payment confirmation** after successful Stripe payment
3. **Update the account page** to show payment history from Stripe
4. **Add payment methods management**

### Modified Checkout Flow
1. User clicks checkout
2. Create Stripe Payment Intent
3. Redirect to Stripe Checkout or use Stripe Elements
4. On successful payment, mark ledger entries as paid
5. Redirect to success page

## Security Considerations
- Never expose secret keys in client-side code
- Use webhook endpoints for payment confirmation
- Validate webhook signatures
- Store minimal payment data in your database

## Testing
- Use Stripe test keys during development
- Test with Stripe's test card numbers
- Implement webhook testing with Stripe CLI

## Next Steps
1. Set up Stripe account and get API keys
2. Choose integration approach (direct .NET or MCP)
3. Implement payment flow
4. Test thoroughly
5. Deploy with production keys
