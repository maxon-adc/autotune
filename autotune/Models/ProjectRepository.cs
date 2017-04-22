﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using autotune.Models;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing.Drawing2D;

namespace autotune.Models
{
    public class ProjectRepository
    {
        ProjectContext pc = new ProjectContext();
        public IEnumerable<Product> Products { get { return pc.Products; } }

        public Product DeleteProduct(int id)
        {
            Product productForDelete = pc.Products.Find(id);
            if (productForDelete != null)
            {
                DeleteImages(productForDelete);
                pc.Products.Remove(productForDelete);
                pc.SaveChanges();
            }
            return productForDelete;
        }

        public void SaveProduct(Product product)
        {
            if (product.Id == 0)
            {
                pc.Products.Add(product);
                product.BigImage = ResizeBigImage(product.Id);
                product.SmallImage = SaveSmallImage(product.Id);
            }
            else
            {
                Product forSave = pc.Products.Find(product.Id);
                if (forSave != null)
                {
                    if (forSave.BigImage != product.BigImage)
                    {
                        DeleteImages(forSave);
                    }
                    forSave.Name = product.Name;
                    forSave.Description = product.Description;
                    forSave.BigImage = ResizeBigImage(product.Id);
                    forSave.SmallImage = SaveSmallImage(product.Id);
                    forSave.Category = product.Category;
                }
            }
            pc.SaveChanges();
        }

        private string ResizeBigImage(int productId)
        {
            String oldImageLocation = HttpContext.Current.Server.MapPath(string.Format("~/Images/img_{0}.jpg", productId));
            Bitmap oldImage = new Bitmap(oldImageLocation);
            Size newSize = new Size(1024, 768);
            Bitmap resizedImage = new Bitmap(oldImage, newSize);
            oldImage.Dispose();
            System.IO.File.Delete(oldImageLocation);
            resizedImage.Save(HttpContext.Current.Server.MapPath(string.Format("~/Images/big_{0}.jpg", productId)), ImageFormat.Jpeg);
            resizedImage.Dispose();
            return string.Format("big_{0}.jpg", productId);
        }

        private string SaveSmallImage(int productId)
        {
            String bigImageLocation = HttpContext.Current.Server.MapPath(string.Format("~/Images/big_{0}.jpg", productId));
            Bitmap bigImage = new Bitmap(bigImageLocation);
            Size newSize = new Size(200, 150);
            Bitmap smallImage = new Bitmap(bigImage, newSize);
            smallImage.Save(HttpContext.Current.Server.MapPath(string.Format("~/Images/small_{0}.jpg", productId)), ImageFormat.Jpeg);
            smallImage.Dispose();
            bigImage.Dispose();
            return string.Format("small_{0}.jpg", productId);
        }

        private void DeleteImages(Product product)
        {
            string smallPath = HttpContext.Current.Server.MapPath(string.Format("~/Images/small_{0}.jpg", product.Id));
            string bigPath = HttpContext.Current.Server.MapPath(string.Format("~/Images/big_{0}.jpg", product.Id));
            if (System.IO.File.Exists(smallPath))
            {
                System.IO.File.Delete(smallPath);
            }
            if (System.IO.File.Exists(bigPath))
            {
                System.IO.File.Delete(bigPath);
            }
        }
    }
}