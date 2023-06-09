﻿using System.Net.Mime;
using AutoMapper;
using BarHand.API.Inventory.Domain.Models;
using BarHand.API.Inventory.Domain.Services;
using BarHand.API.Inventory.Resources;
using BarHand.API.Shared.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace BarHand.API.Inventory.Controllers;

[ApiController]
[Route("/api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class ProductsController:ControllerBase
{
    private readonly IProductService _productService;
    private readonly IMapper _mapper;

    public ProductsController(IProductService productService, IMapper mapper)
    {
        _productService = productService;
        _mapper = mapper;
    }
    
    [HttpGet]
    public async Task<IEnumerable<ProductResource>> GetAllAsync()
    {
        var products = await _productService.ListAsync();
        var resources = _mapper.Map<IEnumerable<Product>, IEnumerable<ProductResource>>(products);
        return resources;
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(long id)
    {
        var result = await _productService.GetByIdAsync(id);
        if (!result.Success)
            return BadRequest(result.Message);

        var productResult = _mapper.Map<Product, ProductResource>(result.Resource);

        return Ok(productResult);
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] SaveProductResource resource)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.GetErrorMessages());

        var product = _mapper.Map<SaveProductResource, Product>(resource);

        var result = await _productService.SaveAsync(product);

        if (!result.Success)
            return BadRequest(result.Message);

        var productResource = _mapper.Map<Product, ProductResource>(result.Resource);
        return Created(nameof(PostAsync),productResource);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsync(long id, [FromBody] SaveProductResource resource)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.GetErrorMessages());
        
        var product = _mapper.Map<SaveProductResource, Product>(resource);

        var result = await _productService.UpdateAsync(id, product);
        
        if (!result.Success)
            return BadRequest(result.Message);

        var productResource = _mapper.Map<Product, ProductResource>(result.Resource);

        return Ok(productResource);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(long id)
    {
        var result = await _productService.DeleteAsync(id);
        
        if (!result.Success)
            return BadRequest(result.Message);

        var productResource = _mapper.Map<Product, ProductResource>(result.Resource);

        return Ok(productResource);
    }
}