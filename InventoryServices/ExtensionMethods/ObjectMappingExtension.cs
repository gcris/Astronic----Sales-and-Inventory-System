using CommonLibrary.Dtos;
using InventoryServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryServices.ExtensionMethods
{
    public static class ObjectMappingExtension
    {
        /// <summary>
        /// Returns an object <see cref="Item"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="ItemDtos"/> that represents the current object.
        /// </returns>
        public static Item AsItem(this ItemDtos source)
        {
            return new Item
            {
                CategoryId = source.CategoryId,
                Id = source.ItemId,
                MinimumStock = source.MinimumStock,
                LastUpdate = source.LastUpdate,
                Size = source.Size,
                QuantityOnHand = source.QuantityOnHand,
                BrandName = source.BrandName,
                Made = source.Made,
                Make = source.Make,
                Model = source.Model,
                PartNo = source.PartNo,
                OtherPartNo = source.OtherPartNo,
                UnitOfMeasure = source.UnitOfMeasure,
                Price1 = source.Price1,
                Price2 = source.Price2,
                CurrentCost = source.CurrentCost,
                SupplierId = source.SupplierId
            };
        }

        /// <summary>
        /// Returns an object <see cref="ItemDtos"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="Item"/> that represents the current object.
        /// </returns>
        public static ItemDtos AsItemDtos(this Item source)
        {
            return new ItemDtos
            {
                CategoryId = source.CategoryId,
                CategoryName = source.Category.Name,
                MinimumStock = source.MinimumStock,
                ItemId = source.Id,
                LastUpdate = source.LastUpdate,
                Size = source.Size,
                QuantityOnHand = source.QuantityOnHand,
                BrandName = source.BrandName,
                Made = source.Made,
                Make = source.Make,
                Model = source.Model,
                PartNo = source.PartNo,
                OtherPartNo = source.OtherPartNo,
                UnitOfMeasure = source.UnitOfMeasure,
                UnitOfMeasureString = source.UnitOfMeasure,
                Price1 = source.Price1,
                Price2 = source.Price2,
                CurrentCost = source.CurrentCost,
                SupplierId = source.Supplier == null ? 0 : source.SupplierId.Value,
                SupplierName = source.Supplier == null ? "" : source.Supplier.Company
            };
        }

        /// <summary>
        /// Returns an object <see cref="Category"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="CategoryDtos"/> that represents the current object.
        /// </returns>
        public static Category AsCategory(this CategoryDtos source)
        {
            return new Category
            {
                Id = source.CategoryId,
                Name = source.Name,
                MinimumStock = source.MinimumStock,
                LastUpdate = source.LastUpdate,
            };
        }

        /// <summary>
        /// Returns an object <see cref="CategoryDtos"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="Category"/> that represents the current object.
        /// </returns>
        public static CategoryDtos AsCategoryDtos(this Category source)
        {
            return new CategoryDtos
            {
                CategoryId = source.Id,
                Name = source.Name,
                MinimumStock = source.MinimumStock,
                LastUpdate = source.LastUpdate,
                //ItemDtosList = source.Items.Count < 1 ? null : source.Items.Select(item => item.AsItemDtos()),
            };
        }

        /// <summary>
        /// Returns an object <see cref="ItemPriceDtos"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="ItemPrice"/> that represents the current object.
        /// </returns>
        /// 
        public static ItemPriceDtos AsItemPriceDtos(this ItemPrice source)
        {
            return new ItemPriceDtos 
            {
                CategoryName = source.Item.Category.Name,
                CurrentCost = source.CurrentCost,
                ItemId = source.ItemId,
                ItemPartNo = source.Item.PartNo,
                ItemPriceId = source.Id,
                LastUpdate = source.LastUpdate,
                Price1 = source.Price1,
                Price2 = source.Price2
            };
        }

        /// <summary>
        /// Returns an object <see cref="ItemPrice"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="ItemPriceDtos"/> that represents the current object.
        /// </returns>
        /// 
        public static ItemPrice AsItemPrice(this ItemPriceDtos source)
        {
            return new ItemPrice
            {
                CurrentCost = source.CurrentCost,
                ItemId = source.ItemId,
                Id = source.ItemPriceId,
                LastUpdate = source.LastUpdate,
                Price1 = source.Price1,
                Price2 = source.Price2
            };
        }

        /// <summary>
        /// Returns an object <see cref="SalesInvoiceDtos"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="SalesInvoice"/> that represents the current object.
        /// </returns>
        public static SalesInvoiceDtos AsSalesInvoiceDtos(this SalesInvoice source)
        {
            return new SalesInvoiceDtos
            {
                CustomerId = source.Customer != null ? source.CustomerId.Value : 0,
                CustomerName = source.Customer != null ? source.Customer.CustomerName : "WAIK-IN",
                Date = source.Date,
                ORNumber = source.ORNumber,
                Remarks = source.Remarks,
                TotalAmount = source.TotalAmount,
                TotalQuantity = source.TotalQuantity,
                TotalDiscount = source.TotalDiscount,
                SalesInvoiceId = source.Id,
                SalesInvoiceDetailDtosList = source.SalesInvoiceDetailList.Select(detail => detail.AsSalesInvoiceDetailDtos()),
                Returned = source.Returned,
                UserId = source.UserId,
                UserName = string.Format("{0}, {1}", source.User.LastName, source.User.FirstName)
            };
        }

        /// <summary>
        /// Returns an object <see cref="SalesInvoice"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="SalesInvoiceDtos"/> that represents the current object.
        /// </returns>
        public static SalesInvoice AsSalesInvoice(this SalesInvoiceDtos source)
        {
            return new SalesInvoice
            {
                CustomerId = source.CustomerId,
                Date = source.Date,
                ORNumber = source.ORNumber,
                Remarks = source.Remarks,
                TotalAmount = source.TotalAmount,
                TotalQuantity = source.TotalQuantity,
                TotalDiscount = source.TotalDiscount,
                Id = source.SalesInvoiceId,
                Returned = source.Returned,
                UserId = source.UserId
            };
        }

        /// <summary>
        /// Returns an object <see cref="SalesInvoiceDetailDtos"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="SalesInvoiceDetail"/> that represents the current object.
        /// </returns>
        public static SalesInvoiceDetailDtos AsSalesInvoiceDetailDtos(this SalesInvoiceDetail source)
        {
            return new SalesInvoiceDetailDtos
            {
                ItemDtos = source.Item == null ? null : source.Item.AsItemDtos(),
                Quantity = source.Quantity,
                SalesInvoiceDetailId = source.Id,
                SalesInvoiceId = source.SalesInvoiceId,
                TotalAmount = source.TotalAmount,
                CurrentStock = source.CurrentStock,
                UnitPrice = source.UnitPrice,
                Discount = source.Discount,
                Price2 = source.Price2,
                CurrentCost = source.CurrentCost
            };
        }

        /// <summary>
        /// Returns an object <see cref="SalesInvoiceDetail"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="SalesInvoiceDetailDtos"/> that represents the current object.
        /// </returns>
        public static SalesInvoiceDetail AsSalesInvoiceDetail(this SalesInvoiceDetailDtos source)
        {
            return new SalesInvoiceDetail
            {
                ItemId = source.ItemDtos.ItemId,
                Quantity = source.Quantity,
                Id = source.SalesInvoiceDetailId,
                SalesInvoiceId = source.SalesInvoiceId,
                TotalAmount = source.TotalAmount,
                UnitPrice = source.UnitPrice,
                Price2 = source.Price2,
                Discount = source.Discount,
                CurrentCost = source.CurrentCost,
                CurrentStock = source.CurrentStock
            };
        }

        /// <summary>
        /// Returns an object <see cref="Customer"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="CustomerDtos"/> that represents the current object.
        /// </returns>
        public static Customer AsCustomer(this CustomerDtos source)
        {
            return new Customer
            {
                Id = source.CustomerId,
                Address = source.Address,
                CustomerName = source.CustomerName,
                ContactNo = source.ContactNo
            };
        }

        /// <summary>
        /// Returns an object <see cref="CustomerDtos"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="Customer"/> that represents the current object.
        /// </returns>
        public static CustomerDtos AsCustomerDtos(this Customer source)
        {
            return new CustomerDtos
            {
                CustomerId = source.Id,
                Address = source.Address,
                CustomerName = source.CustomerName,
                ContactNo = source.ContactNo
            };
        }

        /// <summary>
        /// Returns an object <see cref="PurchaseOrderDtos"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="PurchaseOrder"/> that represents the current object.
        /// </returns>
        public static PurchaseOrderDtos AsPurchaseOrderDtos(this PurchaseOrder source)
        {
            return new PurchaseOrderDtos
            {
                Date = source.Date,
                GrandTotalAmount = source.GrandTotalAmount,
                TotalQuantity = source.TotalQuantity,
                TotalDiscount = source.TotalDiscount,
                PONumber = source.PONumber,
                Remarks = source.Remarks,
                PurchaseOrderId = source.Id,
                SupplierName = source.Supplier.Company,
                SupplierId = source.SupplierId,
                Returned = source.Returned,
                PurchaseOrderDetailDtosList = source.PurchaseOrderDetailList.Select(order => order.AsPurchaseOrderDetailDtos()),
                UserId = source.UserId,
                UserName = string.Format("{0}, {1}", source.User.LastName, source.User.FirstName)
            };
        }

        /// <summary>
        /// Returns an object <see cref="PurchaseOrder"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="PurchaseOrderDtos"/> that represents the current object.
        /// </returns>
        public static PurchaseOrder AsPurchaseOrder(this PurchaseOrderDtos source)
        {
            return new PurchaseOrder
            {
                Date = source.Date,
                GrandTotalAmount = source.GrandTotalAmount,
                TotalQuantity = source.TotalQuantity,
                TotalDiscount = source.TotalDiscount,
                PONumber = source.PONumber,
                Remarks = source.Remarks,
                Id = source.PurchaseOrderId,
                SupplierId = source.SupplierId,
                Returned = source.Returned,
                UserId = source.UserId
            };
        }

        /// <summary>
        /// Returns an object <see cref="PurchaseOrderDetail"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="PurchaseOrderDetailDtos"/> that represents the current object.
        /// </returns>
        public static PurchaseOrderDetail AsPurchaseOrderDetail(this PurchaseOrderDetailDtos source)
        {
            return new PurchaseOrderDetail
            {
                Id = source.PurchaseOrderDetailId,
                ItemId = source.ItemDtos.ItemId,
                PurchaseOrderId = source.PurchaseOrderId,
                Quantity = source.Quantity,
                TotalAmount = source.TotalAmount,
                Discount = source.Discount,
                CurrentStock = source.CurrentStock,
                UnitPrice = source.UnitPrice,
                UOM = source.UOM
            };
        }

        /// <summary>
        /// Returns an object <see cref="PurchaseOrderDetailDtos"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="PurchaseOrderDetail"/> that represents the current object.
        /// </returns>
        public static PurchaseOrderDetailDtos AsPurchaseOrderDetailDtos(this PurchaseOrderDetail source)
        {
            return new PurchaseOrderDetailDtos
            {
                PurchaseOrderDetailId = source.Id,
                ItemDtos = source.Item.AsItemDtos(),
                PurchaseOrderId = source.PurchaseOrderId,
                Quantity = source.Quantity,
                TotalAmount = source.TotalAmount,
                Discount = source.Discount,
                CurrentStock = source.CurrentStock,
                UnitPrice = source.UnitPrice,
                UOM = source.UOM
            };
        }

        /// <summary>
        /// Returns an object <see cref="SalesReturnDetail"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="SalesReturnDetailDtos"/> that represents the current object.
        /// </returns>
        public static SalesReturnDetail AsSalesReturnDetail(this SalesReturnDetailDtos source)
        {
            return new SalesReturnDetail
            {
                Id = source.SalesInvoiceDetailId,
                Quantity = source.Quantity,
                Amount = source.Amount,
                Remarks = source.Remarks,
                SalesInvoiceDetailId = source.SalesInvoiceDetailId,
                SalesReturnId = source.SalesReturnId,
                CurrentStock = source.CurrentStock
            };
        }

        /// <summary>
        /// Returns an object <see cref="SalesReturnDetailDtos"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="SalesReturnDetail"/> that represents the current object.
        /// </returns>
        public static SalesReturnDetailDtos AsSalesReturnDetailDtos(this SalesReturnDetail source)
        {
            return new SalesReturnDetailDtos
            {
                SalesReturnDetailId = source.Id,
                Quantity = source.Quantity,
                Amount = source.Amount,
                Remarks = source.Remarks,
                ItemDtos = source.SalesInvoiceDetail.Item.AsItemDtos(),
                SalesInvoiceDetailId = source.SalesInvoiceDetailId,
                SalesReturnId = source.SalesReturnId,
                CurrentStock = source.CurrentStock
            };
        }

        /// <summary>
        /// Returns an object <see cref="SalesReturn"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="SalesReturnDtos"/> that represents the current object.
        /// </returns>
        public static SalesReturn AsSalesReturn(this SalesReturnDtos source)
        {
            return new SalesReturn
            {
                Id = source.SalesReturnId,
                Date = source.Date,
                ReferenceNumber = source.ReferenceNumber,
                TotalQuantity = source.TotalQuantity,
                TotalAmount = source.TotalAmount,
                UserId = source.UserId
            };
        }

        /// <summary>
        /// Returns an object <see cref="SalesReturnDtos"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="SalesReturn"/> that represents the current object.
        /// </returns>
        public static SalesReturnDtos AsSalesReturnDtos(this SalesReturn source)
        {
            return new SalesReturnDtos
            {
                SalesReturnId = source.Id,
                Date = source.Date,
                ReferenceNumber = source.ReferenceNumber,
                TotalQuantity = source.TotalQuantity,
                TotalAmount = source.TotalAmount,
                SalesReturnDetailDtosList = source.SalesReturnDetailList.Select(detail => detail.AsSalesReturnDetailDtos()),
                UserId = source.UserId,
                UserName = string.Format("{0}, {1}", source.User.LastName, source.User.FirstName)
            };
        }

        /// <summary>
        /// Returns an object <see cref="Supplier"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="SupplierDtos"/> that represents the current object.
        /// </returns>
        public static Supplier AsSupplier(this SupplierDtos source)
        {
            return new Supplier
            {
                Address = source.Address,
                Company = source.Company,
                ContactNo = source.ContactNo,
                ContactPerson = source.ContactPerson,
                Id = source.SupplierId
            };
        }

        /// <summary>
        /// Returns an object <see cref="SupplierDtos"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="Supplier"/> that represents the current object.
        /// </returns>
        public static SupplierDtos AsSupplierDtos(this Supplier source)
        {
            return new SupplierDtos
            {
                Address = source.Address,
                Company = source.Company,
                ContactNo = source.ContactNo,
                ContactPerson = source.ContactPerson,
                SupplierId = source.Id
            };
        }

        /// <summary>
        /// Returns an object <see cref="PurchaseOrderReturnDetail"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="PurchaseOrderReturnDetailDtos"/> that represents the current object.
        /// </returns>
        public static PurchaseOrderReturnDetail AsPurchaseOrderReturnDetail(this PurchaseOrderReturnDetailDtos source)
        {
            return new PurchaseOrderReturnDetail
            {
                Id = source.PurchaseOrderReturnDetailId,
                Quantity = source.Quantity,
                Amount = source.Amount,
                Remarks = source.Remarks,
                PurchaseOrderDetailId = source.PurchaseOrderDetailId,
                PurchaseOrderReturnId = source.PurchaseOrderId,
                CurrentStock = source.CurrentStock
            };
        }

        /// <summary>
        /// Returns an object <see cref="PurchaseOrderReturnDetailDtos"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="PurchaseOrderReturnDetail"/> that represents the current object.
        /// </returns>
        public static PurchaseOrderReturnDetailDtos AsPurchaseOrderReturnDetailDtos(this PurchaseOrderReturnDetail source)
        {
            return new PurchaseOrderReturnDetailDtos
            {
                PurchaseOrderReturnDetailId = source.Id,
                Quantity = source.Quantity,
                Amount = source.Amount,
                Remarks = source.Remarks,
                ItemDtos = source.PurchaseOrderDetail.Item.AsItemDtos(),
                PurchaseOrderDetailId = source.PurchaseOrderDetailId,
                PurchaseOrderId = source.PurchaseOrderReturnId,
                CurrentStock = source.CurrentStock
            };
        }

        /// <summary>
        /// Returns an object <see cref="PurchaseOrderReturn"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="PurchaseOrderReturnDtos"/> that represents the current object.
        /// </returns>
        public static PurchaseOrderReturn AsPurchaseOrderReturn(this PurchaseOrderReturnDtos source)
        {
            return new PurchaseOrderReturn
            {
                Id = source.PurchaseOrderReturnId,
                Date = source.Date,
                ReferenceNumber = source.ReferenceNumber,
                TotalQuantity = source.TotalQuantity,
                TotalAmount = source.TotalAmount,
                UserId = source.UserId
            };
        }

        /// <summary>
        /// Returns an object <see cref="PurchaseOrderReturnDtos"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="PurchaseOrderReturn"/> that represents the current object.
        /// </returns>
        public static PurchaseOrderReturnDtos AsPurchaseOrderReturnDtos(this PurchaseOrderReturn source)
        {
            return new PurchaseOrderReturnDtos
            {
                PurchaseOrderReturnId = source.Id,
                Date = source.Date,
                ReferenceNumber = source.ReferenceNumber,
                TotalQuantity = source.TotalQuantity,
                TotalAmount = source.TotalAmount,
                PurchaseOrderReturnDetailDtosList = source.PurchaseOrderReturnDetailList.Select(detail => detail.AsPurchaseOrderReturnDetailDtos()),
                UserId = source.UserId,
                UserName = string.Format("{0}, {1}", source.User.LastName, source.User.FirstName)
            };
        }

        /// <summary>
        /// Returns an object <see cref="InventorySummaryDtos"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="InventorySummary"/> that represents the current object.
        /// </returns>
        /// 
        public static InventorySummaryDtos AsInventorySummaryDtos(this InventorySummary source)
        {
            return new InventorySummaryDtos 
            {
                End = source.Date,
                Start = source.Date,
                TotalPurchase = source.TotalPurchase,
                TotalPurchaseReturn = source.TotalPurchaseReturn,
                TotalSalesReturn = source.TotalSalesReturn,
                TotalSold = source.TotalSold,
                InventorySummaryDetailDtosList = source.InventorySummaryDetailList.Select(detail => detail.AsInventorySummaryDetailDtos()),
                CategoryId = source.InventorySummaryDetailList.Count < 1 ? 0 : source.InventorySummaryDetailList.FirstOrDefault().Item.CategoryId,
                CategoryName = source.InventorySummaryDetailList.Count < 1 ? null : source.InventorySummaryDetailList.FirstOrDefault().Item.Category.Name
            };
        }

        /// <summary>
        /// Returns an object <see cref="InventorySummary"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="InventorySummaryDtos"/> that represents the current object.
        /// </returns>
        /// 
        public static InventorySummary AsInventorySummary(this InventorySummaryDtos source)
        {
            return new InventorySummary
            {
                Date = source.Start,
                TotalPurchase = source.InventorySummaryDetailDtosList.Sum(item => item.PurchasedItems),
                TotalPurchaseReturn = source.InventorySummaryDetailDtosList.Sum(item => item.PurchaseOrderReturnItems),
                TotalSalesReturn = source.InventorySummaryDetailDtosList.Sum(item => item.SalesReturnItems),
                TotalSold = source.InventorySummaryDetailDtosList.Sum(item => item.SoldItems),
                InventorySummaryDetailList = source.InventorySummaryDetailDtosList.Select(item => item.AsInventorySummaryDetail()).ToList()
            };
        }

        /// <summary>
        /// Returns an object <see cref="InventorySummaryDetailDtos"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="InventorySummaryDetail"/> that represents the current object.
        /// </returns>
        /// 
        public static InventorySummaryDetailDtos AsInventorySummaryDetailDtos(this InventorySummaryDetail source)
        {
            return new InventorySummaryDetailDtos
            {
                BeginningInv = source.BeginningInv,
                ItemDtos = source.Item.AsItemDtos(),
                CategoryName = source.Item.Category.Name,
                EndingInv = source.EndingInv,
                PurchasedItems = source.PurchasedItems,
                PurchaseOrderReturnItems = source.PurchaseOrderReturnItems,
                QOH = source.Item.QuantityOnHand,
                SalesReturnItems = source.SalesReturnItems,
                SoldItems = source.SoldItems,
                Date = source.InventorySummary.Date,
                ItemId = source.ItemId
            };
        }

        /// <summary>
        /// Returns an object <see cref="InventorySummaryDetail"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="InventorySummaryDetailDtos"/> that represents the current object.
        /// </returns>
        /// 
        public static InventorySummaryDetail AsInventorySummaryDetail(this InventorySummaryDetailDtos source)
        {
            return new InventorySummaryDetail
            {
                BeginningInv = source.BeginningInv,
                ItemId = source.ItemId,
                EndingInv = source.EndingInv,
                PurchasedItems = source.PurchasedItems,
                PurchaseOrderReturnItems = source.PurchaseOrderReturnItems,
                SalesReturnItems = source.SalesReturnItems,
                SoldItems = source.SoldItems
            };
        }

        /// <summary>
        /// Returns an object <see cref="UserDtos"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="User"/> that represents the current object.
        /// </returns>
        /// 
        public static UserDtos AsUserDtos(this User source)
        {
            return new UserDtos
            {
                Address = source.Address,
                ContactNumber = source.ContactNumber,
                FirstName = source.FirstName,
                UserId = source.Id,
                IsEnable = source.IsEnable,
                LastName = source.LastName,
                Password = source.Password,
                Username = source.Username,
                PasswordHistoryDtosList = source.PasswordHistories.Select(history => history.AsPasswordHistoryDtos()),
                UserRole = source.UserRole.RoleName,
                UserRoleId = source.UserRoleId,
                UserPrivilegeDtosList = source.UserRole.UserPrivilages.Select(privilege => privilege.AsUserPrivilegeDtos())
            };
        }

        /// <summary>
        /// Returns an object <see cref="User"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="UserDtos"/> that represents the current object.
        /// </returns>
        /// 
        public static User AsUser(this UserDtos source)
        {
            return new User
            {
                Address = source.Address,
                ContactNumber = source.ContactNumber,
                FirstName = source.FirstName,
                Id = source.UserId,
                IsEnable = source.IsEnable,
                LastName = source.LastName,
                Password = source.Password,
                Username = source.Username
            };
        }
        /// <summary>
        /// Returns an object <see cref="PasswordHistoryDtos"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="PasswordHistory"/> that represents the current object.
        /// </returns>
        /// 
        public static PasswordHistoryDtos AsPasswordHistoryDtos(this PasswordHistory source)
        {
            return new PasswordHistoryDtos
            {
                Date = source.Date,
                PasswordHistoryId = source.Id,
                Password = source.Password,
                UserId = source.UserId
            };
        }

        /// <summary>
        /// Returns an object <see cref="PasswordHistory"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="PasswordHistoryDtos"/> that represents the current object.
        /// </returns>
        /// 
        public static PasswordHistory AsPasswordHistory(this PasswordHistoryDtos source)
        {
            return new PasswordHistory
            {
                Date = source.Date,
                Id = source.PasswordHistoryId,
                Password = source.Password,
                UserId = source.UserId
            };
        }

        /// <summary>
        /// Returns an object <see cref="UserRole"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="UserRoleDtos"/> that represents the current object.
        /// </returns>
        /// 
        public static UserRole AsUserRole(this UserRoleDtos source)
        {
            return new UserRole
            {
                Id = source.UserRoleId,
                RoleName = source.RoleName
            };
        }

        /// <summary>
        /// Returns an object <see cref="UserRoleDtos"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="UserRole"/> that represents the current object.
        /// </returns>
        /// 
        public static UserRoleDtos AsUserRoleDtos(this UserRole source)
        {
            return new UserRoleDtos
            {
                UserRoleId = source.Id,
                RoleName = source.RoleName,
                UserPrivilegeDtosList = source.UserPrivilages.Select(item => item.AsUserPrivilegeDtos())
            };
        }

        /// <summary>
        /// Returns an object <see cref="UserPrivilegeDtos"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="UserPrivilege"/> that represents the current object.
        /// </returns>
        /// 
        public static UserPrivilegeDtos AsUserPrivilegeDtos(this UserPrivilege source)
        {
            return new UserPrivilegeDtos
            {
                IsEnable = source.IsEnable,
                Action = source.Action,
                UserRoleId = source.UserRoleId,
                UserPrivilegeId = source.Id
            };
        }

        /// <summary>
        /// Returns an object <see cref="UserPrivilege"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="UserPrivilegeDtos"/> that represents the current object.
        /// </returns>
        /// 
        public static UserPrivilege AsUserPrivilege(this UserPrivilegeDtos source)
        {
            return new UserPrivilege
            {
                IsEnable = source.IsEnable,
                Action = source.Action,
                UserRoleId = source.UserRoleId,
                Id = source.UserPrivilegeId
            };
        }

        /// <summary>
        /// Returns an object <see cref="UserActivityDtos"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="UserActivity"/> that represents the current object.
        /// </returns>
        /// 
        public static UserActivityDtos AsUserActivityDtos(this UserActivity source)
        {
            return new UserActivityDtos
            {
                Action = source.Action,
                Amount = source.Amount,
                Date = source.Date,
                Quantity = source.Quantity,
                CurrentStock = source.CurrentStock,
                ReferenceNumber = source.ReferenceNumber,
                Remarks = source.Remarks,
                Transaction = source.Transaction,
                UserName = string.Format("{0}, {1}", source.User.LastName, source.User.FirstName),
                ItemId = source.ItemId.HasValue ? source.ItemId.Value : 0
            };
        }

        /// <summary>
        /// Returns an object <see cref="UserActivity"/> that represents the current object.
        /// </summary>
        /// <returns>
        /// An object <see cref="UserActivityDtos"/> that represents the current object.
        /// </returns>
        /// 
        public static UserActivity AsUserActivity(this UserActivityDtos source)
        {
            return new UserActivity
            {
                Action = source.Action,
                Amount = source.Amount,
                Date = source.Date,
                Quantity = source.Quantity,
                CurrentStock = source.CurrentStock,
                ReferenceNumber = source.ReferenceNumber,
                Remarks = source.Remarks,
                Transaction = source.Transaction,
                UserId = source.UserId,
                ItemId = source.ItemId
            };
        }
    }
}
