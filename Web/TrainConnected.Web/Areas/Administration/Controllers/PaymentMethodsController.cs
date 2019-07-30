﻿namespace TrainConnected.Web.Areas.Administration.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using TrainConnected.Services.Data.Contracts;
    using TrainConnected.Web.InputModels.PaymentMethods;

    public class PaymentMethodsController : AdministrationController
    {
        private readonly IPaymentMethodsService paymentMethodsService;

        public PaymentMethodsController(IPaymentMethodsService paymentMethodsService)
        {
            this.paymentMethodsService = paymentMethodsService;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var paymentMethods = await this.paymentMethodsService.GetAllAsync();

            return this.View(paymentMethods);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            try
            {
                var paymentMethod = await this.paymentMethodsService.GetDetailsAsync(id);
                return this.View(paymentMethod);
            }
            catch (NullReferenceException)
            {
                return this.NotFound();
            }
        }

        [HttpGet]
        public IActionResult Add()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(PaymentMethodCreateInputModel paymentMethodCreateInputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(paymentMethodCreateInputModel);
            }

            try
            {
                var paymentMethod = await this.paymentMethodsService.CreateAsync(paymentMethodCreateInputModel);
                return this.RedirectToAction(nameof(this.Details), new { id = paymentMethod.Id });
            }
            catch (InvalidOperationException)
            {
                return this.BadRequest();
            }

        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            try
            {
                var paymentMethod = await this.paymentMethodsService.GetDetailsAsync(id);
                return this.View(paymentMethod);
            }
            catch (NullReferenceException)
            {
                return this.NotFound();
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            try
            {
                await this.paymentMethodsService.DeleteAsync(id);
                return this.RedirectToAction(nameof(this.All));
            }
            catch (NullReferenceException)
            {
                return this.NotFound();
            }
        }
    }
}
