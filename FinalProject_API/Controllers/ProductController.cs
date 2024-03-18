using FinalProject_API.Services;
using FinalProject_Data.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject_API.Controllers
{
    [Route("product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductServices _productServices;
        public ProductController(IProductServices productServices)
        {
            _productServices = productServices;
        }
        [HttpGet("get")]
        public ActionResult Get(int id)
        {
            try
            {
                var product = _productServices.Get(id);
                return Ok(product);
            }
            catch
            {
                return BadRequest();
            }

        }
        [HttpPost("create")]
        public ActionResult Create([FromBody] Product product)
        {
            try
            {
                var new_product = _productServices.Create(product);
                return Ok(new_product);
            }
            catch
            {
                return BadRequest();
            }

        }
        [HttpDelete("delete")]
        public ActionResult Delete(int id)
        {
            try
            {
                _productServices.Delete(id);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }

        }
        [HttpPut("edit")]
        public ActionResult Edit([FromBody] Product product)
        {
            try
            {
                var edit_product = _productServices.Edit(product);
                return Ok(edit_product);
            }
            catch
            {
                return BadRequest();
            }

        }
        [HttpGet("search")]
        public ActionResult Search([FromBody] ProductSearching productSearching)
        {
            try
            {
                var searchResult = _productServices.Search(productSearching);
                return Ok(searchResult);
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPost("addtocart")]
        public ActionResult AddToCart(int UserID, int ProductID, int ProductQuantity)
        {
            try
            {
                var cartItem = _productServices.AddToCart(UserID, ProductID, ProductQuantity);
                return Ok(cartItem);
            }
            catch(ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                return BadRequest();
            }

        }
        [HttpPost("removefromcart")]
        public ActionResult RemoveFromCart(int UserID, int ProductID)
        {
            try
            {
                var cartItem = _productServices.RemoveFromCart(UserID, ProductID);
                return Ok(cartItem);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPost("makereceipt")]
        public ActionResult MakeReceipt(int UserID)
        {
            try
            {
                var receipt = _productServices.MakeReceipt(UserID);
                return Ok(receipt);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
