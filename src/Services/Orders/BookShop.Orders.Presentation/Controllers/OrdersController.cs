using BookShop.Orders.Application.Contracts;
using BookShop.Orders.Application.DTOs;
using BookShop.Orders.Domain.Exceptions;
using BookShop.Orders.Presentation.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Orders.Presentation.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<ActionResult<OrderSummary>> PlaceOrder(
        PlaceOrderRequest request)
    {
        try
        {
            var items = request.Items.Select(item =>
                (item.ProductId, item.Quantity, item.UnitPrice));

            var result = await _orderService.PlaceOrderAsync(
                request.CustomerId,
                items);

            return CreatedAtAction(
                nameof(GetOrder),
                new { id = result.Id },
                result);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new
            {
                Code = "INVALID_ORDER",
                exception.Message
            });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderSummary>> GetOrder(Guid id)
    {
        var result = await _orderService.GetOrderSummaryAsync(id);

        if (result is null)
        {
            return NotFound(new
            {
                Code = "ORDER_NOT_FOUND",
                Message = "Order was not found."
            });
        }

        return Ok(result);
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> CancelOrder(Guid id)
    {
        try
        {
            await _orderService.CancelOrderAsync(id);

            return Ok(new
            {
                Message = "Order was cancelled."
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
                Code = "ORDER_NOT_FOUND",
                exception.Message
            });
        }
    }

    [HttpPost("{id:guid}/confirm")]
    public async Task<ActionResult<OrderSummary>> ConfirmOrder(Guid id)
    {
        try
        {
            var result = await _orderService.ConfirmOrderAsync(id);

            return Ok(result);
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
    }
}