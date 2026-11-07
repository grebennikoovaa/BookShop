using BookShop.Orders.Application.Contracts;
using BookShop.Orders.Application.DTOs;
using BookShop.Orders.Domain.Exceptions;
using BookShop.Orders.Presentation.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Orders.Presentation.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IOrderService _orderService;

    public PaymentsController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<ActionResult<PaymentStatusResponse>> InitiatePayment(
        InitiatePaymentRequest request)
    {
        try
        {
            var result = await _orderService.InitiatePaymentAsync(
                request.OrderId,
                request.Amount);

            return CreatedAtAction(
                nameof(GetPayment),
                new { id = result.PaymentId },
                result);
        }
        catch (DomainException exception)
        {
            return UnprocessableEntity(new
            {
                exception.Code,
                exception.Message
            });
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new
            {
                Code = "ORDER_NOT_FOUND",
                exception.Message
            });
        }
        catch (InvalidOperationException exception)
        {
            return UnprocessableEntity(new
            {
                Code = "PAYMENT_NOT_ALLOWED",
                exception.Message
            });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new
            {
                Code = "INVALID_PAYMENT",
                exception.Message
            });
        }
    }

    [HttpPost("{id:guid}/confirm")]
    public async Task<IActionResult> ConfirmPayment(
        Guid id,
        ConfirmPaymentRequest request)
    {
        try
        {
            await _orderService.ConfirmPaymentAsync(
                id,
                request.ProviderReference);

            return Ok(new
            {
                Message = "Payment was received."
            });
        }
        catch (DomainException exception)
        {
            return UnprocessableEntity(new
            {
                exception.Code,
                exception.Message
            });
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new
            {
                Code = "PAYMENT_NOT_FOUND",
                exception.Message
            });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new
            {
                Code = "INVALID_PAYMENT",
                exception.Message
            });
        }
    }

    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> RejectPayment(Guid id)
    {
        try
        {
            await _orderService.RejectPaymentAsync(id);

            return Ok(new
            {
                Message = "Payment attempt was rejected. Retry is allowed."
            });
        }
        catch (DomainException exception)
        {
            return UnprocessableEntity(new
            {
                exception.Code,
                exception.Message
            });
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new
            {
                Code = "PAYMENT_NOT_FOUND",
                exception.Message
            });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PaymentDetails>> GetPayment(Guid id)
    {
        var result = await _orderService.GetPaymentDetailsAsync(id);

        if (result is null)
        {
            return NotFound(new
            {
                Code = "PAYMENT_NOT_FOUND",
                Message = "Payment was not found."
            });
        }

        return Ok(result);
    }
}