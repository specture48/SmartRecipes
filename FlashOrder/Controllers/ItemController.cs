﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using FlashOrder.Data;
using FlashOrder.DTOs;
using FlashOrder.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FlashOrder.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ItemController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ItemController> _logger;
        private readonly HttpContext _currentContext;
        
        public ItemController(ILogger<ItemController> logger, IMapper mapper, IUnitOfWork unitOfWork,IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentContext = httpContextAccessor.HttpContext;
        }
        
        public static string GetImageUrl(HttpContext context, string imageName)
        {
            return Path.Combine(GetBaseUrl(context), "images", imageName);
        }
 
        public static string GetBaseUrl(HttpContext context)
        {
            var request = context.Request;
            var host = request.Host.ToUriComponent();
            var pathBase = request.PathBase.ToUriComponent();
            return $"{request.Scheme}://{host}{pathBase}";
        }
        private async Task<string> SaveFile(IFormFile file)
        {
            var folderName = Path.Combine("wwwroot", "images");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            
            var returnPath = Path.Combine(folderName, fileName).Remove(0,8);;
            
            var fullPath = Path.Combine(pathToSave, fileName);
            
            
            try
            {
              await  using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }            
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"something went wrong in {nameof(SaveFile)}");
            }
        
            return returnPath;
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateItem([FromForm] CreateItemDTO itemDTO)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"invalid post attempt{nameof(CreateItem)}");
                return BadRequest(ModelState);
            }

            try
            {
                var file = itemDTO.ImageFile;
                var path=await  SaveFile(file);
                
                var item = _mapper.Map<Item>(itemDTO);
                item.ImagePath = path;
                
                await _unitOfWork.Items.Insert(item);
                await _unitOfWork.save();
                return Ok();
            }

            catch (Exception e)
            {
                _logger.LogError(e, $"something went wrong in {nameof(CreateItem)}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("{id:int}", Name = "GetItem")]
        public async Task<IActionResult> GetItem(int id)
        {
            if (id<1)
            {
                _logger.LogError($"invalid post attempt{nameof(GetItem)}");
                return NotFound("Your Recipe cannot be found");
            }
            
            try
            {
                var item=await _unitOfWork.Items.Get(q=>q.Id==id);
                var res=_mapper.Map<GetItemDTO>(item);
                return Ok(res);
            }
        
            catch (Exception e)
            {
                _logger.LogError(e, $"something went wrong in {nameof(GetItem)}");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}